using System.Collections.Generic;

namespace Stellamod.Helpers
{
    public class AlphabetComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return x.CompareTo(y);
        }
    }
}
