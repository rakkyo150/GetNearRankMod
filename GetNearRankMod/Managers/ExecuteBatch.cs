using System;
using System.Diagnostics;
using System.IO;

namespace GetNearRankMod.Managers
{
    internal class ExecuteBatch
    {
        public string _fileName { get; set; } = "NearRank";

        private readonly string _batchPath = $".\\Libs\\GetNearRank-master\\GetNearRank.bat";
        private readonly string _firstPlaylistPath = $".\\playlist.bplist";
        internal readonly string _beatSaberPlaylistPath = $".\\Playlists\\NearRank.bplist";

        ExecuteBatch()
        {
            DateTime dt = DateTime.Now;
            _fileName = dt.ToString("yyyyMMdd") + "-RankRange" + PluginConfig.Instance.RankRange.ToString()+
                "-PPFilter"+ PluginConfig.Instance.PPFilter;
            _beatSaberPlaylistPath = $".\\Playlists\\{_fileName}.bplist";
        }

        public void ExecuteScrapeBatch()
        {           
            Process p = new Process();
            p.StartInfo.FileName = _batchPath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.UseShellExecute = false;
            Logger.log.Debug("Start Generating Playlist");
            p.Start();
            p.WaitForExit();
            p.Close();

            if (File.Exists(_beatSaberPlaylistPath))
            {
                File.Delete(_beatSaberPlaylistPath);
            }
            File.Move(_firstPlaylistPath, _beatSaberPlaylistPath);

            Logger.log.Debug("Finish Generating Playlist");
        }
    }
}
