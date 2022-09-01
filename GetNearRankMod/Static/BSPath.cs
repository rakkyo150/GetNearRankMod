using System.IO;

namespace GetNearRankMod.Static
{
    internal static class BSPath
    {
        internal static string PlaylistsPath = $".\\Playlists";
        internal static string GetNearRankModFolderPath = Path.Combine(PlaylistsPath, PluginConfig.Instance.FolderName);
    }
}
