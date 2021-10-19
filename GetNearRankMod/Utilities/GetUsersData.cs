using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;

namespace GetNearRankMod.Utilities
{
    internal class GetUsersData
    {
        string baseRank;

        string baseUrl = $"https://scoresaber.com/global/{PluginConfig.Instance.YourLocalRankPageNumber}?country=jp";
        string nextPageUrl = $"https://scoresaber.com/global/{PluginConfig.Instance.YourLocalRankPageNumber + 1}?country=jp";
        string previouPageUrl = $"https://scoresaber.com/global/{PluginConfig.Instance.YourLocalRankPageNumber - 1}?country=jp";

        // ローカルランクのAPIはないのでスクレイピング

        public async Task<Dictionary<string, string>> GetLocalRankData(string url)
        {
            // Key=>rank Value=>id

            Logger.log.Debug("Start HttpClient");
            HttpClient client = new HttpClient();
            var stream = await client.GetStreamAsync(url);

            Logger.log.Debug("Start Parser");
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(stream);
            var nodes = doc.QuerySelector("tbody");

            var rankAndId = new Dictionary<string, string>();
            foreach (var tr in nodes.QuerySelectorAll("tr"))
            {
                string rank;
                string id;

                var rankNodes = tr.GetElementsByClassName("rank");
                rank = rankNodes[0].TextContent.Replace("#", "").Replace("	", "").Replace("\n", "");

                var idNodes = tr.GetElementsByClassName("player");
                id = idNodes[0].QuerySelector("a").GetAttribute("href")
                    .Replace("/u/", "");

                rankAndId.Add(rank, id);
            }

            return rankAndId;
        }

        public async Task<HashSet<string>> GetLocalTargetedId()
        {
            int lowRank;
            int highRank;
            bool otherPage = false;
            int branchRank = 0;
            HashSet<string> idHashSet = new HashSet<string>();

            Logger.log.Debug("start GetLocalRankData");

            Dictionary<string, string> result = await GetLocalRankData(baseUrl);
            var pair = result.FirstOrDefault(c => c.Value == PluginConfig.Instance.YourId);
            baseRank = pair.Key;
            Logger.log.Debug("Your Local Rank " + baseRank);

            lowRank = Int32.Parse(baseRank) + PluginConfig.Instance.RankRange;
            highRank = Int32.Parse(baseRank) - PluginConfig.Instance.RankRange;

            // トッププレイヤー用
            if (highRank <= 0)
            {
                highRank = 1;
            }

            // ページを跨ぐ場合
            for (int i = 0; highRank + i < lowRank; i++)
            {
                if ((highRank + i) % 50 == 0)
                {
                    otherPage = true;
                    branchRank = highRank + i;
                }
            }

            if (otherPage)
            {
                if (Int32.Parse(baseRank) >= branchRank)
                {
                    Dictionary<string, string> resultSecond = await GetLocalRankData(nextPageUrl);
                    foreach (var resultPair in resultSecond)
                    {
                        result.Add(resultPair.Key, resultPair.Value);
                    }
                }
                else
                {
                    Dictionary<string, string> resultSecond = await GetLocalRankData(nextPageUrl);
                    foreach (var resultPair in resultSecond)
                    {
                        result.Add(resultPair.Key, resultPair.Value);
                    }
                }
            }

            for (int i = 0; lowRank - i > Int32.Parse(baseRank); i++)
            {
                idHashSet.Add(result[(lowRank - i).ToString()]);
                idHashSet.Add(result[(highRank + i).ToString()]);
            }


            // トッププレイヤー用
            if (idHashSet.Contains(result[baseRank]))
            {
                idHashSet.Remove(result[baseRank]);
            }

            return idHashSet;
        }

        public async Task<Dictionary<Tuple<string, string>, string>> GetPlayResult(string id, int pageRange)
        {
            // Key=>(songHash,difficulty),Value=>pp

            int pageNumber = 1;
            var playResult = new Dictionary<Tuple<string, string>, string>();

            for (int i = 0; i + pageNumber <= pageRange; i++)
            {
                string endpoint = $"https://new.scoresaber.com/api/player/{id}/scores/top/{i + pageNumber}";

                HttpClient client = new HttpClient();
                var response = await client.GetAsync(endpoint);
                string jsonString = await response.Content.ReadAsStringAsync();

                dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonString);

                if (jsonDynamic.error == null)
                {
                    foreach (var jsonScores in jsonDynamic["scores"])
                    {
                        string songHash = JsonConvert.SerializeObject(jsonScores["songHash"]).Replace("\"", "");
                        string difficulty = JsonConvert.SerializeObject(jsonScores["difficultyRaw"]);
                        string pp = JsonConvert.SerializeObject(jsonScores["pp"]);
                        var hashAndDifficulty = new Tuple<string, string>(songHash, difficulty);
                        playResult.Add(hashAndDifficulty, pp);
                    }
                }
            }

            if (playResult == null)
            {
                Logger.log.Info("No Play Result");
            }

            return playResult;
        }
    }
}
