using System.IO;
using GetNearRankMod.Managers;
using Newtonsoft.Json;

namespace GetNearRankMod.Override
{
    internal class ChangePlaylistTitleAndImage
    {
        ExecuteBatch _executeBatch;

        ChangePlaylistTitleAndImage(ExecuteBatch executeBatch)
        {
            _executeBatch = executeBatch;
        }

        public void AdjustPlaylist()
        {
            Logger.log.Debug("Start adjustint playlist");
            StreamReader sr = new StreamReader(_executeBatch._beatSaberPlaylistPath);
            dynamic json = JsonConvert.DeserializeObject(sr.ReadToEnd());
            sr.Close();
            json["playlistTitle"] = _executeBatch._fileName;
            string js = JsonConvert.SerializeObject(json, Formatting.Indented);
            StreamWriter sw = new StreamWriter(_executeBatch._beatSaberPlaylistPath, false);
            sw.WriteLine(js);
            sw.Close();
            Logger.log.Debug("Finish adjusting playlist");
        }
    }
}
