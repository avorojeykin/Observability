using Observability.Utilities;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Headers;
using System.Collections.Concurrent;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Observability.LibraryBase.SettingOptions;
using Observability.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Observability.LibraryBase.Logging.Logging_Objects.BgLog
{
    internal class CustomLogTableMgrDI : ICustomLogTableMgrDI
    {
        #region variables
        private static ILogger<ILogObservability> _log;
        private readonly ObservabilityCustomLoggingOptions _observabilityCustomLoggingOptions;
        private readonly static string LOG_DATA_TO_HASH = "POST\n{0}\napplication/json\nx-ms-date:{1}\n/api/logs";
        private readonly static string SIGNATURE = "SharedKey {0}:{1}";
        private readonly static string URL_BASE = "https://{0}.ods.opinsights.azure.com/api/logs?api-version=2016-04-01";
        private static int PeriodicFlushMilliseconds = 2000; //default
        private readonly static int BufferMuliplier = 10; //default
        private static int httptaskthreadcount = 0;
        private static int clThreadLimit = 200;
        private static string _urlBase;
        #endregion

        #region buffers
        private static List<CustomTablesBufferMapper> _table_BufferMap = new List<CustomTablesBufferMapper>();
        #endregion

        #region Constructors
        public CustomLogTableMgrDI(ILogger<ILogObservability> logger, IOptions<ObservabilityCustomLoggingOptions> observabilityCustomLoggingOptions)
        {
            _log = logger;
            _observabilityCustomLoggingOptions = observabilityCustomLoggingOptions.Value;
            _urlBase = string.IsNullOrEmpty(_observabilityCustomLoggingOptions.LogAnalyticsApiUrlBase.Trim()) ? URL_BASE 
                : _observabilityCustomLoggingOptions.LogAnalyticsApiUrlBase;
            //Periodic Flush setting - overrides default value above if exists
            if (Int32.TryParse(_observabilityCustomLoggingOptions.PeriodicFlushMilliseconds, out int result))
            { PeriodicFlushMilliseconds = result; }

            if (Int32.TryParse(_observabilityCustomLoggingOptions.CLTTaskLimit, out int threadlimit))
            { clThreadLimit = threadlimit; }

            //Build Array of Tables and their specific Settings here 
            List<CustomTable_Config> customTable_Configs = _observabilityCustomLoggingOptions.LogAnalyticsCustomTables
                .Select(table => new CustomTable_Config
                {
                    BufferFlushMaxSize = int.TryParse(table.BufferFlushMaxSize, out _) ? Convert.ToInt16(table.BufferFlushMaxSize) : 0,
                    SharedKey = table.SharedKey,
                    TableName = table.TableName,
                    WorkSpaceID = table.WorkSpaceID
                }).ToList();

            foreach (CustomTable_Config ctc in customTable_Configs)
            {
                CustomTablesBufferMapper mapper = new CustomTablesBufferMapper
                {
                    CustomTableConfig = ctc,
                    Buffer = new ConcurrentQueue<string>(),
                    CurrentBufferFlushSize = ctc.BufferFlushMaxSize
                };
                _table_BufferMap.Add(mapper);
            }

            //Start the timed Buffers defined in array  
            StartTimedFlushed();
        }        
        #endregion



        #region Public Methods

        public void EnQueueBuffer(string logDataToPost, string tableNameTarget)
        {
            bool targetfound = false;
            foreach(var buffTarget in _table_BufferMap.Select((value, i) => new { i, value }))
            {
                if (buffTarget.value.CustomTableConfig.TableName == tableNameTarget)
                {
                    targetfound = true;
                    _table_BufferMap[buffTarget.i].Buffer.Enqueue(logDataToPost);
                    SendCustomLogEntry(buffTarget.i, timedflush: false);
                    break;
                }
            }

            if (targetfound == false)
            {
                _log.LogWarning("Buffer Warning: Machine Name:" + Environment.MachineName + " : Attempt to Log for non existing table/table definition. Value was " + tableNameTarget +".");                
            }

        }

        #endregion

        #region

        private void QueueDistressCheck(int tableMapperInt)
        {
            if (_table_BufferMap[tableMapperInt].Buffer.Count >= (_table_BufferMap[tableMapperInt].CurrentBufferFlushSize * BufferMuliplier))
            {
                //increasing  buffer size if appropriate
                _table_BufferMap[tableMapperInt].CurrentBufferFlushSize = _table_BufferMap[tableMapperInt].CurrentBufferFlushSize * 2;
                _log.LogWarning("Stressed Buffer: Machine Name:" + Environment.MachineName + " Name: " + _table_BufferMap[tableMapperInt].CustomTableConfig.TableName + " Count:" + _table_BufferMap[tableMapperInt].Buffer.Count.ToString() + " Current FlushMaxSize Increased:" + _table_BufferMap[tableMapperInt].CurrentBufferFlushSize.ToString());

            }
            else
            {
                //reducing  buffer size if appropriate
                if (   
                    _table_BufferMap[tableMapperInt].CurrentBufferFlushSize > _table_BufferMap[tableMapperInt].CustomTableConfig.BufferFlushMaxSize &&    
                    _table_BufferMap[tableMapperInt].Buffer.Count <= (_table_BufferMap[tableMapperInt].CurrentBufferFlushSize * (BufferMuliplier / 2)  )                              
                    )
                {
                    _table_BufferMap[tableMapperInt].CurrentBufferFlushSize = _table_BufferMap[tableMapperInt].CurrentBufferFlushSize / 2;
                    _log.LogWarning("Recovering Buffer: Machine Name:" + Environment.MachineName + " Name: " + _table_BufferMap[tableMapperInt].CustomTableConfig.TableName + " Count:" + _table_BufferMap[tableMapperInt].Buffer.Count.ToString() + " Current FlushMaxSize decreased:" + _table_BufferMap[tableMapperInt].CurrentBufferFlushSize.ToString());

                }

            }
        }

/*        private void ThreadDistressCheck()
        {
            //To do, check thread count and review how long taking to process thread against block size of post
        }
*/

        private void SendCustomLogEntry(int tableMapperInt, bool timedflush)
        {
            if (
                (_table_BufferMap[tableMapperInt].Buffer.Count < _table_BufferMap[tableMapperInt].CurrentBufferFlushSize
                && timedflush == false) || httptaskthreadcount>=clThreadLimit)
            { return; }

            Console.WriteLine("threadcount=" + httptaskthreadcount.ToString());
            //Check if buffer under stress
            QueueDistressCheck(tableMapperInt);

            try
            {
                if (BuildPostData( tableMapperInt, out String logDataToPost) && logDataToPost != null)
                {
                    ;
                    var logDataToPostBytes = Encoding.UTF8.GetBytes(logDataToPost);
                    string stringToHash = string.Format(LOG_DATA_TO_HASH, logDataToPostBytes.Length, DateTime.UtcNow.ToString("r"));
                    CallApiPostCustomLogEntry(logDataToPost, string.Format(SIGNATURE, _table_BufferMap[tableMapperInt].CustomTableConfig.WorkSpaceID, BuildSignature(stringToHash, _table_BufferMap[tableMapperInt].CustomTableConfig.SharedKey)), _table_BufferMap[tableMapperInt].CustomTableConfig);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "CustomLogging: Error Sending Custom Log Entry");
            }
        }
        
        public void CallBack(object state)
        {
            foreach (var buffTarget in _table_BufferMap.Select((value, i) => new { i, value }))
            {
                while (httptaskthreadcount <= clThreadLimit && _table_BufferMap[buffTarget.i].Buffer.Count > 0)
                {
                    SendCustomLogEntry(buffTarget.i, true);
                }

            }
        }
        public void StartTimedFlushed()
        {
            new Timer(new TimerCallback(CallBack), null, 0, PeriodicFlushMilliseconds);
            //Flush the buffer
           /* var scheduledflush = new PeriodicTimer(TimeSpan.FromMilliseconds(PeriodicFlushMilliseconds));
            while (await scheduledflush.WaitForNextTickAsync())
            {
                //Flush all Buffers
                foreach (var buffTarget in _table_BufferMap.Select((value, i) => new { i, value }))
                {
                    while (httptaskthreadcount <= clThreadLimit && _table_BufferMap[buffTarget.i].Buffer.Count>0)
                    {
                        SendCustomLogEntry(buffTarget.i, true);
                    }

                }
            }*/
            
        }

        public void FinalFlush()
        {
            //Flush all Buffers
            foreach (var buffTarget in _table_BufferMap.Select((value, i) => new { i, value }))
            {
                while (httptaskthreadcount <= clThreadLimit && _table_BufferMap[buffTarget.i].Buffer.Count > 0)
                {
                    SendCustomLogEntry(buffTarget.i, true);
                    _log.LogInformation("Executed FinalFlush Method");
                }
            }
        }

        private static bool BuildPostData(int tableMapperInt, out string result)
        {
            if (_table_BufferMap[tableMapperInt].Buffer.Count > 0)
            {
                StringBuilder sb = new StringBuilder("[");

                int count = 0;
                while (count < _table_BufferMap[tableMapperInt].CurrentBufferFlushSize && _table_BufferMap[tableMapperInt].Buffer.Count > 0)
                {
                    if (_table_BufferMap[tableMapperInt].Buffer.TryDequeue(out result))
                    {
                        sb.Append(result + ",");
                    }

                    count++;
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append("]");

                result = sb.ToString();
                return true;
            }
            else
            {
                result = String.Empty;
                return false;
            }
        }

        private static string BuildSignature(string messageToHash, string _sharedKey)
        {
            try
            {
                var encoding = new System.Text.ASCIIEncoding();
                byte[] keyByte = Convert.FromBase64String(_sharedKey);
                byte[] messageBytes = encoding.GetBytes(messageToHash);
                using (var hmacsha256 = new HMACSHA256(keyByte))
                {
                    byte[] hash = hmacsha256.ComputeHash(messageBytes);
                    return Convert.ToBase64String(hash);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"CustomLogging: Signature building error: {StrFunc.GetExceptionString(ref ex)}, details are in the log");
            }
        }



        private static void CallApiPostCustomLogEntry(string logDataToPost, string signature, CustomTable_Config tableconfig)
        {
            try
            {
                string Start = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                bool success = false;

                httptaskthreadcount++;
                Task.Run(async () =>
                {
                    var guid = new Guid();
                    int retrymax = 10;
                    while (success == false && retrymax > 0)
                    {
                        try
                        {
                            System.Net.Http.HttpClient client = new System.Net.Http.HttpClient(new HttpClientHandler
                            {
                                UseProxy = false
                            });
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            client.DefaultRequestHeaders.Add("Log-Type", tableconfig.TableName);
                            client.DefaultRequestHeaders.Add("Authorization", signature);
                            client.DefaultRequestHeaders.Add("x-ms-date", DateTime.UtcNow.ToString("r"));
                            client.DefaultRequestHeaders.Add("time-generated-field", "");
                            client.Timeout = TimeSpan.FromSeconds(30);

                            System.Net.Http.HttpContent httpContent = new StringContent(logDataToPost, Encoding.UTF8);
                            httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            var response = await client.PostAsync(new Uri(string.Format(_urlBase, tableconfig.WorkSpaceID)), httpContent);

                            String message = Start.ToString() + ".." + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " guid: " + guid.ToString() + " ," + response.StatusCode.ToString() + ": table: " + tableconfig.TableName;
                            if (retrymax < 10) { message += ": Retry" + (10 - retrymax).ToString(); }
                            
                            _log.LogWarning(message);

                            switch (response.StatusCode)
                            {
                                case HttpStatusCode.OK:
                                    success = true;
                                    httptaskthreadcount--;
                                    Console.WriteLine("threads:" + httptaskthreadcount.ToString());
                                    return;
                                default:
                                    //do nothing - sig rebuild moved below
                                    break;
                            }

                            Start = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                        }
                        catch (Exception ex)
                        {
                            _log.LogError(ex,"CustomLogging: guid: "+guid.ToString()+", Exception Writing batch to Log Analytics "+ tableconfig.TableName + ". Attempting Retry");
                            Console.WriteLine(Start.ToString() + ".." + DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt") + " EXCEPTION :" + ex.Message);
                            if (ex.InnerException != null)
                            {
                                Console.WriteLine(ex.InnerException.Message.ToString() + ":innerexception:");
                            }
                        }

                        //rebuild sig 
                        string stringToHash = string.Format(LOG_DATA_TO_HASH, Encoding.UTF8.GetBytes(logDataToPost).Length, DateTime.UtcNow.ToString("r"));
                        signature = string.Format(SIGNATURE, tableconfig.WorkSpaceID, BuildSignature(stringToHash, tableconfig.SharedKey));

                        //cleanup
                        Start = DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");
                        retrymax--;

                        if (retrymax <=0)
                        {
                            //Need to Log this Data Elsewhere as it's not being accepted by Log Analytics
                            //decrement current thread count
                            httptaskthreadcount--;
                        }
                    }
                    
                });

            }
            catch (Exception ex)
            {
                _log.LogError(ex,"CustomLogging: API Post Exception");
            }
        }
        #endregion
    }
}
