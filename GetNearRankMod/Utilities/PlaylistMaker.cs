using GetNearRankMod.Static;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        public List<Difficulties> difficulties { get; set; }
        public string hash { get; set; }
    }

    public class Difficulties
    {
        public string name { get; set; }
        public string characteristic { get; set; }
    }

    internal class PlaylistMaker
    {

        public List<MapData> MakeLowerPPMapList(List<Dictionary<MapData, PPData>> others, Dictionary<MapData, PPData> your)
        {
            // PP比較して負けてたらマップデータに追加

            List<MapData> mapDataList = new List<MapData>();

            foreach (var otherDictionary in others)
            {
                foreach (var keyDictionary in otherDictionary.Keys)
                {
                    if (your.ContainsKey(keyDictionary))
                    {
                        PPData yourPP = your[keyDictionary];
                        PPData otherPP = otherDictionary[keyDictionary];

                        if (otherPP.PP - yourPP.PP < PluginConfig.Instance.PPFilter)
                        {
                            continue;
                        }

                        if (!mapDataList.Contains(keyDictionary))
                        {
                            mapDataList.Add(keyDictionary);
                            Logger.log.Debug($"{keyDictionary.MapHash},{keyDictionary.Difficulty},{otherPP.PP - yourPP.PP}PP");
                        }
                    }
                    else
                    {
                        if (!mapDataList.Contains(keyDictionary))
                        {
                            mapDataList.Add(keyDictionary);
                            Logger.log.Debug($"{keyDictionary.MapHash},{keyDictionary.Difficulty}, MissingData");
                        }
                    }
                }
            }

            return mapDataList;
        }

        public void MakePlaylist(List<MapData> mapDataList)
        {
            // Playlist作成

            SameNamePlaylistDeleter sameNamePlaylistDeleter = new SameNamePlaylistDeleter();
            
            string _fileName;
            string _outputPath;
            string hash;
            string name = "";
            string characteristic = "";
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

            foreach (MapData mapData in mapDataList)
            {
                hash = mapData.MapHash;
                name = SetDifficulty(name, mapData);
                characteristic = SetCaracteristic(characteristic, mapData);

                Songs songs = new Songs();
                List<Difficulties> difficultiesList = new List<Difficulties>();
                Difficulties difficulties = new Difficulties();

                difficulties.name = name;
                difficulties.characteristic = characteristic;
                difficultiesList.Add(difficulties);
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
