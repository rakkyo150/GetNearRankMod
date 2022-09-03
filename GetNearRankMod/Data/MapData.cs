using System.Xml.Linq;

namespace GetNearRankMod
{
    internal class MapData
    {
        private string _mapHash;
        private string _difficulty;

        internal MapData(string mapHash, string difficulty)
        {
            _mapHash = mapHash;
            _difficulty = difficulty;
        }

        internal string MapHash => _mapHash;
        internal string Difficulty => _difficulty;

        // https://mocotan.hatenablog.com/entry/2017/10/31/064738が参考になりました
        public override int GetHashCode()
        {
            // C#では^演算子でXOR(排他的論理和)でビット演算するのが普通
            return MapHash.GetHashCode() ^
                    Difficulty.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            // asでは、objがnullでも例外は発生せずにnullが入ってくる
            MapData other = obj as MapData;
            if (other == null) return false;

            // 何が同じときに、「同じ」と判断してほしいかを記述する
            return this.MapHash == other.MapHash &&
                    this.Difficulty == other.Difficulty;
        }
    }
}
