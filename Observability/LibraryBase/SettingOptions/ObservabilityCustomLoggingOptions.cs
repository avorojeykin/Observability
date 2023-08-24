using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Observability.LibraryBase.Utilities.Models;

namespace Observability.LibraryBase.SettingOptions
{
    public class ObservabilityCustomLoggingOptions
    {
        public const string CONFIG_SECTION_NAME = "ObservabilityCustomLogging";
        public bool IsEnabled { get; set; } = true;
        public string PeriodicFlushMilliseconds { get; set; } = "";
        public string CLTTaskLimit { get; set; } = "";
        public string LogAnalyticsApiUrlBase { get; set; } = "";
        public List<LogAnalyticsCustomTable> LogAnalyticsCustomTables { get; set; } = new List<LogAnalyticsCustomTable>();
        
    }
}

