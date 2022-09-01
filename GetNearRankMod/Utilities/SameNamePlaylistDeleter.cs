using GetNearRankMod.Static;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNearRankMod.Utilities
{
    // 別ディレクトリの同名プレイリストがあるとPlaylistManagerで消せなくなったりするので
    internal class SameNamePlaylistDeleter
    {
        internal void DeleteSameNamePlaylist(string playlistNameWithExtension)
        {
            FileInfo[] playlistsFilesInfo = new DirectoryInfo(BSPath.PlaylistsPath).GetFiles("*", SearchOption.AllDirectories);

            foreach (FileInfo playlistFileInfo in playlistsFilesInfo)
            {
                string fileName = playlistFileInfo.Name;

                if (fileName != playlistNameWithExtension) continue;
                
                try
                {
                    playlistFileInfo.Delete();
                }
                catch(Exception ex)
                {
                    Logger.log.Error(ex.Message);
                }
            }
        }
    }
}
