using Messages;

namespace GameArea.ControllerInterfaces
{
    public interface IGMController
    {
        bool SendMessageToAgent(ulong playerId, string message);
        bool SendMessageToClient(ulong clientId, string message);
        void RegisterClientAsAgent(ulong clientId, string message);
        void CloseGame();
        void RejectJoin(string name, ulong clientId);
        void DataSend(string message, ulong clientId);
        void BeginGame();
    }
}