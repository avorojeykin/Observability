using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Observability.LibraryBase.Utilities.Models;
using Observability.Utilities;
using RestSharp;
using Observability.LibraryBase.Logging.Logging_Objects.BgLog;
using Observability.LibraryBase;
using Observability.LibraryBase.SettingOptions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Observability.LibraryBase.Logging.Logging_Objects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.ApplicationInsights;
using System.Diagnostics;

namespace Observability.Logging
{
    public sealed class Logging_AppInsights : ILogObservability
    {
        private static ILogger<ILogObservability> _log;

        private CustomLogTableMgrDI CustomTableClaimTransaction;
        private readonly ObservabilityCustomLoggingOptions _observabilityCustomLoggingOptions;
        private readonly ObservabilityGenericOptions _observabilityGenericOptions;
        private string _defaultConsoleLogLevel;
        private IHttpContextAccessor _httpContextAccessor;
        private TelemetryClient _telemetryClient;   

        private const string CLAIM_TRANSACTION_CUSTOM_LOG_NAME = "ClaimTransaction";
        private const string ERROR_CUSTOM_LOGGING_WAS_NOT_SET_IN_APPSETTINGS = "Observability Custom Logging is not enabled in caller's appsettings! Flag \"IsEnabled\" must be set to true in order to allow writing to Custom Logs.";

        public Logging_AppInsights(ILogger<ILogObservability> logger, IOptions<ObservabilityCustomLoggingOptions> observabilityCustomLoggingOptions,
            IHttpContextAccessor httpContextAccessor, TelemetryClient telemetry, IOptions<ObservabilityGenericOptions> observabilityGenericOptions)
        {
            _log = logger;
            _observabilityCustomLoggingOptions = observabilityCustomLoggingOptions.Value;
            _observabilityGenericOptions = observabilityGenericOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            if (_observabilityCustomLoggingOptions.IsEnabled)
            {
                CustomTableClaimTransaction = new CustomLogTableMgrDI(_log, observabilityCustomLoggingOptions);
            }
            _telemetryClient = telemetry;
            _defaultConsoleLogLevel = _observabilityGenericOptions.DefaultConsoleOutputLogLevel;
        }        

        #region Debug
        public void LogDebug(string message, params object[] values)
        {           
            var context1 = AddContextWithoutException(UserConfiguration.ConvertToKeyValuePairs(), message, values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && _defaultConsoleLogLevel.ToLower().Equals("debug"))
            {
                Console.WriteLine(message);
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Verbose);               
            }
            else
            {                
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Verbose);               
            }           
        }
        public void LogDebug(string message, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContextWithoutException(CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), message, values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && _defaultConsoleLogLevel.ToLower().Equals("debug"))
            {
                Console.WriteLine(message);
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Verbose);
                
            }
            else
            {
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Verbose);
            }           
        }

        public void LogDebug(Exception exception, params object[] values)
        {
            var context1 = AddContextWithoutMessage(UserConfiguration.ConvertToKeyValuePairs(), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && _defaultConsoleLogLevel.ToLower().Equals("debug"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }            
        }

        public void LogDebug(Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContextWithoutMessage(CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && _defaultConsoleLogLevel.ToLower().Equals("debug"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogDebug(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContext(message, CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && _defaultConsoleLogLevel.ToLower().Equals("debug"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogDebug(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values)
        {
            var context1 = AddContext(message, CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && _defaultConsoleLogLevel.ToLower().Equals("debug"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);               
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);                
            }
        }

        #endregion

        #region Information
        public void LogInfo(string message, params object[] values)
        {
            var context1 = AddContextWithoutException(UserConfiguration.ConvertToKeyValuePairs(), message, values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("warning") && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(message);
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information);
            }
            else
            {
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information);           
            }           
        }
        public void LogInfo(string message, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContextWithoutException(CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), message, values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("warning") && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(message);
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information);
            }
            else
            {
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Information);
            }
        }

        public void LogInfo(Exception exception, params object[] values)
        {
            var context1 = AddContextWithoutMessage(UserConfiguration.ConvertToKeyValuePairs(), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("warning") && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogInfo(Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContextWithoutMessage(CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("warning") && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogInfo(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContext(message, CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("warning") && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogInfo(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values)
        {
            var context1 = AddContext(message, CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("warning") && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        #endregion

        #region Warning
        public void LogWarning(string message, params object[] values)
        {
            var context1 = AddContextWithoutException(UserConfiguration.ConvertToKeyValuePairs(), message, values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(message);
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);                
            }
            else
            {
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);
            }           
        }
        public void LogWarning(string message, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContextWithoutException(CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), message, values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(message);
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);
            }
            else
            {
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Warning);
            }
        }

        public void LogWarning(Exception exception, params object[] values)
        {
            var context1 = AddContextWithoutMessage(UserConfiguration.ConvertToKeyValuePairs(), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);                
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogWarning(Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContextWithoutMessage(CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogWarning(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContext(message, CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogWarning(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values)
        {
            var context1 = AddContext(message, CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled && !_defaultConsoleLogLevel.ToLower().Equals("error"))
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        #endregion

        #region Error
        public void LogError(string message, params object[] values)
        {
            var context1 = AddContextWithoutException(UserConfiguration.ConvertToKeyValuePairs(), message, values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled)
            {
                Console.WriteLine(message);
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);               
            }
            else
            {
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
            }           
        }
        public void LogError(string message, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContextWithoutException(CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), message, values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled)
            {
                Console.WriteLine(message);
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
            }
            else
            {
                _telemetryClient.TrackTrace(context1, Microsoft.ApplicationInsights.DataContracts.SeverityLevel.Error);
            }
        }

        public void LogError(Exception exception, params object[] values)
        {
            var context1 = AddContextWithoutMessage(UserConfiguration.ConvertToKeyValuePairs(), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled)
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogError(Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContextWithoutMessage(CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled)
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogError(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            var context1 = AddContext(message, CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled)
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }

        public void LogError(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values)
        {
            var context1 = AddContext(message, CombineKeyValuePairs(context, UserConfiguration.ConvertToKeyValuePairs()), values);
            if (_observabilityGenericOptions.ConsoleOuputEnabled)
            {
                Console.WriteLine(exception.Message);
                _telemetryClient.TrackException(exception, context1);
            }
            else
            {
                _telemetryClient.TrackException(exception, context1);
            }
        }
        public void TelemetryFlush()
        {
            _telemetryClient.Flush();
        }

        public void LoggingTest([Optional] string debug, [Optional] string information, [Optional] string warning, [Optional] string error)
        {
            if (!string.IsNullOrEmpty(debug))
                LogDebug(debug);
            if (!string.IsNullOrEmpty(information))
                LogInfo(information);
            if (!string.IsNullOrEmpty(warning))
                LogWarning(warning);
            if (!string.IsNullOrEmpty(error))
                LogError(error);
        }
        #endregion

        #region Private method
        private IDictionary<string, string> AddContext(string message, List<KeyValuePair<string, object>> context, [Optional] params object[] values)
        {
            var newParams = new KeyValuePair<string, object>();
            var newMessage = new KeyValuePair<string, object>("Message", message);
            if (context == null)
            {
                context = new List<KeyValuePair<string, object>>();
            }
            if (values.Any())
            {
                if (values.Length > 1)
                {
                    string concatenated = string.Join(",", values.Select(x => x.ToString()).ToArray());
                    newParams = new KeyValuePair<string, object>("Additional Message Details", concatenated);
                }
                else if (values.Length == 1)
                {
                    newParams = new KeyValuePair<string, object>("Additional Message Details", values[0].ToString());
                }
                context.Insert(0, newMessage);
                context.Insert(1, newParams);
            }
            else
            {
                context.Insert(0, newMessage);
            }
            var dictcontext = context.ToDictionary(x => x.Key, x => x.Value?.ToString() ?? "");
            var jsoncontext = Newtonsoft.Json.JsonConvert.SerializeObject(dictcontext);
            return (IDictionary<string, string>)dictcontext;
        }

        private IDictionary<string, string> AddContextWithoutMessage(List<KeyValuePair<string, object>> context, [Optional] params object[] values)
        {
            var newParams = new KeyValuePair<string, object>();
            if (context == null)
            {
                context = new List<KeyValuePair<string, object>>();
            }
            if (values.Any())
            {
                if (values.Length > 1)
                {
                    string concatenated = string.Join(",", values.Select(x => x.ToString()).ToArray());
                    newParams = new KeyValuePair<string, object>("Additional Message Details", concatenated);
                }
                else if (values.Length == 1)
                {
                    newParams = new KeyValuePair<string, object>("Additional Message Details", values[0].ToString());
                }
                context.Insert(0, newParams);
            }
            var dictcontext = context.ToDictionary(x => x.Key, x => x.Value?.ToString() ?? "");
            var jsoncontext = Newtonsoft.Json.JsonConvert.SerializeObject(dictcontext);
            return dictcontext;
        }
        private string AddContextWithoutException(List<KeyValuePair<string, object>> context, string message, [Optional] params object[] values)
        {
            var newParams = new KeyValuePair<string, object>();
            var newMessage = new KeyValuePair<string, object>("Message", message);
            if (context == null)
            {
                context = new List<KeyValuePair<string, object>>();
            }                    
            if (values.Any())
            {
                if (values.Length > 1)
                {
                    string concatenated = string.Join(",", values.Select(x => x.ToString()).ToArray());
                    newParams = new KeyValuePair<string, object>("Additional Message Details", concatenated);
                }
                else if (values.Length == 1)
                {
                    newParams = new KeyValuePair<string, object>("Additional Message Details", values[0].ToString());
                }               
                context.Insert(0, newMessage);
                context.Insert(1, newParams);
            }
            else
            {
                context.Insert(0, newMessage);
            }
            var dictcontext = context.ToDictionary(x => x.Key, x => x.Value?.ToString() ?? "");
            var jsoncontext = Newtonsoft.Json.JsonConvert.SerializeObject(dictcontext);
            return jsoncontext;
        }

        private List<KeyValuePair<string, object>> CombineKeyValuePairs(List<KeyValuePair<string, object>> keyValuePairOne, List<KeyValuePair<string, object>> keyValuePairTwo)
        {
            List<KeyValuePair<string, object>> keyValuePairs = new List<KeyValuePair<string, object>>();
            if (keyValuePairOne != null)
            {
                keyValuePairs.AddRange(keyValuePairOne);
                keyValuePairs.AddRange(keyValuePairTwo);
                return keyValuePairs;
            }
            else
            {
                keyValuePairs.AddRange(keyValuePairTwo);
                return keyValuePairs;
            }
        }

        public void ConfigureLogger(string appName = null, object appVersion = null)
        {
            if (appName != null)
            {
                UserConfiguration.AppName = appName;
            }
            if (appVersion != null)
            {
                UserConfiguration.AppVersion = appVersion;
            }
        }
        #endregion

        #region Custom Logging
        public string ConnectivityCheck()
        {           
            return
                $"Periodic Flush Milliseconds: {_observabilityCustomLoggingOptions.PeriodicFlushMilliseconds ?? string.Empty}\r\n" +
                $"CLT Task Limit: {_observabilityCustomLoggingOptions.CLTTaskLimit ?? string.Empty}\r\n" +
                $"Log Analytics Api Base Url: {_observabilityCustomLoggingOptions.LogAnalyticsApiUrlBase ?? string.Empty}\r\n" +                
                $"Log Analytics Custom Tables: {(_observabilityCustomLoggingOptions.IsEnabled && _observabilityCustomLoggingOptions.LogAnalyticsCustomTables.Any() ? string.Join(", ", _observabilityCustomLoggingOptions.LogAnalyticsCustomTables.Select(item => item.TableName).ToList()) : string.Empty)}\r\n" +
                $"Log Analytics Custom WorkSpaceID: {(_observabilityCustomLoggingOptions.IsEnabled && _observabilityCustomLoggingOptions.LogAnalyticsCustomTables.Any() ? string.Join(", ", _observabilityCustomLoggingOptions.LogAnalyticsCustomTables.Select(item => item.WorkSpaceID).ToList()) : string.Empty)}\r\n" +
                $"Log Analytics Custom SharedKey: {(_observabilityCustomLoggingOptions.IsEnabled && _observabilityCustomLoggingOptions.LogAnalyticsCustomTables.Any() ? string.Join(", ", _observabilityCustomLoggingOptions.LogAnalyticsCustomTables.Select(item => item.SharedKey.Substring(0, 4)).ToList()) : string.Empty)}\r\n" +
                $"Log Analytics Custom BufferSize: {(_observabilityCustomLoggingOptions.IsEnabled && _observabilityCustomLoggingOptions.LogAnalyticsCustomTables.Any() ? string.Join(", ", _observabilityCustomLoggingOptions.LogAnalyticsCustomTables.Select(item => item.BufferFlushMaxSize).ToList()) : string.Empty)}\r\n" +
                $"Request.Host.Value : {_httpContextAccessor?.HttpContext.Request.Host.Value ?? string.Empty}\r\n";
        }

        public void ClaimTransactionInsert(string logEntry)
        {
            if (!_observabilityCustomLoggingOptions.IsEnabled)
            {
                LogError(new Exception(ERROR_CUSTOM_LOGGING_WAS_NOT_SET_IN_APPSETTINGS));
                return;
            }

            var customLog = _observabilityCustomLoggingOptions.LogAnalyticsCustomTables
                .FirstOrDefault(customTable => customTable.TableName == CLAIM_TRANSACTION_CUSTOM_LOG_NAME);
            if (customLog == null || !customLog.PopulateCustomLog)
            {
                return;
            }

            try
            {
                JObject logEntryJson = JObject.Parse(logEntry);
                logEntryJson.Add("Observability_AppName", UserConfiguration.AppName);
                logEntryJson.Add("Observability_AppVersion", UserConfiguration.AppVersion?.ToString());
                logEntryJson.Add("Observability_HostName", UserConfiguration.HostName);
                logEntryJson.Add("Observability_HostUrl", _httpContextAccessor?.HttpContext?.Request?.Host.Value ?? string.Empty);
                logEntryJson.Add("CloudRoleName", _observabilityGenericOptions.CloudRoleName ?? string.Empty);
                logEntry = logEntryJson.ToString();
            }
            catch (Exception ex)
            {
                this.LogError(ex, logEntry);
                return;
            }           
            CustomTableClaimTransaction.EnQueueBuffer(logEntry, CLAIM_TRANSACTION_CUSTOM_LOG_NAME);
        }       

        #endregion
    }
}
