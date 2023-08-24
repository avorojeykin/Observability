using System;
using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Observability.Telemetry;
using Observability.Logging;
using Microsoft.Extensions.Configuration;
using Observability.LibraryBase.Logging.Logging_Objects.BgLog;
using Observability.LibraryBase.SettingOptions;
using Microsoft.Extensions.Hosting;
using Microsoft.ApplicationInsights.Extensibility;
using Observability.LibraryBase.Logging.Logging_Objects;
using System.Reflection;
using System.Net;
using Azure.Core;
using Newtonsoft.Json;
using Observability;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ObservabilityExtensions
    {
        public static IServiceCollection AddSourceApplicationInsights(this IServiceCollection services, IConfiguration configuration)
        {
            var aiOptions = new ApplicationInsights.AspNetCore.Extensions.ApplicationInsightsServiceOptions
            {

                // Disables adaptive sampling using default method - see below
                EnableAdaptiveSampling = false
            };

            //Uses this Sampling settings instead
            services.Configure<TelemetryConfiguration>((config) =>
            {

                var builder = config.DefaultTelemetrySink.TelemetryProcessorChainBuilder;
                var defaultSampling = new AppInsightsAdaptiveSamplingOptions();

                if (Int32.TryParse(configuration.GetSection("ApplicationInsightsSamplingOptions:MaxTelemetryItemsPerSecond").Value, out int result))
                {
                    defaultSampling.MaxTelemetryItemsPerSecond = result;
                }

                builder.UseAdaptiveSampling( 
                    maxTelemetryItemsPerSecond:  defaultSampling.MaxTelemetryItemsPerSecond, 
                    includedTypes: (configuration.GetSection("ApplicationInsightsSamplingOptions:IncludedTypes").Value ?? defaultSampling.IncludedTypes) , 
                    excludedTypes: (configuration.GetSection("ApplicationInsightsSamplingOptions:ExcludedTypes").Value ?? defaultSampling.ExcludedTypes));
                builder.Build();
            });

            //to make "ObservabilityCustomLogging" section optional on the client's side
            var observabilityCustomLoggingSection = configuration.GetSection(ObservabilityCustomLoggingOptions.CONFIG_SECTION_NAME);
            var observabilityGenericSection = configuration.GetSection(ObservabilityGenericOptions.CONFIG_SECTION_NAME);            
            var observabilityCustomLogging = observabilityCustomLoggingSection.Get<ObservabilityCustomLoggingOptions>();            
            var observabilityGeneric = observabilityGenericSection.Get<ObservabilityGenericOptions>();
            services.Configure<ObservabilityCustomLoggingOptions>(configuration.GetSection(ObservabilityCustomLoggingOptions.CONFIG_SECTION_NAME));
            services.Configure<ObservabilityGenericOptions>(configuration.GetSection(ObservabilityGenericOptions.CONFIG_SECTION_NAME));
            if (observabilityCustomLogging.IsEnabled)
            {               
                services.AddSingleton<ICustomLogTableMgrDI, CustomLogTableMgrDI>();
                services.AddTransient<IHostedService, GracefulShutdown>();               
            }

            services.AddSingleton<ITelemetry, Telemetry_AppInsights>();
            services.AddSingleton<ILogObservability, Logging_AppInsights>();
            services.AddApplicationInsightsTelemetry(aiOptions);
          
                if (!string.IsNullOrEmpty(observabilityGeneric?.CloudRoleName))
                {
                    services.AddApplicationInsightsTelemetryProcessor<TelemetryProcessorExtender>();
                }           
            
            GetAppServerConfigurationDetails();
            
            services.AddHttpContextAccessor();

            return services;
        }

        public static IServiceCollection AddSourceOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ITelemetry, Telemetry_OpenTelemetry>();
            services.AddSingleton<ILogObservability, Logging_OpenTelemetry>();
 
            services.AddOpenTelemetryTracing(b =>
            {
                b
                .AddConsoleExporter()
                .AddAzureMonitorTraceExporter(o => {
 
                o.ConnectionString = configuration.GetValue<string>("ApplicationInsights:ConnectionString");
                })
                .AddSource(configuration["serviceName"])
                .SetSampler(new TraceIdRatioBasedSampler(1.00))
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: configuration["serviceName"], serviceVersion: configuration["serviceVersion"]))
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddSqlClientInstrumentation();
            });

            GetAppServerConfigurationDetails();

            return services; 
        }

        private static void GetAppServerConfigurationDetails()
        {
            UserConfiguration.AppName = Assembly.GetEntryAssembly().GetName().Name;
            UserConfiguration.AppVersion = Assembly.GetEntryAssembly().GetName().Version;
            UserConfiguration.HostName = Dns.GetHostEntry("").HostName;
        }
    }
}
