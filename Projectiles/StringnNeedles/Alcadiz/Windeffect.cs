using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.StringnNeedles.Alcadiz
{
    public class Windeffect : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("windeffect");
			Main.projFrames[Projectile.type] = 9;
		}
		public override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.alpha = 125;
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.scale = 0.7f;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.timeLeft = 172;
			DrawOriginOffsetX = -140;
			DrawOriginOffsetY = -38;
		}
		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.Center = player.Center;
			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 9)
				{
					Projectile.frame = 0;
				}
			}
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(0, 255, 255, 255) * (1f - Projectile.alpha / 255f);
		}
		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			overPlayers.Add(index);
		}
	}
}