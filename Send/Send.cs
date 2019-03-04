using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;

namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue:"hello",
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null
                );

                int i=0;

                while(true)
                {

                    string message = "Hello World #" + i++;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                    routingKey:"hello",
                    basicProperties: null,
                    body: body);

                    System.Console.WriteLine($"Sent {message}");

                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }
            }
        }
    }
}
