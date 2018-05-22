using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Messages;

namespace GameArea.AppMessages
{
    public class GameMessage : PlayerMessage, IToBase<Game>
    {
        public GameObjects.Player[] Players { get; set; }
        public GameObjects.GameBoard Board { get; set; }
        public GameObjects.Location PlayerLocation { get; set; }

        public GameMessage(ulong playerId ):base(playerId) {}
        public GameMessage(Game game):base(game)
        {
            Players = game.Players?.Select(q => new GameObjects.Player(q)).ToArray();
            Board = new GameObjects.GameBoard((int)game.Board.width, (int)game.Board.tasksHeight, (int)game.Board.goalsHeight);
            PlayerLocation = new GameObjects.Location(game.PlayerLocation);
        }
        public override string Serialize()
        {
            return MessageParser.Serialize(ToBase());
        }

        public Game ToBase()
        {
            return new Game()
            {
                Board = Board.ToBase(),
                playerId = PlayerId,
                PlayerLocation = PlayerLocation.ToBase(),
                Players = Players.Select(q => q.ToBase()).ToArray()
            };
        }
    }
}
