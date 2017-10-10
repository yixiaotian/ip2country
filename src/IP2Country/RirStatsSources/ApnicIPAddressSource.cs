namespace IP2Country.RirStatsSources
{
    public class ApnicIPAddressSource : RirStatsSourceBase
    {
        public override string Name { get; } = "Apnic";
        public override string Url { get; } = "http://ftp.apnic.net/pub/stats/apnic/delegated-apnic-extended-latest";
    }
}