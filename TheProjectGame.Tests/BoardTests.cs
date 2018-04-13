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
            var board = new Board(5, 5, 3);
            Assert.AreEqual(5, board.BoardWidth);
        }

        [TestMethod]
        public void BoardPieceAreaHeight()
        {
            var board = new Board(5, 5, 3);
            Assert.AreEqual(5, board.TaskAreaHeight);
        }

        [TestMethod]
        public void BoardGoalAreaHeight()
        {
            var board = new Board(5, 5, 3);
            Assert.AreEqual(3, board.GoalAreaHeight);
        }

        [TestMethod]
        public void BoardHeight()
        {
            var board = new Board(5, 5, 3);
            Assert.AreEqual(5 + 2 * 3, board.BoardHeight);
        }

        [TestMethod]
        public void BoardGetValidField()
        {
            var board = new Board(5, 5, 3);
            var field = board.GetField(0, 0);
            Assert.IsNotNull(field);
        }

        [TestMethod]
        public void BoardGetFieldYBiggerThanIndex()
        {
            var board = new Board(5, 5, 3);
            var field = board.GetField(0, board.BoardHeight);
            Assert.IsNull(field);
        }

        [TestMethod]
        public void BoardGetFieldXBiggerThanIndex()
        {
            var board = new Board(5, 5, 3);
            var field = board.GetField(board.BoardWidth, 0);
            Assert.IsNull(field);
        }

        [TestMethod]
        public void BoardGoalAreaVerify()
        {
            var board = new Board(5, 5, 3);
            for (int i = 0; i < board.GoalAreaHeight; i++)
            {
                for (int j = 0; j < board.BoardWidth; j++)
                {
                    Assert.AreEqual(typeof(GameArea.GoalField), board.GetField(j, i).GetType());
                    Assert.AreEqual(TeamColour.blue, ((GameArea.GoalField)board.GetField(j, i)).GetOwner);
                    Assert.AreEqual(typeof(GameArea.GoalField), board.GetField(j, i + board.TaskAreaHeight + board.GoalAreaHeight).GetType());
                    Assert.AreEqual(TeamColour.red, ((GameArea.GoalField)board.GetField(j, i + board.TaskAreaHeight + board.GoalAreaHeight)).GetOwner);
                }
            }

            for (int i = board.GoalAreaHeight; i < board.BoardHeight - board.GoalAreaHeight; i++)
            {
                for (int j = 0; j < board.BoardWidth; j++)
                {
                    Assert.AreEqual(typeof(GameArea.TaskField), board.GetField(j, i).GetType());
                    Assert.IsNull(((GameArea.TaskField)board.GetField(j, i)).GetPiece);
                }
            }
        }

        [TestMethod]
        public void BoardValidTaskField()
        {
            var board = new Board(5, 5, 3);
            var field = board.GetTaskField(4, 4);
            Assert.IsNotNull(field);
        }

        [TestMethod]
        public void BoardInvalidYTaskField()
        {
            var board = new Board(5, 5, 3);
            var field = board.GetTaskField(4, 1);
            Assert.IsNull(field);
            field = board.GetTaskField(4, 9);
            Assert.IsNull(field);
        }

        [TestMethod]
        public void BoardInvalidXTaskField()
        {
            var board = new Board(5, 5, 3);
            var field = board.GetTaskField(6, 4);
            Assert.IsNull(field);
        }

        [TestMethod]
        public void BoardValidGoalField()
        {
            var board = new Board(5, 5, 3);
            var field = board.GetGoalField(1, 1);
            Assert.IsNotNull(field);
        }

        [TestMethod]
        public void BoardInvalidXGoalField()
        {
            var board = new Board(5, 5, 3);
            var field = board.GetGoalField(5, 4);
            Assert.IsNull(field);
        }

        [TestMethod]
        public void BoardInvalidYGoalField()
        {
            var board = new Board(5, 5, 3);
            var field = board.GetGoalField(1, 6);
            Assert.IsNull(field);
        }

        [TestMethod]
        public void GetTaskFields()
        {
            var board = new Board(5, 5, 3);
            var taskFields = board.TaskFields;
            Assert.AreEqual(board.TaskAreaHeight * board.BoardWidth, taskFields.Count);
            foreach (var field in taskFields)
            {
                Assert.AreEqual(typeof(GameArea.TaskField), field.GetType());
            }
        }

        [TestMethod]
        public void GetBlueTeamGoalAreaFields()
        {
            var board = new Board(5, 5, 3);
            var blueGoalAreaFields = board.GetBlueGoalAreaFields;
            Assert.IsNotNull(blueGoalAreaFields);
            Assert.AreEqual(board.BoardWidth * board.GoalAreaHeight, blueGoalAreaFields.Count);
            foreach(var field in blueGoalAreaFields)
            {
                Assert.AreEqual(FieldType.Goal, field.GetFieldType);
                Assert.AreEqual(TeamColour.blue, field.GetOwner);
            }
        }

        [TestMethod]
        public void GetRedTeamGoalAreaFields()
        {
            var board = new Board(5, 5, 3);
            var redGoalAreaFields = board.GetRedGoalAreaFields;
            Assert.IsNotNull(redGoalAreaFields);
            Assert.AreEqual(board.BoardWidth * board.GoalAreaHeight, redGoalAreaFields.Count);
            foreach (var field in redGoalAreaFields)
            {
                Assert.AreEqual(FieldType.Goal, field.GetFieldType);
                Assert.AreEqual(TeamColour.red, field.GetOwner);
            }
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
