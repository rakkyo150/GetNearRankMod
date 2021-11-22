using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GetNearRankMod.Utilities
{
    internal class GetUsersData
    {
        string baseRank;

        string basePageEndpoint = $"https://scoresaber.com/api/players?page={PluginConfig.Instance.YourLocalRankPageNumber}&countries=jp";
        string nextPageEndpoint = $"https://scoresaber.com/api/players?page={PluginConfig.Instance.YourLocalRankPageNumber + 1}&countries=jp";
        string previouPageEndpoint = $"https://scoresaber.com/api/players?page={PluginConfig.Instance.YourLocalRankPageNumber - 1}&countries=jp";

        // 新API対応

        public async Task<Dictionary<string, string>> GetCountryRankData(string endpoint)
        {
            // Key=>rank Value=>id
            var countryRankAndId = new Dictionary<string, string>();

            Logger.log.Debug("Start HttpClient");
            HttpClient client = new HttpClient();
            var response=await client.GetAsync(endpoint);
            string jsonStr = await response.Content.ReadAsStringAsync();

            dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonStr);

            foreach (var jd in jsonDynamic)
            {
                string rank = JsonConvert.SerializeObject(jd["countryRank"]);
                string id = JsonConvert.SerializeObject(jd["id"]).Replace($"\"","");

                countryRankAndId.Add(rank, id);
            }

            if (countryRankAndId == null)
            {
                Logger.log.Info($"No Country Rank and Id at {endpoint}");
            }

            return countryRankAndId;
        }

        public async Task<HashSet<string>> GetLocalTargetedId()
        {
            int lowRank;
            int highRank;
            bool otherPage = false;
            int branchRank = 0;
            HashSet<string> idHashSet = new HashSet<string>();

            Logger.log.Debug("Start GetLocalRankData");

            Dictionary<string, string> result = await GetCountryRankData(basePageEndpoint);
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
                    Dictionary<string, string> resultSecond = await GetCountryRankData(nextPageEndpoint);
                    foreach (var resultPair in resultSecond)
                    {
                        result.Add(resultPair.Key, resultPair.Value);
                    }
                }
                else
                {
                    Dictionary<string, string> resultSecond = await GetCountryRankData(nextPageEndpoint);
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
            var playScores = new Dictionary<Tuple<string, string>, string>();

            for (int i = 0; i + pageNumber <= pageRange; i++)
            {
                string playerScoresEndpoint = $"https://scoresaber.com/api/player/{id}/scores?page={i + pageNumber}";

                HttpClient client = new HttpClient();
                var response = await client.GetAsync(playerScoresEndpoint);
                string jsonString = await response.Content.ReadAsStringAsync();

                dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonString);

                foreach (var jsonScores in jsonDynamic)
                {
                    string songHash = JsonConvert.SerializeObject(jsonScores["leaderboard"]["songHash"]).Replace("\"","");
                    string difficulty = JsonConvert.SerializeObject(jsonScores["leaderboard"]["difficultyRaw"]);
                    string pp = JsonConvert.SerializeObject(jsonScores["score"]["pp"]);
                    var hashAndDifficulty = new Tuple<string, string>(songHash, difficulty);
                    playScores.Add(hashAndDifficulty, pp);
                }
            }

            if (playScores == null)
            {
                Logger.log.Info($"No {id}'s Play Scores");
            }

            return playScores;
        }
    }
}
