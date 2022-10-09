using System.IO;

namespace GetNearRankMod.Path
{
    internal static class BSPath
    {
        internal static string PlaylistsPath = $".\\Playlists";
        internal static string GetNearRankModFolderPath = System.IO.Path.Combine(PlaylistsPath, PluginConfig.Instance.FolderName);
    }
}
