using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs
{
    public class Elegance : ModBuff
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Petal Dance");
			// Description.SetDefault("Death Dance");
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}
		public override void Update(Player player, ref int buffIndex)
		{
			player.statDefense += 40;
			player.moveSpeed += 1f;
			player.maxRunSpeed += 1f;
			player.noKnockback = true;
			Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)), DustID.GoldCoin, Vector2.Zero);

		}
	}
}