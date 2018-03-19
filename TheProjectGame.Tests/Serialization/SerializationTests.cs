using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameArea;
using Messages;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System;
using GameArea.Serialization;
using System.Xml.Linq;

namespace TheProjectGame.Tests.Serialization
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void SerializeMethodTestForTestPieceMessage()
        {
            var serializerTestPiece = new MessageSerializator<TestPiece>();
            var testPieceMessage = new TestPiece
            {
                gameId = 1,
                playerGuid = "ee4455"
            };

            var serializationResult = serializerTestPiece.Serialize(testPieceMessage);

            XmlSerializer xs = new XmlSerializer(typeof(TestPiece));
            var deserializedResult = xs.Deserialize(new StringReader(serializationResult)) as TestPiece;
            

            Assert.AreEqual(testPieceMessage.gameId, deserializedResult.gameId);
            Assert.AreEqual(testPieceMessage.playerGuid, deserializedResult.playerGuid);
        }


        [TestMethod]
        public void DeserializeMethodTestForTestPieceMessageCheckTypes()
        {
            var serializerTestPiece = new MessageSerializator<TestPiece>();
            var testPieceMessage = new TestPiece
            {
                gameId = 1,
                playerGuid = "ee4455"
            };
            var message = serializerTestPiece.Serialize(testPieceMessage);

            var deserializationResult = serializerTestPiece.Deserialize(message);

            Assert.AreEqual(typeof(TestPiece), deserializationResult.GetType());
        }

        [TestMethod]
        public void DeserializeMethodTestForTestPieceMessageCheckValues()
        {
            var serializerTestPiece = new MessageSerializator<TestPiece>();
            var testPieceMessage = new TestPiece
            {
                gameId = 1,
                playerGuid = "ee4455"
            };
            var message = serializerTestPiece.Serialize(testPieceMessage);

            var deserializationResult = serializerTestPiece.Deserialize(message);

            Assert.AreEqual(testPieceMessage.gameId, deserializationResult.gameId);
            Assert.AreEqual(testPieceMessage.playerGuid, deserializationResult.playerGuid);
        }

        [TestMethod]
        public void DeserializeMethodTestForTestPieceMessageIncorrectType()
        {
            var serializerTestPiece = new MessageSerializator<GameMessage>();
            var testPieceMessage = new TestPiece
            {
                gameId = 1,
                playerGuid = "ee4455"
            };
            var message = serializerTestPiece.Serialize(testPieceMessage);

            var deserializationResult = serializerTestPiece.Deserialize(message);

            Assert.IsNull(deserializationResult);
        }

        //[TestMethod]
        //public void SerializerTest()
        //{
        //    var message = new TestPiece();
        //    var serializer = new XmlSerializer(typeof(TestPiece));
        //    var xml = "";
        //    using (var streamWriter = new StringWriter())
        //    {
        //        using (XmlWriter writer = XmlWriter.Create(streamWriter))
        //        {
        //            serializer.Serialize(writer, message);
        //            xml = streamWriter.ToString();
        //        }
        //    }
        //    var streamReader = new StringReader(xml);
        //    var xmlReader = XmlReader.Create(streamReader);
        //    var wrongSerializer = new XmlSerializer(typeof(GameMessage));
        //    Object deserializedMessage;
        //    if (wrongSerializer.CanDeserialize(xmlReader))
        //    {
        //        deserializedMessage = wrongSerializer.Deserialize(xmlReader);
        //        MessageReader((GameMessage)deserializedMessage);
        //    }
        //    else
        //    {
        //        deserializedMessage = serializer.Deserialize(xmlReader);
        //        MessageReader((TestPiece)deserializedMessage);
        //    }
        //}

        //private void MessageReader(GameMessage message)
        //{
        //    var w = "";
        //}

        //private void MessageReader(TestPiece message)
        //{
        //    var w = "";
        //}
    }
}
