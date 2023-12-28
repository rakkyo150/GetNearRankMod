using BeatSaberMarkupLanguage.MenuButtons;
using GetNearRankMod.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace GetNearRankMod.Button
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly string _buttonName = "Near Rank Playlist";
        internal readonly MenuButton _menuButton;
        private readonly UsersDataGetter _usersDataGetter;
        private readonly PlaylistMaker _playlistMaker;
        private TextMeshPro progressStatus;

        public MenuButtonManager(UsersDataGetter usersDataGetter, PlaylistMaker playlistMaker)
        {
            _menuButton = new MenuButton(_buttonName, "Generate Near Rank Playlist", async () => await GeneratePlaylist(), true);
            _usersDataGetter = usersDataGetter;
            _playlistMaker = playlistMaker;
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            MenuButtons.instance.UnregisterButton(_menuButton);
        }

        public async Task GeneratePlaylist()
        {
            IProgress<string> progress = new Progress<string>(onProgressChanged);
            progressStatus = new GameObject("progressStatus").AddComponent<TextMeshPro>();
            progressStatus.transform.position = new Vector3(0, 3.1f, 4.2f);
            progressStatus.fontSize = 5;
            progressStatus.alignment = TextAlignmentOptions.Center;
            try
            {
                List<Dictionary<MapData, PPData>> othersPlayResults = new List<Dictionary<MapData, PPData>>();

                await _usersDataGetter.GetYourId();

                // For test
                // PluginConfig.Instance.YourId = "76561198404774259";

                progress.Report("Getting Your Rank");
                int yourRank = await _usersDataGetter.GetYourCountryAndRank();


                progress.Report("Getting Rivals' Player Info");
                HashSet<PlayerInfo> targetedPlayerInfoList = await _usersDataGetter.GetTargetedPlayersInfo(yourRank);

                progress.Report("Getting Your Play Results");
                PlayerInfo yourPlayerInfo = new PlayerInfo(yourRank.ToString(), PluginConfig.Instance.YourId);
                Dictionary<MapData, PPData> yourPlayResult = await _usersDataGetter.GetPlayResult(yourPlayerInfo, PluginConfig.Instance.YourPageRange);

                progress.Report($"Getting Rivals' Play Results");
                foreach (PlayerInfo targetedPlayerInfo in targetedPlayerInfoList)
                {
                    Dictionary<MapData, PPData> otherPlayResult = await _usersDataGetter.GetPlayResult(targetedPlayerInfo, PluginConfig.Instance.OthersPageRange);
                    othersPlayResults.Add(otherPlayResult);
                }

                progress.Report("Making Lower PP Map List");
                Dictionary<MapData, PPData> MapDataAndPPDiffList = _playlistMaker.MakeLowerPPMapList(othersPlayResults, yourPlayResult);

                progress.Report("Sort Playlist by PPDiff");
                Dictionary<MapData, PPData> SortedMapDataAndPPDiffList = _playlistMaker.SortPlaylist(MapDataAndPPDiffList);

                progress.Report("Making Playllist");
                _playlistMaker.MakePlaylist(SortedMapDataAndPPDiffList);

                SongCore.Loader.Instance.RefreshSongs(false);

                progress.Report("Success!");
            }
            catch (Exception e)
            {
                progress.Report("Error");
                Logger.log.Error("Error Message: " + e.Message);
            }
            finally
            {
                await Task.Delay(3000);
                progress.Report(_buttonName);
                GameObject.Destroy(progressStatus);
            }
        }

        internal void onProgressChanged(string debug)
        {
            progressStatus.text = debug;
            Logger.log.Debug(debug);
        }
    }
}
