using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Gustbeak.Projectiles
{
    internal class WindBlast : BaseWindProjectile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.timeLeft = 180;
            Projectile.light = 0.2f;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            float chargeProgress = Timer / 60f;
            int divisor = (int)MathHelper.Lerp(6, 6, chargeProgress);
            if (Timer % divisor == 0)
            {
                //Spawn new slashes on our little wind orb
                float range = MathHelper.Lerp(16, 16, chargeProgress);
                Vector2 offset = Main.rand.NextVector2CircularEdge(range, range);
                float rotation = offset.ToRotation();
               // rotation += Main.rand.NextFloat(-1f, 1f);
                Wind.NewSlash(offset, rotation);

                offset = Main.rand.NextVector2CircularEdge(range, range);
                rotation = offset.ToRotation();
                //rotation += Main.rand.NextFloat(-1f, 1f);
                Wind.NewSlash(offset, rotation);
            }

            Projectile.velocity *= 1.012f;
            Wind.ExpandMultiplier = 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.PreDraw(ref lightColor);
            
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive);

            for(float f = 0f; f < 1f; f += 0.25f)
            {
                Vector2 drawPos = Projectile.Center - Main.screenPosition;
                float rotation = f * MathHelper.TwoPi;
                Vector2 offset = rotation.ToRotationVector2() * 6;
                drawPos += offset;
                DrawWindBall(drawPos, ref lightColor);
            }
            DrawWindBall(Projectile.Center - Main.screenPosition, ref lightColor);
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
