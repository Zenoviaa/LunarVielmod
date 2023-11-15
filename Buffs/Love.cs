using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class Love : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Zielie's Love");
			// Description.SetDefault("Experience her power of her love towards you!");
			Main.debuff[Type] = false;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = false;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 10;
			player.moveSpeed += 0.5f;
			player.maxRunSpeed += 0.5f;
			Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)), DustID.FireworkFountain_Red, Vector2.Zero);

		}
	}
}