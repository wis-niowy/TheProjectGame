namespace CommunicationServer.ServerObjects
{
    public interface IGMController
    {
        bool SendMessageToAgent(uint playerId, string message);

        bool SendMessageToClient(string message);

        void RegisterClientAsAgent(string message, ulong playerId);
    }
}