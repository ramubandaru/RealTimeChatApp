using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChatApp.Models
{
    public class RabbitMessage
    {
        public string SocketId { get; set; }
        public string Message { get; set; }
        public int ChatRoomId { get; set; }
        public string SentFrom { get; set; }
    }
}
