using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.StringnNeedles.Alcadiz
{
	public class Windeffect : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("windeffect");
			Main.projFrames[Projectile.type] = 14;
		}

		public override void SetDefaults()
		{
			Projectile.width = 48;
			Projectile.height = 48;
			Projectile.alpha = 75;
			Projectile.scale = 0.4f;
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
		}


		public override void AI()
		{
			Player player = Main.player[Projectile.owner];
			Projectile.Center = player.Center;
			if (++Projectile.frameCounter >= 5)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 14)
				{
					Projectile.frame = 0;
				}
			}
		
		}

    }
}