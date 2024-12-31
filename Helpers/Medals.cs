using Microsoft.Xna.Framework;
using Terraria.GameContent.UI;

namespace Stellamod.Helpers
{
    public class Medals : CustomCurrencySingleCoin
	{
		public Medals(int coinItemID, long currencyCap, string CurrencyTextKey) : base(coinItemID, currencyCap)
		{
			this.CurrencyTextKey = CurrencyTextKey;
			CurrencyTextColor = Color.LightGoldenrodYellow;
		}

		
	}
}