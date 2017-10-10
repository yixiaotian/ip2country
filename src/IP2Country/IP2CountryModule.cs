using System.Reflection;
using Abp.Castle.Logging.Log4Net;
using Abp.Dependency;
using Abp.Modules;
using Abp.Threading.BackgroundWorkers;
using IP2Country.BackgroundWorkers;
using IP2Country.Dependency;

namespace IP2Country
{
    [DependsOn(typeof(AbpCastleLog4NetModule))]
    public class IP2CountryModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new IP2CountryConventionalRegistrar());
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            if (Configuration.BackgroundJobs.IsJobExecutionEnabled)
            {
                using (var w = IocManager.ResolveAsDisposable<IBackgroundWorkerManager>())
                {
                    w.Object.Add(IocManager.Resolve<UpdateRirStatsSourceWorker>());
                }
            }
        }
    }
}