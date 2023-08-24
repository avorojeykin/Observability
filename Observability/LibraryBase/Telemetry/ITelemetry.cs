using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
namespace Observability.Telemetry
{
    public interface ITelemetry
    {
        /// <summary>
        /// Add Trace Custom Property - resides with the current trace id.
        /// </summary>
        void AddCustomProperty( string key, string value);
        /// <summary>
        /// Add Trace Custom Span - subspan for current activity - Only works for OpenTelemtry, not AppInsights.
        /// </summary>
        Activity AddCustomSpan( string spanname, string tagkey, string tagvalue);

        /// <summary>
        /// Add Trace Custom Baggage - Carries key/pair values through dependency activities and other APIs.
        /// </summary>
        void AddCustomBaggage(string key, string value);

        /// <summary>
        /// Add RequestBody
        /// </summary>
        void AddRequestBody();

        /// <summary>
        /// Events.
        /// </summary>
        void AddEvent( string eventName, IDictionary<string,string> customProperties);

        /// <summary>
        /// Manual Trace.
        /// </summary>
        void AddTrace(string eventName, byte severityLevel, IDictionary<string, string> customProperties);

        /// <summary>
        /// Add TraceId 
        /// </summary>

        void AddInstrumentation(string eventName, long time, byte severityLevel = 0);

        /// <summary>
        /// Add TraceId 
        /// </summary>
        string GetCurrentOperationId();
    }
}
