using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;

namespace Stellamod.Helpers.Manifestments
{
    public class ManifestmentOfCommittment : CustomCurrencySingleCoin
    {
        public ManifestmentOfCommittment(int coinItemID, long currencyCap, string CurrencyTextKey) : base(coinItemID, currencyCap)
        {
            this.CurrencyTextKey = CurrencyTextKey;
            CurrencyTextColor = Color.Red;
        }


    }
}