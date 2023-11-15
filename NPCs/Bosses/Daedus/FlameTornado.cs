using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Daedus
{
    public class FlameTornado : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Sigil");
			Main.projFrames[Projectile.type] = 9;
		}
		
		public override void SetDefaults()
		{
			Projectile.friendly = false;
			Projectile.width = 1008;
			Projectile.height = 356;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 36;
			Projectile.scale = 1f;
            Projectile.hostile = true;
            Projectile.damage = 100;
		
			
		}
		public float Timer
		{
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
        public override void AI()
        {
		
			Vector3 RGB = new(0.89f, 2.53f, 2.55f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);


			Timer++;
			if (Timer == 2)
            {
				Projectile.scale *= 0.98f;
				
				float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, -10f);
				float speedYa = Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.00f + Main.rand.Next(-10, -9) * 1.0f;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 180, speedXa * 0.3f, speedYa * 0.5f, ProjectileID.DD2BetsyFireball, 20, 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 183, speedXa * -0.3f, speedYa * 0.5f, ProjectileID.DD2BetsyFireball, 20, 0f, Projectile.owner, 0f, 0f);


				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 180, speedXa * 0.6f, speedYa * 0.8f, ProjectileID.DD2BetsyFireball, 40, 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 183, speedXa * -0.6f, speedYa * 0.8f, ProjectileID.DD2BetsyFireball, 40, 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 180, speedXa * 0.8f, speedYa * 1f, ProjectileID.DD2BetsyFireball, 30, 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 183, speedXa * -0.8f, speedYa * 1f, ProjectileID.DD2BetsyFireball, 30, 0f, Projectile.owner, 0f, 0f);

				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 180, speedXa * 1f, speedYa * 1.2f, ProjectileID.DD2BetsyFireball, 40, 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 183, speedXa * -1f, speedYa * 1.2f, ProjectileID.DD2BetsyFireball, 40, 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 180, speedXa * 1.1f, speedYa * 1.2f, ProjectileID.DD2BetsyFireball, 40, 0f, Projectile.owner, 0f, 0f);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + 504, Projectile.position.Y + 183, speedXa * -1.1f, speedYa * 1.2f, ProjectileID.DD2BetsyFireball, 40, 0f, Projectile.owner, 0f, 0f);
			}
			

			if (Projectile.scale == 0f)
            {
				Projectile.Kill();
            }


			
		}
		
		public override bool PreAI()
		{
			Projectile.tileCollide = false;
			if (++Projectile.frameCounter >= 4)
			{
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= 9)
				{
					Projectile.frame = 0;
				}
			}
			return true;

			
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return new Color(200, 200, 200, 0) * (1f - Projectile.alpha / 50f);
		}
		

	}
}