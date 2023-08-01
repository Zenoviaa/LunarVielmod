using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Weapons.Gun
{
	public class DragonBolt : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Dragon Bolt");
		}

		public override void SetDefaults()
		{
			Projectile.width = 4;       //projectile width
			Projectile.height = 4;  //projectile height
			Projectile.friendly = true;      //make that the projectile will not damage you
			Projectile.DamageType = DamageClass.Ranged;         // 
			Projectile.tileCollide = true;   //make that the projectile will be destroed if it hits the terrain
			Projectile.penetrate = 2;      //how many npc will penetrate
			Projectile.timeLeft = 70;   //how many time projectile projectile has before disepire // projectile light
			Projectile.ignoreWater = true;
			Projectile.alpha = 255;
			Projectile.aiStyle = -1;
			Projectile.hide = true;
		}

		public override void AI()
		{
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

			if (Projectile.alpha < 170)
			{
				for (int i = 0; i < 10; i++)
				{
					float x = Projectile.position.X - 3 - Projectile.velocity.X / 10f * i;
					float y = Projectile.position.Y - 3 - Projectile.velocity.Y / 10f * i;
					int num = Dust.NewDust(new Vector2(x, y), 2, 2, 180);
					Main.dust[num].alpha = Projectile.alpha;
					Main.dust[num].velocity = Vector2.Zero;
                    Main.dust[num].noGravity = true;
					int num2 = Dust.NewDust(new Vector2(x, y), 2, 2, 226);
					Main.dust[num2].alpha = Projectile.alpha;
					Main.dust[num2].velocity = Vector2.Zero;
					Main.dust[num2].noGravity = true;
				}
			}
			Projectile.alpha = Math.Max(0, Projectile.alpha - 25);
		}
	}
}