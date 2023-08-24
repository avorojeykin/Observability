using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Observability.LibraryBase.Logging.Logging_Objects.BgLog
{
    internal class CustomTable_Config
    {
        public string TableName { get; set; } = "";
        public string SharedKey { get; set; } = "";
        public string WorkSpaceID { get; set; } = "";
        public int BufferFlushMaxSize { get; set; }
    }

    internal class CustomTablesBufferMapper
    {

        public ConcurrentQueue<string> Buffer {get; set; }

        public CustomTable_Config CustomTableConfig;

        public int CurrentBufferFlushSize { get; set; } = 0;

    }
}
