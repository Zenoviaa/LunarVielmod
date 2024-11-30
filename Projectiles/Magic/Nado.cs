using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Gustbeak.Projectiles;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class Nado : BaseWindProjectile
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
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
            Projectile.light = 0.2f;
            Projectile.penetrate = 3;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.damage /= 2;

        }

        public override void AI()
        {
            base.AI();
            if (Timer == 1)
            {
                SoundStyle soundStyle = SoundID.DD2_WyvernDiveDown;
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            if(Timer % 24 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Sand);
            }

            Wind.ColorFunc = DefaultColorFunction;
            float chargeProgress = Timer / 60f;
            int divisor = (int)MathHelper.Lerp(6, 6, chargeProgress);
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

            Projectile.velocity *= 1.015f;
            Wind.ExpandMultiplier = 0.25f;
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
                Vector2 offset = rotation.ToRotationVector2() * 3;
                drawPos += offset;
                DrawWindBall(drawPos, ref lightColor);
            }
            DrawWindBall(Projectile.Center - Main.screenPosition, ref lightColor);
            spriteBatch.RestartDefaults();
            return false;
        }

        private Color DefaultColorFunction(float progress)
        {
            float easedProgress = Easing.SpikeOutCirc(progress);
            return Color.Lerp(Color.Transparent, Color.SandyBrown, easedProgress);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<WindBoomFriendly>(), Projectile.damage / 4, Projectile.knockBack, Projectile.owner);
        }

    }
}