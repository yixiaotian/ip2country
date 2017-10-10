using System.Collections.Generic;
using IP2Country.Dto;

namespace IP2Country.Publish
{
    public interface IIP2CountryDataExporter
    {
        string Export(IReadOnlyCollection<RirStatsListDto> items);
    }
}