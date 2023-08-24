using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Observability.LibraryBase.SettingOptions
{
    public class AppInsightsAdaptiveSamplingOptions
    {
        public const string CONFIG_SECTION_NAME = "ApplicationInsightsSamplingOptions";
        public string IncludedTypes { get; set; } = "Dependency;Event;Trace";
        public string ExcludedTypes { get; set; } = "Exception;PageView;Request";
        public int MaxTelemetryItemsPerSecond { get; set; } = 5;
    }
}
