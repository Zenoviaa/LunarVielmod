using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
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
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
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



		}
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DimGray, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawDimLight(Projectile, 70, 30, 10, Color.OrangeRed, lightColor, 2);
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.OrangeRed, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.DarkGray, 0.5f).noGravity = true;
            }
            ShakeModSystem.Shake = 4;
			float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
			float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.position.X + speedXa + 30, target.position.Y + speedYa, speedXa * 0, speedYa * 0, ModContent.ProjectileType<HornetKaboom>(), (int)(Projectile.damage * 1.2), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/flameup"));
			
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.5f).noGravity = true;
            }
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 150, Color.DarkGray, 0.5f).noGravity = true;
            }
            ShakeModSystem.Shake = 4;
			float speedX = Projectile.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
			float speedY = Projectile.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX + 30, Projectile.position.Y + speedY, speedX * 0, speedY * 0, ModContent.ProjectileType<HornetKaboom>(), (int)(Projectile.damage * 1.2), 0f, Projectile.owner, 0f, 0f);
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/flameup"));
			Projectile.Kill();
			return false;
		}

    }
}