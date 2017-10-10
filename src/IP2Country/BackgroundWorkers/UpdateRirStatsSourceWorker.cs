using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using IP2Country.Events;
using IP2Country.Extensions;

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

        public IEventBus EventBus { private get; set; } = NullEventBus.Instance;

        protected override void DoWork()
        {
            var versions = new Dictionary<Guid, string>();
            foreach (var source in _rirStatsSources)
            {
                try
                {
                    Logger.DebugFormat("Update RirStatsSource:{0}", source.Name);
                    if (source.TryUpdate(out var version))
                    {
                        Logger.InfoFormat("RirStatsSource:{0} has new version : {1}.", source.Name, version);
                    }
                    else
                    {
                        Logger.DebugFormat("RirStatsSource:{0} are up to date.", source.Name);
                    }
                    versions[source.Id] = version;
                }
                catch (Exception e)
                {
                    Logger.ErrorFormat(e, "Update error , source {0} .", source.Name);
                }
            }
            var anyUpdate = false;
            foreach (var source in _rirStatsSources)
            {
                if (anyUpdate)
                {
                    break;
                }
                var path = Path.Combine(source.Id.MapWorkspacePath(), "latest.ver");
                if (!File.Exists(path) || versions[source.Id] != File.ReadAllText(path))
                {
                    anyUpdate = true;
                }
            }
            if (anyUpdate)
            {
                Logger.Info("Some RirStatsSource is updated,we need rebuild something.");
                EventBus.Trigger(new RirStatsSourceUpdatedEventData());
                foreach (var source in _rirStatsSources)
                {
                    var path = Path.Combine(source.Id.MapWorkspacePath(), "latest.ver");
                    File.WriteAllText(path, versions[source.Id]);
                }
            }
        }
    }
}