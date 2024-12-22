using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Gustbeak.Projectiles
{
    internal class AverageWindBall : BaseWindProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.hostile = true;
            Projectile.timeLeft = 420;
            Projectile.light = 0.2f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();

            Wind.ColorFunc = WindColorFunction;
            float chargeProgress = Timer / 60f;
            int divisor = (int)MathHelper.Lerp(6, 6, chargeProgress);
            if (Timer % divisor == 0)
            {
                //Spawn new slashes on our little wind orb
                float range = 80;
                Vector2 offset = Main.rand.NextVector2CircularEdge(range, range);
                float rotation = offset.ToRotation();
                Wind.NewSlash(offset, rotation);

                offset = Main.rand.NextVector2CircularEdge(range, range);
                rotation = offset.ToRotation();
                Wind.NewSlash(offset, rotation);
            }

            if (Timer < 180)
            {
                Wind.ExpandMultiplier = MathHelper.Lerp(2f, 0.5f, Timer / 180f);
                Projectile.velocity *= 0.99f;
            }

            if (Timer == 180)
            {
                Projectile.velocity = -Vector2.UnitY * 8f;
                for (float f = 0; f < 24; f++)
                {
                    float rot = (f / 24f) * MathHelper.TwoPi;
                    Vector2 velOffset = rot.ToRotationVector2() * 6;
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, velOffset, Scale: 1f);
                    d.noGravity = true;
                }
            }

            if (Timer > 180)
            {
                Wind.ExpandMultiplier = MathHelper.Lerp(Wind.ExpandMultiplier, 2f, 0.1f);
                float maxDetectDistance = 2400;
                Player player = PlayerHelper.FindClosestPlayer(Projectile.Center, maxDetectDistance);
                if (player != null)
                {
                    Vector2 dirToNpc = (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity += dirToNpc * 0.05f;
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, degreesToRotate: 1f);
                }
            }

            if (Timer > 420)
            {
                ShadowScale = MathHelper.Lerp(1f, 0f, (Timer - 420) / 20f);
            }
        }

        private Color WindColorFunction(float progress)
        {
            float easedProgress = Easing.SpikeOutCirc(progress);
            Color color = Color.Lerp(Color.Transparent, Color.White, easedProgress);
            if (Timer > 420)
            {
                color = Color.Lerp(color, Color.Transparent, (Timer - 420) / 20f);
            }
            return color;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive);

            for (float f = 0f; f < 1f; f += 0.25f)
            {
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                float rotation = f * MathHelper.TwoPi;
                Vector2 offset = rotation.ToRotationVector2() * 2;
                drawPos += offset;
                DrawWindBall(drawPos, ref lightColor);
            }

            spriteBatch.RestartDefaults();
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            var source = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(source, Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<WindBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
