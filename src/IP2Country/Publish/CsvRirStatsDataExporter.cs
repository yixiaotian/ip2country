using System.Collections.Generic;
using System.IO;
using System.Text;
using IP2Country.Dto;
using IP2Country.Net;

namespace IP2Country.Publish
{
    public class CsvRirStatsDataExporter : IRirStatsDataExporter
    {
        public string Extension { get; } = ".csv";

        public string Export(IReadOnlyCollection<RirStatsListDto> items)
        {
            var path = Path.GetTempFileName();
            using (var fs = File.OpenWrite(path))
            {
                using (var sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    sw.WriteLine("BeginIPAddress,EndIPAddress,BeginIPAddressCode,EndIPAddressCode,Value,Registry,Country,Date,Status");
                    foreach (var item in items)
                    {
                        sw.WriteLine(string.Join(
                            ",",
                            item.BeginIPAddress,
                            item.EndIPAddress,
                            item.BeginIPAddress.GetCode(),
                            item.EndIPAddress.GetCode(),
                            item.Value,
                            item.Registry,
                            item.Country,
                            item.Date,
                            item.Status
                        ));
                    }
                }
            }
            return path;
        }
    }
}