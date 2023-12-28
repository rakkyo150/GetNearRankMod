namespace GetNearRankMod
{
    internal class PlayerInfo
    {
        private readonly string _rank;
        private readonly string _id;

        internal PlayerInfo(string rank, string id)
        {
            _rank = rank;
            _id = id;
        }

        internal string Rank => _rank;
        internal string Id => _id;
    }
}
