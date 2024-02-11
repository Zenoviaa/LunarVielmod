using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class FireflyBomb : ModProjectile, IPixelPrimitiveDrawer
    {
        public override void SetStaticDefaults()
        {
            // Total count animation frames
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.light = 0.5f;
            Projectile.penetrate = 2;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player

            float distX = targetCenter.X - Projectile.Center.X;
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - Projectile.Center.Y;
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y += accel;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
            }
        }

        public Vector3 HuntrianColorXyz;
        public float HuntrianColorOffset;
        public float Timer;
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawDimLight(Projectile, HuntrianColorXyz.X, HuntrianColorXyz.Y, HuntrianColorXyz.Z, Color.Yellow, lightColor, 0);
            return base.PreDraw(ref lightColor);
        }

        public override void AI()
        {
            base.AI();
            Projectile.ai[0] += 1f;

            HuntrianColorXyz = DrawHelper.HuntrianColorOscillate(
                new Vector3(85, 45, 15),
                new Vector3(15, 60, 60),
                new Vector3(3, 3, 3), HuntrianColorOffset);

            Timer++;
            if (Timer <= 2)
            {
                HuntrianColorOffset = Main.rand.NextFloat(-1f, 1f);
            }

            SummonHelper.SearchForTargets(Main.player[Projectile.owner], Projectile,
                out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (foundTarget && distanceFromTarget < 384)
            {
                AI_Movement(targetCenter, 15);
            }

            Projectile.velocity *= 0.98f;

            //Animate It
            if (++Projectile.frameCounter >= 4)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }

            //Despawn after 3 seconds.
            if (Projectile.ai[0] >= 180f)
                Projectile.Kill();
        }

        public override void OnKill(int timeLeft)
        {
            int count = 8;
            float degreesPer = 360 / (float)count;
            for (int k = 0; k < count; k++)
            {
                float degrees = k * degreesPer;
                Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
                Vector2 vel = direction * 2;
                Dust.NewDust(Projectile.position, 0, 0, DustID.CopperCoin, vel.X, vel.Y);
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Starblast"), Projectile.position);
        }

        public float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(8f, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.Yellow;
            Color endColor = Color.Transparent;
            return Color.Lerp(startColor, endColor, completionRatio);
        }

        internal PrimitiveTrail BeamDrawer;
        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);

            TrailRegistry.LaserShader.UseColor(Color.LightYellow);
            TrailRegistry.LaserShader.SetShaderTexture(TrailRegistry.BeamTrail);

            BeamDrawer.DrawPixelated(Projectile.oldPos, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
