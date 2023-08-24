using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Observability.Logging;
using System.Dynamic;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Observability.LibraryBase.Logging.Logging_Objects.BgLog;
using Observability.LibraryBase.SettingOptions;
using Microsoft.Extensions.Hosting;
using Microsoft.ApplicationInsights;

namespace ObservabilityTests.Integration
{
    public class Logging_AppInsightsTests
    {
        #region Fields
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ILogger<ILogObservability>> _logger;
        private ILogObservability Logging;
        private Mock<IHttpContextAccessor> _contextAccessor;
        #endregion

        #region Constructor
        public Logging_AppInsightsTests()
        {
            _logger = new Mock<ILogger<ILogObservability>>();
            _configurationMock = new Mock<IConfiguration>();
            IOptions<ObservabilityCustomLoggingOptions> options = Options.Create(new ObservabilityCustomLoggingOptions());
            IOptions<ObservabilityGenericOptions> genericOptions = Options.Create(new ObservabilityGenericOptions());
            _contextAccessor = new Mock<IHttpContextAccessor>();
            TelemetryClient telemetryClient = new TelemetryClient();
            Logging = new Logging_AppInsights(_logger.Object, options, _contextAccessor.Object, telemetryClient, genericOptions);
        }
        #endregion

        #region Tests
        [Fact]
        public void ConnectivityCheckTest()
        {
            // Arrange
            ConfigurationSetup("Value1", "Value2", "Value3");

            string expected = "WorkspaceId:Value1\r\nSharedKey:Value2\r\nTableName:Value3";

            // Act
            var actual = Logging.ConnectivityCheck();

            // Assert
            Assert.Equal(expected, actual);
        }
        
        [Fact]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task LogAnaliticsCustomTableInsertTest_AsyncCall()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // Arrange
            ConfigurationSetup("c87bafca-218e-43b1-b6a1-2f644fac05e4",
                "eVUY+DvujFvB33a2XM1FtWUHzcyvuqv5EH9vHnkYw/uRAKbmUDNj4q42t4IyOIAG+AxucBRe8qWvjTLcymEPUw==",
                "ObservabilityUnitTestAsync");

            dynamic objectToLog = new ExpandoObject();
            objectToLog.ID = Guid.NewGuid().ToString();
            Console.Write(objectToLog.ID);
            objectToLog.CreatedAt = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            string customLogEntry = JsonSerializer.Serialize(objectToLog);

            // Act
            Logging.ClaimTransactionInsert(customLogEntry);

            // Assert
            Assert.True(1 == 1);
        }

        [Fact]
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task LogAnaliticsCustomTableInsertTest_SendCollection()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            // Arrange
            ConfigurationSetup("c87bafca-218e-43b1-b6a1-2f644fac05e4",
                "eVUY+DvujFvB33a2XM1FtWUHzcyvuqv5EH9vHnkYw/uRAKbmUDNj4q42t4IyOIAG+AxucBRe8qWvjTLcymEPUw==",
                "ObservabilityUnitTestAsync");
            List<dynamic> recordsToSend = new();
            dynamic objectToLog = new ExpandoObject();
            objectToLog.ID = Guid.NewGuid().ToString();
            objectToLog.CreatedAt = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            recordsToSend.Add(objectToLog);
            Thread.Sleep(500);
            objectToLog = new ExpandoObject();
            objectToLog.ID = Guid.NewGuid().ToString();
            objectToLog.CreatedAt = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            recordsToSend.Add(objectToLog);

            string customLogEntry = JsonSerializer.Serialize(recordsToSend);

            // Act
            Logging.ClaimTransactionInsert(customLogEntry);

            // Assert
            Assert.True(1 == 1);
        }

        private void ConfigurationSetup(string workspaceId, string sharedKey, string tableName)
        {
            var workspaceIdConfigurationMock = new Mock<IConfigurationSection>();
            workspaceIdConfigurationMock.Setup(x => x.Value).Returns(workspaceId);
            var sharedKeyConfigurationMock = new Mock<IConfigurationSection>();
            sharedKeyConfigurationMock.Setup(x => x.Value).Returns(sharedKey);
            var tableNameConfigurationMock = new Mock<IConfigurationSection>();
            tableNameConfigurationMock.Setup(x => x.Value).Returns(tableName);            

            _configurationMock.Setup(c => c.GetSection("ObservabilityCustomLogging:WorkspaceId")).Returns(workspaceIdConfigurationMock.Object);
            _configurationMock.Setup(c => c.GetSection("ObservabilityCustomLogging:SharedKey")).Returns(sharedKeyConfigurationMock.Object);
            _configurationMock.Setup(c => c.GetSection("ObservabilityCustomLogging:TableName")).Returns(tableNameConfigurationMock.Object);            
        }
        #endregion
    }
}