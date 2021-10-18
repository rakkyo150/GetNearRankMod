using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using AngleSharp.Html.Parser;
using Newtonsoft.Json;

namespace GetNearRankMod.Utilities
{
    internal class GetUsersData
    {
        string baseRank;
        string baseUrl= $"https://scoresaber.com/global/{PluginConfig.Instance.YourLocalRankPageNumber}?country=jp";
        
        // ローカルランクのAPIはないのでスクレイピング

        public async Task<Dictionary<string, string>> GetLocalRankData()
        {
            Logger.log.Debug("Start HttpClient");
            HttpClient client = new HttpClient();
            var stream = await client.GetStreamAsync(baseUrl);
            var response = await client.GetAsync(baseUrl);

            string responseStr = await response.Content.ReadAsStringAsync();


            Logger.log.Debug(responseStr);
            Logger.log.Debug("Start Parser");
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(stream);
            var nodes = doc.QuerySelector("tbody");

            var rankAndId = new Dictionary<string, string>();
            foreach (var tr in nodes.QuerySelectorAll("tr"))
            {
                string rank;
                string id;
                var rankNodes=tr.GetElementsByClassName("rank");
                rank = rankNodes[0].TextContent.Replace("	", "").Replace("\n","");
                Logger.log.Debug(rank);
                var idNodes = tr.GetElementsByClassName("player");
                id = idNodes[0].QuerySelector("a").GetAttribute("href")
                    .Replace("/u/","");
                Logger.log.Debug(id);
                rankAndId.Add(rank, id);
            }

            foreach(var pair in rankAndId)
            {
                Logger.log.Debug(pair.Key);
                Logger.log.Debug(pair.Value);
            }

            return rankAndId;
        }

        public async Task<List<string>> GetLocalTargetedId()
        {
            int lowRank;
            int upRank;
            List<string> idList = new List<string>();

            Logger.log.Debug("start GetLocalRankData");
            Dictionary<string,string> result =await GetLocalRankData();
            var pair = result.FirstOrDefault(c => c.Value == PluginConfig.Instance.YourId);
            baseRank = pair.Key;
            Logger.log.Debug(baseRank);
            lowRank=Int32.Parse(baseRank.Replace("#","")) + PluginConfig.Instance.RankRange;
            upRank = Int32.Parse(baseRank.Replace("#","")) - PluginConfig.Instance.RankRange;
            Logger.log.Debug(lowRank.ToString());
            Logger.log.Debug(result[baseRank]);
            Logger.log.Debug(result["#22"]);
            for (int i = 0; lowRank-i > Int32.Parse(baseRank.Replace("#","")); i++)
            {
                idList.Add(result["#"+(lowRank-i).ToString()]);
                idList.Add(result["#"+(upRank+i).ToString()]);
            }

            foreach(string a in idList)
            {
                Logger.log.Debug(a);
            }

            return idList;
        }

        public async Task<Dictionary<Tuple<string,string>,string>> GetPlayResult(string id,int pageRange)
        {
            // Key=>(songHash,difficulty),Value=>pp
            
            int pageNumber=1;
            var playResult = new Dictionary<Tuple<string, string>,string>();
            
            for(int i = 0; i + pageNumber <= pageRange; i++)
            {
                string endpoint = $"https://new.scoresaber.com/api/player/{id}/scores/top/{i+pageNumber}";

                HttpClient client = new HttpClient();
                var response = await client.GetAsync(endpoint);
                string jsonString = await response.Content.ReadAsStringAsync();
                Logger.log.Debug(jsonString);
                dynamic jsonDynamic = JsonConvert.DeserializeObject(jsonString);
                foreach(var jsonScores in jsonDynamic["scores"])
                {
                    Logger.log.Debug(JsonConvert.SerializeObject(jsonScores));
                    string songHash = JsonConvert.SerializeObject(jsonScores["songHash"]).Replace("\"","");
                    string difficulty = JsonConvert.SerializeObject(jsonScores["difficultyRaw"]);
                    string pp = JsonConvert.SerializeObject(jsonScores["pp"]);
                    var hashAndDifficulty=new Tuple<string,string>(songHash, difficulty);
                    playResult.Add(hashAndDifficulty, pp);
                }
            }

            return playResult;
        }
    }
}
