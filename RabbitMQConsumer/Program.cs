using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Data.SqlClient;

namespace RabbitMQConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "moneyOperations", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    var connetionString = "Server=(localdb)\\mssqllocaldb;Database=moneydb;Trusted_Connection=True;";
                    using (SqlConnection cnn = new SqlConnection(connetionString))
                    {
                        var sql = "insert into Logs ([Message]) values(@message)";
                        cnn.Open();
                        using (SqlCommand cmd = new SqlCommand(sql, cnn))
                        {
                            cmd.Parameters.AddWithValue("@message", message);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "moneyOperations", autoAck: true, consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
