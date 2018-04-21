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

        /// <summary>
        /// Gets a request xml from an Agent and returns response xml with serialized Data object
        /// </summary>
        /// <param name="requestXml">Xml received from Agent</param>
        /// <returns>Serialized Data object</returns>
        public string HandleActionRequest(string requestXml)
        {
            Data responseData = null;
            MessageParser messageParser = new MessageParser();

            GameMessage msg = null;
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(requestXml);
            switch(xmlDoc.DocumentElement.Name)
            {
                case "TestPiece":
                    msg = messageParser.DeserializeXmlToObject<TestPiece>(requestXml);
                    responseData = HandleTestPieceRequest(msg as TestPiece);
                    break;
                case "Destroy":
                    msg = messageParser.DeserializeXmlToObject<DestroyPiece>(requestXml);
                    responseData = HandleDestroyPieceRequest(msg as DestroyPiece);
                    break;
                case "PlacePiece":
                    msg = messageParser.DeserializeXmlToObject<PlacePiece>(requestXml);
                    responseData = HandlePlacePieceRequest(msg as PlacePiece);
                    break;
                case "PickUpPiece":
                    msg = messageParser.DeserializeXmlToObject<PickUpPiece>(requestXml);
                    responseData = HandlePickUpPieceRequest(msg as PickUpPiece);
                    break;
                case "Move":
                    msg = messageParser.DeserializeXmlToObject<Move>(requestXml);
                    responseData = HandleMoveRequest(msg as Move);
                    break;
                case "Discover":
                    msg = messageParser.DeserializeXmlToObject<Discover>(requestXml);
                    responseData = HandleDiscoverRequest(msg as Discover);
                    break;
            }

            return messageParser.SerializeObjectToXml<Data>(responseData);
        }

        // --------------------------------------    API
        #region API
        /// <summary>
        /// Method to request a Test Piece action
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandleTestPieceRequest(TestPiece msg)
        {
            string playerGuid = msg.playerGuid;
            ulong gameId = msg.gameId;
            Monitor.Enter(lockObject);
            Piece pieceDataToSend = null;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            try
            {
                if (Player.GetPiece != null)
                {
                    pieceDataToSend = new Piece()
                    {
                        type = Player.GetPiece.type,
                        id = Player.GetPiece.id,
                        playerId = Player.ID,
                        timestamp = DateTime.Now
                    };
                }
            }
            finally
            {
                Monitor.Exit(lockObject);
            }
            PrintBoardState();
            Thread.Sleep((int)GetCosts.TestDelay);
            return new Data()
            {
                gameFinished = IsGameFinished,
                playerId = Player.ID,
                Pieces = new Piece[] { pieceDataToSend }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="playerGuid"></param>
        /// <param name="gameId"></param>
        /// <returns></returns>
        public Data HandleDestroyPieceRequest(DestroyPiece msg)
        {
            string playerGuid = msg.playerGuid;
            ulong gameId = msg.gameId;
            Monitor.Enter(lockObject);
            Piece pieceDataToSend = null;
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
            return new Data()
            {
                gameFinished = IsGameFinished,
                playerId = Player.ID,
                Pieces = new Piece[] { pieceDataToSend }
            };
        }

        /// <summary>
        /// Method to request a Place Piece action
        /// Player cannot place the piece if the field is already claimed
        /// </summary>
        /// <param name="playerGuid">guid of player requesting an action</param>
        /// <param name="gameId">id of current game</param>
        /// <returns></returns>
        public Data HandlePlacePieceRequest(PlacePiece msg)
        {
            string playerGuid = msg.playerGuid;
            ulong gameId = msg.gameId;
            var location = Players.Where(q => q.GUID == playerGuid).First().GetLocation;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            // basic information
            var response = new Data()
            {
                playerId = Player.ID,
                gameFinished = IsGameFinished
            };

            Monitor.Enter(lockObject);
            try
            {
                // player posseses a piece
                if (Player.GetPiece != null)
                {
                    // player is on the TaskField
                    if (actualBoard.GetField(location.x, location.y) is GameArea.TaskField)
                    {
                        response.TaskFields = TryPlacePieceOnTaskField(location, playerGuid);
                    }
                    // player carries a sham piece and is on a GoalField - he receives no data about a current GoalField and cannot place a piece
                    else if (actualBoard.GetField(location.x, location.y) is GameArea.GoalField && Player.GetPiece.type == PieceType.sham)
                    {
                        response.GoalFields = TryPlaceShamPieceOnGoalField(location, playerGuid);
                    }
                    // the goal field is a goal or nongoal and player carries a normal piece
                    else if (actualBoard.GetField(location.x, location.y) is GameArea.GoalField)
                    {
                        response.GoalFields = TryPlaceNormalPieceOnGoalField(location, playerGuid);
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
        public Data HandlePickUpPieceRequest(PickUpPiece msg)
        {
            string playerGuid = msg.playerGuid;
            ulong gameId = msg.gameId;
            var location = Players.Where(a => a.GUID == playerGuid).First().GetLocation;
            var Player = Players.Where(q => q.GUID == playerGuid).First();
            Piece[] pieces = new Piece[] { null };

            var response = new Data()
            {
                playerId = Player.ID,
                Pieces = pieces
            };

            Monitor.Enter(lockObject);
            try
            {
                var currentTaskField = actualBoard.GetField(location.x, location.y) as GameArea.TaskField;
                // the TaskField contains a piece
                if (currentTaskField != null && currentTaskField.GetPiece != null)
                {
                    Piece pieceDataToSend = new Piece()
                    {
                        type = PieceType.unknown,
                        id = currentTaskField.GetPiece.id,
                        playerId = Player.ID,
                        timestamp = DateTime.Now
                    };

                    response.Pieces[0] = pieceDataToSend;

                    var piece = currentTaskField.GetPiece;
                    Player.SetPiece(piece); // Player picks up a piece
                    currentTaskField.SetPiece(null); // the piece is no longer on the field  
                    UpdateDistancesFromAllPieces();
                }

                // player is either on an empty TaskField or on a GoalField
                response.gameFinished = IsGameFinished;
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
        public Data HandleMoveRequest(Move msg)
        {
            string playerGuid = msg.playerGuid;
            ulong gameId = msg.gameId;
            MoveType direction = msg.direction;
            var currentLocation = Players.Where(a => a.GUID == playerGuid).First().GetLocation;
            var team = Players.Where(a => a.GUID == playerGuid).First().GetTeam;
            var Player = Players.Where(q => q.GUID == playerGuid).First();

            // perform location delta and get future field
            var futureLocation = PerformLocationDelta(direction, currentLocation, team);
            var futureBoardField = actualBoard.GetField(futureLocation.x, futureLocation.y);

            //basic info for response
            Data response = new Data()
            {
                playerId = Player.ID,
                TaskFields = new Messages.TaskField[] { },
                GoalFields = null,
                PlayerLocation = currentLocation,
            };

            //player tried to step out of the board or enetr wrong GoalArea
            if (!ValidateFieldPosition(futureLocation.x, futureLocation.y, team))
                return response;

            Messages.Field responseField;
            Field field = actualBoard.GetField(futureLocation.x, futureLocation.y);

            Monitor.Enter(lockObject);
            try
            {
                // what type of field are we trying to enter - Task or Goal?
                // we set info about a FutureField
                if (futureBoardField is GameArea.TaskField)
                {
                    //TryMovePlayerToTaskField(response, futureLocation, playerGuid, out piece, out field);
                    List<Messages.TaskField> tempList = new List<Messages.TaskField>();
                    SetInfoAboutDiscoveredTaskField(futureLocation, 0, 0, field, tempList);
                    response.TaskFields = tempList.ToArray();
                    responseField = response.TaskFields[0];
                }
                else //if (futureBoardField is GameArea.GoalField)
                {
                    //TryMovePlayerToGoalField(response, futureLocation, playerGuid, out field);
                    response.GoalFields = new Messages.GoalField[] { };
                    response.TaskFields = null;

                    List<Messages.GoalField> tempList = new List<Messages.GoalField>();
                    SetInfoAboutDiscoveredGoalField(futureLocation, 0, 0, field, tempList);
                    response.GoalFields = tempList.ToArray();
                    responseField = response.GoalFields[0];
                }

                // check if there is another Player on the field we're trying to enter
                // if so, we don't actually move, just get an update on the field
                if (!responseField.playerIdSpecified)
                {
                    // perform move action
                    PerformMoveAction(currentLocation, futureLocation, playerGuid, response);
                }

                response.gameFinished = IsGameFinished;
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
        public Data HandleDiscoverRequest(Discover msg)
        {
            string playerGuid = msg.playerGuid;
            ulong gameId = msg.gameId;
            var location = Players.Where(a => a.GUID == playerGuid).First().GetLocation;
            var team = Players.Where(q => q.GUID == playerGuid).First().GetTeam;
            List<Messages.TaskField> TaskFieldList = new List<Messages.TaskField>();
            List<Messages.GoalField> GoalFieldList = new List<Messages.GoalField>();

            Monitor.Enter(lockObject);
            try
            {
                for (int dx = -1; dx <= 1; ++dx)
                {
                    for (int dy = -1; dy <= 1; ++dy)
                    {
                        if (dx == 0 && dy == 0) continue;
                        if (ValidateFieldPosition((location.x + dx),(location.y + dy), team))
                        {
                            Field field = actualBoard.GetField(location.x + dx, location.y + dy);
                            // discovered field is a TaskField - can contain players and pieces
                            if (field is TaskField)
                            {
                                SetInfoAboutDiscoveredTaskField(location, dx, dy, field, TaskFieldList);
                            }

                            // discovered field is a GoalField - can contain players
                            else if (field is GoalField)
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
            return new Data()
            {
                gameFinished = IsGameFinished,
                playerId = Players.Where(q => q.GUID == playerGuid).First().ID,
                TaskFields = TaskFieldList.ToArray(),
                GoalFields = GoalFieldList.ToArray()
            };
        }

        #endregion

    }
}
