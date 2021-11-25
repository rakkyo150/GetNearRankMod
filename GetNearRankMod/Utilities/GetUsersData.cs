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
        // 新API対応

        public async Task<int> GetMyCountryRank()
        {
            int myCountryRank;

            string MyBasicPlayerDataEndpoint = $"https://scoresaber.com/api/player/{PluginConfig.Instance.YourId}/basic";

            Logger.log.Debug("Start GetMyCountryRankPageNumber");

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(MyBasicPlayerDataEndpoint);
            string jsonString = await response.Content.ReadAsStringAsync();

            dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonString);

            string myCountryRankStr = jsonDynamic["countryRank"];
            myCountryRank = Int32.Parse(myCountryRankStr);

            Logger.log.Debug("Your Local Rank " + myCountryRank);

            return myCountryRank;

        }
        
        public async Task<HashSet<string>> GetLocalTargetedId(int myCountryRank)
        {
            int myCountryRankPageNumber = 1 + myCountryRank / 50;

            string basePageEndpoint = $"https://scoresaber.com/api/players?page={myCountryRankPageNumber}&countries=jp";
            string lowerRankPageEndpoint = $"https://scoresaber.com/api/players?page={myCountryRankPageNumber + 1}&countries=jp";
            string higherRankPageEndpoint = $"https://scoresaber.com/api/players?page={myCountryRankPageNumber - 1}&countries=jp";

            int lowRank;
            int highRank;
            bool otherPage = false;
            int branchRank = 0;
            HashSet<string> idHashSet = new HashSet<string>();

            Logger.log.Debug("Start GetLocalRankData");

            Dictionary<string, string> result = await GetCountryRankData(basePageEndpoint);

            lowRank = myCountryRank + PluginConfig.Instance.RankRange;
            highRank = myCountryRank - PluginConfig.Instance.RankRange;

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
                if (myCountryRank >= branchRank)
                {
                    Dictionary<string, string> resultSecond = await GetCountryRankData(lowerRankPageEndpoint);
                    foreach (var resultPair in resultSecond)
                    {
                        result.Add(resultPair.Key, resultPair.Value);
                    }
                }
                else
                {
                    Dictionary<string, string> resultSecond = await GetCountryRankData(higherRankPageEndpoint);
                    foreach (var resultPair in resultSecond)
                    {
                        result.Add(resultPair.Key, resultPair.Value);
                    }
                }
            }

            for (int i = 0; lowRank - i > myCountryRank; i++)
            {
                idHashSet.Add(result[(lowRank - i).ToString()]);
                idHashSet.Add(result[(highRank + i).ToString()]);
            }


            // トッププレイヤー用
            if (idHashSet.Contains(result[myCountryRank.ToString()]))
            {
                idHashSet.Remove(result[myCountryRank.ToString()]);
            }

            return idHashSet;
        }

        public async Task<Dictionary<Tuple<string, string>, string>> GetPlayResult(string id, int pageRange)
        {
            // Key=>(songHash,difficulty),Value=>pp

            int topScoresPageNumber = 1;
            var playScores = new Dictionary<Tuple<string, string>, string>();

            for (int i = 0; i + topScoresPageNumber <= pageRange; i++)
            {
                string playerScoresEndpoint = $"https://scoresaber.com/api/player/{id}/scores?page={i + topScoresPageNumber}";

                HttpClient client = new HttpClient();
                var response = await client.GetAsync(playerScoresEndpoint);
                string jsonString = await response.Content.ReadAsStringAsync();

                dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonString);

                foreach (var jsonScores in jsonDynamic)
                {
                    string songHash = JsonConvert.SerializeObject(jsonScores["leaderboard"]["songHash"]).Replace("\"","");
                    string difficulty = JsonConvert.SerializeObject(jsonScores["leaderboard"]["difficulty"]["difficultyRaw"]);
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

        public async Task<Dictionary<string, string>> GetCountryRankData(string endpoint)
        {
            // Key=>rank Value=>id
            var countryRankAndId = new Dictionary<string, string>();

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(endpoint);
            string jsonStr = await response.Content.ReadAsStringAsync();

            dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonStr);

            foreach (var jd in jsonDynamic)
            {
                string rank = JsonConvert.SerializeObject(jd["countryRank"]);
                string id = JsonConvert.SerializeObject(jd["id"]).Replace($"\"", "");

                countryRankAndId.Add(rank, id);
            }

            if (countryRankAndId == null)
            {
                Logger.log.Info($"No Country Rank and Id at {endpoint}");
            }

            return countryRankAndId;
        }
    }
}
