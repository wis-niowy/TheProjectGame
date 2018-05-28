using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GameArea;
using GameArea.AppConfiguration;
using GameArea.AppMessages;
using Messages;

namespace Player.Tests
{
    public class PlayerControllerMock : IPlayerController
    {
        public IPlayer Player { get; set; }
        public AgentState State { get; set; }
        public ActionType ActionToComplete { get; set; }

        private List<GameArea.GameObjects.GameInfo> GamesList { get; set; }
        TeamColour PrefferedColor { get; set; }
        PlayerRole PrefferedRole { get; set; }
        public PlayerSettingsConfiguration Settings { get; set; }
        public string GameName { get; set; }

        public PlayerControllerMock()
        {

        }

        public void BeginRead()
        {

        }

        public void BeginSend(string message)
        {
            if (Player.LastActionTaken == ActionType.Move)
            {
                (Player as Player).Location = (Player as Player).CalculateFutureLocation((Player as Player).Location, (Player as Player).LastMoveTaken.Value);
            }
        }

        public bool ConnectToServer(IPAddress ip, int port)
        {
            return true;
        }

        public void EndRead(IAsyncResult result)
        {

        }

        public void EndSend(IAsyncResult result)
        {

        }

        public void StartPerformance()
        {

        }

        public void RegisteredGames(RegisteredGamesMessage messageObject)
        {
            GamesList = messageObject.Games?.ToList();
            State = AgentState.Joining;
            ActionToComplete = ActionType.none;
        }

        public void ConfirmJoiningGame(ConfirmJoiningGameMessage info)
        {
            var gameId = info.GameId;
            var id = info.PlayerId; //u nas serwerowe ID i playerId na planszy to jedno i to samo
            var guid = info.GUID;
            var team = info.PlayerDefinition.Team;
            Player = info.PlayerDefinition.Role == PlayerRole.leader ? new Leader(team, PlayerRole.leader, Settings, this, guid) : new Player(team, PlayerRole.leader, Settings, this, guid);
            State = AgentState.AwaitingForStart;
            ActionToComplete = ActionType.none;
        }
    }
}
