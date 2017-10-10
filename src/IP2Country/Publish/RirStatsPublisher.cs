using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Abp.IO;
using IP2Country.Events;
using IP2Country.Net;

namespace IP2Country.Publish
{
    public class RirStatsPublisher : IEventHandler<RirStatsSourceUpdatedEventData>, ITransientDependency
    {
        private readonly List<IRirStatsDataExporter> _rirStatsDataExporters;
        private readonly List<IRirStatsSource> _rirStatsSources;

        public RirStatsPublisher(IIocResolver iocResolver)
        {
            _rirStatsSources = iocResolver.ResolveAll<IRirStatsSource>().ToList();
            _rirStatsDataExporters = iocResolver.ResolveAll<IRirStatsDataExporter>().ToList();
        }

        public void HandleEvent(RirStatsSourceUpdatedEventData eventData)
        {
            if (_rirStatsDataExporters.Any())
            {
                var items = _rirStatsSources
                    .SelectMany(i => i.GetRirStats())
                    .OrderBy(i => i.BeginIPAddress, new IPAddressComparer())
                    .ToList();
                var dist = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dist");
                DirectoryHelper.CreateIfNotExists(dist);
                foreach (var exporter in _rirStatsDataExporters)
                {
                    var path = exporter.Export(items);
                    File.Copy(path, Path.Combine(dist, "latest" + exporter.Extension), true);
                    File.Delete(path);
                }
            }
        }
    }
}