using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class BlackEyeLaserProj  : ModProjectile
    {
        internal PrimitiveTrail BeamDrawer;
        public ref float Time => ref Projectile.ai[0];

        public override string Texture => TextureRegistry.EmptyTexture;
        private const int NumSamplePoints = 3;

        private const float MaxBeamLength = 2400f;

        public float BeamLength;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 48;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90;
            Projectile.alpha = 255;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        public override void AI()
        {
            float targetBeamLength = PerformBeamHitscan();
            BeamLength = targetBeamLength;
            if (Time == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RekLaser2"), Projectile.position);
            }
            float maxDetectDistance = 2400;
            NPC npc = NPCHelper.FindClosestNPC(Projectile.position, maxDetectDistance);
            if (npc != null)
            {
                Vector2 velocityToTarget = Projectile.Center.DirectionTo(npc.Center);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocityToTarget, 0.2f);
            }

            if (Time % 2 == 0)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                Vector2 vel = Projectile.velocity * 8;
                float scale = Main.rand.NextFloat(2.5f, 3.75f);
                Dust.NewDustPerfect(pos, ModContent.DustType<GlowDust>(), vel, 0, Color.OrangeRed, scale).noGravity = true;
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDustPerfect(pos, ModContent.DustType<TSmokeDust>(), vel, 0, Color.OrangeRed, scale / 2).noGravity = true;
                }
            }

            // Fade in.
            Projectile.alpha = Utils.Clamp(Projectile.alpha - 25, 0, 255);
            if (Projectile.scale < 1f || Time <= 1)
            {
                Projectile.scale = MathF.Sin(Time / 600f * MathHelper.Pi) * 3f;
                if (Projectile.scale > 1f)
                    Projectile.scale = 1f;
            }

            // And create bright light.
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 1.5f);
            Time++;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * (MaxBeamLength - 80f);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public float WidthFunction(float completionRatio)
        {
            float mult = 1;
            if (Projectile.timeLeft < 60)
            {
                mult = Projectile.timeLeft / (float)60;
            }
            return Projectile.width * Projectile.scale * 1.3f * mult;
        }

        public override bool ShouldUpdatePosition() => false;
        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.Orange, Color.Red, 0.2f);
            return color * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
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

        public override bool PreDraw(ref Color lightColor)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.Lerp(Color.White, Color.OrangeRed, 0.3f));
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.WaterTrail);


            List<Vector2> points = new();
            for (int i = 0; i <= 16; i++)
            {
                Vector2 targetPos = Projectile.Center + Projectile.velocity * MaxBeamLength;
                points.Add(Vector2.Lerp(Projectile.Center, targetPos, i / 8f));
            }

            BeamDrawer.DrawPixelated(points, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }
}
