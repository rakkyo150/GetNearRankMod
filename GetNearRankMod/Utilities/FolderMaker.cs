using GetNearRankMod.Static;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace GetNearRankMod
{
    internal class FolderMaker
    {

        internal void MakeGetNearRankModFolder()
        {
            if (Directory.Exists(BSPath.GetNearRankModFolderPath)) return;

            Directory.CreateDirectory(BSPath.GetNearRankModFolderPath);
            FileInfo[] playlistsFilesInfo = new DirectoryInfo(BSPath.PlaylistsPath).GetFiles("*", SearchOption.TopDirectoryOnly);

            foreach (FileInfo playlistFileInfo in playlistsFilesInfo)
            {
                string fileName = playlistFileInfo.Name;

                // GetNearRankModで生成したプレイリストなら
                if (!DateTime.TryParseExact(fileName.Substring(0, 8), "yyyyMMdd", CultureInfo.CurrentCulture, DateTimeStyles.None, out _) ||
                    !ExistsGetNearRankPlaylistWords(fileName) ||
                    !fileName.EndsWith(".bplist"))
                {
                    continue;
                }

                if (!File.Exists(Path.Combine(BSPath.GetNearRankModFolderPath, $"{fileName}")))
                {
                    playlistFileInfo.MoveTo(Path.Combine(BSPath.GetNearRankModFolderPath, $"{fileName}"));
                    continue;
                }
                
                try
                {
                    File.Delete(Path.Combine(BSPath.GetNearRankModFolderPath, $"{fileName}"));
                    playlistFileInfo.MoveTo(Path.Combine(BSPath.GetNearRankModFolderPath, $"{fileName}"));
                }
                catch(Exception ex)
                {
                    Logger.log.Error(ex.Message);
                }
            }
        }

        private static bool ExistsGetNearRankPlaylistWords(string fileName)
        {
            List<string> oldVersionPlaylistNameList = new List<string> { "RankRange", "PPFilter" };
            List<string> playlistNameList = new List<string>() { "RR", "PF", "YPR", "OPR" };

            return (
                oldVersionPlaylistNameList.Where(x => fileName.Contains(x)).Count() == oldVersionPlaylistNameList.Count() ||
                playlistNameList.Where(x => fileName.Contains(x)).Count() == playlistNameList.Count()
            );
        }
    }
}
