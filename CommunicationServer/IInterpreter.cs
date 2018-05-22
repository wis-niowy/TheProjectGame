namespace CommunicationServer
{
    public interface IInterpreter
    {
        void ReadMessage(string message, ulong clientId);
    }
}