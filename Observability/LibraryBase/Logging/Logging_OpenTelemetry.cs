using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Observability.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Observability.Logging
{
    public sealed class Logging_OpenTelemetry : ILogObservability
    {
        private static ILogger<ILogObservability> _log;

        public Logging_OpenTelemetry(ILogger<ILogObservability> logger)
        {
            _log = logger;
        }

        #region Debug

        public void LogDebug(string message, params object[] values)
        {
            _log.LogDebug(StrFunc.CleanMessage(message), values);
        }

        public void LogDebug(string message, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogDebug(StrFunc.CleanMessage(message), values);
        }

        public void LogDebug(Exception exception, params object[] values)
        {
            _log.LogDebug(exception, StrFunc.CleanMessage(exception?.Message ?? string.Empty), values);
        }

        public void LogDebug(Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogDebug(exception, StrFunc.CleanMessage(exception?.Message ?? string.Empty), values);
        }

        public void LogDebug(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogDebug(exception, StrFunc.CleanMessage(message), values);
        }

        public void LogDebug(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values)
        {
            _log.LogDebug(exception, StrFunc.CleanMessage(message), values);
        }

        #endregion

        #region Information

        public void LogInfo(string message, params object[] values)
        {
            _log.LogInformation(StrFunc.CleanMessage(message), values);
        }

        public void LogInfo(string message, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogInformation(StrFunc.CleanMessage(message), values);
        }

        public void LogInfo(Exception exception, params object[] values)
        {
            _log.LogInformation(exception, StrFunc.CleanMessage(exception?.Message ?? string.Empty), values);
        }

        public void LogInfo(Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogInformation(exception, StrFunc.CleanMessage(exception?.Message ?? string.Empty), values);
        }

        public void LogInfo(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogInformation(exception, StrFunc.CleanMessage(message), values);
        }

        public void LogInfo(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values)
        {
            _log.LogInformation(exception, StrFunc.CleanMessage(message), values);
        }

        #endregion

        #region Warning

        public void LogWarning(string message, params object[] values)
        {
            _log.LogWarning(StrFunc.CleanMessage(message), values);
        }

        public void LogWarning(string message, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogWarning(StrFunc.CleanMessage(message), values);
        }

        public void LogWarning(Exception exception, params object[] values)
        {
            _log.LogWarning(exception, StrFunc.CleanMessage(exception?.Message ?? string.Empty), values);
        }

        public void LogWarning(Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogWarning(exception, StrFunc.CleanMessage(exception?.Message ?? string.Empty), values);
        }

        public void LogWarning(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogWarning(exception, StrFunc.CleanMessage(message), values);
        }

        public void LogWarning(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values)
        {
            _log.LogWarning(exception, StrFunc.CleanMessage(message), values);
        }

        #endregion

        #region Error

        public void LogError(string message, params object[] values)
        {
            _log.LogError(StrFunc.CleanMessage(message), values);
        }

        public void LogError(string message, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogError(StrFunc.CleanMessage(message), values);
        }

        public void LogError(Exception exception, params object[] values)
        {
            _log.LogError(exception, StrFunc.CleanMessage(exception?.Message ?? string.Empty), values);
        }

        public void LogError(Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogError(exception, StrFunc.CleanMessage(exception?.Message ?? string.Empty), values);
        }

        public void LogError(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values)
        {
            _log.LogError(exception, StrFunc.CleanMessage(message), values);
        }

        public void LogError(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values)
        {
            _log.LogError(exception, StrFunc.CleanMessage(exception?.Message ?? string.Empty), context, message, values);
        }
        #endregion

        #region Custom Logging

        public void TelemetryFlush()
        {
            throw new NotImplementedException();
        }

        public string ConnectivityCheck()
        {
            throw new NotImplementedException();
        }

        public void LoggingTest([Optional] string debug, [Optional] string information, [Optional] string warning, [Optional] string error)
        {
            throw new NotImplementedException();
        }

        public void ClaimTransactionInsert(string logEntry)
        {
            throw new NotImplementedException();
        }      
        #endregion

    }
}