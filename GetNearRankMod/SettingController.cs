using BeatSaberMarkupLanguage.Attributes;

namespace GetNearRankMod
{
    public class SettingController : PersistentSingleton<SettingController>
    {
        [UIValue("RankRange")]
        public int RankRange
        {
            get => PluginConfig.Instance.RankRange;
            set => PluginConfig.Instance.RankRange = value;
        }

        [UIValue("PPFilter")]
        public int PPFilter
        {
            get => PluginConfig.Instance.PPFilter;
            set => PluginConfig.Instance.PPFilter = value;
        }
        [UIValue("YourPageRange")]
        public int YourPageRange
        {
            get => PluginConfig.Instance.YourPageRange;
            set => PluginConfig.Instance.YourPageRange = value;
        }
        [UIValue("OthersPageRange")]
        public int OthersPageRange
        {
            get => PluginConfig.Instance.OthersPageRange;
            set => PluginConfig.Instance.OthersPageRange = value;
        }
    }
}
