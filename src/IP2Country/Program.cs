using System;
using System.IO;
using Abp;
using Abp.Castle.Logging.Log4Net;
using Abp.PlugIns;
using Castle.Facilities.Logging;
using Topshelf;

namespace IP2Country
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var log4NetConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
            HostFactory.Run(
                cfg =>
                {
                    cfg.UseLog4Net(log4NetConfigFile);
                    cfg.Service<AbpBootstrapper>(config =>
                    {
                        config.ConstructUsing(AbpBootstrapper.Create<IP2CountryModule>);
                        config.WhenStarted(bootstrapper =>
                        {
                            //bootstrapper.PlugInSources.AddFolder(AppDomain.CurrentDomain.BaseDirectory);
                            bootstrapper
                                .IocManager
                                .IocContainer
                                .AddFacility<LoggingFacility>(
                                    loggingFacility => loggingFacility.UseAbpLog4Net()
                                        .WithConfig(log4NetConfigFile)
                                );
                            bootstrapper.Initialize();
                        });
                        config.WhenStopped(bootstrapper => bootstrapper.Dispose());
                    });

                    cfg.RunAsLocalSystem();
                    cfg.SetServiceName(typeof(Program).Namespace);
                    cfg.SetDisplayName(typeof(Program).Namespace);
                }
            );
        }
    }
}