using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Sylia;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class PolarisLaserProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        //Don't change the sample points, 3 is good enough
        private const int NumSamplePoints = 3;

        private const float MaxBeamLength = 2400f;

        public float BeamLength;
        public List<Vector2> BeamPoints;
        internal PrimitiveTrail BeamDrawer;

        //No texture for this
        public override string Texture => TextureRegistry.EmptyTexture;

        ref float Size => ref Projectile.ai[0];
        float Timer;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 25;
            BeamPoints = new List<Vector2>();
        }

        public override void AI()
        {
            float targetBeamLength = PerformBeamHitscan();
            BeamLength = targetBeamLength;
            Timer++;
            if(Timer == 1)
            {
                switch (Main.rand.Next(2))
                {
                    case 0:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlast1, Projectile.position);
                        break;
                    case 1:
                        SoundEngine.PlaySound(SoundRegistry.Niivi_LaserBlast2, Projectile.position);
                        break;
                }

                for (int i = 0; i < 14; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1f).noGravity = true;
                }

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 explosionCenter = Projectile.Center + direction * BeamLength;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(explosionCenter, 1024f, 32f);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), explosionCenter, Vector2.Zero,
                    ModContent.ProjectileType<SiriusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f * Size;
            Vector2 start = Projectile.Center;

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * (BeamLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        private float PerformBeamHitscan()
        {
            // By default, the hitscan interpolation starts at the Projectile's center.
            // If the host Prism is fully charged, the interpolation starts at the Prism's center instead.
            Vector2 samplingPoint = Projectile.Center;

            // Perform a laser scan to calculate the correct length of the beam.
            // Alternatively, if you want the beam to ignore tiles, just set it to be the max beam length with the following line.
            // return MaxBeamLength;
            float[] laserScanResults = new float[NumSamplePoints];


            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Collision.LaserScan(samplingPoint, direction, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
            float averageLengthSample = 0f;
            for (int i = 0; i < laserScanResults.Length; ++i)
            {
                averageLengthSample += laserScanResults[i];
            }
            averageLengthSample /= NumSamplePoints;
            return averageLengthSample;
        }


        public float WidthFunction(float completionRatio)
        {
            float osc = VectorHelper.Osc(0.75f, 1f);

            float width = (float)Projectile.timeLeft / 60f;
            return (Projectile.width * Projectile.scale) * osc * width * Size;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.LightCyan, Color.White, VectorHelper.Osc(0, 1));
            return color;
        }

        public override bool PreDraw(ref Color lightColor) => false;
        public override bool ShouldUpdatePosition() => false;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.White);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            //Put in the points
            //This is just a straight beam that collides with tiles
            BeamPoints.Clear();
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            for (int i = 0; i <= 8; i++)
            {
                BeamPoints.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + direction * BeamLength, i / 8f));
            }

            BeamDrawer.DrawPixelated(BeamPoints, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
