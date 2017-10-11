namespace IP2Country.RirStatsSources
{
    public class AfrinicIPAddressSource : RirStatsSourceBase
    {
        public override string Url { get; } =
            "http://ftp.afrinic.net/pub/stats/afrinic/delegated-afrinic-extended-latest";

        public override string Name { get; } = "Afrinic";
    }
}