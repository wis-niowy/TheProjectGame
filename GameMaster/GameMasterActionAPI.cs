using GameArea.AppMessages;
using GameArea.Parsers;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace GameArea
{
    public partial class GameMaster : IGameMaster
    {
        public bool GameReady
        {
            get
            {
                var redPlayersNumber = GetPlayersByTeam(TeamColour.red).Count;
                var bluePlayersNumber = GetPlayersByTeam(TeamColour.blue).Count;

                return redPlayersNumber + bluePlayersNumber == 2 * gameSettings.GameDefinition.NumberOfPlayersPerTeam;
            }
        }

        public void HandleConfirmGameRegistration(ConfirmGameRegistrationMessage msg)
        {
            ConsoleWriter.Show("Succesfull game registration. Awaiting for clients...");
            State = GameMasterState.AwaitingPlayers;
            GameId = msg.GameId;
        }

        private string[] PrepareGameReadyMessages()
        {
            List<string> msgs = new List<string>();
            msgs.Add(new GameStartedMessage(GameId).Serialize());
            foreach (var player in Players)
                msgs.Add(PrepareGameMessageForPlayer(player).Serialize());
            return msgs.ToArray();
        }

        private AppMessages.GameMessage PrepareGameMessageForPlayer(Player.Player player)
        {
            return new AppMessages.GameMessage(player.ID)
            {
                PlayerLocation = player.GetLocation,
                Players = Players.Select(q => new GameObjects.Player(q.ID, q.GetTeam, q.Role)).ToArray(),
                Board = new GameObjects.GameBoard(GetBoard.Width, GetBoard.TaskAreaHeight, GetBoard.GoalAreaHeight)
            };
        }

        private string GetUniqueGUID()
        {
            return "TEMP-GUID-" + (new Random()).Next(1000);
        }

        private ConfirmJoiningGame PrepareConfirmationMsg(ulong id, Messages.Player player)
        {
            return new ConfirmJoiningGame()
            {
                gameId = id,
                PlayerDefinition = player,
                playerId = player.id,
                privateGuid = GetUniqueGUID()
            };
        }

        private RejectJoiningGame PrepareRejectionMsg(string name, ulong id)
        {
            return new RejectJoiningGame()
            {
                gameName = name,
                playerId = id,
            };
        }

        private Player.Player PreparePlayerObject(TeamColour colour, ulong id)
        {
            return new Player.Player(colour)
            {
                ID = id,
                GameId = 0
            };
        }

        public string[] HandleJoinGameRequest(JoinGameMessage joinGame)
        {
            var expectedPlayersNumberPerTeam = gameSettings.GameDefinition.NumberOfPlayersPerTeam;
            var redPlayersNumber = GetPlayersByTeam(TeamColour.red).Count;
            var bluePlayersNumber = GetPlayersByTeam(TeamColour.blue).Count;
            var prefferedTeam = joinGame.PrefferedTeam;
            var prefferedRole = joinGame.PrefferedRole;
            var playerId = (ulong)joinGame.PlayerId;
            var responseData = new string[] { };

            Player.Player player = null;

            if (redPlayersNumber + bluePlayersNumber < 2 * expectedPlayersNumberPerTeam)
            // player can join one of two teams
            {
                ConsoleWriter.Show("Join request accepted...");
                if (GetPlayersByTeam(prefferedTeam).Count < expectedPlayersNumberPerTeam)
                // player can join the team he prefers
                {
                    player = PreparePlayerObject(prefferedTeam, playerId);
                }
                else
                // player joins another team
                {
                    prefferedTeam = prefferedTeam == TeamColour.red ? TeamColour.blue : TeamColour.red;
                    player = PreparePlayerObject(prefferedTeam, playerId);
                }
                
                var messagePlayerObject = player.ConvertToMessagePlayer();
                var leaders = GetPlayersByTeam(prefferedTeam).Where(p => p.Role == PlayerRole.leader);
                var canBeLeader = prefferedRole == PlayerRole.leader && leaders.Count() == 0;
                if (canBeLeader)
                {
                    messagePlayerObject.Role = PlayerRole.leader;
                    player.Role = PlayerRole.leader;
                }
                else
                {
                    messagePlayerObject.Role = PlayerRole.member;
                    player.Role = PlayerRole.member;
                }
                RegisterPlayer(player); // GameMaster rejestruje playera i umieszcza na boardzie

                responseData =  new string[] { new ConfirmJoiningGameMessage(GameId, messagePlayerObject, GetUniqueGUID(), player.ID).Serialize() };
            }
            else
            // player cannot join any of two teams
            {
                ConsoleWriter.Show("Join request rejected...");
                responseData = new string[] { new RejectGameRegistrationMessage(gameSettings.GameDefinition.GameName).Serialize() };
            }
            if (GameReady)
            {
                ConsoleWriter.Show("Required number of clients connected. Sending GameReady messages...");
                State = GameMasterState.GameInprogress;
                var additionalData = PrepareGameReadyMessages();
                responseData = responseData.Union(additionalData).ToArray();
            }
            return responseData;



            //jeżeli jest już komplet graczy to zwróc rejectjoininggame
            //dopasuj gracza do jego preferencji w miare możliwości
            //doczytac w dokumentacji czy w razie jak nikt nie chciał być liderem to nie ma lidera w team (sprawdzić, ajkby co to ustalamy że ostatni z dołączających do teamu zostaje liderem, jeżeli jeszcze go nie ma)
            //np. jeżeli chce być red i np. Lider to sprawdź czy jest wolne miejsce w red, a potem sprawdź czy nie ma lidera (najpierw dopasowanie po team, potem po roli)
            //dajmy na to że red jest full ale blue nie ma lidera więc ten agent trafia do blue team jako lider
            //dodajemy gracza do listy agentów gry 
            //nadajemy mu unikalne GUID, playerId jest przekazywane z serwera
            //wysyłamy do gracza ConfirmJoiningGame

            //sprawdzamy czy mamy komplet graczy
            //{
            //wysyłamy do każdeggo z gracza wiadomość Game z danymi gry
            //wysyłamy na serwer wiadomość GameStarted
            //zmieniamy stan GameMastera na Inprogress z GameMasterState
            //}
        }

        public void HandlePlayerDisconnectedRequest(PlayerDisconnectedMessage playerDisconnected)
        {
            ConsoleWriter.Show("Player disconnected...");
            UnregisterPlayer(playerDisconnected.PlayerID);
        }

        // --------------------------------------    API
        #region API
        /// <summary>
        /// Method to request a Test Piece action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public DataMessage HandleTestPieceRequest(TestPieceMessage msg)
        {
            ConsoleWriter.Show("Received TestingPiece...");
            string playerGuid = msg.PlayerGUID;
            ulong gameId = msg.GameId;
            Monitor.Enter(lockObject);
            GameObjects.Piece pieceDataToSend = null;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            try
            {
                if (Player.GetPiece != null)
                {
                    pieceDataToSend = new GameObjects.Piece(Player.GetPiece.ID, DateTime.Now, Player.GetPiece.Type, (long)Player.ID);
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            PrintBoardState();
            Thread.Sleep((int)GetCosts.TestDelay);
            return new DataMessage(Player.ID)
            {
                GameFinished = IsGameFinished,
                Pieces = new GameObjects.Piece[] { pieceDataToSend }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerGuid"></param>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public DataMessage HandleDestroyPieceRequest(DestroyPieceMessage msg)
        {
            ConsoleWriter.Show("Received DestroyPiece...");
            string playerGuid = msg.PlayerGUID;
            ulong gameId = msg.GameId;
            Monitor.Enter(lockObject);
            GameObjects.Piece pieceDataToSend = null;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            try
            { 
                if (Player.GetPiece != null)
                {
                    pieceDataToSend = null;
                    Player.SetPiece(null);
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            PrintBoardState();
            Thread.Sleep((int)GetCosts.TestDelay);
            return new DataMessage(Player.ID)
            {
                GameFinished = IsGameFinished,
                Pieces = new GameObjects.Piece[] { pieceDataToSend }
            };
        }

        /// <summary>
        /// Method to request a Place Piece action
        /// Player cannot place the piece if the field is already claimed
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public DataMessage HandlePlacePieceRequest(PlacePieceMessage msg)
        {
            ConsoleWriter.Show("Received PlacePiece...");
            string playerGuid = msg.PlayerGUID;
            ulong gameId = msg.GameId;
            var location = Players.Where(q => q.GUID == playerGuid).First().GetLocation;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            // basic information
            var response = new DataMessage(Player.ID)
            {
                GameFinished = IsGameFinished
            };

            Monitor.Enter(lockObject);
            try
            {
                // player posseses a piece
                if (Player.GetPiece != null)
                {
                    // player is on the TaskField
                    if (actualBoard.GetField(location.X, location.Y) is GameObjects.TaskField)
                    {
                        response.Tasks = TryPlacePieceOnTaskField(location, playerGuid);
                    }
                    // player carries a sham piece and is on a GoalField - he receives no data about a current GoalField and cannot place a piece
                    else if (actualBoard.GetField(location.X, location.Y) is GameObjects.GoalField && Player.GetPiece.Type == PieceType.sham)
                    {
                        response.Goals = TryPlaceShamPieceOnGoalField(location, playerGuid);
                    }
                    // the goal field is a goal or nongoal and player carries a normal piece
                    else if (actualBoard.GetField(location.X, location.Y) is GameObjects.GoalField)
                    {
                        response.Goals = TryPlaceNormalPieceOnGoalField(location, playerGuid);
                    }
                }
                // the field is task field and is claimed - action rejected
                // or the player doesn't posses a piece
                ////response.gameFinished = IsGameFinished;
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            PrintBoardState();
            Thread.Sleep((int)GetCosts.PlacingDelay);
            return response;

        }

        /// <summary>
        /// Method to request a Pick Up Piece action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public DataMessage HandlePickUpPieceRequest(PickUpPieceMessage msg)
        {
            ConsoleWriter.Show("Received PickUpPiece...");
            string playerGuid = msg.PlayerGUID;
            ulong gameId = msg.GameId;
            var location = Players.Where(a => a.GUID == playerGuid).First().GetLocation;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            GameObjects.Piece[] pieces = new GameObjects.Piece[] { null };

            var response = new DataMessage(Player.ID)
            {
                Pieces = pieces
            };

            Monitor.Enter(lockObject);
            try
            {
                var currentTaskField = actualBoard.GetField(location.X, location.Y) as GameObjects.TaskField;
                // the TaskField contains a piece
                if (currentTaskField != null && currentTaskField.Piece != null)
                {
                    GameObjects.Piece pieceDataToSend = new GameObjects.Piece(currentTaskField.Piece.ID, DateTime.Now, playerId: (long)Player.ID);

                    response.Pieces[0] = pieceDataToSend;

                    var piece = currentTaskField.Piece;
                    Player.SetPiece(piece); // Player picks up a piece
                    currentTaskField.Piece =null; // the piece is no longer on the field  
                    UpdateDistancesFromAllPieces();
                }

                // player is either on an empty TaskField or on a GoalField
                response.GameFinished = IsGameFinished;
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            PrintBoardState();
            Thread.Sleep((int)GetCosts.PickUpDelay);
            return response;
        }

        /// <summary>
        /// Method to request a Move action
        /// </summary>
        /// <param name="direction">direction requested by a Player</param>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public DataMessage HandleMoveRequest(MoveMessage msg)
        {
            ConsoleWriter.Show("Received Move ...");
            string playerGuid = msg.PlayerGUID;
            ulong gameId = msg.GameId;
            MoveType direction = (MoveType)msg.Direction;
            var currentLocation = Players.Where(a => a.GUID == playerGuid).First().GetLocation;
            var team = Players.Where(a => a.GUID == playerGuid).First().GetTeam;
            var Player = Players.Where(q => q.GUID == playerGuid).First();

            // perform location delta and get future field
            var futureLocation = PerformLocationDelta(direction, currentLocation, team);
            var futureBoardField = actualBoard.GetField(futureLocation.X, futureLocation.Y);

            //basic info for response
            DataMessage response = new DataMessage(Player.ID)
            {
                Tasks = new GameObjects.TaskField[] { },
                Goals = null,
                PlayerLocation = currentLocation,
            };

            //player tried to step out of the board or enetr wrong GoalArea
            if (!ValidateFieldPosition(futureLocation.X, futureLocation.Y, team))
                return response;

            GameObjects.Field responseField;
            GameObjects.Field field = actualBoard.GetField(futureLocation.X, futureLocation.Y);

            Monitor.Enter(lockObject);
            try
            {
                // what type of field are we trying to enter - Task or Goal?
                // we set info about a FutureField
                if (futureBoardField is GameObjects.TaskField)
                {
                    //TryMovePlayerToTaskField(response, futureLocation, playerGuid, out piece, out field);
                    List<GameObjects.TaskField> tempList = new List<GameObjects.TaskField>();
                    SetInfoAboutDiscoveredTaskField(futureLocation, 0, 0, field, tempList);
                    response.Tasks = tempList.ToArray();
                    responseField = response.Tasks[0];
                }
                else //if (futureBoardField is GameArea.GoalField)
                {
                    //TryMovePlayerToGoalField(response, futureLocation, playerGuid, out field);
                    response.Goals = new GameObjects.GoalField[] { };
                    response.Tasks = null;

                    List<GameObjects.GoalField> tempList = new List<GameObjects.GoalField>();
                    SetInfoAboutDiscoveredGoalField(futureLocation, 0, 0, field, tempList);
                    response.Goals = tempList.ToArray();
                    responseField = response.Goals[0];
                }

                // check if there is another Player on the field we're trying to enter
                // if so, we don't actually move, just get an update on the field
                if (responseField.Player == null)
                {
                    // perform move action
                    PerformMoveAction(currentLocation, futureLocation, playerGuid, response);
                }

                response.GameFinished = IsGameFinished;
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            PrintBoardState();
            Thread.Sleep((int)GetCosts.MoveDelay);
            return response;

        }

        /// <summary>
        /// Method to request a Discover action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public DataMessage HandleDiscoverRequest(DiscoverMessage msg)
        {
            ConsoleWriter.Show("Received Discover ...");
            string playerGuid = msg.PlayerGUID;
            ulong gameId = msg.GameId;
            var location = Players.Where(a => a.GUID == playerGuid).First().GetLocation;
            var team = Players.Where(q => q.GUID == playerGuid).First().GetTeam;
            List<GameObjects.TaskField> TaskFieldList = new List<GameObjects.TaskField>();
            List<GameObjects.GoalField> GoalFieldList = new List<GameObjects.GoalField>();

            Monitor.Enter(lockObject);
            try
            {
                for (int dx = -1; dx <= 1; ++dx)
                {
                    for (int dy = -1; dy <= 1; ++dy)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (ValidateFieldPosition((location.X + dx),(location.Y + dy), team))
                        {
                            GameObjects.Field field = actualBoard.GetField(location.X + dx, location.Y + dy);
                            // discovered field is a TaskField - can contain players and pieces
                            if (field is GameObjects.TaskField)
                            {
                                SetInfoAboutDiscoveredTaskField(location, dx, dy, field, TaskFieldList);
                            }

                            // discovered field is a GoalField - can contain players
                            else if (field is GameObjects.GoalField)
                            {
                                SetInfoAboutDiscoveredGoalField(location, dx, dy, field, GoalFieldList);
                            }
                        }
                    }
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            PrintBoardState();
            Thread.Sleep((int)GetCosts.DiscoverDelay);
            return new DataMessage(Players.Where(q => q.GUID == playerGuid).First().ID)
            {
                GameFinished = IsGameFinished,
                Tasks = TaskFieldList.ToArray(),
                Goals = GoalFieldList.ToArray()
            };
        }

        public RegisterGameMessage RegisterGame()
        {
            return new RegisterGameMessage(gameSettings.GameDefinition.GameName, (ulong)gameSettings.GameDefinition.NumberOfPlayersPerTeam, (ulong)gameSettings.GameDefinition.NumberOfPlayersPerTeam);
        }

        public void HandlerErrorMessage(AppMessages.ErrorMessage error)
        {
            ConsoleWriter.Warning("Received an error from server:\n Type:" + error.Type + "\nCause: " + error.CauseParameterName + "\nMessage: " + error.Message);
        }

        #endregion

    }
}
