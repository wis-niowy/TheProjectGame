using Messages;

namespace CommunicationServer.ServerObjects
{
    internal interface IAgentController
    {
        void SendMessageToGameMaster(string message);
        void RemoveClientOrAgent(ulong clientId);
    }
}