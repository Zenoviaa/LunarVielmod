using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class FrostSwProj2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("FrostSwProj2");
			Main.projFrames[base.Projectile.type] = 6;
		}
		public override void SetDefaults()
		{
			Projectile.width = 56;
			Projectile.height = 56;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.tileCollide = false;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 11;
			Projectile.ignoreWater = true;
		}
		public override void AI()
		{
			Projectile.frameCounter++;
			if (Projectile.frameCounter >= 2)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			Vector2 angle = new Vector2(Projectile.ai[0], Projectile.ai[1]);
			Projectile.rotation = angle.ToRotation();
			Player player = Main.player[Projectile.owner];
			Projectile.position = player.Center + angle - new Vector2(Projectile.width / 2, Projectile.height / 2);
			if (Projectile.timeLeft == 2)
			{
				Projectile.friendly = false;
			}
			if (Projectile.timeLeft % 3 == 0)
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<CryoDust>());

			Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
		}
		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Vector2 angle = new(Projectile.ai[0], Projectile.ai[1]);
			angle *= 0.105f;
			Player player = Main.player[Projectile.owner];
			if (angle.Y > 0 && player.velocity.Y != 0)
			{
				angle *= 2.5f;
				player.velocity.Y = -angle.Y;
			}

			target.AddBuff(ModContent.BuffType<DeathMultiplier>(), 360);
			base.OnHitNPC(target, hit, damageDone);
		}
	}
}