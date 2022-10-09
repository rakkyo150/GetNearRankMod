using GetNearRankMod.Views;
using IPA.Config.Stores;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace GetNearRankMod
{
    [IPA.Config.Stores.Attributes.NotifyPropertyChanges]
    public class PluginConfig : INotifyPropertyChanged, IDisposable
    {
        public event Action<PluginConfig> OnReloaded;
        public event Action<PluginConfig> ChangedEvent;
        public event PropertyChangedEventHandler PropertyChanged;

        public static PluginConfig Instance { get; set; }

        public PluginConfig()
        {
            OnReloaded += OnReloadMethod;
            PropertyChanged += PropertyChangedMethod;
        }

        private void PropertyChangedMethod(object sender, PropertyChangedEventArgs e)
        {
            // Logger.log.Info($"{e.PropertyName} property changed");
            SettingController.Instance.SyncWithPluginConfig(e.PropertyName);
        }

        private void OnReloadMethod(PluginConfig pluginConfig)
        {
            // Logger.log.Info("Reload Method");
        }

        // IPAによって自動的に呼び出される
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// This is called whenever BSIPA reads the config from disk (including when file changes are detected).
        /// </summary>
        public virtual void OnReload()
        {
            // Do stuff after config is read from disk.
            this.OnReloaded?.Invoke(this);
        }

        /// <summary>
        /// Call this to force BSIPA to update the config file. This is also called by BSIPA if it detects the file was modified.
        /// </summary>
        public virtual void Changed()
        {
            // Do stuff when the config is changed.
            this.ChangedEvent?.Invoke(this);
        }

        /// <summary>
        /// Call this to have BSIPA copy the values from <paramref name="other"/> into this config.
        /// </summary>
        public virtual void CopyFrom(PluginConfig other)
        {
            // This instance's members populated from other
        }

        public void Dispose()
        {
            OnReloaded -= OnReloadMethod;
            PropertyChanged -= PropertyChangedMethod;
        }

        public string YourId { get; set; } = string.Empty;
        public string YourCountry { get; set; } = string.Empty;
        public int RankRange { get; set; } = 3;
        public int PPFilter { get; set; } = 20;
        public int YourPageRange { get; set; } = 10;
        public int OthersPageRange { get; set; } = 3;
        public bool GlobalMode { get; set; } = false;
        public bool FolderMode { get; set; } = true;
        public string FolderName { get; set; } = "GetNearRankMod";
    }
}
