namespace IP2Country.RirStatsSources
{
    public class ArinIPAddressSource : RirStatsSourceBase
    {
        public override string Name { get; } = "Arin";
        public override string Url { get; } = "http://ftp.arin.net/pub/stats/arin/delegated-arin-extended-latest";
    }
}