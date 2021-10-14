namespace GetNearRankMod
{
    public class PluginConfig
    {
        public static PluginConfig Instance { get; set; }
        public int RankRange { get; set; } = 3;
        public int PPFilter { get; set; } = 20;
    }
}
