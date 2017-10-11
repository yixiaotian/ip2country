using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Abp;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Events.Bus.Handlers;
using Castle.Core.Logging;
using IP2Country.Extensions;
using IP2Country.Net;

namespace IP2Country.Events
{
    public class RirStatsSourceUpdatedEventHandler : IEventHandler<RirStatsSourceUpdatedEventData>, ITransientDependency
    {
        private readonly List<IRirStatsDataExporter> _rirStatsDataExporters;
        private readonly List<IRirStatsSource> _rirStatsSources;

        public RirStatsSourceUpdatedEventHandler(IIocResolver iocResolver)
        {
            _rirStatsSources = iocResolver.ResolveAll<IRirStatsSource>().ToList();
            _rirStatsDataExporters = iocResolver.ResolveAll<IRirStatsDataExporter>().ToList();
        }

        public IEventBus EventBus { private get; set; } = NullEventBus.Instance;

        public ILogger Logger { get; set; } = NullLogger.Instance;
        public Guid Id => GetType().FullName.AsGuid();

        public void HandleEvent(RirStatsSourceUpdatedEventData eventData)
        {
            if (_rirStatsDataExporters.Any())
            {
                Logger.DebugFormat("RirStatsDataExporter count {0} .", _rirStatsDataExporters.Count);
                var items = _rirStatsSources
                    .SelectMany(i => i.GetRirStats())
                    .OrderBy(i => i.BeginIPAddress, new IPAddressComparer())
                    .ToList();
                Logger.DebugFormat("Items {0} .", items.Count);
                var dir = Id.MapWorkspacePath();
                foreach (var exporter in _rirStatsDataExporters)
                {
                    Logger.InfoFormat("Export to {0} .", exporter.Extension);
                    string tmp = null;
                    try
                    {
                        tmp = exporter.Export(items);
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorFormat(e, "Export to {0} error.", exporter.Extension);
                    }
                    if (!string.IsNullOrWhiteSpace(tmp))
                    {
                        var dist = Path.Combine(
                            dir,
                            SequentialGuidGenerator
                                .Instance
                                .Create(SequentialGuidGenerator.SequentialGuidType.SequentialAsString)
                                .ToString("N") +
                            exporter.Extension
                        );
                        File.Copy(tmp, dist, true);
                        File.Delete(tmp);
                        var data = new RirStatsDataExportedEventData
                        {
                            Extension = exporter.Extension,
                            Path = dist
                        };
                        EventBus.Trigger(data);
                    }
                }
                items.Clear();
            }
        }
    }
}