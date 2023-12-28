using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace GetNearRankMod.Utilities
{
    internal class UsersDataGetter
    {
        private readonly IPlatformUserModel _userModel;

        private UsersDataGetter(IPlatformUserModel userModel)
        {
            _userModel = userModel;
        }

        public async Task GetYourId()
        {
            UserInfo userId = await _userModel.GetUserInfo(CancellationToken.None);
            PluginConfig.Instance.YourId = userId.platformUserId;

            Logger.log.Debug("Your Id " + PluginConfig.Instance.YourId);
        }

        public async Task<int> GetYourCountryAndRank()
        {
            string yourRankStr = string.Empty;
            int yourCountryRank = int.MinValue;

            string yourBasicPlayerInfoEndpoint = $"https://scoresaber.com/api/player/{PluginConfig.Instance.YourId}/basic";

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(yourBasicPlayerInfoEndpoint);
            string jsonString = await response.Content.ReadAsStringAsync();

            dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonString);

            PluginConfig.Instance.YourCountry = JsonConvert.SerializeObject(jsonDynamic["country"]);
            PluginConfig.Instance.YourCountry = PluginConfig.Instance.YourCountry.Replace("\"", "");
            Logger.log.Info(PluginConfig.Instance.YourCountry);

            if (PluginConfig.Instance.GlobalMode)
            {
                yourRankStr = JsonConvert.SerializeObject(jsonDynamic["rank"]);
                yourCountryRank = Int32.Parse(yourRankStr);
                Logger.log.Debug("Your Local Rank " + yourCountryRank);
            }
            else
            {
                yourRankStr = JsonConvert.SerializeObject(jsonDynamic["countryRank"]);
                yourCountryRank = Int32.Parse(yourRankStr);
                Logger.log.Debug("Your Global Rank " + yourCountryRank);
            }

            return yourCountryRank;
        }

        public async Task<HashSet<PlayerInfo>> GetTargetedPlayersInfo(int yourRank)
        {
            int yourRankPageNumber = 1 + (yourRank - 1) / 50;

            string basePageEndpoint = $"https://scoresaber.com/api/players?page={yourRankPageNumber}";
            string lowerRankPageEndpoint = $"https://scoresaber.com/api/players?page={yourRankPageNumber + 1}";
            string higherRankPageEndpoint = $"https://scoresaber.com/api/players?page={yourRankPageNumber - 1}";

            if (!PluginConfig.Instance.GlobalMode)
            {
                basePageEndpoint += $"&countries={PluginConfig.Instance.YourCountry}";
                lowerRankPageEndpoint += $"&countries={PluginConfig.Instance.YourCountry}";
                higherRankPageEndpoint += $"&countries={PluginConfig.Instance.YourCountry}";
            }

            int lowRank;
            int highRank;
            bool otherPage = false;
            int branchRank = 0;

            HashSet<PlayerInfo> allPlayersInfoOnRankPage = await GetPlayersInfo(basePageEndpoint);
            HashSet<PlayerInfo> targetdPlayersInfo = new HashSet<PlayerInfo>();

            lowRank = yourRank + PluginConfig.Instance.RankRange;
            highRank = yourRank - PluginConfig.Instance.RankRange;

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
                if (branchRank < yourRank)
                {
                    HashSet<PlayerInfo> otherPagesResult = await GetPlayersInfo(higherRankPageEndpoint);
                    foreach (PlayerInfo otherPagesPlayerInfo in otherPagesResult)
                    {
                        allPlayersInfoOnRankPage.Add(otherPagesPlayerInfo);
                    }
                }
                else
                {
                    HashSet<PlayerInfo> otherPagesResult = await GetPlayersInfo(lowerRankPageEndpoint);
                    foreach (PlayerInfo otherPagesPlayerInfo in otherPagesResult)
                    {
                        allPlayersInfoOnRankPage.Add(otherPagesPlayerInfo);
                    }
                }
            }

            foreach (PlayerInfo playerInfo in allPlayersInfoOnRankPage)
            {
                if (highRank <= int.Parse(playerInfo.Rank) && int.Parse(playerInfo.Rank) <= lowRank)
                {
                    // トッププレイヤー用
                    if (playerInfo.Rank == yourRank.ToString()) continue;

                    targetdPlayersInfo.Add(playerInfo);
                }
            }

            foreach (PlayerInfo playerInfo in targetdPlayersInfo)
            {
                Logger.log.Info("rank-" + playerInfo.Rank + "-" + "id-" + playerInfo.Id);
            }

            return targetdPlayersInfo;
        }

        public async Task<Dictionary<MapData, PPData>> GetPlayResult(PlayerInfo playerInfo, int pageRange)
        {
            int topScoresPageNumber = 1;
            Dictionary<MapData, PPData> playResults = new Dictionary<MapData, PPData>();

            for (int i = 0; i + topScoresPageNumber <= pageRange; i++)
            {
                string playerScoresEndpoint = $"https://scoresaber.com/api/player/{playerInfo.Id}/scores?page={i + topScoresPageNumber}";

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(playerScoresEndpoint);
                string jsonString = await response.Content.ReadAsStringAsync();

                dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonString);

                foreach (dynamic jsonScores in jsonDynamic["playerScores"])
                {
                    string songName = JsonConvert.SerializeObject(jsonScores["leaderboard"]["songName"]).Replace("\"", "");
                    string mapHash = JsonConvert.SerializeObject(jsonScores["leaderboard"]["songHash"]).Replace("\"", "");
                    string difficulty = JsonConvert.SerializeObject(jsonScores["leaderboard"]["difficulty"]["difficultyRaw"]);
                    MapData mapData = new MapData(songName, mapHash, difficulty);
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

        public async Task<HashSet<PlayerInfo>> GetPlayersInfo(string rankPagesEndpoint)
        {
            HashSet<PlayerInfo> playerInfoList = new HashSet<PlayerInfo>();

            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(rankPagesEndpoint);
            string jsonStr = await response.Content.ReadAsStringAsync();

            dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonStr);

            foreach (dynamic jd in jsonDynamic["players"])
            {
                string rank = string.Empty;

                if (PluginConfig.Instance.GlobalMode)
                {
                    rank = JsonConvert.SerializeObject(jd["rank"]);
                }
                else
                {
                    rank = JsonConvert.SerializeObject(jd["countryRank"]);
                }

                string id = JsonConvert.SerializeObject(jd["id"]).Replace($"\"", "");
                PlayerInfo playerInfo = new PlayerInfo(rank, id);

                playerInfoList.Add(playerInfo);
            }

            if (playerInfoList == null)
            {
                Logger.log.Info($"No Rank and Id at {rankPagesEndpoint}");
            }

            return playerInfoList;
        }
    }
}
