using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Receive
{
     public class Order 
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
            Console.WriteLine("Hello World!");

           // ConsumerTest();


            ConsumeOrder();

         //  DirectTest();

         // HeadersTest();


        }

        private static void ConsumeOrder()
        {
            var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var model = connection.CreateModel())
            {
                string queueName = "orders-queue";

                model.QueueDeclare(queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);


                var  consumer = new OrderConsumer(model);

                consumer.OnReceive(order => System.Console.WriteLine($"Send {order.CustomerNumber}"));

                // var consumer = new EventingBasicConsumer(channel);
                // consumer.Received += (model, ea) => 
                // {
                //     var message =  Encoding.UTF8.GetString(ea.Body);

                //     System.Console.WriteLine($"Received order {message}");

                //     Order order = JsonConvert.DeserializeObject<Order>(message);

                //     System.Console.WriteLine($"Send order {order.CustomerNumber} to <marcin.sulecki@sulmar.pl>");
                // };

                model.BasicConsume(queue: queueName,
                autoAck: true,
                consumer: consumer);

                Console.WriteLine(" Press any to exit.");
                Console.ReadKey();
               
            }
        }

        static void ConsumerTest()
        {
            var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "my-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

                 MyConsumer consumer = new MyConsumer(channel);

                channel.BasicConsume(queue: "my-queue",
                autoAck: true,
                consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }

        }

        static void DirectTest()
        {
            var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "my-queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => 
                {
                    var message =  Encoding.UTF8.GetString(ea.Body);
                    System.Console.WriteLine($"Received {message}");
                };

                channel.BasicConsume(queue: "my-queue",
                autoAck: true,
                consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
               
            }
        }

        static void HeadersTest()
        {
            var factory = new ConnectionFactory() { HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                // channel.QueueDeclare(queue: "ReportPDF",
                // durable: true,
                // exclusive: false,
                // autoDelete: false,
                // arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => 
                {
                    var message =  Encoding.UTF8.GetString(ea.Body);
                    System.Console.WriteLine($"Received {message}");
                };

                channel.BasicConsume(queue: "ReportPDF",
                autoAck: false,
                consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
               
            }
        }

    }

    public class OrderConsumer : EntityConsumer<Order>
    {
        public OrderConsumer(IModel model) 
        : base(model)
        {
        }
    }

    public class EntityConsumer<T> : DefaultBasicConsumer
    {
        private readonly IModel model;
        private Action<T> execute;

        public void OnReceive(Action<T> execute)
        {
            this.execute = execute;
        }

        public EntityConsumer(IModel model) : base(model)
        {
            this.model = model;
        }


        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            Console.WriteLine($"Consuming Headers Message");
            Console.WriteLine(string.Concat("Message received from the exchange ", exchange));
            Console.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            Console.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            Console.WriteLine(string.Concat("Routing tag: ", routingKey));
        
             var message =  Encoding.UTF8.GetString(body);

            System.Console.WriteLine($"Received order {message}");

            T entity = JsonConvert.DeserializeObject<T>(message);

            execute?.Invoke(entity);

           // Order order = JsonConvert.DeserializeObject<Order>(message);

            // System.Console.WriteLine($"Send order {order.CustomerNumber} to <marcin.sulecki@sulmar.pl>");

            model.BasicAck(deliveryTag, false);
        }

    }

    public class MyConsumer : DefaultBasicConsumer
    {
        private readonly IModel model;

        public MyConsumer(IModel model) : base(model)
        {
            this.model = model;
        }


        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            Console.WriteLine($"Consuming Headers Message");
            Console.WriteLine(string.Concat("Message received from the exchange ", exchange));
            Console.WriteLine(string.Concat("Consumer tag: ", consumerTag));
            Console.WriteLine(string.Concat("Delivery tag: ", deliveryTag));
            Console.WriteLine(string.Concat("Routing tag: ", routingKey));
            Console.WriteLine(string.Concat("Message: ", Encoding.UTF8.GetString(body)));
            model.BasicAck(deliveryTag, false);

        }
    }
}
