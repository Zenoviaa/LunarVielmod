using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.StringnNeedles.Alcadiz
{
    public class Needle : ModProjectile
	{
		// public override void SetStaticDefaults() => DisplayName.SetDefault("Needlechain");

		public override void SetDefaults()
		{
			Projectile.width = 40;
			Projectile.height = 34;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.DamageType = DamageClass.Summon;
			Projectile.tileCollide = false;
		}

		public override bool PreAI()
		{
			Vector2 position = Projectile.Center + Vector2.Normalize(Projectile.velocity) * 10;

			ProjectileExtras.FlailAI(Projectile.whoAmI);
			return false;
		}
		public override bool PreDraw(ref Color lightColor)
		{
			ProjectileExtras.DrawChain(Projectile.whoAmI, Main.player[Projectile.owner].MountedCenter, "Stellamod/Projectiles/StringnNeedles/Alcadiz/Needle_Chain");
			ProjectileExtras.DrawAroundOrigin(Projectile.whoAmI, lightColor);
			return false;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			float speedX = Projectile.velocity.X * 1;
			float speedY = Projectile.velocity.Y * 1;

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ProjectileID.StyngerShrapnel, Projectile.damage * 1, 0f, Projectile.owner, 0f, 0f);
			if (target.life <= 0)
			{
				for (int i = 0; i < 20; i++)
				{
					int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BubbleBurst_White, 0f, -2f, 0, default, 2f);
					Main.dust[num].noGravity = true;
					Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
					Main.dust[num].scale *= .25f;
					if (Main.dust[num].position != Projectile.Center)
						Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
				}
			}
		}
	}
}