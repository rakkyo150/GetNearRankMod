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
            Progress<string> progress = new Progress<string>(onProgressChanged);

            _menuButton = new MenuButton(_buttonName, "Generate Near Rank Playlist", async () =>  await GeneratePlaylist(progress), true);
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

        public async Task GeneratePlaylist(IProgress<string> iProgress)
        {
            try
            {
                var othersPlayResults = new List<Dictionary<Tuple<string, string>, string>>();

                iProgress.Report("Getting Your ID");
                _getUsersData.GetYourId();


                iProgress.Report("Getting Your Local Rank");
                var yourCountryRank = await _getUsersData.GetYourCountryRank();


                iProgress.Report("Getting Rivals' ID");
                var targetedIdList = await _getUsersData.GetLocalTargetedId(yourCountryRank);
                

                Logger.log.Debug("Start Getting YourPlayResult");
                iProgress.Report("Getting Your Play Results");
                var yourPlayResult = await _getUsersData.GetPlayResult(PluginConfig.Instance.YourId, PluginConfig.Instance.YourPageRange);

                iProgress.Report($"Getting Rivals' Play Results");
                foreach (string targetedId in targetedIdList)
                {
                    Logger.log.Debug("Targeted Id " + targetedId);
                    var otherPlayResult = await _getUsersData.GetPlayResult(targetedId, PluginConfig.Instance.OthersPageRange);
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
            catch(Exception e)
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
