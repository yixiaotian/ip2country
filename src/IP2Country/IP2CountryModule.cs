using System.Reflection;
using Abp.Castle.Logging.Log4Net;
using Abp.Modules;

namespace IP2Country
{
    [DependsOn(typeof(AbpCastleLog4NetModule))]
    public class IP2CountryModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}