using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RealTimeChatApp.Controllers;
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebSocketManager;

namespace RealTimeChatApp.RabbitMQ
{
    public class RabbitWork : IRabbitWork
    {
        //ConnectionFactory _factory = new ConnectionFactory();
        //private readonly IConnectionFactory _connectionFactory;
        //private readonly IConnection _connection;
        private readonly IModel _channel;
        //private readonly EventingBasicConsumer _basicConsumer;

        public string queueName = "ChatQueue";
        public string exchange = "ChatExchange";
        public string routingKey = "ChatRoutingKey";
        public RabbitWork(IConnection connection)
        {
            //_connectionFactory = connectionFactory;
            //_connection = _factory.CreateConnection();
            _channel = connection.CreateModel();
            StartRabbit();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ConsumerReceived;
            //string message = string.Empty;
            //consumer.Received += (model, ea) =>
            //{
            //    var body = ea.Body;
            //    message = Encoding.UTF8.GetString(body);

            //};
            //Console.WriteLine(" [x] Received {0}", message);
            //QueueController queueController = new QueueController();
            //queueController.ShowMessage(message);
            _channel.BasicConsume(queueName, false, consumer);

        }

        public void StartRabbit()
        {
            _channel.ExchangeDeclare(exchange, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName);
            _channel.QueueBind(queueName, exchange, routingKey);

        }

        public void Publish(RabbitMessage rabbitMessage, string exchange, string routingKey)
        {
            //StartRabbit();
            var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rabbitMessage));
            _channel.BasicPublish(exchange, routingKey, null, sendBytes);
            
        }

        public void ConsumerReceived(object sender, BasicDeliverEventArgs ea)
        {
            var consumer = new EventingBasicConsumer(_channel);
            
            string message = Encoding.UTF8.GetString(ea.Body);

            var rabbitResponse = JsonConvert.DeserializeObject<RabbitMessage>(message);
            
            ChatHandler chatHandler = new ChatHandler(this, null, new WebSocketConnectionManager());
            Task.Run(() =>chatHandler.PingMesssage(rabbitResponse.SocketId, rabbitResponse.SentFrom, rabbitResponse.Message));

            _channel.BasicConsume("ChatQueue", false, consumer);
        }

       


    }
}
