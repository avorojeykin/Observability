using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
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
    public sealed class Telemetry_AppInsights : ITelemetry
    {
        private static HttpContext HttpContext => new HttpContextAccessor().HttpContext;
        private readonly ILogObservability _logc;
        private TelemetryClient _telemetry;

        public Telemetry_AppInsights(IConfiguration _configuration, ILogObservability loggingcontrol, TelemetryClient telemetry)
        {
            _logc = loggingcontrol;
            _telemetry = telemetry;
        }


    public  void AddCustomProperty( string key, string value)
        {
            try
            {
                if (HttpContext != null)
                {
                    var telemetryReq = HttpContext.Features.Get<RequestTelemetry>() ?? null;

                    if (telemetryReq != null && !String.IsNullOrEmpty(value) && !String.IsNullOrEmpty(key))
                    {
                        telemetryReq.Properties.Add(key, value);
                    }
                }
            }
            catch(Exception ex)
            {
                _logc.LogError(ex, "Failed to create trace custom property {0}, {1}", key, value);
            }
        }

        public Activity AddCustomSpan(string spanname, string tagkey, string tagvalue)
        {
            Activity activity = null;

            try
            {
                if (HttpContext != null)
                {
                    var telemetryReq = HttpContext.Features.Get<RequestTelemetry>();

                    if (telemetryReq != null && !String.IsNullOrEmpty(tagkey) && !String.IsNullOrEmpty(tagkey))
                    {
                        telemetryReq.Properties.Add(tagkey, tagvalue);
                    }
                }
            }
            catch(Exception ex)
            {
                _logc.LogError(ex, "Failed to create trace custom span property {0}, {1}", tagkey, tagvalue);
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
                _logc.LogError(ex, "Failed to create trace custom baggage {0}, {1}", key, value);
            }

        }

        public void AddRequestBody()
        {
            if (HttpContext != null)
            {
                var telemetryReq = HttpContext.Features.Get<RequestTelemetry>();

                if (telemetryReq != null)
                {
                    telemetryReq.Properties.Add("RequestBody",HttpContext.Request.Body.ToString());
                }
            }
        }


        public  void AddEvent(string eventName, IDictionary<string, string> customProperties)
        {
            _telemetry.TrackEvent(eventName, customProperties);
        }

        public void AddTrace(string eventName, byte severityLevel, IDictionary<string, string> customProperties)
        {
            SeverityLevel severity;

            switch (severityLevel)
            {
                case 1:
                    severity = SeverityLevel.Information;
                    break;
                case 2:
                     severity = SeverityLevel.Warning;
                    break;
                case 3:
                    severity = SeverityLevel.Error;
                    break;
                default:
                    severity = SeverityLevel.Information;
                    break;
            }

            _telemetry.TrackTrace(eventName, severity, customProperties);
        }

        public void AddInstrumentation(string eventName, long time, byte severityLevel = 0)
        {
            double milliseconds = time / TimeSpan.TicksPerMillisecond;
            milliseconds = Math.Round(milliseconds, 4);
            SeverityLevel severity; //default

            switch (severityLevel)
            {
                case 1:
                    severity = SeverityLevel.Information;
                    break;
                case 2:
                    severity = SeverityLevel.Warning;
                    break;
                case 3:
                    severity = SeverityLevel.Error;
                    break;
                default:
                    severity = SeverityLevel.Information;
                    break;
            }

            IDictionary<string, string> duration = new Dictionary<string, string>
            {
                { "Duration", milliseconds.ToString() }
            };

            _telemetry.TrackTrace(eventName, severity, duration);
        }

        public string GetCurrentOperationId()
        {
            return Activity.Current.TraceId.ToString() ?? string.Empty;
        }

    }
}