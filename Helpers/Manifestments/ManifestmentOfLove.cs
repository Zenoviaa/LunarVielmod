using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;

namespace Stellamod.Helpers.Manifestments
{
    public class ManifestmentOfLove : CustomCurrencySingleCoin
    {
        public ManifestmentOfLove(int coinItemID, long currencyCap, string CurrencyTextKey) : base(coinItemID, currencyCap)
        {
            this.CurrencyTextKey = CurrencyTextKey;
            CurrencyTextColor = Color.SpringGreen;
        }


    }
}