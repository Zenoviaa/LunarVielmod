using Microsoft.Xna.Framework;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles
{
    public class HornetLob : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Flask of KABOOM");
		}

		public override void SetDefaults()
		{
			Projectile.width = 20;
			Projectile.height = 20;

			Projectile.aiStyle = 2;

			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.tileCollide = true;
		}
        public override void AI()
        {
			Vector3 RGB = new(1.00f, 0.37f, 0.30f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

			int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f);
			Main.dust[dust].scale = 0.6f;
		}


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			ShakeModSystem.Shake = 4;
			float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
			float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ProjectileID.DaybreakExplosion, Projectile.damage * 0, 0f, Projectile.owner, 0f, 0f);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedXa, Projectile.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<HornetKaboom>(), (int)(Projectile.damage * 1.2), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/flameup"));
			
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			ShakeModSystem.Shake = 4;
			float speedX = Projectile.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
			float speedY = Projectile.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0, speedY * 0, ProjectileID.DaybreakExplosion, Projectile.damage * 0, 0f, Projectile.owner, 0f, 0f);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX * 0, speedY * 0, ModContent.ProjectileType<HornetKaboom>(), (int)(Projectile.damage * 1.2), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/flameup"));
			Projectile.Kill();
			return false;
		}

    }
}