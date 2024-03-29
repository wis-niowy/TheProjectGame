﻿using Messages;
using Player;
using System;
using System.Collections.Generic;
using System.Linq;
using GameArea;
using Configuration;
using System.Threading;
using System.Timers;
using GameArea.AppConfiguration;

namespace GameArea
{
    public partial class GameMaster : IGameMaster
    {
        static readonly object lockObject = new object();
        public static object LockObject { get { return lockObject; } }
        public ulong GameId { get; set; }
        public GameMasterState State { get; set; }
        public List<ExchengeRequestContainer> exchangeRequestList { get; set; }
        public DateTime GameStartDate { get; set; }
        public DateTime GameEndDate { get; set; }

        private ulong nextPieceId = 0;
        private Random random;
        private List<GameArea.GameObjects.Piece> pieces;
        public GameMasterSettingsConfiguration Settings { get; set; }
        private GameObjects.GameBoard actualBoard;
        private System.Timers.Timer pieceAdder;

        public ulong GoalsRedLeft { get; set; }
        public ulong GoalsBlueLeft { get; set; }

        public bool IsGameFinished
        {
            get
            {
                return GoalsBlueLeft == 0 || GoalsRedLeft == 0;
            }
        }
        public List<Player.Player> Players { get; set; }

        public GameMasterState GetState
        {
            get
            {
                return State;
            }
        }

        public GameMasterSettingsGameDefinitionConfiguration GetGameDefinition
        {
            get
            {
                return Settings.GameDefinition;
            }
        }

        public GameMasterSettingsActionCostsConfiguration GetCosts
        {
            get
            {
                return Settings.ActionCosts;
            }
        }


        public List<Player.Player> GetPlayersByTeam(Messages.TeamColour team)
        {
            return Players.Where(q => q.Team == team).ToList();
        }

        public Player.Player GetPlayerByGuid(string guid)
        {
            return Players.Where(q => q.GUID == guid).FirstOrDefault();
        }

        public Player.Player GetPlayerById(ulong id)
        {
            return Players.Where(q => q.ID == id).FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Player"></param>
        /// <param name="guid"></param>
        /// <param name="findFreeLocationAndPlacePlayer">Ustaw na false aby Game Master nie przydzielal znalezionego przez siebie miejsca i nie ustawial gracza na pozycji</param>
        public void RegisterPlayer(Player.Player Player, string guid = null, bool findFreeLocationAndPlacePlayer = true)
        {
            Player.GUID = guid != null ? guid:  Guid.NewGuid().ToString();
            Player.SetBoard(new GameObjects.GameBoard((int)GetGameDefinition.BoardWidth, (int)GetGameDefinition.TaskAreaLength, (int)GetGameDefinition.GoalAreaLength));
            if (findFreeLocationAndPlacePlayer)
            {
                var playerField = GetEmptyFieldForPlayer(Player.Team);
                Player.SetLocation(playerField);
                playerField.Player = new GameObjects.Player(Player.ID, Player.Team);
            }
            Players.Add(Player);
            ConsoleWriter.Show("Registered Player with params: GUID: " + Player.GUID + ", ID: " + Player.ID + " , Location: " + Player.Location + ", Team: " + Player.Team);

            if (Players.Count == 2 * GetGameDefinition.NumberOfPlayersPerTeam)
            {
                State = GameMasterState.GameInprogress;
                PrintBoardState();
            }
        }

        private string GeneratePlayerGUID(ulong ID)
        {
            var random = new Random((int)ID);
            int length = 32;
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var GUID = new char[length];

            for (int i = 0; i < GUID.Length; i++)
            {
                if (i == 8 || i == 13 || i == 18 || i == 23)
                    GUID[i] = '-';
                else
                    GUID[i] = chars[random.Next(chars.Length)];
            }

            return new String(GUID);
        }

        public void UnregisterPlayer(ulong id)
        {
            var player = Players.Where(p => p.ID == id).FirstOrDefault();
            if (player != null)
            {
                GetBoard.GetField(player.Location.X, player.Location.Y).Player = null;
                Players.Remove(player);
            }
        }

        public GameObjects.GameBoard GetBoard
        {
            get
            {
                return actualBoard;
            }
        }

        public GameObjects.GameBoard SetBoard
        {
            set { actualBoard = value; }
        }

        public GameMaster(GameMasterSettingsConfiguration settings)
        {
            State = GameMasterState.RegisteringGame;
            random = new Random();
            Players = new List<Player.Player>();
            pieces = new List<GameObjects.Piece>();
            exchangeRequestList = new List<ExchengeRequestContainer>();
            Settings = settings;
            InitPieceAdder();
        }

        public string[] RestartGame()
        {
            var gameFinishedMessages = PrepareGameFinishedMessages();
            if (gameFinishedMessages.Count() != Settings.GameDefinition.NumberOfPlayersPerTeam * 2 + 1)
                Console.WriteLine("FATAL ERROR! \n\n\n GAMEMASTER HAS NOT PREPARED ENOUGH DATA MESSAGES!");
            for (int i = 0; i < gameFinishedMessages.Count(); i++)
                Console.WriteLine("\nGAMEMASTER GAME FINISHED MESSAGE numer:" + i + "\n" + gameFinishedMessages[i]);
            InitBoard(GetGameDefinition);
            Players = new List<Player.Player>();

            State = GameMasterState.RegisteringGame;
            GameStartDate = DateTime.Now;
            return gameFinishedMessages;    
        }

        private void InitPieceAdder()
        {
            pieceAdder = new System.Timers.Timer(GetGameDefinition.PlacingNewPiecesFrequency);
            pieceAdder.Elapsed += PieceAdder_Elapsed;
            pieceAdder.AutoReset = true;
            pieceAdder.Start();
        }

        public void InitBoard(GameMasterSettingsGameDefinitionConfiguration settings)
        {
            Monitor.Enter(lockObject);
            actualBoard = new GameObjects.GameBoard((int)settings.BoardWidth, (int)settings.TaskAreaLength, (int)settings.GoalAreaLength,GoalFieldType.nongoal);
            PlaceInitialPieces((int)settings.InitialNumberOfPieces);
            PlaceInitialGoals(settings.Goals.Select(q=>q).ToArray());
            Monitor.Exit(lockObject);
        }

        private void PlaceInitialGoals(GameObjects.GoalField[] goals)
        {
            foreach (var goal in goals)
            {
                actualBoard.SetGoalField(new GameArea.GameMasterGoalField(goal.X, goal.Y,DateTime.Now, goal.Team, goal.Type));
            }
        }


        /// <summary>
        /// Places a piece in TaskArea in a random way
        /// </summary>
        /// <param name="piecesCount"></param>
        private void PlaceInitialPieces(int piecesCount)
        {
            for (int i = 0; i < piecesCount; i++)
            {
                var piece = CreatePiece();
                var field = GetEmptyFieldForPiece();
                field.Piece = piece;
            }
        }

        private bool PlaceNewPiece()
        {
            var piece = CreatePiece();
            var field = GetEmptyFieldForPiece();
            if (field != null)
            {
                field.Piece = piece;
                UpdateDistancesFromAllPieces();
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private GameObjects.Piece CreatePiece()
        {
            var possibleSham = random.NextDouble();
            GameArea.GameObjects.Piece piece;
            if (possibleSham <= GetGameDefinition.ShamProbability)
                piece = new GameObjects.Piece(nextPieceId++, DateTime.Now, PieceType.sham);
            else
                piece = new GameObjects.Piece(nextPieceId++, DateTime.Now, PieceType.normal);
            pieces.Add(piece);
            return piece;
        }

        public GameArea.GameObjects.Piece GetPieceById(ulong id)
        {
            List<GameArea.GameObjects.Piece> P = pieces.Where(p => p.ID == id).ToList();
            if (P.Count > 0)
                return P[0];
            else return null;
        }

        public void RemovePieceById(ulong id)
        {
            pieces.RemoveAll(p => p.ID == id);
        }

        private GameObjects.TaskField GetEmptyFieldForPiece()
        {
            int x, y;
            GameObjects.TaskField field;
            if (actualBoard.TaskFields.Where(q => q.Piece != null).Count() == (int)(actualBoard.Width * actualBoard.TaskAreaHeight))
                return null;
            do
            {
                y = random.Next() % actualBoard.TaskAreaHeight + actualBoard.GoalAreaHeight;
                x = random.Next() % actualBoard.Width;
                field = actualBoard.GetTaskField(x, y);
            }
            while (field == null || field.Piece != null);
            return field;
        }

        private GameArea.GameObjects.Field GetEmptyFieldForPlayer(TeamColour team)
        {
            int x = 0, y = team == TeamColour.blue ? (int)GetGameDefinition.GoalAreaLength : (int)GetGameDefinition.GoalAreaLength + (int)GetGameDefinition.TaskAreaLength - 1;
            GameArea.GameObjects.TaskField field = actualBoard.GetTaskField(x, y);
            while (field == null || field.Player != null)
            {
                x++;
                if (x / GetGameDefinition.BoardWidth == 1)
                {
                    x = 0;
                    y = team == TeamColour.blue ? y + 1 : y - 1;
                }
                field = actualBoard.GetTaskField(x, y);
            }
            return field;
        }

        /// <summary>
        /// method provides Player with lists myTeam and otherTeam
        /// </summary>
        /// <param name="Player"></param>
        private void SetGameInfo(Player.Player Player)
        {
            foreach (var otherPlayer in Players)
            {
                Player.myTeam = new List<GameObjects.Player>();
                Player.otherTeam = new List<GameObjects.Player>();
                //player from another team
                if (otherPlayer.Team != Player.Team)
                    Player.otherTeam.Add(otherPlayer.ConvertToMessagePlayer());
                //player from the same team
                else
                    Player.myTeam.Add(otherPlayer.ConvertToMessagePlayer());
            }
        }

        public void UpdateDistancesFromAllPieces()
        {
            List<GameObjects.TaskField> taskFields = actualBoard.TaskFields;
            foreach (var field in taskFields)
            {
                //if there is a piece on this field - distance = 0
                if (field.Piece != null)
                {
                    field.DistanceToPiece = 0;
                    continue;
                }
                else    //no piece -> check if there is a piece 1, 2, 3... fields away
                {
                    int currentDist = 1;
                    int x, y;
                    while (currentDist < field.DistanceToPiece && currentDist < taskFields.Count)
                    {
                        for (x = 0; x <= currentDist; x++)
                        {
                            y = currentDist - x;


                            if (CheckIfNotOutOfTaskArea(field.X + x, field.Y + y) && ((actualBoard.GetField(field.X + x, field.Y + y)) as GameObjects.TaskField).Piece != null)
                            {
                                field.DistanceToPiece = currentDist;
                                break;
                            }//first quarter
                            if (CheckIfNotOutOfTaskArea(field.X - x, field.Y + y) && ((actualBoard.GetField(field.X - x, field.Y + y)) as GameObjects.TaskField).Piece != null)
                            {
                                field.DistanceToPiece = currentDist;
                                break;
                            }//second quarter
                            if (CheckIfNotOutOfTaskArea(field.X + x, field.Y - y) && ((actualBoard.GetField(field.X + x, field.Y - y)) as GameObjects.TaskField).Piece != null)
                            {
                                field.DistanceToPiece = currentDist;
                                break;
                            }
                            if (CheckIfNotOutOfTaskArea(field.X - x, field.Y - y) && ((actualBoard.GetField(field.X - x, field.Y - y)) as GameObjects.TaskField).Piece != null)
                            {
                                field.DistanceToPiece = currentDist;
                                break;
                            }//fourth quarter

                        }
                        currentDist++;
                    }

                }
            }
        }

        private bool CheckIfNotOutOfTaskArea(int x, int y)
        {
            if (x < 0 || x >= actualBoard.Width ||
                y < actualBoard.GoalAreaHeight || y >= actualBoard.GoalAreaHeight + actualBoard.TaskAreaHeight)
                return false;
            return true;
        }

        
    }


}
