using System;
using System.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ikoLite.Messaging.Services
{
	public class Consumer : IDisposable
	{
		private readonly string _queueName;
		private readonly string _hostName;
		private readonly IConnection _connection;
        private readonly IModel _channel;



        public Consumer(string queueName, string hostName)
		{
			_queueName = queueName;
			_hostName = hostName;
			var factory = new ConnectionFactory()
			{
				HostName = _hostName,
				Port = 5671,
				UserName = "nvyrywte",
				Password = "LfN6kkcHwTenNa5ZG7YSMhx3ymMT2FS4",
				VirtualHost = "nvyrywte",
			};
			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();
		}



		public void Recieve(EventHandler<BasicDeliverEventArgs> recieveCallback)
		{
			_channel.ExchangeDeclare(exchange: "some_exchange", type:"direct");
			_channel.QueueDeclare(queue: _queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
			_channel.QueueBind(queue: _queueName, exchange: "some_exchange", routingKey: _queueName);

			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += recieveCallback;

			_channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
		}


        public void Dispose()
        {
			_connection?.Dispose();
			_channel?.Dispose();
        }
    }
}

