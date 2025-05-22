using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class MorrowShotArrow : ModProjectile
	{
		private ref float Timer => ref Projectile.ai[0];
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("morrowshotarrow");
			Main.projFrames[Projectile.type] = 1;
			ProjectileID.Sets.TrailCacheLength[Type] = 8;
			ProjectileID.Sets.TrailingMode[Type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.width = 24;
			Projectile.height = 24;
			Projectile.light = 1.5f;
			Projectile.friendly = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = true;
			Projectile.maxPenetrate = 1;
		}

        public override void AI()
        {
            base.AI();
			Timer++;
			if(Timer % 6 == 0)
			{
				if(Main.rand.NextBool(2))
					Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<SalfaceDust>(), Vector2.Zero, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
				else
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.CadetBlue, Scale: Main.rand.NextFloat(0.5f, 1f));
            }

			float maxDetectDistance = 196;
			NPC closest = ProjectileHelper.FindNearestEnemy(Projectile.position, maxDetectDistance);
			if(closest != null)
			{
				Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, 3);
			}

			Projectile.velocity *= 1.01f;
			Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightPink, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.LoveTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.CadetBlue;
            glowColor.A = 0;
            glowColor *= Timer / 30f;
            for (int i = 0; i < 3; i++)
            {
                Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.CadetBlue,
                outerGlowColor: Color.Black, duration: 25, baseSize: Main.rand.NextFloat(0.06f, 0.12f));

            for (float i = 0; i < 8; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                rot += Main.rand.NextFloat(-0.5f, 0.5f);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.CadetBlue,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.04f, 0.12f),
                    duration: Main.rand.NextFloat(15, 25));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
        }
    }
}
