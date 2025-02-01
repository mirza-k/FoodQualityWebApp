namespace Services.Interface
{
    public interface IMessagingBus
    {
        void Publish(string queue, string message);
        void Subscribe(string queue, Func<string, Task> handleMessage);
    }
}
