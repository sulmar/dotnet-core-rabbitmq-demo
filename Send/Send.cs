using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Send
{

    class Order 
    {
        public DateTime OrderDate { get; set; }
        public string CustomerNumber { get; set; }

        public Order()
        {
            OrderDate = DateTime.Now;

        }
        public Order(string customerNumber)
        : this()
        {
            this.CustomerNumber = customerNumber;
        }
    }

    

    class Program
    {
        static void Main(string[] args)
        {
           
         //   HelloTest("Hello RabbitMQ!");

          //  TTLTest();

         //   HeadersTest();

            Order order = new Order("1000");

            SendOrder(order);

         //  DirectTest();
        }


        private static void TTLTest()
        {
            var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var model = connection.CreateModel())
            {
               // var args = new Dictionary<string, object>();
               // int ttl = (int) TimeSpan.FromMinutes(1).TotalMilliseconds;
             //   args.Add("x-message-ttl", ttl);

             

             //   model.QueueDeclare(queue: "my-queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                var message = "Hello TTL 2";

                 var body = Encoding.UTF8.GetBytes(message);

                    model.BasicPublish(exchange: string.Empty,
                    routingKey: "mazda",
                    body: body);

                    System.Console.WriteLine($"Sent {message}");

                    System.Console.WriteLine("Press any key to exit.");

                    Console.ReadKey();
            }

        }


        private static void HeadersTest()
        {
            var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var model = connection.CreateModel())
            {
                var headers = new Dictionary<string, object>();
                headers.Add("x-match", "all");
                headers.Add("format", "pdf");
                headers.Add("color", "red");

                string exchangeName = "headers.exchange";
                string queueName = "ReportPDF";

                model.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Headers);
                model.QueueDeclare(queue: queueName, durable: true, autoDelete: false, exclusive: false);
                model.QueueBind(queueName, exchangeName, string.Empty, headers);

                int i=0;

                while(true)
                {
                    string message = "File PDF #" + i++;

                    var properties = model.CreateBasicProperties();
                    properties.Headers = new Dictionary<string, object>();
                    properties.Headers.Add("format", "pdf");
                    properties.Headers.Add("color", "blue");

                    var body = Encoding.UTF8.GetBytes(message);

                    model.BasicPublish(exchange:exchangeName,
                    routingKey:string.Empty,
                    basicProperties: properties,
                    body: body);

                    System.Console.WriteLine($"Sent {message}");

                    Thread.Sleep(TimeSpan.FromMilliseconds(100));
                }

            }
        }


        private static void SendOrder(Order order)
        {
            var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue:"orders-queue",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null
                );

                // add package Newtonsoft.Json
                var message = JsonConvert.SerializeObject(order);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                routingKey:"orders-queue",
                basicProperties: null,
                body: body);

                System.Console.WriteLine($"Sent {message}");
                
            }
        }

        private static void HelloTest(string message)
        {
            var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue:"my-queue",
                                    durable: true,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null
                );

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                routingKey:"my-queue",
                basicProperties: null,
                body: body);

                System.Console.WriteLine($"Sent {message}");

                
            }
            
        }

        private static void DirectTest()
        {
              var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                // channel.QueueDeclare(queue:"hello",
                //                     durable: false,
                //                     exclusive: false,
                //                     autoDelete: false,
                //                     arguments: null
                // );

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
