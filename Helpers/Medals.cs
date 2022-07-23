using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.UI;
using Terraria.Localization;

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