using GetNearRankMod.Path;
using System;
using System.IO;

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
                catch (Exception ex)
                {
                    Logger.log.Error(ex.Message);
                }
            }
        }
    }
}
