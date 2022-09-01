using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNearRankMod.Static
{
    internal static class BSPath
    {
        internal static string PlaylistsPath = $".\\Playlists";
        internal static string GetNearRankModFolderPath = Path.Combine(PlaylistsPath, PluginConfig.Instance.FolderName);
    }
}
