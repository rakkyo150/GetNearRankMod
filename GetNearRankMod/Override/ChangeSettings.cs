﻿using System.IO;

namespace GetNearRankMod.Override
{
    internal class ChangeSettings
    {
        string _PsPath = $".\\Libs\\GetNearRank-master\\GetNearRank.ps1";

        internal void OverrideSettings()
        {
            string line = "";

            Logger.log.Debug("Start override rank range");

            if (File.Exists(_PsPath))
            {
                StreamReader sr = new StreamReader(_PsPath);
                while (sr.Peek() >= 0)
                {
                    string s = sr.ReadLine();
                    if (s.Contains("$GET_RANK_RANGE ="))
                    {
                        s = $"$GET_RANK_RANGE ={PluginConfig.Instance.RankRange}";
                    }
                    if (s.Contains("$PP_FILTER      ="))
                    {
                        s = $"$PP_FILTER      ={PluginConfig.Instance.PPFilter}";
                    }
                    line += s + "\n";
                }
                sr.Close();
                
                StreamWriter wr = new StreamWriter(_PsPath, false);
                wr.WriteLine(line);
                wr.Close();

                Logger.log.Debug("Finish override rank range");
            }
            else
            {
                Logger.log.Critical($@"{ _PsPath} does not exist");
            }
        }
    }
}
