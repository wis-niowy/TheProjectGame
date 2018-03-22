using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea;
using Messages;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System;

namespace TheProjectGame.Tests
{
    [TestClass]
    public class AgentTests
    {
        [TestMethod]
        public void PlayerWithoutPieceTestsPiece()
        {
            Configuration.GameMasterSettingsGameDefinition conf = new Configuration.GameMasterSettingsGameDefinition()
            {
                //BoardWidth = "5",
                //GoalAreaLength = "2",
                //TaskAreaLength = "4",
                //ShamProbability = 0.5,
                //InitialNumberOfPieces = 5,
                //NumberOfPlayersPerTeam = "4",
                //GameName = "Test Game",
                //PlacingNewPiecesFrequency = 4

            };
            var gameMaster = new GameMaster(conf);
            var agent = new Player.Agent(TeamColour.red, 3);
            agent.SetGuid("testGUID-0000");
            gameMaster.RegisterAgent(agent);

            var testResult = agent.TestPiece(gameMaster);

            Assert.AreEqual(false, testResult);
        }

        
    }
}
