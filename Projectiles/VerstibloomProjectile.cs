using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class VerstibloomProjectile : ModProjectile
	{
		public int timer = 0;
		public bool launch = false;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verstibloom");
		}
		public override void SetDefaults()
		{
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.light = 0.5f;
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.timeLeft = 180;
			Projectile.friendly = true;
			Projectile.damage = 20;
			Projectile.scale = 1.1f;
			Projectile.penetrate = 5;
		}

		public override void OnKill(int timeLeft)
		{
			for (int num623 = 0; num623 < 25; num623++)
			{
				int num622 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.PlanteraBulb, 0f, 0f, 100, default, 2f);
				Main.dust[num622].noGravity = true;
				Main.dust[num622].velocity *= 1f;
				Main.dust[num622].scale = 1f;
			}
		}
		public override bool PreAI()
		{
			Projectile.tileCollide = true;
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CrystalPulse2, 0f, 0f);
			return base.PreAI();
		}
		public override void AI()
		{
			if (timer < 150)
				Projectile.velocity *= 0.97f;
			timer++;
			Vector2 targetPos = Projectile.Center;
			float targetDist = 1000f;
			bool targetAcquired = false;

			//loop through first 200 NPCs in Main.npc
			//this loop finds the closest valid target NPC within the range of targetDist pixels
			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].CanBeChasedBy(Projectile) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
				{
					float dist = Projectile.Distance(Main.npc[i].Center);
					if (dist < targetDist)
					{
						targetDist = dist;
						targetPos = Main.npc[i].Center;
						targetAcquired = true;
					}
				}
			}

			//change trajectory to home in on target
			Projectile.rotation += 0.2f;
			if (timer > 100)
			{
				Projectile.rotation += 0.1f;
				int num622 = Dust.NewDust(new Vector2(Projectile.position.X - Projectile.velocity.X, Projectile.position.Y - Projectile.velocity.Y), Projectile.width, Projectile.height, DustID.CrystalSerpent, 0f, 0f, 100, default, 2f);
				Main.dust[num622].noGravity = true;
				Main.dust[num622].scale = 1.5f;

			}
			if (targetAcquired && timer > 150 && !launch)
			{
				float homingSpeedFactor = 25f;
				Vector2 homingVect = targetPos - Projectile.Center;
				homingVect.Normalize();
				homingVect *= homingSpeedFactor;

				Projectile.velocity = homingVect;
				launch = true;
			}

			Vector3 RGB = new Vector3(1.5f, 0f, 1f);
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);
		}

		public override bool PreDraw(ref Color lightColor)
		{
			Vector2 drawOrigin = new(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
			for (int k = 0; k < Projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
				Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
				Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
	}
}