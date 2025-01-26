using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class EelLightningBolt : ModProjectile
    {
        public float BeamLength;
        public float BeamWidthMultiplier;
        public Vector2[] BeamPoints;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float Timer => ref Projectile.ai[0];
        public CommonLightning Lightning { get; set; } = new CommonLightning();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 8;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                BeamWidthMultiplier = Main.rand.NextFloat(0.15f, 1f);
            }
            float targetBeamLength = ProjectileHelper.PerformBeamHitscan(Projectile.position, Projectile.velocity.SafeNormalize(Vector2.Zero), 600);
            BeamLength = targetBeamLength;
            if (Timer == 1)
            {
                //Sound Effect Goooo

                Vector2 lightningHitPos = Projectile.position + Projectile.velocity.SafeNormalize(Vector2.Zero) * BeamLength;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(lightningHitPos, 1024, 3);
            }

            for (int i = 0; i < Lightning.Trails.Length; i++)
            {
                float progress = (float)i / (float)Lightning.Trails.Length;
                var trail = Lightning.Trails[i];
                trail.LightningRandomOffsetRange = MathHelper.Lerp(16, 4, progress) * MathHelper.Lerp(2f, 0, Timer / 8f);
                trail.LightningRandomExpand = MathHelper.Lerp(16, 8, progress);
                trail.PrimaryColor = Color.Lerp(Color.White, Color.LightSkyBlue, progress);
                trail.NoiseColor = Color.Lerp(Color.White, Color.LightSkyBlue, progress);
            }

            //Setup lightning stuff
            //Should make it scale in/out
            float lightningProgress = Timer / 8f;
            float easedLightningProgress = Easing.SpikeOutCirc(lightningProgress);
            Lightning.WidthMultiplier = MathHelper.Lerp(0f, 4, easedLightningProgress) * BeamWidthMultiplier;
            if (Timer % 3 == 0)
            {
                List<Vector2> beamPoints = new List<Vector2>();
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                for (int i = 0; i <= 12f; i++)
                {
                    float maxProgress = MathHelper.Lerp(0f, 1f, Easing.OutExpo(Timer / 3f));
                    float progress = MathHelper.Lerp(0f, maxProgress, i / 12f);
                    Vector2 end = Projectile.Center + direction * BeamLength;
                    end = end.RotatedByRandom(MathHelper.ToRadians(0.02f));
                    Vector2 lerpedPoint = Vector2.Lerp(Projectile.Center, end, progress);


                    beamPoints.Add(lerpedPoint);
                }
                BeamPoints = beamPoints.ToArray();
                Lightning.RandomPositions(BeamPoints);
            }
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

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
            Vector2 end = start + direction * (BeamLength);
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, width, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Lightning.Draw(spriteBatch, BeamPoints, Projectile.oldRot);
            return false;
        }
    }
}
