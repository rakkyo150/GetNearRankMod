using System.IO;
using System.Text;

namespace GetNearRankMod.Override
{
    internal class ChangeSettings
    {
        string _PsPath = $".\\Libs\\GetNearRank-master\\GetNearRank.ps1";
        string s;
        string line;

        internal void OverrideSettings()
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
                if(s.Contains("$PP_FILTER      ="))
                {
                    s = $"$PP_FILTER      ={PluginConfig.Instance.PPFilter}";
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
