using System;
using System.Collections.Generic;
using IP2Country.Dto;

namespace IP2Country
{
    public interface IRirStatsSource
    {
        String Name { get; }
        Guid Id { get; }
        bool TryUpdate(out string version);
        IEnumerable<RirStatsListDto> GetRirStats();
    }
}