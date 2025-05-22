using Microsoft.Xna.Framework;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.ID;


namespace Stellamod.Projectiles
{
    class MorrowValswaProj : SlasherProj
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Morrow Valswa");
			Main.projFrames[Projectile.type] = 2;
		}
		public override void Smash(Vector2 position)
		{
			Player player = Main.player[Projectile.owner];
			for (int k = 0; k <= 100; k++)
			{

				ShakeModSystem.Shake = 14;

				float speedX = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
				float speedY = -Projectile.velocity.Y * Main.rand.Next(40, 70) * 0.01f + Main.rand.Next(-20, 21) * 0.4f;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ProjectileID.Spark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
				Dust.NewDustPerfect(Projectile.oldPosition + new Vector2(Projectile.width / 2, Projectile.height / 2), DustID.Firework_Yellow, new Vector2(0, 1).RotatedByRandom(1) * Main.rand.NextFloat(-1, 1) * Projectile.ai[0] / 10f);
			}
		}
		public MorrowValswaProj() : base(54, 12, 20, -1, 58, 5, 8, 1.7f, 12f) { }
		public override void OnKill(int timeLeft)
		{
			// This code and the similar code above in OnTileCollide spawn dust from the tiles collided with. SoundID.Item10 is the bounce sound you hear.
			Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
			SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
			timeLeft = (int)480f;
		}
	}
}