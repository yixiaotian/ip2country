using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;

namespace IP2Country.BackgroundWorkers
{
    public class UpdateRirStatsSourceWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly List<IRirStatsSource> _rirStatsSources;

        public UpdateRirStatsSourceWorker(AbpTimer timer, IIocResolver iocResolver) : base(timer)
        {
            timer.Period = 1000 * 60 * 10;
            timer.RunOnStart = true;

            _rirStatsSources = iocResolver.ResolveAll<IRirStatsSource>().ToList();
        }

        protected override void DoWork()
        {
            var anyUpdate = false;
            foreach (var source in _rirStatsSources)
            {
                try
                {
                    Logger.DebugFormat("Update RirStatsSource:{0}", source.Name);
                    if (source.Update())
                    {
                        Logger.InfoFormat("RirStatsSource:{0} has new version.", source.Name);
                        anyUpdate = true;
                    }
                    else
                    {
                        Logger.DebugFormat("RirStatsSource:{0} update complated.", source.Name);
                    }
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat(e, "Update error , source {0} .", source.Name);
                }
            }
            if (anyUpdate)
            {
                Logger.Info("Some RirStatsSource is updated,we need rebuild something.");
            }
        }
    }
}