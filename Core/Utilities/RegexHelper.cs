using System.Text;

namespace Stellamod.Core.Utilities
{
    public static class RegexHelper
    {
        public static string SplitByCapital(string input)
        {
            var splitName = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsUpper(c) && splitName.Length > 0 && splitName[^1] != ' ')
                {
                    splitName.Append(' ');
                }
                splitName.Append(c);
            }
            return splitName.ToString();
        }
    }
}
