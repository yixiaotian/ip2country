using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using IP2Country.Events;
using IP2Country.Net;

namespace IP2Country.Publish
{
    public class RirStatsPublisher : IEventHandler<RirStatsSourceUpdatedEventData>, ITransientDependency
    {
        private readonly List<IRirStatsSource> _rirStatsSources;

        public RirStatsPublisher(IIocResolver iocResolver)
        {
            _rirStatsSources = iocResolver.ResolveAll<IRirStatsSource>().ToList();
        }

        public void HandleEvent(RirStatsSourceUpdatedEventData eventData)
        {
            var items = _rirStatsSources
                .SelectMany(i => i.GetRirStats())
                .OrderBy(i => i.BeginIPAddress, new IPAddressComparer())
                .ToList();

        }
    }
}