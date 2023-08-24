using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Observability.LibraryBase.SettingOptions;

namespace Observability
{
    public class TelemetryProcessorExtender : ITelemetryProcessor
    {

        private ObservabilityGenericOptions _observabilityGenericOptions;

        private ITelemetryProcessor _next { get; set; }

        public TelemetryProcessorExtender(ITelemetryProcessor next, IOptions<ObservabilityGenericOptions> observabilityGenericOptions)
        {
            _observabilityGenericOptions = observabilityGenericOptions.Value;
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is TraceTelemetry trace && trace.SeverityLevel == SeverityLevel.Verbose)
            {
                // Skip debug-level traces
                return;
            }
            item.Context.Cloud.RoleName = _observabilityGenericOptions.CloudRoleName;
            _next.Process(item);
        }
    }
}