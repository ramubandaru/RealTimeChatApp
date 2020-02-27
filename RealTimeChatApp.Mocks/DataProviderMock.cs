using Moq;
using System;
using System.Data;
using System.Data.SqlClient;
using Xunit;

namespace RealTimeChatApp.Mocks
{
    public class DataProviderMock
    {

        public string ConnectionString = "Server=DESKTOP-3NR80PA;Database=Chat;Integrated Security=true;";
        
        public Mock<IDbConnection> DataProvider_withNoInput()
        {
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            return connection;

        }

        public Mock<IDbConnection> DataProvider_withNoConnection()
        {
            Mock<IDbConnection> connection = new Mock<IDbConnection>();
            return connection;

        }

        public Mock<SqlConnection> DataProvider_withConnection()
        {

            //Mock<IDbConnection> connection = new Mock<IDbConnection>();
            var response = new Mock<SqlConnection>(ConnectionString);
            return response ;
        }
    }
}
