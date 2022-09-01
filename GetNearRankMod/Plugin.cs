using BeatSaberMarkupLanguage.Settings;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using SiraUtil.Zenject;
using System;
using IPALogger = IPA.Logging.Logger;

namespace GetNearRankMod
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        public static string PluginName => "GetNearRankMod";

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public void Init(IPALogger logger, Config cfgProvider, Zenjector injector)
        {
            Instance = this;
            Logger.log = logger;
            PluginConfig.Instance = cfgProvider.Generated<PluginConfig>();
            BSMLSettings.instance.AddSettingsMenu("GetNearRankMod", $"GetNearRankMod.Settings.bsml", SettingController.instance);
            injector.Install<Installers.MenuButtonInstaller>(Location.Menu);

            // 万が一エラーだして止まるのはまずいので、丁寧に例外処理いれておく
            try
            {
                // 基本敵に初回だけ
                // ボタン押してからやInitializeからでもPlaylistManagerにフォルダが認識されないし認識させるのも難しい
                // グローバル変数も特にないのでメモリリークの心配も多分ない
                FolderMaker folderMaker = new FolderMaker();
                folderMaker.MakeGetNearRankModFolder();
            }
            catch(Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            Logger.log.Info("GetNearRankMod initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Logger.log.Debug("OnApplicationStart");

        }

        [OnExit]
        public void OnApplicationQuit()
        {
            Logger.log.Debug("OnApplicationQuit");

        }
    }
}
