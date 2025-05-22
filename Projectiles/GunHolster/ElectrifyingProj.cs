using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.Trails;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Utilities;

namespace Stellamod.Projectiles.GunHolster
{
    internal class ElectrifyingProj : ModProjectile, IPixelPrimitiveDrawer
    {
        float Timer;
        bool FadeOut;
        public override void SetStaticDefaults()
        {
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.width = 16;
            Projectile.height = 16;
         
            Projectile.penetrate = -1;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Projectile.timeLeft;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 4;
        }

        private ref float AI_Timer => ref Projectile.ai[0];
        private ref float Distance => ref Projectile.ai[1];
        private ref float Seed => ref Projectile.ai[2];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Projectile.oldPos[i] = Projectile.position;
            }
        }

        public override void AI()
        {
            AI_Timer++;

            if (AI_Timer % 2 == 0)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Distance = Main.rand.NextFloat(2, 8);
                    Seed = Main.rand.Next(0, int.MaxValue);
                    Projectile.netUpdate = true;
                }
            }

            if(Distance != 0)
            {
                //Randomly teleport to make the jagged effect
                UnifiedRandom random = new UnifiedRandom((int)Seed);
                float maxRadians = MathHelper.ToRadians(210);
                double radians = random.NextDouble() * maxRadians - Main.rand.NextDouble() * maxRadians;

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                direction = direction.RotatedByRandom(radians);
                Projectile.Center = Projectile.Center + direction * Distance;
                Distance = 0;
            }

            if (FadeOut)
            {
                Timer++;
            }

            //Dunno if this is needed but whatever
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Electrifying!!!! nEMIES!!!
            target.AddBuff(BuffID.Electrified, 120);
            SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);

            for (int i = 0; i < 8; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1, 1);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FadeOut = true;
            return false;
        }
        public float WidthFunction(float completionRatio)
        {
            float fadeScale = Timer / 30;
            fadeScale = MathHelper.Clamp(fadeScale, 0f, 1f);
            fadeScale = 1f - fadeScale;
            float baseWidth = Projectile.scale * 8 * fadeScale;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Cyan;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            //This damages everything in the trail
            Vector2[] positions = Projectile.oldPos;
            float collisionPoint = 0;
            for (int i = 1; i < positions.Length; i++)
            {
                Vector2 position = positions[i];
                Vector2 previousPosition = positions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 1, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        internal PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.LightCyan);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            BeamDrawer.DrawPixelated(Projectile.oldPos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
