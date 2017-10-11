using System.Collections.Generic;
using IP2Country.Dto;

namespace IP2Country
{
    public interface IRirStatsDataExporter
    {
        string Extension { get; }
        string Export(IReadOnlyCollection<RirStatsListDto> items);
    }
}