using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage.MenuButtons;
using GetNearRankMod.Utilities;
using Zenject;

namespace GetNearRankMod.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly string _buttonName = "Near Rank Playlist";
        internal readonly MenuButton _menuButton;
        private readonly GetUsersData _getUsersData;
        private readonly PlaylistMaker _playlistMaker;

        public MenuButtonManager(GetUsersData getUsersData, PlaylistMaker playlistMaker)
        {
            _menuButton = new MenuButton(_buttonName, "Generate Near Rank Playlist", GeneratePlaylist, true);
            _getUsersData = getUsersData;
            _playlistMaker = playlistMaker;
        }

        public void Initialize()
        {
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
            var othersPlayResults = new List<Dictionary<Tuple<string, string>, string>>();

            _menuButton.Text = "Generating...";

            var myCountryRank = await _getUsersData.GetMyCountryRank();

            var targetedIdList = await _getUsersData.GetLocalTargetedId(myCountryRank);

            var myPlayResult = await _getUsersData.GetPlayResult(PluginConfig.Instance.YourId, PluginConfig.Instance.YourPageRange);
            foreach (string targetedId in targetedIdList)
            {
                Logger.log.Debug("Targeted Id " + targetedId);
                var otherPlayResult = await _getUsersData.GetPlayResult(targetedId, PluginConfig.Instance.OthersPageRange);
                othersPlayResults.Add(otherPlayResult);
            }

            var hashAndDifficultyList = _playlistMaker.MakeLowerPPMapList(othersPlayResults, myPlayResult);
            _playlistMaker.MakePlaylist(hashAndDifficultyList);

            SongCore.Loader.Instance.RefreshSongs(false);

            _menuButton.Text = "Finish!";
            await Task.Delay(3000);
            _menuButton.Text = _buttonName;
        }
    }
}
