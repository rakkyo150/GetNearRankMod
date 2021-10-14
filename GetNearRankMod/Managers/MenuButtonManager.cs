using System;
using BeatSaberMarkupLanguage.MenuButtons;
using GetNearRankMod.Override;
using Zenject;
using System.Threading.Tasks;

namespace GetNearRankMod.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly string _buttonName = "Near Rank Playlist";
        internal readonly MenuButton _menuButton;
        private readonly ChangeSettings _changeRankRange;
        private readonly ExecuteBatch _executeBatch;
        private readonly ChangePlaylistTitleAndImage _changePlaylistTitleAndImage;


        public MenuButtonManager(ChangeSettings changeRankRange, ExecuteBatch executeBatch,ChangePlaylistTitleAndImage changePlaylistTitleAndImage)
        {
            Logger.log.Debug("test");
            _menuButton = new MenuButton(_buttonName, "Generate Near Rank Playlist", GeneratePlaylist, true);
            _changeRankRange = changeRankRange;
            _executeBatch = executeBatch;
            _changePlaylistTitleAndImage = changePlaylistTitleAndImage;
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

        public async void GeneratePlaylist()
        {
            _menuButton.Text = "Generating...";
            await Task.Run(_changeRankRange.OverrideSettings);
            await Task.Run(_executeBatch.ExecuteScrapeBatch);
            _changePlaylistTitleAndImage.AdjustPlaylist();
            SongCore.Loader.Instance.RefreshSongs(false);
            _menuButton.Text = "Finish!";
            await Task.Delay(3000);
            _menuButton.Text = _buttonName;
        }
    }
}
