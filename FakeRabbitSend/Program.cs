using System;
using System.Text;
using RabbitMQ.Client;

namespace FakeRabbitSend
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                try
                {
                    channel.QueueDeclare(queue: "moneyOperations", durable: false, exclusive: false, autoDelete: false, arguments: null);
                }
                catch { }

                string message = "Hello World!1";
                string message2 = "Hello World!2";
                string message3 = "Hello World!3";
                var body = Encoding.UTF8.GetBytes(message);
                var body2 = Encoding.UTF8.GetBytes(message2);
                var body3 = Encoding.UTF8.GetBytes(message3);

                channel.BasicPublish(exchange: "", routingKey: "moneyOperations", basicProperties: null, body: body);
                channel.BasicPublish(exchange: "", routingKey: "moneyOperations", basicProperties: null, body: body2);
                channel.BasicPublish(exchange: "", routingKey: "moneyOperations", basicProperties: null, body: body3);
                Console.WriteLine(" [x] Sent {0}", message);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
