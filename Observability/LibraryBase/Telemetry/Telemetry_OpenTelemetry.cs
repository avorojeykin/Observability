using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Observability.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Observability.Telemetry
{
    public  sealed class Telemetry_OpenTelemetry : ITelemetry
    {
   
        private readonly string _serviceName;
        private readonly string _serviceVersion;
        private readonly ILogObservability _logc;


        public Telemetry_OpenTelemetry(IConfiguration configuration, ILogObservability loggingcontrol)
        {
            _serviceName = configuration["serviceName"];
            _serviceVersion = configuration["serviceVersion"];
            _logc = loggingcontrol;
        }

        public void AddCustomProperty(string key, string value)
        {
            try
            {
                if (Activity.Current != null && !String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(key))
                {
                    Activity.Current.SetTag(key, value);
                }
            }
            catch (Exception ex)
            {
                _logc.LogError(ex, "Failed to create trace custom property {0}, {1}", key, value);
            }
        }

        public Activity AddCustomSpan( string spanname, string tagkey, string tagvalue)
        {

            ActivitySource aactivitysource = new ActivitySource(_serviceName ?? "", _serviceVersion ?? "");

            var activity = aactivitysource?.StartActivity(spanname);

            try
            {

                if (!String.IsNullOrEmpty(spanname) && !String.IsNullOrEmpty(tagkey) && !String.IsNullOrEmpty(tagvalue))
                {
                    activity?.SetTag(tagkey, tagvalue);
                }
            }
            catch (Exception ex)
            {
                _logc.LogError(ex, "Failed to create trace custom span {0}, {1}", tagkey, tagvalue);
            }

            return activity;
        }

        public void AddCustomBaggage(string key, string value)
        {
            try
            {
                if (Activity.Current != null && !String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(key))
                {
                    Activity.Current.AddBaggage(key, value);
                }
            }
            catch (Exception ex)
            {
                _logc.LogError(ex, "Failed to create trace custom Baggae {0}, {1}", key, value);
            }
        }

        public void AddRequestBody()
        {

        }

        public void AddEvent(string eventName, IDictionary<string, string> customProperties)
        {

        }
        public void AddTrace(string eventName, byte severityLevel, IDictionary<string, string> customProperties)
        {
            
        }

        public string GetCurrentOperationId()
        {
            return Activity.Current.TraceId.ToString() ?? string.Empty;
        }

        public void AddInstrumentation(string eventName, long time, byte severityLevel = 0)
        {
        
        }
    }
}
