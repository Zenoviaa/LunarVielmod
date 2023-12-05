using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{
    internal class SyliaScissorSmall2 : ModProjectile
    {
        public Vector2 startCenter;
        public Vector2 targetCenter;
        public int delay;
        public bool playedSound;
        public bool setRotation;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 100;
        }

        public override void AI()
        {

            //THIS PROJECTILE IS MOVED BY SYLIA
            delay--;

            Vector2 direction = (targetCenter - startCenter).SafeNormalize(Vector2.Zero);
            if (!setRotation)
            {
                Projectile.Center = Vector2.Lerp(Projectile.Center, startCenter, 1f);
                float targetRotation = direction.ToRotation() + MathHelper.ToRadians(45);
                Projectile.rotation = MathHelper.Lerp(Projectile.rotation, targetRotation, 1f);
                setRotation = true;
            }

            if (delay <= 0)
            {
                Projectile.velocity = direction * 24;
                if (!playedSound)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RipperSlash2"), Projectile.position);
                    playedSound = true;
                }
            }

            Visuals();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(60, 0, 118, 175), Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(60, 0, 118), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }
    }
}
