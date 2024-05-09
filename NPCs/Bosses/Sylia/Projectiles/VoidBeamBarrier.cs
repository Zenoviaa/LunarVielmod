
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{
    internal class VoidBeamBarrier : ModProjectile,
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

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = int.MaxValue;
            BeamPoints = new List<Vector2>();
            CooldownSlot = ImmunityCooldownID.Bosses;
        }

        public override void AI()
        {
            //Kill yoself if no Sylia
            if (!NPC.AnyNPCs(ModContent.NPCType<Sylia>()))
            {
                Projectile.Kill();
                return;
            }

            float targetBeamLength = PerformBeamHitscan();
            BeamLength = MathHelper.Lerp(BeamLength, targetBeamLength, 0.2f);
            Projectile.velocity.Y += 1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float _ = 0f;
            float width = Projectile.width * 0.8f;
            Vector2 start = Projectile.Center;
            Vector2 end = start + -Vector2.UnitY * (BeamLength - 80f);
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
            Collision.LaserScan(samplingPoint, -Vector2.UnitY, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
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
            return (Projectile.width * Projectile.scale * 1.3f) * osc;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(ColorFunctions.MiracleVoid, Color.White, VectorHelper.Osc(0, 1));
            return color;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.Black);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            //Put in the points
            //This is just a straight beam that collides with tiles
            BeamPoints.Clear();
            for (int i = 0; i <= 8; i++)
            {
                BeamPoints.Add(Vector2.Lerp(Projectile.Center, Projectile.Center + -Vector2.UnitY * BeamLength, i / 8f));
            }

            BeamDrawer.DrawPixelated(BeamPoints, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
