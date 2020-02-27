
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketManager;

namespace RealTimeChatApp
{
    public class ChatHandler : WebSocketHandler
    {
        private readonly IRabbitWork _rabbitWork;
        private readonly IDataProvider _dataProvider;
        private readonly IModel _channel;
        public string queueName = "ChatQueue";
        public string exchange = "ChatExchange";
        public string routingKey = "ChatRoutingKey";


        List<int> employeesInSocket = new List<int>();
        Dictionary<string, int> socketIdEmployee = new Dictionary<string, int>();
        List<Employees> employees = new List<Employees>();


        public ChatHandler(IConnection connection, IDataProvider dataProvider
            , WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            _channel = connection.CreateModel();
            StartRabbit();

            _channel = connection.CreateModel();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += ConsumerReceived;
            _channel.BasicConsume(queueName, true, consumer);
            _dataProvider = dataProvider;
        }

       
        public void StartRabbit()
        {
            _channel.ExchangeDeclare(exchange, ExchangeType.Direct, true, false, null);
            _channel.QueueDeclare(queueName);
            _channel.QueueBind(queueName, exchange, routingKey);

        }

        public async Task SendMessage(string socketId, string message, string chatRoomId)
        {
            string empName = string.Empty;
            int empId = 0;
            //List<int> employeesInSocket = new List<int>();
            //Dictionary<string, int> socketIdEmployee = new Dictionary<string, int>();
            //List<Employees> employees = new List<Employees>();

            employees = await Task.Run(() => _dataProvider.GetEmployees());
            
            if(socketIdEmployee.ContainsKey(socketId))
            {
                empId = socketIdEmployee[socketId];
                empName = employees.Where(m => m.EmpID == empId).SingleOrDefault().Name;
            }
            else
            {
                empId = employees[employeesInSocket.Count].EmpID;
                empName = employees[employeesInSocket.Count ].Name;
                socketIdEmployee[socketId] = empId;
                employeesInSocket.Add(empId);
            }
           
            

            var rabbitMessage = new RabbitMessage
            {
                SocketId = socketId,
                Message = message,
                SentFrom = empName,
                ChatRoomId = Convert.ToInt32(chatRoomId)
            };

            await Task.Run(() => Publish(rabbitMessage, exchange, routingKey));
            

            var messages = new Messages()
            {
                Message = rabbitMessage.Message,
                RoomId = rabbitMessage.ChatRoomId,
                EmpId = empId,
                DateSent = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
            };

            await Task.Run(() =>_dataProvider.InsertIntoMessages(messages));
        }

        
        public async Task PingMesssage (string socketId, string sentFrom, string message, string chatRoomId)
        {
            await InvokeClientMethodToAllAsync("pingMessage", socketId, sentFrom, message, chatRoomId);  
            //, message.SocketId, message.SentFrom, message.Message);
            
        }


        public async Task GetMessages(string socketId, string chatRoomId)
        {
            List<Messages> messages = await Task.Run(() => _dataProvider.GetMessages(Convert.ToInt32(chatRoomId)));
            if (messages.Count > 0)
            {
                foreach (var m in messages)
                { 
                    await PingMesssage(socketId, m.Name, m.Message, chatRoomId);
                }
            }
        }

        public void Publish(RabbitMessage rabbitMessage, string exchange, string routingKey)
        {
            //StartRabbit();
            var sendBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(rabbitMessage));
            _channel.BasicPublish(exchange, routingKey, null, sendBytes);
            

        }


        public async void ConsumerReceived(object sender, BasicDeliverEventArgs ea)
        {
            var consumer = new EventingBasicConsumer(_channel);
            string message = Encoding.UTF8.GetString(ea.Body);
            var rabbitResponse = JsonConvert.DeserializeObject<RabbitMessage>(message);
            await Task.Run(() => PingMesssage(rabbitResponse.SocketId, rabbitResponse.SentFrom, rabbitResponse.Message, rabbitResponse.ChatRoomId.ToString()));
            _channel.BasicConsume("ChatQueue", true, consumer);

            //_channel.BasicAck(ea.DeliveryTag, false);
        }


    }
}
