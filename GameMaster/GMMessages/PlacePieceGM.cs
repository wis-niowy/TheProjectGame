using GameArea;
using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace GameMaster.GMMessages
{
    public class PlacePieceGM : PlacePieceMessage, IGMMessage
    {
        public bool Prioritised => false;
        public string GUID => PlayerGUID;
        public PlacePieceGM(string guid, ulong gameId) : base(guid, gameId)
        {
        }

        public string[] Process(IGameMaster gameMaster)
        {
            if (gameMaster.GameEndDate > ReceiveDate || gameMaster.GameStartDate > ReceiveDate || gameMaster.IsGameFinished)
            {
                return null;
            }
            var messages = new  string[] { gameMaster.HandlePlacePieceRequest(this)?.Serialize() };
            gameMaster.LockObject();
            try
            {
                if (gameMaster.IsGameFinished && gameMaster.State == GameMasterState.GameResolved)
                {
                    var newGameMessages = gameMaster.RestartGame();
                    messages = messages.Union(newGameMessages).ToArray();
                }
            }
            catch (Exception e)
            { }
            finally
            {
                gameMaster.UnlockOject();
            }
            return messages;
        }
    }
}
