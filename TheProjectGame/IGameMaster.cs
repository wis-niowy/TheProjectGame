using GameArea.AppConfiguration;
using GameArea.AppMessages;
using Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameArea
{
    public enum GameMasterState {RegisteringGame, AwaitingPlayers, GameInprogress, GameOver, GameResolved };
    public interface IGameMaster
    {
        void LockObject();
        void UnlockOject();
        GameMasterState State { get; set; }
        bool IsGameFinished { get; }
        GameMasterSettingsConfiguration Settings { get; set; }
        DataMessage HandleTestPieceRequest(TestPieceMessage msg);
        DataMessage HandlePlacePieceRequest(PlacePieceMessage msg);
        DataMessage HandlePickUpPieceRequest(PickUpPieceMessage msg);
        DataMessage HandleMoveRequest(MoveMessage msg);
        DataMessage HandleDiscoverRequest(DiscoverMessage msg);
        DataMessage HandleDestroyPieceRequest(DestroyPieceMessage msg);
        void HandleConfirmGameRegistration(ConfirmGameRegistrationMessage msg);
        string[] HandleJoinGameRequest(JoinGameMessage msg);
        void HandlePlayerDisconnectedRequest(PlayerDisconnectedMessage playerDisconnected);
        RegisterGameMessage RegisterGame();
        string[] RestartGame();
        BetweenPlayersAbstractMessage HandleAuthorizeKnowledgeExchange(AuthorizeKnowledgeExchangeMessage msg);
        RejectKnowledgeExchangeMessage HandleRejectKnowledgeExchange(RejectKnowledgeExchangeMessage msg);
        //AcceptExchangeRequestMessage HandleAcceptKnowledgeExchange(AcceptExchangeRequestMessage msg);
        SuggestActionMessage HandleSuggestAction(SuggestActionMessage msg);
        SuggestActionResponseMessage HandleSuggestActionResponse(SuggestActionResponseMessage msg);
        string[] HandleData(DataMessage data);
        DateTime GameEndDate { get; set; }
        DateTime GameStartDate { get; set; }

        void HandlerErrorMessage(AppMessages.ErrorMessage error);
    }
}
