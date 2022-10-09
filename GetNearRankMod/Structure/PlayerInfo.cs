﻿namespace GetNearRankMod
{
    internal class PlayerInfo
    {
        private string _rank;
        private string _id;

        internal PlayerInfo(string rank, string id)
        {
            _rank = rank;
            _id = id;
        }

        internal string Rank => _rank;
        internal string Id => _id;
    }
}