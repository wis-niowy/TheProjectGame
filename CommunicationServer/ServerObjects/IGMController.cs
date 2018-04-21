using Messages;

namespace CommunicationServer.ServerObjects
{
    public interface IGMController
    {
        bool SendMessageToAgent(ulong playerId, string message);
        bool SendMessageToClient(string message);
        void RegisterClientAsAgent(ConfirmJoiningGame message);
        void CloseGame();
        void RejectJoin(RejectJoiningGame messageObject);
        void DataSend(Data messageObject);
        void BeginGame();
    }
}