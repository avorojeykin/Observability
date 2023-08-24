using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.LibraryBase.Utilities.Models
{
    public class LogAnalyticsCustomTable
    {
        public string TableName { get; set; } = "";
        public string SharedKey { get; set; } = "";
        public string WorkSpaceID { get; set; } = "";
        public string BufferFlushMaxSize { get; set; } = "";
        public bool PopulateCustomLog { get; set; } = true;
    }
}
