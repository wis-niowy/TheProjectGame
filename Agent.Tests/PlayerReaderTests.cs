using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea;
using Messages;
using Configuration;
using Player.PlayerMessages;

namespace Player.Tests
{
    [TestClass]
    public class PlayerReaderTests
    {
        [TestMethod]
        public void RegisteredGamesTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<RegisteredGames xmlns = \"https://se2.mini.pw.edu.pl/17-results/\">\n" +
                            "<!-- Numbers of players indicate how many slots are left for each team -->\n" +
                            "<GameInfo gameName = \"Easy game\" blueTeamPlayers = \"2\" redTeamPlayers = \"2\" />\n" +
                            "<GameInfo gameName = \"Hard for blue game\" blueTeamPlayers = \"5\" redTeamPlayers = \"10\" />\n" +
                         "</RegisteredGames>";

            var obj = PlayerReader.GetObjectFromXML(xml);
            var msg = obj as RegisteredGamesAgent;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(RegisteredGamesAgent));
            Assert.AreEqual(2, msg.Games.Length);
            Assert.AreEqual("Easy game", msg.Games[0].GameName);
            Assert.AreEqual(2ul, msg.Games[0].BlueTeamPlayers);
            Assert.AreEqual(2ul, msg.Games[0].RedTeamPlayers);
            Assert.AreEqual("Hard for blue game", msg.Games[1].GameName);
            Assert.AreEqual(5ul, msg.Games[1].BlueTeamPlayers);
            Assert.AreEqual(10ul, msg.Games[1].RedTeamPlayers);
        }

        [TestMethod]
        public void ConfirmJoiningGameTest()
        {

            string xml = "<?xml version = \"1.0\" encoding = \"utf-8\"?>" +
                           "<ConfirmJoiningGame xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" " + 
                           "xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\" " + 
                           "playerId = \"2\" " + 
                           "gameId = \"1\" " +
                           "privateGuid = \"c094cab7-da7b-457f-89e5-a5c51756035f\" " + 
                           "xmlns = \"https://se2.mini.pw.edu.pl/17-results/\" >" + 
                           "<PlayerDefinition team = \"blue\" role = \"member\" id = \"2\" /></ConfirmJoiningGame >";


            var obj = PlayerReader.GetObjectFromXML(xml);
            var msg = obj as ConfirmJoiningGameAgent;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(ConfirmJoiningGameAgent));
            Assert.AreEqual(1ul, msg.GameId);
            Assert.AreEqual(2ul, msg.PlayerId);
            Assert.AreEqual("c094cab7-da7b-457f-89e5-a5c51756035f", msg.GUID);
            Assert.AreEqual(2ul, msg.PlayerDefinition.ID);
            Assert.AreEqual(TeamColour.blue, msg.PlayerDefinition.Team);
            Assert.AreEqual(PlayerRole.member, msg.PlayerDefinition.Role);
        }

        [TestMethod]
        public void RejectJoiningGameTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                         "<RejectJoiningGame xmlns = \"https://se2.mini.pw.edu.pl/17-results/\" " +
                            "xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" " +
                            "xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\" " +
                                "gameName = \"Easy game\" " +
                                "playerId = \"2\" >" +
                         "</RejectJoiningGame>";

            var obj = PlayerReader.GetObjectFromXML(xml);
            var msg = obj as RejectJoiningGameAgent;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(RejectJoiningGameAgent));
            Assert.AreEqual("Easy game", msg.GameName);
            Assert.AreEqual(2ul, msg.PlayerId);
        }

        [TestMethod]
        public void GameMessageTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<Game xmlns = \"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                                "xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\"\n" +
                                "xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\"\n" +
                                "playerId = \"2\">\n" +
                            "<Players>\n" +
                                "<Player team = \"red\" role = \"leader\" id = \"5\" />\n" +
                                "<Player team = \"red\" role = \"member\" id = \"6\" />\n" +
                                "<Player team = \"red\" role = \"member\" id = \"7\" />\n" +
                                "<Player team = \"blue\" role = \"leader\" id = \"1\" />\n" +
                                "<Player team = \"blue\" role = \"member\" id = \"2\" />\n" +
                                "<Player team = \"blue\" role = \"member\" id = \"3\" />\n" +
                            "</Players>\n" +
                            "<Board width = \"5\" tasksHeight = \"5\" goalsHeight = \"3\" />\n" +
                            "<PlayerLocation x = \"0\" y = \"3\" />\n" +
                        "</Game>";

            var obj = PlayerReader.GetObjectFromXML(xml);
            var msg = obj as GameAgent;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(GameAgent));
            Assert.AreEqual(2ul, msg.PlayerId);
            Assert.AreEqual(6, msg.Players.Length);

            Assert.AreEqual(TeamColour.red, msg.Players[0].Team);
            Assert.AreEqual(PlayerRole.leader, msg.Players[0].Role);
            Assert.AreEqual(5ul, msg.Players[0].ID);
            Assert.AreEqual(TeamColour.red, msg.Players[1].Team);
            Assert.AreEqual(PlayerRole.member, msg.Players[1].Role);
            Assert.AreEqual(6ul, msg.Players[1].ID);
            Assert.AreEqual(TeamColour.red, msg.Players[2].Team);
            Assert.AreEqual(PlayerRole.member, msg.Players[2].Role);
            Assert.AreEqual(7ul, msg.Players[2].ID);

            Assert.AreEqual(TeamColour.blue, msg.Players[3].Team);
            Assert.AreEqual(PlayerRole.leader, msg.Players[3].Role);
            Assert.AreEqual(1ul, msg.Players[3].ID);
            Assert.AreEqual(TeamColour.blue, msg.Players[4].Team);
            Assert.AreEqual(PlayerRole.member, msg.Players[4].Role);
            Assert.AreEqual(2ul, msg.Players[4].ID);
            Assert.AreEqual(TeamColour.blue, msg.Players[5].Team);
            Assert.AreEqual(PlayerRole.member, msg.Players[5].Role);
            Assert.AreEqual(3ul, msg.Players[5].ID);

            Assert.AreEqual(5, msg.Board.Width);
            Assert.AreEqual(5, msg.Board.TaskAreaHeight);
            Assert.AreEqual(3, msg.Board.GoalAreaHeight);
            Assert.AreEqual(0, msg.PlayerLocation.X);
            Assert.AreEqual(3, msg.PlayerLocation.Y);
        }

        [TestMethod]
        public void DataMessageTest()
        {

            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<Data xmlns = \"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                                "xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\"\n" +
                                "xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\"\n" +
                                "playerId = \"1\"\n" +
                                "gameFinished=\"false\">\n" +
                            "<TaskFields>\n" +
                                "<TaskField x=\"1\" y=\"5\" timestamp=\"2017-02-23T17:20:11\"\n" +
                                    "distanceToPiece=\"5\" />\n" +
                            "</TaskFields>\n" +
                            "<PlayerLocation x=\"1\" y=\"5\" />\n" +
                         "</Data>";

            var obj = PlayerReader.GetObjectFromXML(xml);
            var msg = obj as DataAgent;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(DataAgent));
            Assert.AreEqual(1ul, msg.PlayerId);
            Assert.IsFalse(msg.GameFinished);
            Assert.AreEqual(1, msg.PlayerLocation.X);
            Assert.AreEqual(5, msg.PlayerLocation.Y);
            Assert.AreEqual(1, msg.Tasks.Length);
            Assert.AreEqual(1, msg.Tasks[0].X);
            Assert.AreEqual(5, msg.Tasks[0].Y);
            Assert.AreEqual(5, msg.Tasks[0].DistanceToPiece);
        }

        [TestMethod]
        public void InvalidXML()
        {

            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<Data xmlns = \"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                                "xmlns:xsi = \"http://www.w3.org/2001/XMLSchema-instance\" " +
                                "xmlns:xsd = \"http://www.w3.org/2001/XMLSchema\" " +
                                "playerId = \"1\"\n" +
                                "gameFinished=\"false\">\n" +
                            "<TaskFields>\n" +
                                "<TaskField x=\"1\" y=\"5\" timestamp=\"2017-02-23T17:20:11\"\n" +
                                    "distanceToPiece=\"5\" />\n" +
                            "</TaskFields>\n" +
                            "<PlayerLocation x=\"1\" y=\"5\" />\n" +
                         "</Data";

            var obj = PlayerReader.GetObjectFromXML(xml);
            var msg = obj as ErrorMessageAgent;

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(ErrorMessageAgent));
        }
    }
}
