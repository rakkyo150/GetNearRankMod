using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GetNearRankMod.Utilities
{
    internal class UsersDataGetter
    {
        // 新API対応

        IPlatformUserModel _userModel;

        UsersDataGetter(IPlatformUserModel userModel)
        {
            _userModel = userModel;
        }

        public async Task GetYourId()
        {
            var userId = await _userModel.GetUserInfo();
            PluginConfig.Instance.YourId = userId.platformUserId;

            Logger.log.Debug("Your Id " + PluginConfig.Instance.YourId);
        }

        public async Task<int> GetYourJapanRank()
        {
            int yourCountryRank;

            string yourBasicPlayerInfoEndpoint = $"https://scoresaber.com/api/player/{PluginConfig.Instance.YourId}/basic";

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(yourBasicPlayerInfoEndpoint);
            string jsonString = await response.Content.ReadAsStringAsync();

            dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonString);

            string yourCountryRankStr = JsonConvert.SerializeObject(jsonDynamic["countryRank"]);
            yourCountryRank = Int32.Parse(yourCountryRankStr);

            Logger.log.Debug("Your Local Rank " + yourCountryRank);

            return yourCountryRank;
        }

        public async Task<HashSet<PlayerInfo>> GetJapanTargetedPlayerInfo(int yourCountryRank)
        {
            int yourCountryRankPageNumber = 1 + (yourCountryRank - 1) / 50;

            string basePageEndpoint = $"https://scoresaber.com/api/players?page={yourCountryRankPageNumber}&countries=jp";
            string lowerRankPageEndpoint = $"https://scoresaber.com/api/players?page={yourCountryRankPageNumber + 1}&countries=jp";
            string higherRankPageEndpoint = $"https://scoresaber.com/api/players?page={yourCountryRankPageNumber - 1}&countries=jp";

            int lowRank;
            int highRank;
            bool otherPage = false;
            int branchRank = 0;

            HashSet<PlayerInfo> allPlayerInfoOnRankPage = await GetJapanesePlayerInfo(basePageEndpoint);
            HashSet<PlayerInfo> targetdPlayerInfo = new HashSet<PlayerInfo>();

            lowRank = yourCountryRank + PluginConfig.Instance.RankRange;
            highRank = yourCountryRank - PluginConfig.Instance.RankRange;

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
                if (branchRank < yourCountryRank)
                {
                    HashSet<PlayerInfo> otherPagesResult = await GetJapanesePlayerInfo(higherRankPageEndpoint);
                    foreach (PlayerInfo otherPagesPlayerInfo in otherPagesResult)
                    {
                        allPlayerInfoOnRankPage.Add(otherPagesPlayerInfo);
                    }
                }
                else
                {
                    HashSet<PlayerInfo> otherPagesResult = await GetJapanesePlayerInfo(lowerRankPageEndpoint);
                    foreach (PlayerInfo otherPagesPlayerInfo in otherPagesResult)
                    {
                        allPlayerInfoOnRankPage.Add(otherPagesPlayerInfo);
                    }
                }
            }

            foreach (PlayerInfo playerInfo in allPlayerInfoOnRankPage)
            {
                if (highRank <= int.Parse(playerInfo.Rank) && int.Parse(playerInfo.Rank) <= lowRank)
                {
                    // トッププレイヤー用
                    if (playerInfo.Rank == yourCountryRank.ToString()) continue;

                    targetdPlayerInfo.Add(playerInfo);
                }
            }

            return targetdPlayerInfo;
        }

        public async Task<Dictionary<MapData, PPData>> GetPlayResult(PlayerInfo playerInfo, int pageRange)
        {
            int topScoresPageNumber = 1;
            var playResults = new Dictionary<MapData, PPData>();

            for (int i = 0; i + topScoresPageNumber <= pageRange; i++)
            {
                string playerScoresEndpoint = $"https://scoresaber.com/api/player/{playerInfo.Id}/scores?page={i + topScoresPageNumber}";

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(playerScoresEndpoint);
                string jsonString = await response.Content.ReadAsStringAsync();

                dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonString);

                foreach (var jsonScores in jsonDynamic["playerScores"])
                {
                    string mapHash = JsonConvert.SerializeObject(jsonScores["leaderboard"]["songHash"]).Replace("\"", "");
                    string difficulty = JsonConvert.SerializeObject(jsonScores["leaderboard"]["difficulty"]["difficultyRaw"]);
                    MapData mapData = new MapData(mapHash, difficulty);
                    string pp = JsonConvert.SerializeObject(jsonScores["score"]["pp"]);
                    PPData pPData = new PPData(pp);

                    playResults.Add(mapData, pPData);
                }
            }

            if (playResults == null)
            {
                Logger.log.Info($"No {playerInfo.Id}'s Play Scores");
            }

            return playResults;
        }

        public async Task<HashSet<PlayerInfo>> GetJapanesePlayerInfo(string rankPagesEndpoint)
        {
            HashSet<PlayerInfo> playerInfoList = new HashSet<PlayerInfo>();

            HttpClient client = new HttpClient();
            var response = await client.GetAsync(rankPagesEndpoint);
            string jsonStr = await response.Content.ReadAsStringAsync();

            dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonStr);

            foreach (var jd in jsonDynamic["players"])
            {
                string rank = JsonConvert.SerializeObject(jd["countryRank"]);
                string id = JsonConvert.SerializeObject(jd["id"]).Replace($"\"", "");
                PlayerInfo playerInfo = new PlayerInfo(rank, id);

                playerInfoList.Add(playerInfo);
            }

            if (playerInfoList == null)
            {
                Logger.log.Info($"No Country Rank and Id at {rankPagesEndpoint}");
            }

            return playerInfoList;
        }
    }
}
