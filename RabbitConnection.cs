using System;
using RabbitMQ.Client;

namespace RabbitMQSeminar
{
    /// <summary>
    /// Rabbitmq connections are meant to be singletons for each application, because creating and tearing down tcp connections is expensive
    /// for both the client and the host. We achieve concurrency instead by creating virtual channels over a single tcp connection
    /// </summary>
    public class RabbitConnection
    {
        private static readonly Lazy<IConnection> _connection = new Lazy<IConnection>(CreateNewConnection);

        public static IConnection Instance => _connection.Value;

        private static IConnection CreateNewConnection()
        {
            var connectionString = Settings.GetSetting("rabbitmq");
            var connectionUri = new Uri(connectionString);
            var factory = new ConnectionFactory
            {
                TopologyRecoveryEnabled = true,
                Uri = connectionUri
            };
            var tcpConnection = factory.CreateConnection();

            //these events will always trigger for better visibility
            tcpConnection.ConnectionShutdown += (sender, eventArgs) =>
            {
                Console.WriteLine("connection closed");
            };
            tcpConnection.RecoverySucceeded += (sender, eventArgs) =>
            {
                Console.WriteLine("connection recovered");
            };

            return tcpConnection;
        }
    }
}
