using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using RealTimeChatApp;
using RealTimeChatApp.Mocks;

namespace RealTimeChatApp.Tests
{
    public class DbProviderTests
    {
        [Fact]
        public void DataProvider_WithNoConnection_ShouldReturnNull()
        {
            var response = new DataProviderMock().DataProvider_withNoConnection();
            Assert.Null(response.Object.ConnectionString);

        }

        [Fact]
        public void GetMessages_WithNoData_ShouldReturnNull()
        {
           
            var response = new DataProviderMock().DataProvider_withNoInput();
            Assert.Null(response.Object.ConnectionString);
        }
        
        [Fact]
        public void GetMessages_WithDataandConnection_ShouldReturnList()
        {
            string actual = "";
            var response = new DataProviderMock().DataProvider_withConnection();
            Assert.Equal(response.Name, actual );

        }


    }
}
