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
        void AddCustomProperty( string key, string value);
        Activity AddCustomSpan( string spanname, string tagkey, string tagvalue);
        void AddCustomBaggage(string key, string value);
        void AddRequestBody();
        void AddEvent( string eventName, IDictionary<string,string> customProperties);
        void AddTrace(string eventName, byte severityLevel, IDictionary<string, string> customProperties);
        void AddInstrumentation(string eventName, long time, byte severityLevel = 0);
        string GetCurrentOperationId();
    }
}
