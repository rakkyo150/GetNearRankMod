﻿using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage.Util;
using GetNearRankMod.Views;
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
            injector.Install<Installers.MenuButtonInstaller>(Location.Menu);

            if (!PluginConfig.Instance.FolderMode)
            {
                Logger.log.Info("GetNearRankMod initialized.");
                return;
            }

            try
            {
                // ボタン押してからやInitializeからでもPlaylistManagerにフォルダが認識されないし認識させるのも難しい
                // グローバル変数も特にないのでメモリリークの心配も多分ない
                FolderMaker folderMaker = new FolderMaker();
                folderMaker.MakeGetNearRankModFolder();
            }
            catch (Exception ex)
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
            MainMenuAwaiter.MainMenuInitializing += AddSettingMenu;
            Logger.log.Debug("OnApplicationStart");
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            MainMenuAwaiter.MainMenuInitializing -= AddSettingMenu;
            Logger.log.Debug("OnApplicationQuit");
        }

        private void AddSettingMenu()
        {
            BSMLSettings.Instance.AddSettingsMenu("GetNearRankMod", $"GetNearRankMod.Views.Settings.bsml", SettingController.Instance);
        }
    }
}
