﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNearRankMod
{
    internal class MapData
    {
        private string _mapHash;
        private string _difficulty;

        internal MapData(string mapHash,string difficulty)
        {
            _mapHash = mapHash;
            _difficulty = difficulty;
        }


        internal string MapHash => _mapHash;
        internal string Difficulty => _difficulty; 
    }
}