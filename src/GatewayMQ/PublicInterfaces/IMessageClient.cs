namespace Gateway.MQ.Interfaces
{
    public interface IMessageClient
    {
        void SetupQueue(string queueName);
        void PushMessageIntoQueue(byte[] message, string queue);
        byte[] ReadMessageFromQueue(string queueName);
        void PushAuditMessage(byte[] message);
    }
}
