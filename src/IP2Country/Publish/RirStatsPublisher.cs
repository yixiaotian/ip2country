using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Events.Bus.Handlers;
using IP2Country.Dto;
using IP2Country.Events;

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
            var items = new List<RirStatsListDto>();
            foreach (var source in _rirStatsSources)
            {
                items.AddRange(source.GetRirStats());
            }
        }
    }
}