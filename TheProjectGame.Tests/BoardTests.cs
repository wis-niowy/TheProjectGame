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
    public class BoardTests
    {
        [TestMethod]
        public void BoardWidth()
        {
            var board = new Board(5u, 5u, 3u);
            Assert.AreEqual(5u, board.BoardWidth);
        }

        [TestMethod]
        public void BoardPieceAreaHeight()
        {
            var board = new Board(5u, 5u, 3u);
            Assert.AreEqual(5u, board.PieceAreaHeight);
        }

        [TestMethod]
        public void BoardGoalAreaHeight()
        {
            var board = new Board(5u, 5u, 3u);
            Assert.AreEqual(3u, board.GoalAreaHeight);
        }

        [TestMethod]
        public void BoardHeight()
        {
            var board = new Board(5u, 5u, 3u);
            Assert.AreEqual(5u + 2u * 3u, board.BoardHeight);
        }

        [TestMethod]
        public void SerializerTest()
        {
            var message = new TestPiece();
            var serializer = new XmlSerializer(typeof(TestPiece));
            var xml = "";
            using (var streamWriter = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(streamWriter))
                {
                    serializer.Serialize(writer, message);
                    xml = streamWriter.ToString();
                }
            }
            var streamReader = new StringReader(xml);
            var xmlReader = XmlReader.Create(streamReader);
            var wrongSerializer = new XmlSerializer(typeof(GameMessage));
            Object deserializedMessage;
            if (wrongSerializer.CanDeserialize(xmlReader))
            {
                deserializedMessage = wrongSerializer.Deserialize(xmlReader);
                MessageReader((GameMessage)deserializedMessage);
            }
            else
            {
                deserializedMessage = serializer.Deserialize(xmlReader);
                MessageReader((TestPiece)deserializedMessage);
            }
        }

        private void MessageReader(GameMessage message)
        {
            var w = "";
        }

        private void MessageReader(TestPiece message)
        {
            var w = "";
        }
    }
}
