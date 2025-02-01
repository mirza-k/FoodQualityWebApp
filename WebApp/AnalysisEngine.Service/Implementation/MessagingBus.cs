using AnalysisEngine.Service.Interface;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace AnalysisEngine.Service.Implementation
{
    public class MessagingBus : IMessagingBus
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessagingBus(string hostname)
        {
            var factory = new ConnectionFactory()
            {
                HostName = hostname,
                UserName = "guest",
                Password = "guest"
            };


            // retry logic for estalishing connection with rabbit mq cause
            // in moment of connecting rabbit mq is not yet full ready
            int maxRetries = 10;
            int retryDelaySeconds = 5;

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    _connection = factory.CreateConnection();
                    _channel = _connection.CreateModel();
                    Console.WriteLine("AnalysisEngine: Rabbitmq connected successfully");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"AnalysisEngine: RabbitMQ connection failed, will try again: {ex.Message}");
                    Thread.Sleep(retryDelaySeconds * 1000);
                }
            }
        }

        public void Publish(string queue, string message)
        {
            _channel.QueueDeclare(queue, false, false, false, null);
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish("", queue, null, body);
        }

        public void Subscribe(string queue, Func<string, Task> handleMessage)
        {
            _channel.QueueDeclare(queue, false, false, false, null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                await handleMessage(message);
            };

            _channel.BasicConsume(queue, true, consumer);
        }
    }
}
