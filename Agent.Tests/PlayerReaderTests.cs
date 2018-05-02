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

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(RegisteredGamesAgent));
        }

        [TestMethod]
        public void ConfirmJoiningGameTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<ConfirmJoiningGame xmlns = \"https://se2.mini.pw.edu.pl/17-results/\">\n" +
                                "gameId = \"1\"\n" +
                                "playerId = \"2\"\n" +
                                "privateGuid = \"c094cab7-da7b-457f-89e5-a5c51756035f\" >\n" +
                            "<PlayerDefinition id = \"2\" team = \"blue\" role = \"member\" />\n" +
                         "</ConfirmJoiningGame>";

            var obj = PlayerReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(ConfirmJoiningGameAgent));
        }

        [TestMethod]
        public void RejectJoiningGameTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<RejectJoiningGame xmlns = \"https://se2.mini.pw.edu.pl/17-results/\">\n" +
                                "gameName = \"Easy game\"\n" +
                                "playerId = \"2\"\n" +
                         "</RejectJoiningGame>";

            var obj = PlayerReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(RejectJoiningGameAgent));
        }

        [TestMethod]
        public void GameMessageTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<Game xmlns = \"https://se2.mini.pw.edu.pl/17-results/\">\n" +
                                "playerId = \"2\"\n" +
                            "<Players>\n" +
                                "<Player team = \"red\" role = \"leader\" id = \"5\" />\n" +
                                "<Player team = \"red\" role = \"member\" id = \"6\" />\n" +
                                "<Player team = \"red\" role = \"member\" id = \"7\" />\n" +
                                "<Player team = \"red\" role = \"member\" id = \"8\" />\n" +
                                "<Player team = \"blue\" role = \"leader\" id = \"1\" />\n" +
                                "<Player team = \"blue\" role = \"member\" id = \"2\" />\n" +
                                "<Player team = \"blue\" role = \"member\" id = \"3\" />\n" +
                                "<Player team = \"blue\" role = \"member\" id = \"4\" />\n" +
                            "</Players>\n" +
                            "<Board width = \"5\" tasksHeight = \"5\" goalsHeight = \"3\" />\n" +
                            "<PlayerLocation x = \"0\" y = \"3\" />\n" +
                        "</Game>";

            var obj = PlayerReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(GameAgent));
        }

        [TestMethod]
        public void DataMessageTest()
        {

            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<Data xmlns = \"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                                "playerId = \"1\"\n" +
                                "gameFinished=\"false\">\n" +
                            "<TaskFields>\n" +
                                "<TaskField x=\"1\" y=\"5\" timestamp=\"2017-02-23T17:20:11\"\n" +
                                    "distanceToPiece=\"5\" />\n" +
                            "</TaskFields>\n" +
                            "<PlayerLocation x=\"1\" y=\"5\" />\n" +
                         "</Data>";

            var obj = PlayerReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(DataAgent));
        }

        [TestMethod]
        public void InvalidXML()
        {

            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                         "<Data xmlns = \"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                                "playerId = \"1\"\n" +
                                "gameFinished=\"false\">\n" +
                            "<TaskFields>\n" +
                                "<TaskField x=\"1\" y=\"5\" timestamp=\"2017-02-23T17:20:11\"\n" +
                                    "distanceToPiece=\"5\" />\n" +
                            "</TaskFields>\n" +
                            "<PlayerLocation x=\"1\" y=\"5\" />\n" +
                         "</Data";

            var obj = PlayerReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(ErrorMessageAgent));
        }
    }
}
