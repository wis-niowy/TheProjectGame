using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameMaster.GMMessages;

namespace GameMaster.Tests
{
    [TestClass]
    public class GameMasterReaderTests
    {
        [TestMethod]
        public void AcceptExchangeRequestGMTest()
        {

            string xml = "<?xml version = \"1.0\" encoding = \"utf-8\" ?>\n" +
                           "<AcceptExchangeRequest xmlns = \"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                            "playerId = \"1\"\n" +
                            "senderPlayerId = \"2\"\n" +
                            "/>";

            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(AcceptExchangeRequestGM));
        }

        [TestMethod]
        public void AuthorizeKnowledgeExchangeGMGMTest()
        {
            string xml = "<?xml version = \"1.0\" encoding = \"utf-8\"?>\n" +
                          "<AuthorizeKnowledgeExchange xmlns = \"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                          "withPlayerId = \"2\"\n" +
                          "gameId = \"1\"\n" +
                          "playerGuid = \"c1797885-8773-43ea-b454-d78315341a02\"\n" +
                          "/>";

            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(AuthorizeKnowledgeExchangeGM));
        }

        [TestMethod]
        public void ConfirmGameRegistrationGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf - 8\"?>\n" +
                            "<ConfirmGameRegistration xmlns = \"https://se2.mini.pw.edu.pl/17-results/\" gameId = \"1\"/>";

            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(ConfirmGameRegistrationGM));
        }

        [TestMethod]
        public void DataGMTest()
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

            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(DataGM));
        }

        [TestMethod]
        public void DestroyPieceGMTest()
        {
            string xml = "<?xml version = \"1.0\" encoding = \"utf-8\"?>\n" +
                           "<DestroyPiece xmlns = \"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                              "gameId = \"1\"\n" +
                              "playerGuid = \"c094cab7-da7b-457f-89e5-a5c51756035f\" />";


            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(DestroyPieceGM));
        }

        [TestMethod]
        public void DiscoverGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                            "<Discover\n" +
                            "gameId=\"1\"\n" +
                            "playerGuid=\"c094cab7-da7b-457f-89e5-a5c51756035f\"\n" +
                            "xmlns=\"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                               "/>";


            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(DiscoverGM));
        }

        [TestMethod]
        public void ErrorMessageGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                            "<Discover\n" +
                            "gameId=\"1\"\n" +
                            "playerGuid=\"c094cab7-da7b-457f-89e5-a5c51756035f\"\n" +
                            "xmlns=\"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                               "/";


            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(ErrorMessageGM));
        }

        [TestMethod]
        public void JoinGameGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                        "<JoinGame\n" +
                          "xmlns=\"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                          "gameName=\"Easy game\"\n" +
                          "preferredRole=\"leader\"\n" +
                          "preferredTeam=\"blue\"/>";


            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(JoinGameGM));
        }

        [TestMethod]
        public void MoveGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                        "<Move xmlns=\"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                          "gameId=\"1\"\n" +
                          "playerGuid=\"c094cab7-da7b-457f-89e5-a5c51756035f\"\n" +
                          "direction=\"up\"/>";


            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(MoveGM));
        }

        [TestMethod]
        public void PickUpPieceGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                            "<PickUpPiece\n" +
                            "xmlns=\"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                            "gameId=\"1\"\n" +
                            "playerGuid=\"c094cab7-da7b-457f-89e5-a5c51756035f\"/>";


            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(PickUpPieceGM));
        }

        [TestMethod]
        public void PlacePieceGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                           "<PlacePiece xmlns=\"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                          "gameId=\"1\"\n" +
                          "playerGuid=\"c094cab7-da7b-457f-89e5-a5c51756035f\"/>\n";


            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(PlacePieceGM));
        }

        [TestMethod]
        public void PlayerDisconnectedGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n" +
                            "<PlayerDisconnected playerId=\"34\" xmlns=\"https://se2.mini.pw.edu.pl/17-results/\" />";


            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(PlayerDisconnectedGM));
        }

        [TestMethod]
        public void RejectGameRegistrationGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                        "<RejectGameRegistration xmlns=\"https://se2.mini.pw.edu.pl/17-results/\" gameName=\"easy clone\"/>";
            
            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(RejectGameRegistrationGM));
        }

        [TestMethod]
        public void RejectKnowledgeExchangeGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                        "<RejectKnowledgeExchange xmlns=\"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                          "permanent=\"false\"\n" +
                          "playerId=\"1\"\n" +
                          "senderPlayerId=\"2\"\n" +
                          "playerGuid=\"c094cab7-da7b-457f-89e5-a5c51756035f\"\n" +
                          "/>";

            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(RejectKnowledgeExchangeGM));
        }

        [TestMethod]
        public void SuggestActionGMTest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                        "<SuggestAction xmlns=\"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                          "senderPlayerId=\"1\"\n" +
                          "playerId=\"2\"\n" +
                          "gameId=\"1\"\n" +
                          "playerGuid=\"c1797885-8773-43ea-b454-d78315341a02\">\n" +
                          "<TaskFields>\n" +
                            "<TaskField x=\"1\" y=\"2\" distanceToPiece=\"-1\" timestamp=\"2018-01-02T23:59:56\"/>\n" +
                            "<TaskField x=\"3\" y=\"5\" distanceToPiece=\"-1\" timestamp=\"2018-01-02T23:59:56\"/>]n"+
                          "</TaskFields>\n" +
                          "<GoalFields>\n" +
                            "<GoalField x=\"0\" y=\"0\" type=\"unknown\" team=\"blue\" timestamp=\"2018-01-02T23:59:56\"/>\n" +
                            "<GoalField x=\"1\" y=\"0\" type=\"unknown\" team=\"blue\" timestamp=\"2018-01-02T23:59:56\"/>\n"+
                          "</GoalFields>\n" +
                        "</SuggestAction>";

            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(SuggestActionGM));
        }

        [TestMethod]
        public void TestPieceGMest()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                          "<TestPiece xmlns=\"https://se2.mini.pw.edu.pl/17-results/\"\n" +
                          "gameId=\"1\"\n" +
                          "playerGuid=\"c094cab7-da7b-457f-89e5-a5c51756035f\"/>";

            var obj = GMReader.GetObjectFromXML(xml);

            Assert.IsNotNull(obj);
            Assert.IsTrue(obj.GetType() == typeof(TestPieceGM));
        }
    }
}
