using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Buffs.Whipfx
{
    public class Whipdeath : ModBuff
		{
			public override void SetStaticDefaults()
			{
				// DisplayName.SetDefault("Charm Buff!");
				// Description.SetDefault("30- Defense and youre burning oooo :0");
				Main.debuff[Type] = true;
				Main.pvpBuff[Type] = true;
				Main.buffNoTimeDisplay[Type] = true;
			}
			public override void Update(Player player, ref int buffIndex)
			{
				player.statDefense -= 30;
				Dust.NewDustPerfect(new Vector2(player.position.X + Main.rand.Next(player.width), player.position.Y + player.height - Main.rand.Next(7)), DustID.GoldFlame, Vector2.Zero);

			}
		}
	}

