using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Observability.LibraryBase.SettingOptions;
using Observability.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.ApplicationInsights;

namespace Observability.LibraryBase.Logging.Logging_Objects.BgLog
{
    internal class GracefulShutdown : IHostedService
    {
        //private readonly IHostApplicationLifetime appLifetime;
        private readonly IApplicationLifetime appLifeTime;
        private readonly ILogObservability logger;
        private readonly ICustomLogTableMgrDI _customLogTableMgrDI;
        public GracefulShutdown(ILogObservability logger,ICustomLogTableMgrDI customLogTableMgrDI, IApplicationLifetime appLifeTime)
        { //<--- INJECTED DEPENDENCY
            this.logger = logger;
            _customLogTableMgrDI = customLogTableMgrDI;
            this.appLifeTime = appLifeTime;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            appLifeTime.ApplicationStarted.Register(OnStarted);
            appLifeTime.ApplicationStopping.Register(OnStopping);
            appLifeTime.ApplicationStopped.Register(OnStopped);
            return Task.CompletedTask;
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
        private void OnStarted()
        {
            logger.LogInfo("The application has been started");
            // Perform post-startup activities here
       
        }
        private void OnStopping()
        {
            logger.LogInfo("The application is stopping");            
            _customLogTableMgrDI.FinalFlush();
            logger.TelemetryFlush();
            Thread.Sleep(5000);
            // Perform on-stopping activities here
        }
        private void OnStopped()
        {
            logger.LogInfo("The Application has Stopped");          
            _customLogTableMgrDI.FinalFlush();
            logger.TelemetryFlush();
            Thread.Sleep(5000);
            // Perform post-stopped activities here
        }
    }
}