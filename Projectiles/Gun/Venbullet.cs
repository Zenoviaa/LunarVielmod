using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    public class Venbullet : ModProjectile
	{
		private ref float Timer => ref Projectile.ai[0];
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Frost Shot");
			Main.projFrames[Projectile.type] = 1;
			//The recording mode
		}

		public override void SetDefaults()
		{
			Projectile.damage = 10;
			Projectile.width = 40;
			Projectile.height = 40;
			Projectile.light = 1.5f;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			Projectile.tileCollide = true;
			Projectile.penetrate = 2;
			Projectile.hostile = false;
		}

		public override void AI()
		{
			Projectile.velocity *= 0.98f;
			if(Projectile.velocity.Length() <= 1f)
			{
				Projectile.Kill();
			}
			Timer++;
			if (Timer == 3)
			{
				if(Main.myPlayer == Projectile.owner)
				{
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(),Projectile.Center, Vector2.Zero,
						ModContent.ProjectileType<VenShotIN>(), 0, 0f, Projectile.owner, 0f, 0f);
                }
	
				Timer = 0;
			}
		}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            FXUtil.GlowCircleBoom(target.Center,
              innerColor: Color.White,
              glowColor: Color.LightCyan,
              outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.06f);

            FXUtil.GlowCircleBoom(target.Center,
               innerColor: Color.White,
               glowColor: Color.LightPink,
               outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.03f);
			Projectile.velocity *= 0.25f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= 0.95f;
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
			for(float f = 0; f < 16; f++)
			{
				float progress = f / 16f;
				float rot = progress * MathHelper.ToRadians(360);
				Vector2 vel = rot.ToRotationVector2() * 4;
				Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), vel, newColor: Color.LightBlue);
			}
            FXUtil.GlowCircleBoom(Projectile.Center,
			  innerColor: Color.White,
			  glowColor: Color.LightCyan,
			  outerGlowColor: Color.DarkBlue, duration: 25, baseSize: 0.24f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Vinger"), Projectile.position);

        }
    }
}
