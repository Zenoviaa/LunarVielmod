using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Astrasilk
{
    public class AstrasilkGigaStarProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle starSound;
                switch (Main.rand.Next(2))
                {
                    default:
                    case 0:
                        starSound = AssetRegistry.Sounds.Stars.Starsingle3;
                        break;
                    case 1:
                        starSound = AssetRegistry.Sounds.Stars.Starsingle5;
                        break;
                }
                starSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(starSound, Projectile.position);


                var p = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, -Projectile.velocity * 2, newColor: Color.Lavender);
                p.Scale *= 0.33f;
            }

            if (Timer % 5 == 0)
            {
                SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center + Main.rand.NextVector2Circular(64, 64), Vector2.Zero, Color.White, Main.rand.NextFloat(0.25f, 0.5f));
                sp.innerColor = Color.Lavender;
                sp.outerColor = Color.Violet;
                sp.gravity = 0f;
            }

            if (Timer >= 60)
            {
                Projectile.tileCollide = true;
            }
         
            Projectile.velocity *= 1.05f;
            Projectile.rotation += Projectile.velocity.Length() * 0.025f;
            NPC closest = NPCHelper.FindClosestNPC(Projectile.position, 1000);
            if (closest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center);
            }
        }

        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(32, 0, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightPink, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.scale = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(Timer / 15f));
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D glowTexture = AssetManager.GlowMask.SimpleGlowCircle.Value;
            PixelationManager.QueuePrimitivesDrawAction(DrawTrailing);
            drawOrigin = glowTexture.Size() * 0.5f;
            for (int i = 1; i < Projectile.oldPos.Length; i++)
            {
                if (i % 3 != 0)
                    continue;
                Vector2 oldPosition = Projectile.oldPos[i];
                Vector2 oldDrawPosition = oldPosition - Main.screenPosition;
                oldDrawPosition += Projectile.Size * 0.5f;
                float rot = (oldPosition - Projectile.oldPos[i - 1]).ToRotation();
                Color afterImageGlowColor = Color.Lavender;
                afterImageGlowColor = Color.Lerp(Color.Lavender, Color.Violet, ExtraMath.Osc(0f, 0.4f, speed: 8));

                float ratio = (float)i / (float)Projectile.oldPos.Length;
                afterImageGlowColor = Color.Lerp(afterImageGlowColor, Color.Black, MathHelper.SmoothStep(0.85f, 0.9f, ratio));
                afterImageGlowColor.A = 0;

                float scale = MathHelper.SmoothStep(1f, 0f, ratio);
                spriteBatch.Draw(glowTexture, oldDrawPosition, null, afterImageGlowColor, rot, drawOrigin,
                            scale * Projectile.scale * ExtraMath.Osc(1.1f, 1.2f, speed: 8) * 0.3f * new Vector2(2f, 0.6f), SpriteEffects.None, 0);
            }

            drawOrigin = texture.Size() * 0.5f;
            spriteBatch.Draw(texture, drawCenter, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);


            drawOrigin = glowTexture.Size() * 0.5f;
            Color glowColor = Color.Lerp(Color.Lavender, Color.Black, 0.85f);
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, drawCenter, null, glowColor, Projectile.rotation, drawOrigin, Projectile.scale * 0.3f, SpriteEffects.None, 0);
            return false;
        }

        private void DrawTrailing(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.Lavender * 0.3f;
            shader.InnerColor = Color.BlueViolet * 0.3f;
            shader.OuterColor = Color.Blue * 0.3f;
            shader.BloomTexture = AssetManager.LaserTextures.TexturedLaser; 
            shader.LaserTexture = TrailRegistry.TwistingTrail;
            TrailDrawer.Draw(Main.spriteBatch, Projectile.oldPos, ColorFunction, WidthFunction, shader, Projectile.Size * 0.5f);
        }

        public override void OnKill(int timeLeft)
        {
            FXUtil.GlowCircleBoom(Projectile.Center, Color.Lavender, Color.Violet, Color.Black, baseSize: 0.18f);
            float numDust = 16;
            for (int n = 0; n < numDust; n++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                SparkleParticle sp = SparkleParticle.Spawn(Projectile.Center, velocity, Color.White, Main.rand.NextFloat(0.5f, 1f));
                sp.flickering = true;
                sp.innerColor = Color.Lavender;
                sp.outerColor = Color.Violet;
                sp.gravity = 0f;
                sp.dampening = 0.1f;
            }

            numDust *= 0.4f;
            for (int n = 0; n < numDust; n++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Lavender,
                    outerColor = Color.Violet,
                    scaleRange = new Vector2(0.4f, 0.7f)
                };

                DustParticle.Spawn(Projectile.Center, velocity, spawnParams);
            }
            for (int n = 0; n < numDust; n++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, velocity, Scale: 1f);
                d.noGravity = true;
            }


            for (int n = 0; n < numDust; n++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    innerColor = Color.Lavender,
                    outerColor = Color.Violet,
                    scaleRange = new Vector2(0.4f, 0.7f)
                };

                var smokeParticle = SmokeParticle.SpawnInAlphaLayer(Projectile.Center + Main.rand.NextVector2Circular(64, 64), -Vector2.UnitY, Color.DarkGray, Main.rand.NextFloat(0.5f, 2f));
                smokeParticle.initialColor = Color.Lerp(Color.Lavender, Color.Black, 0.7f);
            }

            SoundStyle starBoom = new SoundStyle("Stellamod/Assets/Sounds/StarFlower3");
            starBoom.PitchVariance = 0.3f;
            starBoom.Volume = 0.3f;
            SoundEngine.PlaySound(starBoom, Projectile.position);
        }
    }
}
