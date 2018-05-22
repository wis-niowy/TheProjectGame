using Messages;

namespace GameArea.ControllerInterfaces
{
    public interface IAgentController
    {
        void SendMessageToGameMaster(string message);
        void RemoveClientOrAgent(ulong clientId);
        void SendKeepAlive(ulong clientId);
    }
}