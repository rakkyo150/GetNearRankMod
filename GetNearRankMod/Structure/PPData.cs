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

        internal void ChangePP(string pp)
        {
            _pP = pp;
        }
    }
}
