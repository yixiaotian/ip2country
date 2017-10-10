namespace IP2Country.RirStatsSources
{
    public class LacnicIPAddressSource : RirStatsSourceBase
    {
        public override string Name { get; } = "Lacnic";
        public override string Url { get; } = "http://ftp.lacnic.net/pub/stats/lacnic/delegated-lacnic-extended-latest";
    }
}