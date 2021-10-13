using System;
using BeatSaberMarkupLanguage.MenuButtons;
using GetNearRankMod.Override;
using Zenject;

namespace GetNearRankMod.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        internal readonly MenuButton _menuButton;
        private readonly ChangeSettings _changeRankRange;
        private readonly ExecuteBatch _executeBatch;


        public MenuButtonManager(ChangeSettings changeRankRange, ExecuteBatch executeBatch)
        {
            Logger.log.Debug("test");
            _menuButton = new MenuButton("Near Rank Playlist", "Generate Near Rank Playlist", GeneratePlaylist, true);
            _changeRankRange = changeRankRange;
            _executeBatch = executeBatch;
        }

        public void Initialize()
        {
            Logger.log.Debug("test2");
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
        }

        public void GeneratePlaylist()
        {
            _changeRankRange.OverrideSettings();
            _executeBatch.ExecuteScrapeBatch();
        }
    }
}
