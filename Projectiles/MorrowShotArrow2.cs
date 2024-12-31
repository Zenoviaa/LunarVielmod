using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class MorrowShotArrow2 : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("morrowshotarrow");
			Main.projFrames[Projectile.type] = 1;
			//The recording mode
		}
		public override void SetDefaults()
		{
			Projectile.damage = 12;
			Projectile.width = 12;
			Projectile.height = 24;
			Projectile.light = 1.5f;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.maxPenetrate = 20;
			Projectile.CloneDefaults(ProjectileID.BoneArrow);
			Projectile.ownerHitCheck = true;
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public override bool PreAI()
		{
			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, 0f, 0f);
			Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<SalfaceDust>(), 0f, 0f);
			Main.dust[dust].scale = 0.5f;
			return true;
		}
		public override void AI()
		{
			Timer++;
			if (Timer > 3)
			{
				// Our timer has finished, do something here:
				// Main.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.
				float speedX = Projectile.velocity.X;
				float speedY = Projectile.velocity.Y;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), speedX, speedY, speedX, speedY * 2, ModContent.ProjectileType<SalfaCircle>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
			}
			float distance = 0f;
			Player player = Main.player[Projectile.owner];

			if (Timer <= 1)
				Projectile.position = player.position + Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Projectile.ai[1])) * distance;
			if (Timer < 2)
				Projectile.velocity = 20 * Vector2.Normalize(Projectile.DirectionTo(Main.MouseWorld));

			Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
			Projectile.rotation = Projectile.velocity.ToRotation();

			if (Projectile.velocity.Y > 16f)
				Projectile.velocity.Y = 16f;
			
			// Since our sprite has an orientation, we need to adjust rotation to compensate for the draw flipping.
			if (Projectile.spriteDirection == -1)
				Projectile.rotation += MathHelper.Pi;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public override void OnKill(int timeLeft)
		{
			for (int i = 0; i < 10; i++)
			{
				Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.KryptonMoss, 0, 0, 100, Color.Blue, 0.8f);
				dust.noGravity = true;
				dust.velocity *= 4f;
				dust.scale = 0.2f;
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			Projectile.Kill();
		}
	}
}
