using Dapper;
using Microsoft.Extensions.Configuration;
using RealTimeChatApp.Interfaces;
using RealTimeChatApp.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RealTimeChatApp
{
    public class DataProvider : IDataProvider
    {
        private readonly IConfiguration _configuration;
        
        public DataProvider(IConfiguration config)
        {
            _configuration = config;
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_configuration["SqlConnectionStrings:MyConnectionString"]);
            }
        }

        public async Task<List<Messages>> GetMessages(int RoomId)
        {
            try
            {
                using (IDbConnection conn = Connection)
                {
                    string sQuery = $"SELECT m.*, e.Name FROM Messages m LEFT JOIN Employees e ON e.EmpId = m.EmpId WHERE m.RoomId = {RoomId} order by MessageId asc";
                    conn.Open();
                    var response = await conn.QueryAsync<Messages>(sQuery);
                    return response.ToList();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR connecting to database");
                return null;
            }

        }

        public async Task<bool> InsertIntoMessages(Messages message)
        {
            try
            {
                using (IDbConnection conn = Connection)
                {
                    string sQuery = $"INSERT INTO [dbo].[Messages] ([RoomId], [EmpId], [DateSent],[Message]) VALUES ( {message.RoomId}, {message.EmpId}, '{message.DateSent}', '{message.Message}')";
                    conn.Open();
                    var result = await conn.QueryAsync(sQuery);
                    return true;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed connecting to database or failed retrieving messages from database");
                return false;
            }
            
        }

        public async Task<List<Employees>> GetEmployees()
        {
            using (IDbConnection conn = Connection)
            {
                string sQuery = $"SELECT * FROM [dbo].[Employees]";
                conn.Open();
                var response = await conn.QueryAsync<Employees>(sQuery);
                return response.ToList();
            }
        }
    }
}
