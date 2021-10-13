using System;
using System.Diagnostics;
using System.IO;

namespace GetNearRankMod.Managers
{
    internal class ExecuteBatch
    {
        string _fileName = "NearRank";

        string _batchPath = $".\\Libs\\GetNearRank-master\\GetNearRank.bat";
        string _firstPlaylistPath = $".\\playlist.bplist";
        string _beatSaberPlaylistPath = $".\\Playlists\\NearRank.bplist";

        ExecuteBatch()
        {
            DateTime dt = DateTime.Now;
            _fileName = dt.ToString("yyyyMMdd") + "-range" + PluginConfig.Instance.RankRange.ToString();
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

            File.Move(_firstPlaylistPath, _beatSaberPlaylistPath);
            SongCore.Loader.Instance.RefreshSongs(false);

            Logger.log.Debug("Finish Generating Playlist");
        }
    }
}
