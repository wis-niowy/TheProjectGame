namespace CommunicationServer.ServerObjects
{
    internal interface IAgentController
    {
        void SendMessageToGameMaster(string message);
    }
}