using System;
using System.IO;
using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using Abp.IO;

namespace IP2Country.Events
{
    public class RirStatsDataExportedEventHandler : IEventHandler<RirStatsDataExportedEventData>, ITransientDependency
    {
        public void HandleEvent(RirStatsDataExportedEventData eventData)
        {
            var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dist");
            DirectoryHelper.CreateIfNotExists(dir);
            var path = $"latest{eventData.Extension}";
            File.Copy(eventData.Path, path, true);
        }
    }
}