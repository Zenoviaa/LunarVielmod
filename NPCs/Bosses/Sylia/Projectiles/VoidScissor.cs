using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.Sylia.Projectiles
{
    internal class VoidScissor : ModProjectile
    {
        public Vector2 TargetVelocity;
        ref float Timer => ref Projectile.ai[0];
        public bool IsStuck;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 66;
            Projectile.height = 54;
            Projectile.tileCollide = true;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 240;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                TargetVelocity = Projectile.velocity;
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RipperSlash2");
                soundStyle.PitchVariance = 0.5f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            float progress = Timer / 60;
            float easedProgress = Easing.InOutExpo(progress);
            Projectile.velocity = TargetVelocity * easedProgress;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Visuals();
        }

        public override bool ShouldUpdatePosition()
        {
            return !IsStuck;
        }

        public override bool? CanDamage()
        {
            return !IsStuck;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!IsStuck)
            {
                IsStuck = true;
                Projectile.position += oldVelocity;
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeHit");
                soundStyle.Pitch = -0.5f;
                soundStyle.PitchVariance = 0.15f;
                soundStyle.Volume = 0.3f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            return false;
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
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.78f);
        }
    }
}
