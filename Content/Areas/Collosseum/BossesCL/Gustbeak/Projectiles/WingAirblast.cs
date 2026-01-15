using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.Gustbeak.Projectiles
{
    public class WingAirblast : AbstractWindProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.light = 0.2f;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            if(Timer == 1)
            {
                DrawScale = Main.rand.NextFloat(0.6f, 1f);
            }
            float chargeProgress = Timer / 60f;
            int divisor = (int)MathHelper.Lerp(12, 12, chargeProgress);
            if (Timer % divisor == 0)
            {
                //Spawn new slashes on our little wind orb
                float range = MathHelper.Lerp(16, 16, chargeProgress);
                Vector2 offset = Main.rand.NextVector2CircularEdge(range, range);
                float rotation = offset.ToRotation();
                Wind.NewSlash(offset, rotation);

                offset = Main.rand.NextVector2CircularEdge(range, range);
                rotation = offset.ToRotation();
                Wind.NewSlash(offset, rotation);
            }
            if (Timer % 16 == 0)
            {
                ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
                {
                    innerColor = Color.White,
                    outerColor = Color.DarkGray
                };
                ShockOvalParticle sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.4f, spawnParams);
                sp.color *= 0.2f;
                sp.Scale *= 0.9f;

                sp = ShockOvalParticle.Spawn(Projectile.Center, -Projectile.velocity * 0.2f, spawnParams);
                sp.color *= 0.2f;
                sp.Scale *= 0.6f;
            }
            if (Projectile.velocity.Length() < 10f)
            {
                Projectile.velocity *= 1.1f;
            }
            else
            {
                Projectile.extraUpdates = 1;
            }

                Wind.ExpandMultiplier = 0.25f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);
            for (float f = 0f; f < 1f; f += 0.25f)
            {
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                float rotation = f * MathHelper.TwoPi;
                Vector2 offset = rotation.ToRotationVector2() * 3;
                drawPos += offset;
                DrawWindBall(drawPos, ref lightColor);
            }
            DrawWindBall(Projectile.Center - Main.screenPosition, ref lightColor);
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
