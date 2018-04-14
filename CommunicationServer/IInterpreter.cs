namespace CommunicationServer
{
    public interface IInterpreter
    {
        void ReadMessage(string message, int clientId);
    }
}