using Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace GameMaster.Tests
{
    [TestClass]
    public class GameMasterTests
    {
        GameMasterSettingsGameDefinition defaultSettings = new GameMasterSettingsGameDefinition();
        GameMaster defaultGameMaster = new GameMaster(new GameMasterSettingsGameDefinition());
        [TestMethod]
        public void InitBoard()
        {
            var taskFields = defaultGameMaster.Board.TaskFields;
            Assert.AreEqual(defaultSettings.InitialNumberOfPieces, (uint)taskFields.Where(q => q.GetPiece != null).Count());
            var ids = new List<ulong>();
            foreach (var piece in taskFields.Where(q=>q.GetPiece!= null).Select(q=> q.GetPiece))
            {
                Assert.IsFalse(ids.Contains(piece.id));
                ids.Add(piece.id);
            }
        }

        [TestMethod]
        public void InitAgentsList()
        {
            Assert.IsNotNull(defaultGameMaster.GetAgents);
            Assert.AreEqual(0, defaultGameMaster.GetAgents.Count);
        }

        [TestMethod]
        public void GameMasterAfterInitState()
        {
            Assert.AreEqual(GameMasterState.AwaitingPlayers, defaultGameMaster.GetState);
        }
    }
}
