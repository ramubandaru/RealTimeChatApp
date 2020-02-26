
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebSocketManager;

namespace RealTimeChatApp
{
    public class ChatHandler : WebSocketHandler
    {
        private readonly IRabbitWork _rabbitWork;
        private readonly IDataProvider _dataProvider;
        
        public ChatHandler(IRabbitWork rabbitWork, IDataProvider dataProvider, WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
            _rabbitWork = rabbitWork;
            _dataProvider = dataProvider;
        }

        Dictionary<int, string> socketIdEmployee = new Dictionary<int, string>();
        List<Employees> employees = new List<Employees>();

        public async Task SendMessage(string socketId, string message, string chatRoomId)
        {
            string empName = string.Empty;
            int empId = 0;
            employees = await Task.Run(() => _dataProvider.GetEmployees());
            foreach (var m in socketIdEmployee)
            {
                foreach (var e in employees)
                {
                    if(e.EmpID == m.Key)
                    {
                        empName = e.Name;
                        empId = e.EmpID;
                    }
                }
            }

            if(empName == string.Empty)
            {
                empName = employees[0].Name;
                empId = employees[0].EmpID;
                socketIdEmployee[empId] = socketId;
            }


            var rabbitMessage = new RabbitMessage
            {
                SocketId = socketId,
                Message = message,
                SentFrom = empName,
                ChatRoomId = Convert.ToInt32(chatRoomId)
            };

            await Task.Run(() =>_rabbitWork.Publish(rabbitMessage, "ChatExchange", "ChatRoutingKey"));

            var messages = new Messages()
            {
                Message = rabbitMessage.Message,
                RoomId = rabbitMessage.ChatRoomId,
                EmpId = empId,
                DateSent = DateTime.Now
            };

            await Task.Run(() =>_dataProvider.InsertIntoMessages(messages));
        }

        
        public async Task PingMesssage (string socketId, string sentFrom, string message)
        {
            await InvokeClientMethodToAllAsync("pingMessage", socketId, sentFrom, message);    //, message.SocketId, message.SentFrom, message.Message);
            
        }


        public async Task GetMessages(string socketId, string chatRoomId)
        {
            List<Messages> messages = await Task.Run(() => _dataProvider.GetMessages(Convert.ToInt32(chatRoomId)));

            if (messages.Count > 0)
            {
                foreach (var m in messages)
                { 
                    await PingMesssage(socketId, m.EmpName, m.Message);
                }
            }
            
        }

    }
}
