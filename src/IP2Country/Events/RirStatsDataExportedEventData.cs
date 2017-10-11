using Abp.Events.Bus;

namespace IP2Country.Events
{
    public class RirStatsDataExportedEventData : EventData
    {
        public string Extension { get; set; }
        public string Path { get; set; }
    }
}