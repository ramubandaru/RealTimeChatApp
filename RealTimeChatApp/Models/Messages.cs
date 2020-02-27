using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChatApp.Models
{
    public class Messages
    {
        public int MessageId { get; set; }
        public int EmpId { get; set; }
        public string Name { get; set; }
        public int RoomId { get; set; }
        public string Message { get; set; }
        public string DateSent { get; set; }
    }
}
