using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.LibraryBase.Logging.Logging_Objects.BgLog
{
    internal interface ICustomLogTableMgrDI
    {
        void EnQueueBuffer(string logDataToPost, string tableTarget);

        void StartTimedFlushed();

        void FinalFlush();
    }
}
