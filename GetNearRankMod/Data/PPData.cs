using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNearRankMod
{
    internal class PPData
    {
        private string _pP;
        
        internal PPData(string pP)
        {
            _pP = pP;
        }

        internal double PP => double.Parse(_pP);
    }
}
