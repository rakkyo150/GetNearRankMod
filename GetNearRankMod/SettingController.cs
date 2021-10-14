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
    }
}
