using GameArea;
using Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Player.PlayerMessages;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Player.Tests
{
    public partial class PlayerTests
    {
        public Leader GetLeader(string guid, ulong id, TeamColour tc, ActionType action, IPlayerController controller = null)
        {
            var leader = new Leader(tc, PlayerRole.leader, guid: guid)
            {
                ID = id,
                LastActionTaken = action,
                Controller = controller,
                State = AgentState.Playing
            };
            controller.Player = leader;

            return leader;
        }


        [TestMethod]
        public void HandleKnowledgeExchangeRequestWhenLeaderOnTaskFieldTest()
        {
            InitGameMaster();
            var Player = GetLeader("testGUID-0004", 10, TeamColour.blue, ActionType.PickUpPiece);
            Player.SetLocation(2, 5);

            RegisterPlayer(Player, Player.GUID);
            Player.GetBoard.GetTaskField(2, 7).Player = new GameArea.GameObjects.Player(50, TeamColour.red, PlayerRole.member);
            Player.GetBoard.GetGoalField(2, 0).Player = new GameArea.GameObjects.Player(60, TeamColour.red, PlayerRole.leader);
            Player.GetBoard.GetTaskField(3, 5).Piece = new GameArea.GameObjects.Piece(150, DateTime.Now, PieceType.unknown);

            KnowledgeExchangeRequestAgent request = new KnowledgeExchangeRequestAgent(10, 5);

            var responseArray = request.Process(Player.Controller);
            var responseData = PlayerReader.GetObjectFromXML(responseArray[0]) as DataAgent;

            Assert.AreEqual(5ul, responseData.PlayerId);

            Assert.AreEqual(10ul, responseData.Tasks.Where(f => f.X == 2 && f.Y == 5).FirstOrDefault().Player.ID);
            Assert.AreEqual(10, responseData.Tasks.Where(f => f.X == 2 && f.Y == 5).FirstOrDefault().PlayerId);

            Assert.AreEqual(50ul, responseData.Tasks.Where(f => f.X == 2 && f.Y == 7).FirstOrDefault().Player.ID);
            Assert.AreEqual(50, responseData.Tasks.Where(f => f.X == 2 && f.Y == 7).FirstOrDefault().PlayerId);
            Assert.AreEqual(PlayerRole.member, responseData.Tasks.Where(f => f.X == 2 && f.Y == 7).FirstOrDefault().Player.Role);

            Assert.AreEqual(60ul, responseData.Goals.Where(f => f.X == 2 && f.Y == 0).FirstOrDefault().Player.ID);
            Assert.AreEqual(60, responseData.Goals.Where(f => f.X == 2 && f.Y == 0).FirstOrDefault().PlayerId);

            Assert.AreEqual(150ul, responseData.Tasks.Where(f => f.X == 3 && f.Y == 5).FirstOrDefault().Piece.ID);
        }

        [TestMethod]
        public void HandleKnowledgeExchangeRequestWhenLeaderOnGoalieldTest()
        {
            InitGameMaster();
            var Player = GetLeader("testGUID-0004", 10, TeamColour.blue, ActionType.PickUpPiece);
            Player.SetLocation(2, 2);

            RegisterPlayer(Player, Player.GUID);
            Player.GetBoard.GetTaskField(2, 7).Player = new GameArea.GameObjects.Player(50, TeamColour.red, PlayerRole.member);

            RegisterPlayer(Player, Player.GUID);
            Player.GetBoard.GetTaskField(2, 7).Player = new GameArea.GameObjects.Player(50, TeamColour.red, PlayerRole.member);
            Player.GetBoard.GetGoalField(2, 0).Player = new GameArea.GameObjects.Player(60, TeamColour.red, PlayerRole.leader);
            Player.GetBoard.GetTaskField(3, 5).Piece = new GameArea.GameObjects.Piece(150, DateTime.Now, PieceType.unknown);

            KnowledgeExchangeRequestAgent request = new KnowledgeExchangeRequestAgent(10, 5);

            var responseArray = request.Process(Player.Controller);
            var responseData = PlayerReader.GetObjectFromXML(responseArray[0]) as DataAgent;

            Assert.AreEqual(5ul, responseData.PlayerId);

            Assert.AreEqual(10ul, responseData.Goals.Where(f => f.X == 2 && f.Y == 2).FirstOrDefault().Player.ID);
            Assert.AreEqual(10, responseData.Goals.Where(f => f.X == 2 && f.Y == 2).FirstOrDefault().PlayerId);

            Assert.AreEqual(50ul, responseData.Tasks.Where(f => f.X == 2 && f.Y == 7).FirstOrDefault().Player.ID);
            Assert.AreEqual(50, responseData.Tasks.Where(f => f.X == 2 && f.Y == 7).FirstOrDefault().PlayerId);
            Assert.AreEqual(PlayerRole.member, responseData.Tasks.Where(f => f.X == 2 && f.Y == 7).FirstOrDefault().Player.Role);

            Assert.AreEqual(60ul, responseData.Goals.Where(f => f.X == 2 && f.Y == 0).FirstOrDefault().Player.ID);
            Assert.AreEqual(60, responseData.Goals.Where(f => f.X == 2 && f.Y == 0).FirstOrDefault().PlayerId);

            Assert.AreEqual(150ul, responseData.Tasks.Where(f => f.X == 3 && f.Y == 5).FirstOrDefault().Piece.ID);
        }
    }
}
