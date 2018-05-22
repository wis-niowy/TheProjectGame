using System;
using System.Collections.Generic;
using System.Text;
using Messages;

namespace GameArea.GameObjects
{
    public class GameInfo : IToBase<Messages.GameInfo>
    {
        public string GameName { get; set; }
        public ulong RedTeamPlayers { get; set; }
        public ulong BlueTeamPlayers { get; set; }

        public GameInfo(Messages.GameInfo gameInfo):this(gameInfo.gameName,gameInfo.redTeamPlayers,gameInfo.blueTeamPlayers) { }

        public GameInfo(string name, ulong red, ulong blue)
        {
            GameName = name;
            RedTeamPlayers = red;
            BlueTeamPlayers = blue;
        }
        public string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public Messages.GameInfo ToBase()
        {
            return new Messages.GameInfo()
            {
                blueTeamPlayers = BlueTeamPlayers,
                gameName = GameName,
                redTeamPlayers = RedTeamPlayers
            };
        }
    }
}
