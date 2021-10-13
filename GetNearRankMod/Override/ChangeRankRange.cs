using System.IO;
using System.Text;

namespace GetNearRankMod.Override
{
    internal class ChangeRankRange
    {
        string _PsPath = $".\\Libs\\GetNearRank-master\\GetNearRank.ps1";
        string s;
        string line;

        internal void OverrideRange()
        {
            Logger.log.Debug("Start override rank range");
            
            StreamReader sr = new StreamReader(_PsPath);
            while (sr.Peek() >= 0)
            {
                s = sr.ReadLine();
                if (s.Contains("$GET_RANK_RANGE ="))
                {
                    s = $"$GET_RANK_RANGE ={PluginConfig.Instance.RankRange}";
                }
                line += s+"\n";
            }
            sr.Close();

            StreamWriter wr = new StreamWriter(_PsPath);
            wr.WriteLine(line);
            wr.Close();

            Logger.log.Debug("Finish override rank range");
        }
    }
}
