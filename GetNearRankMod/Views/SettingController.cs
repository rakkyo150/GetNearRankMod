using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using Zenject;

namespace GetNearRankMod.Views
{
    [HotReload(RelativePathToLayout = @"Settings.bsml")]
    [ViewDefinition("GetNearRankMod.Views.Settings.bsml")]
    public class SettingController : BSMLAutomaticViewController
    {
        public static SettingController Instance { get; private set; }=new SettingController();
        
        public void SyncWithPluginConfig(string propetyName) 
        {
            System.Reflection.PropertyInfo[] thisPropetyInfo =typeof(SettingController).GetProperties();
            foreach(var propertyInfo in thisPropetyInfo)
            {
                if (propertyInfo.Name != propetyName) continue;

                NotifyPropertyChanged(propetyName);
                break;
            }
        }
        
        [UIValue("RankRange")]
        public int RankRange
        {
            get => PluginConfig.Instance.RankRange;
            set
            {
                PluginConfig.Instance.RankRange = value;
                NotifyPropertyChanged(nameof(RankRange));
            }
        }

        [UIValue("PPFilter")]
        public int PPFilter
        {
            get => PluginConfig.Instance.PPFilter;
            set
            {
                PluginConfig.Instance.PPFilter = value;
                NotifyPropertyChanged(nameof(PPFilter));
            }
        }
        [UIValue("YourPageRange")]
        public int YourPageRange
        {
            get => PluginConfig.Instance.YourPageRange;
            set
            {
                PluginConfig.Instance.YourPageRange = value;
                NotifyPropertyChanged(nameof(YourPageRange));
            }
        }
        [UIValue("OthersPageRange")]
        public int OthersPageRange
        {
            get => PluginConfig.Instance.OthersPageRange;
            set
            {
                PluginConfig.Instance.OthersPageRange = value;
                NotifyPropertyChanged(nameof(OthersPageRange));
            }
        }
        [UIValue("FolderMode")]
        public bool FolderMode
        {
            get => PluginConfig.Instance.FolderMode;
            set
            {
                PluginConfig.Instance.FolderMode = value;
                NotifyPropertyChanged(nameof(FolderMode));
            }
        }
    }
}
