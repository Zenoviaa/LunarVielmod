using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Fenix.Projectiles;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Stellamod.Core.Effects.ITrailer;

namespace Stellamod.Projectiles.Gun
{
    public class CinderFlameball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.height = 16;
            Projectile.width = 16;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.33f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            float radius = 1 / 6f;
            if (Main.rand.NextBool(12))
            {
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.OrangeRed,
                    outerColor = Color.DarkRed,
                    gravity=0f
                };
                DustParticle.Spawn(Projectile.Center, Vector2.Zero, spawnParams);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkOrange, Color.Transparent, completionRatio);
        }
        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(24, 0, completionRatio);
        }

        private Color GetTrailColor(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p);
            return trailColor;
        }

        private void DrawPixelFlameTrail(GraphicsDevice graphicsDevice)
        {
            RichLaserShader laserShader = RichLaserShader.Instance;
            laserShader.LaserColor = Color.Goldenrod;
            laserShader.InnerColor = Color.Red;
            laserShader.OuterColor = Color.DarkRed;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, GetTrailColor, GetTrailWidth, laserShader, Projectile.Size * 0.5f);
        }

        private void DrawPixelFlameBall(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D ballTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            Vector2 drawOrigin = ballTexture.Size() * 0.5f;
            Vector2 drawCenter = Projectile.Center - screenPos;

            Color glowColor = Color.OrangeRed;
            glowColor.A = 0;
            spriteBatch.Draw(ballTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.06f, SpriteEffects.None, 0);

            glowColor = Color.Goldenrod;
            glowColor.A = 0;
            spriteBatch.Draw(ballTexture, drawCenter, null, glowColor, 0, drawOrigin, Projectile.scale * 0.06f, SpriteEffects.None, 0);
        }
        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelFlameTrail);
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelFlameBall);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            float boomSize = Main.rand.NextFloat(0.03f, 0.04f);
            for (float n = 0; n < 2f; n++)
            {
                var spawnParams = new DustParticleSpawnParams();
                spawnParams.innerColor = Color.OrangeRed;
                spawnParams.outerColor = Color.Red;
                spawnParams.scaleRange = new Vector2(0.1f, 1f);
                DustParticle.Spawn(Projectile.Center, -Projectile.oldVelocity.RotatedByRandom(1.5f) * Main.rand.NextFloat(0.5f, 1f), spawnParams);
            }

            SmokeParticle sp = Particle<SmokeParticle>.SpawnInAlphaLayer(Projectile.Center, -Vector2.UnitY, Color.White, Scale: 1f);
            sp.initialColor = Color.White * 0.14f;

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.InfernoFork, speed);
                d.noGravity = true;
            }
        }
    }
}
