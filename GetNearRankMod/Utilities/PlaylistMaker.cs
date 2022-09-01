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

        public List<Tuple<string, string>> MakeLowerPPMapList(List<Dictionary<Tuple<string, string>, string>> others, Dictionary<Tuple<string, string>, string> your)
        {
            // PP比較して負けてたらリストに追加

            var hashAndDifficultyList = new List<Tuple<string, string>>();


            foreach (var otherDictionary in others)
            {
                foreach (var keyDictionary in otherDictionary.Keys)
                {
                    if (your.ContainsKey(keyDictionary))
                    {
                        string yourPP = your[keyDictionary];
                        string otherPP = otherDictionary[keyDictionary];

                        if (double.Parse(otherPP) - double.Parse(yourPP) >= PluginConfig.Instance.PPFilter)
                        {
                            if (!hashAndDifficultyList.Contains(keyDictionary))
                            {
                                hashAndDifficultyList.Add(keyDictionary);
                                Logger.log.Debug($"{keyDictionary.Item1},{keyDictionary.Item2},{double.Parse(otherPP) - double.Parse(yourPP)}PP");
                            }
                        }
                    }
                    else
                    {
                        if (!hashAndDifficultyList.Contains(keyDictionary))
                        {
                            hashAndDifficultyList.Add(keyDictionary);
                            Logger.log.Debug($"{keyDictionary.Item1},{keyDictionary.Item2}, MissingData");
                        }
                    }
                }
            }

            return hashAndDifficultyList;
        }

        public void MakePlaylist(List<Tuple<string, string>> hashAndDifficultyList)
        {
            // Playlist作成

            string _fileName;
            string _beatSaberPlaylistPath;
            string hash;
            string name = "";
            string characteristic = "";
            string _jsonFinish;

            DateTime dt = DateTime.Now;
            _fileName = dt.ToString("yyyyMMdd") + "-RR" + PluginConfig.Instance.RankRange.ToString() +
            "-PF" + PluginConfig.Instance.PPFilter + "-YPR" + PluginConfig.Instance.YourPageRange +
            "-OPR" + PluginConfig.Instance.OthersPageRange;
            _beatSaberPlaylistPath = $".\\Playlists\\{_fileName}.bplist";

            Playlist playlistEdit = new Playlist();
            playlistEdit.playlistTitle = _fileName;
            playlistEdit.playlistAuthor = "GetNearRankMod";
            playlistEdit.image = GetCoverImage();
            List<Songs> songsList = new List<Songs>();

            foreach (var hashAndDifficulty in hashAndDifficultyList)
            {
                hash = hashAndDifficulty.Item1;
                if (hashAndDifficulty.Item2.Contains("ExpertPlus"))
                {
                    name = "expertPlus";
                }
                else if (hashAndDifficulty.Item2.Contains("Expert"))
                {
                    name = "expert";
                }
                else if (hashAndDifficulty.Item2.Contains("Hard"))
                {
                    name = "hard";
                }
                else if (hashAndDifficulty.Item2.Contains("Normal"))
                {
                    name = "normal";
                }
                else if (hashAndDifficulty.Item2.Contains("Easy"))
                {
                    name = "easy";
                }

                if (hashAndDifficulty.Item2.Contains("Standard"))
                {
                    characteristic = "Standard";
                }
                else if (hashAndDifficulty.Item2.Contains("NoArrow"))
                {
                    characteristic = "NoArrow";
                }
                else if (hashAndDifficulty.Item2.Contains("SingleSaber"))
                {
                    characteristic = "SingleSaber";
                }

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


            StreamWriter wr = new StreamWriter(_beatSaberPlaylistPath, false);
            wr.WriteLine(_jsonFinish);
            wr.Close();
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
