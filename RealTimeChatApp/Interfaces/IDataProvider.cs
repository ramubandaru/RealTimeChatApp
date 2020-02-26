using RealTimeChatApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChatApp.Interfaces
{
    public interface IDataProvider
    {
        Task<List<Messages>> GetMessages(int RoomId);
        Task<bool> InsertIntoMessages(Messages message);
        Task<List<Employees>> GetEmployees();
    }
}
