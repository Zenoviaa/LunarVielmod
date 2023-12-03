using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{
    internal class SyliaScissor : ModProjectile
    {
        public Vector2 startCenter;
        public Vector2 targetCenter;
        public int delay;
        public bool playedSound;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            //THIS PROJECTILE IS MOVED BY SYLIA
            delay--;
            Vector2 direction = (targetCenter - startCenter).SafeNormalize(Vector2.Zero);
            if (delay <= 0)
            {
                Projectile.velocity = direction * 24;
                if (!playedSound)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlash2"), Projectile.position);
                    playedSound = true;
                }
            }
            else
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, startCenter, 0.15f);
                float targetRotation = direction.ToRotation() + MathHelper.ToRadians(45);
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 0.15f);
            }

            Visuals();
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(60, 0, 118), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }
    }
}
