using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;
using GetNearRankMod.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zenject;

namespace GetNearRankMod.Button
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
                List<Dictionary<MapData, PPData>> othersPlayResults = new List<Dictionary<MapData, PPData>>();

                iProgress.Report("Getting Your ID");
                await _usersDataGetter.GetYourId();

                // For test
                // PluginConfig.Instance.YourId = "76561198404774259";

                iProgress.Report("Getting Your Rank");
                int yourRank = await _usersDataGetter.GetYourCountryAndRank();


                iProgress.Report("Getting Rivals' Player Info");
                HashSet<PlayerInfo> targetedPlayerInfoList = await _usersDataGetter.GetTargetedPlayersInfo(yourRank);

                iProgress.Report("Getting Your Play Results");
                PlayerInfo yourPlayerInfo = new PlayerInfo(yourRank.ToString(), PluginConfig.Instance.YourId);
                Dictionary<MapData, PPData> yourPlayResult = await _usersDataGetter.GetPlayResult(yourPlayerInfo, PluginConfig.Instance.YourPageRange);

                iProgress.Report($"Getting Rivals' Play Results");
                foreach (PlayerInfo targetedPlayerInfo in targetedPlayerInfoList)
                {
                    Dictionary<MapData, PPData> otherPlayResult = await _usersDataGetter.GetPlayResult(targetedPlayerInfo, PluginConfig.Instance.OthersPageRange);
                    othersPlayResults.Add(otherPlayResult);
                }

                iProgress.Report("Making Lower PP Map List");
                Dictionary<MapData, PPData> MapDataAndPPDiffList = _playlistMaker.MakeLowerPPMapList(othersPlayResults, yourPlayResult);

                iProgress.Report("Sort Playlist by PPDiff");
                Dictionary<MapData, PPData> SortedMapDataAndPPDiffList = _playlistMaker.SortPlaylist(MapDataAndPPDiffList);

                iProgress.Report("Making Playllist");
                _playlistMaker.MakePlaylist(SortedMapDataAndPPDiffList);

                SongCore.Loader.Instance.RefreshSongs(false);

                iProgress.Report("Success!");
            }
            catch (Exception e)
            {
                iProgress.Report("Error");
                Logger.log.Error("Error Message: " + e.Message);
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
            Logger.log.Debug(debug);
        }
    }
}
