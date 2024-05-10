using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;

namespace Stellamod.Helpers.Manifestments
{
    public class ManifestmentOfBravery : CustomCurrencySingleCoin
    {
        public ManifestmentOfBravery(int coinItemID, long currencyCap, string CurrencyTextKey) : base(coinItemID, currencyCap)
        {
            this.CurrencyTextKey = CurrencyTextKey;
            CurrencyTextColor = Color.CadetBlue;
        }


    }
}