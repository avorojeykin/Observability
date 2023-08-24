
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Observability.Logging
{
    public interface ILogObservability
    { 

        #region Debug

        /// <summary>
        /// LogDebug
        /// </summary>
         void LogDebug(string message, params object[] values);

        /// <summary>
        /// LogDebug
        /// </summary>
         void LogDebug(string message, List<KeyValuePair<string, object>> context, params object[] values);

        /// <summary>
        /// LogDebug
        /// </summary>
         void LogDebug(Exception exception, params object[] values);

        /// <summary>
        /// LogDebug
        /// </summary>
         void LogDebug(Exception exception, List<KeyValuePair<string, object>> context, params object[] values);

        /// <summary>
        /// LogDebug
        /// </summary>
         void LogDebug(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values);
         void LogDebug(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values);

        #endregion

        #region Information

        /// <summary>
        /// LogInfo
        /// </summary>
         void LogInfo(string message, params object[] values);

        /// <summary>
        /// LogInfo
        /// </summary>
         void LogInfo(string message, List<KeyValuePair<string, object>> context, params object[] values);

        /// <summary>
        /// LogInfo
        /// </summary>
         void LogInfo(Exception exception, params object[] values);

        /// <summary>
        /// LogInfo
        /// </summary>
         void LogInfo(Exception exception, List<KeyValuePair<string, object>> context, params object[] values);

        /// <summary>
        /// LogInfo
        /// </summary>
         void LogInfo(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values);

        /// <summary>
        /// LogInfo
        /// </summary>
         void LogInfo(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values);
        #endregion

        #region Warning

        /// <summary>
        /// LogWarning
        /// </summary>
         void LogWarning(string message, params object[] values);

        /// <summary>
        /// LogWarning
        /// </summary>
         void LogWarning(string message, List<KeyValuePair<string, object>> context, params object[] values);

        /// <summary>
        /// LogWarning
        /// </summary>     
         void LogWarning(Exception exception, params object[] values);

        /// <summary>
        /// LogWarning
        /// </summary>  
         void LogWarning(Exception exception, List<KeyValuePair<string, object>> context, params object[] values);

        /// <summary>
        /// LogWarning
        /// </summary>   
         void LogWarning(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values);

        /// <summary>
        /// LogWarning
        /// </summary>   
         void LogWarning(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values);

        #endregion

        #region Error

        /// <summary>
        /// LogException
        /// </summary>  
         void LogError(string message, params object[] values);
        
        /// <summary>
        /// LogException
        /// </summary>  
         void LogError(string message, List<KeyValuePair<string, object>> context, params object[] values);
        
        /// <summary>
        /// LogException
        /// </summary>  
         void LogError(Exception exception, params object[] values);
        
        /// <summary>
        /// LogException
        /// </summary>  
         void LogError(Exception exception, List<KeyValuePair<string, object>> context, params object[] values);
  
        /// <summary>
        /// LogException
        /// </summary>  
         void LogError(string message, Exception exception, List<KeyValuePair<string, object>> context, params object[] values);
        
        /// <summary>
        /// LogException
        /// </summary>  
         void LogError(Exception exception, List<KeyValuePair<string, object>> context, string message, params object[] values);

        #endregion

        #region Custom Logging

        void TelemetryFlush();
        void ClaimTransactionInsert(string logEntry);
       
        string ConnectivityCheck();

        void LoggingTest([Optional] string debug, [Optional] string information, [Optional] string warning,
            [Optional] string error);

        #endregion
    }
}
