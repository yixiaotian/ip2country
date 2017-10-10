namespace IP2Country.RirStatsSources
{
    public class RipenccIPAddressSource : RirStatsSourceBase
    {
        public override string Name { get; } = "Ripencc";
        public override string Url { get; } = "http://ftp.ripe.net/pub/stats/ripencc/delegated-ripencc-extended-latest";
    }
}