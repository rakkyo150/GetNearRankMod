using System;
using System.Diagnostics;
using System.IO;

namespace GetNearRankMod.Managers
{
    internal class ExecuteBatch
    {
        public string _fileName { get; set; } = "NearRank";
        public string _beatSaberPlaylistPath { get; set; } = $".\\Playlists\\NearRank.bplist";

        private readonly string _batchPath = $".\\Libs\\GetNearRank-master\\GetNearRankMod.bat";
        private readonly string _firstPlaylistPath = $".\\playlist.bplist";

        ExecuteBatch()
        {
            DateTime dt = DateTime.Now;
            _fileName = dt.ToString("yyyyMMdd") + "-RankRange" + PluginConfig.Instance.RankRange.ToString() +
                "-PPFilter" + PluginConfig.Instance.PPFilter;
            _beatSaberPlaylistPath = $".\\Playlists\\{_fileName}.bplist";
        }

        public void ExecuteScrapeBatch()
        {
            Process p = new Process();
            try
            {
                p.StartInfo.FileName = _batchPath;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.UseShellExecute = false;
                Logger.log.Debug("Start Generating Playlist");
                p.Start();
                p.WaitForExit();
                p.Close();
                Logger.log.Debug("Finish Generating Playlist");
            }
            catch(Exception e)
            {
                Logger.log.Debug(e);
                Logger.log.Critical(@"Something wrong with Beat Saber/Libs/GetNearRank-master/....");
                p.Close();
            }


            if (File.Exists(_firstPlaylistPath))
            {
                if (File.Exists(_beatSaberPlaylistPath))
                {
                    File.Delete(_beatSaberPlaylistPath);
                }
                File.Move(_firstPlaylistPath, _beatSaberPlaylistPath);
            }
            else
            {
                Logger.log.Critical($@"{_firstPlaylistPath} does not exist");
            }
        }
    }
}
