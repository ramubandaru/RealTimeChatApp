using RealTimeChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChatApp.Interfaces
{
    public interface IRabbitWork
    {
        void Publish(RabbitMessage rabbitMessage, string exchange, string routingKey);
        //void Consume(string message);
    }
}
