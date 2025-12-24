using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles
{
    public class DescendingBigBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int Variant => (int)Projectile.ai[1];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            ShakeModSystem.Shake = 10;
            if (Timer == 1)
            {
                SoundStyle boomSound = AssetRegistry.Sounds.SteamPunking.DescendingBoom;
                boomSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(boomSound, Projectile.position);

                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = LegacyParticle.NewParticle<ZapParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.innerColor = Color.White;
                    spark.outerColor = GetTwinColor();
                    spark.fadeToColor = Color.Blue;
                }

                var part = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part.Scale *= 2;
                part.noStretch = true;
                part.innerColor = GetTwinColor();
                part.outerColor = Color.Lerp(GetTwinColor(), Color.Blue, 0.25f);
                part.fadeToColor = Color.Lerp(GetTwinColor(), Color.Black, 0.5f);
                for (float f = 0; f < 8; f++)
                {
                    float radius = 800;
                    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                    Vector2 velocity = Projectile.Center - spawnPos;
                    velocity = velocity.SafeNormalize(Vector2.Zero);
                    velocity *= Main.rand.NextFloat(8, 32);
                    var p = FXUtil.GlowStretch(spawnPos, velocity);
                    p.InnerColor = GetTwinColor();
                    p.GlowColor = Color.Lerp(GetTwinColor(), Color.Blue, 0.25f);
                    p.OuterGlowColor = Color.Lerp(GetTwinColor(), Color.Black, 0.5f);
                    p.Scale *= 3f;
                }

                FXUtil.ShakeCamera(Projectile.position, 1024, 10);
                for (float f = 0; f < 8; f++)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                    pVelocity *= Main.rand.NextFloat(0.5f, 8f);
                    var spark = LegacyParticle.NewParticle<EmberParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }

                float numDust = 16;
                for (float n = 0; n < numDust; n++)
                {
                    SpawnFlameDust(Projectile.Center, Main.rand.NextVector2Circular(16, 16));
                    SpawnGlowDust(Projectile.Center, Main.rand.NextVector2Circular(64, 64));
                }
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: GetTwinColor(),
                        outerGlowColor: Color.Lerp(GetTwinColor(), Color.DarkBlue, 0.5f),
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                    particle.Scale *= 4f;
                }
            }
        }

        private Color GetTwinColor() => DescendingTwins.GetTwinColor(Variant);

        private void SpawnFlameDust(Vector2 position, Vector2 velocity)
        {
            var p = LegacyParticle.NewParticle<GlowFragmentParticle>(position, velocity, Color.White, Scale: 4f);
            Color twinColor = GetTwinColor();
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }
        private void SpawnGlowDust(Vector2 position, Vector2 velocity)
        {
            var d = Particle<DustParticle>.Spawn(position, velocity, color: GetTwinColor(), Scale: Main.rand.NextFloat(0.5f, 1.5f));
            d.outerColor = GetTwinColor();
            // Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), velocity, newColor: GetTwinColor(), Scale: 2f);
        }
    }
}
