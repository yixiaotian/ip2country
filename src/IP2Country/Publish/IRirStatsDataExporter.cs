using System;
using System.Collections.Generic;
using IP2Country.Dto;

namespace IP2Country.Publish
{
    public interface IRirStatsDataExporter
    {
        String Extension { get; }
        string Export(IReadOnlyCollection<RirStatsListDto> items);
    }
}