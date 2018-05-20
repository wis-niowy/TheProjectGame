using Messages;
using System;
using System.Collections.Generic;
using System.Text;
using GameArea;
using GameArea.AppMessages;
using System.Linq;

namespace Player
{
    public class Leader : Player
    {
        public Leader(TeamColour team,PlayerRole role, GameArea.AppConfiguration.PlayerSettingsConfiguration settings = null,
                                                                IPlayerController controller = null, string guid = "TEST_GUID", ulong id = 0) : base(team, role, settings, controller, guid, null, id)
        {
        }

        public override DataMessage HandleKnowledgeExchangeRequest(KnowledgeExchangeRequestMessage messageObject)
        {
            // Leader zawsze odpowiada, jezeli request jest z jego druzyny
            
            DataMessage responseData = new DataMessage(messageObject.SenderPlayerId)
            {
                Goals = GetBoard.GetRedGoalAreaFields.Union(GetBoard.GetBlueGoalAreaFields).Select(f => new GameArea.GameObjects.GoalField(f)).ToArray(),
                Tasks = GetBoard.TaskFields.Select(q => new GameArea.GameObjects.TaskField(q)).ToArray()
            };
            var xCoord = Location.X;
            var yCoord = Location.Y;

            // do Data musi też dodać, na Field na ktorym stoi, swoj stan !!!
            if (GetBoard.GetField(xCoord, yCoord) is GameArea.GameObjects.GoalField)
            {
                var field = responseData.Goals.Where(f => f.X == xCoord && f.Y == yCoord).FirstOrDefault();
                field.Player = new GameArea.GameObjects.Player(this.ID, this.Team, this.Role);
                field.TimeStamp = DateTime.Now;
                field.PlayerId = (long)this.ID;
            }
            else // is TaskField
            {
                var field = responseData.Tasks.Where(f => f.X == xCoord && f.Y == yCoord).FirstOrDefault();
                field.Player = new GameArea.GameObjects.Player(this.ID, this.Team, this.Role);
                if (this.HasPiece)
                    field.Piece = new GameArea.GameObjects.Piece(this.GetPiece.ID, this.GetPiece.TimeStamp, this.GetPiece.Type, this.GetPiece.PlayerId);
                field.TimeStamp = DateTime.Now;
                field.PlayerId = (long)this.ID;
            }

            return responseData;
        }

        public override void HandleRejectKnowledgeExchange(RejectKnowledgeExchangeMessage messageObject)
        {
            // poki co - Player olewa
        }
    }
}
