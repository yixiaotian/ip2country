using System.Net;

namespace IP2Country.Dto
{
    public class RirStatsListDto
    {
        public string Raw { get; set; }
        public IPAddress BeginIPAddress { get; set; }
        public IPAddress EndIPAddress { get; set; }
        public string Registry { get; set; }

        public string Country { get; set; }

        public string Type { get; set; }
        public string Start { get; set; }

        public string Value { get; set; }

        public string Date { get; set; }

        public string Status { get; set; }
        public override string ToString()
        {
            return Raw;
        }
    }
}