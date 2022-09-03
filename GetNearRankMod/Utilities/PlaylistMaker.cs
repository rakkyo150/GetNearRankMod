using GetNearRankMod.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GetNearRankMod.Utilities
{
    public class Playlist
    {
        public string playlistTitle { get; set; }
        public List<Songs> songs { get; set; }
        public string playlistAuthor { get; set; }
        public string image { get; set; }
    }

    public class Songs
    {
        public string songName { get; set; }
        public List<Difficulties> difficulties { get; set; }
        public string hash { get; set; }
    }

    public class Difficulties
    {
        public string name { get; set; }
        public string characteristic { get; set; }
        public string pPDiff { get; set; }
    }

    internal class PlaylistMaker
    {

        public Dictionary<MapData,PPData> MakeLowerPPMapList(List<Dictionary<MapData, PPData>> others, Dictionary<MapData, PPData> your)
        {
            // PP比較して負けてたらマップデータに追加

            Dictionary<MapData,PPData> mapDataAndPPDiffList = new Dictionary<MapData, PPData>();

            foreach (Dictionary<MapData, PPData> otherDictionary in others)
            {
                foreach (MapData otherMapData in otherDictionary.Keys)
                {
                    PPData pPDiff = new PPData((0).ToString());

                    // MapDataのオーバーライドしたメソッドがないと参照アドレスの比較になるのでダメ
                    if (your.ContainsKey(otherMapData))
                    {
                        PPData yourPP = your[otherMapData];
                        PPData otherPP = otherDictionary[otherMapData];

                        if (otherPP.PP - yourPP.PP < PluginConfig.Instance.PPFilter) continue;

                        if (!mapDataAndPPDiffList.ContainsKey(otherMapData))
                        {
                            pPDiff = new PPData((otherPP.PP - yourPP.PP).ToString());
                            mapDataAndPPDiffList.Add(otherMapData,pPDiff);
                            Logger.log.Debug($"{otherMapData.MapHash},{otherMapData.Difficulty},{pPDiff.PP}PP");
                            continue;
                        }

                        if (mapDataAndPPDiffList[otherMapData].PP < (otherPP.PP - yourPP.PP))
                        {
                            mapDataAndPPDiffList[otherMapData].ChangePP((otherPP.PP - yourPP.PP).ToString());
                        }
                    }

                    if (mapDataAndPPDiffList.ContainsKey(otherMapData)) continue;

                    pPDiff = new PPData((-1).ToString());
                    mapDataAndPPDiffList.Add(otherMapData,pPDiff);
                    Logger.log.Debug($"{otherMapData.MapHash},{otherMapData.Difficulty}, MissingData");
                }
            }

            return mapDataAndPPDiffList;
        }

        internal Dictionary<MapData, PPData> SortPlaylist(Dictionary<MapData, PPData> mapDataAndPPDiffList)
        {
            Dictionary<MapData, PPData> sortedMapDataAndPPDiff = new Dictionary<MapData, PPData>();
            sortedMapDataAndPPDiff = mapDataAndPPDiffList.OrderByDescending(x => x.Value.PP)
                .ToDictionary(pair=>pair.Key,pair=>pair.Value);

            return sortedMapDataAndPPDiff;
        }

        public void MakePlaylist(Dictionary<MapData,PPData> mapDataList)
        {
            // Playlist作成

            SameNamePlaylistDeleter sameNamePlaylistDeleter = new SameNamePlaylistDeleter();

            string _fileName;
            string _outputPath;
            string songName = "";
            string hash;
            string name = "";
            string characteristic = "";
            string pPDiff = "";
            string _jsonFinish;

            DateTime dt = DateTime.Now;
            _fileName = dt.ToString("yyyyMMdd") + "-RR" + PluginConfig.Instance.RankRange.ToString() +
            "-PF" + PluginConfig.Instance.PPFilter + "-YPR" + PluginConfig.Instance.YourPageRange +
            "-OPR" + PluginConfig.Instance.OthersPageRange;

            sameNamePlaylistDeleter.DeleteSameNamePlaylist(_fileName + ".bplist");

            if (Directory.Exists(BSPath.GetNearRankModFolderPath) && PluginConfig.Instance.FolderMode)
            {
                _outputPath = Path.Combine(BSPath.GetNearRankModFolderPath, $"{_fileName}.bplist");
            }
            else
            {
                _outputPath = Path.Combine(BSPath.PlaylistsPath, $"{_fileName}.bplist");
            }

            Playlist playlistEdit = new Playlist();
            playlistEdit.playlistTitle = _fileName;
            playlistEdit.playlistAuthor = "GetNearRankMod";
            playlistEdit.image = GetCoverImage();
            List<Songs> songsList = new List<Songs>();

            foreach (KeyValuePair<MapData, PPData> mapDataAndPPDiff in mapDataList)
            {
                songName = mapDataAndPPDiff.Key.SongName;
                hash = mapDataAndPPDiff.Key.MapHash;
                name = SetDifficulty(name, mapDataAndPPDiff.Key);
                characteristic = SetCaracteristic(characteristic, mapDataAndPPDiff.Key);
                pPDiff = mapDataAndPPDiff.Value.PP.ToString();

                Songs songs = new Songs();
                List<Difficulties> difficultiesList = new List<Difficulties>();
                Difficulties difficulties = new Difficulties();

                difficulties.name = name;
                difficulties.characteristic = characteristic;
                difficulties.pPDiff = pPDiff;
                difficultiesList.Add(difficulties);
                songs.songName = songName;
                songs.difficulties = difficultiesList;
                songs.hash = hash;
                songsList.Add(songs);
            }

            playlistEdit.songs = songsList;

            _jsonFinish = JsonConvert.SerializeObject(playlistEdit, Formatting.Indented);


            StreamWriter wr = new StreamWriter(_outputPath, false);
            wr.WriteLine(_jsonFinish);
            wr.Close();
        }

        private static string SetCaracteristic(string characteristic, MapData mapData)
        {
            if (mapData.Difficulty.Contains("Standard"))
            {
                characteristic = "Standard";
            }
            else if (mapData.Difficulty.Contains("NoArrow"))
            {
                characteristic = "NoArrow";
            }
            else if (mapData.Difficulty.Contains("SingleSaber"))
            {
                characteristic = "SingleSaber";
            }

            return characteristic;
        }

        private static string SetDifficulty(string name, MapData mapData)
        {
            if (mapData.Difficulty.Contains("ExpertPlus"))
            {
                name = "expertPlus";
            }
            else if (mapData.Difficulty.Contains("Expert"))
            {
                name = "expert";
            }
            else if (mapData.Difficulty.Contains("Hard"))
            {
                name = "hard";
            }
            else if (mapData.Difficulty.Contains("Normal"))
            {
                name = "normal";
            }
            else if (mapData.Difficulty.Contains("Easy"))
            {
                name = "easy";
            }

            return name;
        }

        public string GetCoverImage()
        {
            try
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"GetNearRankMod.Resources.GetNearRankModImage.png"))
                {
                    var b = new byte[stream.Length];
                    stream.Read(b, 0, (int)stream.Length);
                    return Convert.ToBase64String(b);
                }
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message + "\n" + "Fail to complete GetCoverImage method");
            }

            return "";
        }
    }
}
