using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using GetNearRankMod.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zenject;

namespace GetNearRankMod.Managers
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly string _buttonName = "Near Rank Playlist";
        internal readonly MenuButton _menuButton;
        private readonly UsersDataGetter _usersDataGetter;
        private readonly PlaylistMaker _playlistMaker;

        public MenuButtonManager(UsersDataGetter usersDataGetter, PlaylistMaker playlistMaker)
        {
            Progress<string> progress = new Progress<string>(onProgressChanged);

            _menuButton = new MenuButton(_buttonName, "Generate Near Rank Playlist", async () => await GeneratePlaylist(progress), true);
            _usersDataGetter = usersDataGetter;
            _playlistMaker = playlistMaker;
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            if (BSMLParser.IsSingletonAvailable && MenuButtons.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
        }

        public async Task GeneratePlaylist(IProgress<string> iProgress)
        {
            try
            {
                var othersPlayResults = new List<Dictionary<Tuple<string, string>, string>>();

                iProgress.Report("Getting Your ID");
                await _usersDataGetter.GetYourId();


                iProgress.Report("Getting Your Local Rank");
                var yourCountryRank = await _usersDataGetter.GetYourCountryRank();


                iProgress.Report("Getting Rivals' ID");
                var targetedIdList = await _usersDataGetter.GetLocalTargetedId(yourCountryRank);


                Logger.log.Debug("Start Getting YourPlayResult");
                iProgress.Report("Getting Your Play Results");
                var yourPlayResult = await _usersDataGetter.GetPlayResult(PluginConfig.Instance.YourId, PluginConfig.Instance.YourPageRange);

                iProgress.Report($"Getting Rivals' Play Results");
                foreach (string targetedId in targetedIdList)
                {
                    Logger.log.Debug("Targeted Id " + targetedId);
                    var otherPlayResult = await _usersDataGetter.GetPlayResult(targetedId, PluginConfig.Instance.OthersPageRange);
                    othersPlayResults.Add(otherPlayResult);
                }

                iProgress.Report("Making Lower PP Map List");
                var hashAndDifficultyList = _playlistMaker.MakeLowerPPMapList(othersPlayResults, yourPlayResult);

                iProgress.Report("Making Playllist");
                _playlistMaker.MakePlaylist(hashAndDifficultyList);

                SongCore.Loader.Instance.RefreshSongs(false);

                Logger.log.Debug("Success!");
                iProgress.Report("Success!");
            }
            catch (Exception e)
            {
                Logger.log.Debug("Error Message: " + e.Message);
                iProgress.Report("Error");
            }
            finally
            {
                await Task.Delay(3000);
                iProgress.Report(_buttonName);
            }
        }

        internal void onProgressChanged(string debug)
        {
            _menuButton.Text = debug;
        }
    }
}
