using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace CrystalMoon.Content.Bases
{
    internal abstract class BasePillarExplosionProjectile : ModProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        private Vector2[] _primPos = new Vector2[32];

        private ref float Countertimer => ref Projectile.ai[0];
        private float _duration;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
            Length = 256;
        }

        public float Length { get; set; }
        public float GlobalWidth { get; set; }
        public Color GlobalColor { get; set; }
        public override void AI()
        {
            base.AI();
            Countertimer++;
            if (Countertimer == 1)
            {
                _duration = Projectile.timeLeft;
            }

            float progress = Countertimer / _duration;
            GlobalWidth = GlobalWidthFunction(progress);
            GlobalColor = GlobalColorFunction(progress);

            Vector2 velocity = Projectile.velocity;
            for (int i = 0; i < _primPos.Length; i++)
            {
                float f = i;
                float p = f / _primPos.Length;
                float dist = DistanceFunction(p);
                Vector2 primPos = Projectile.Center + velocity * dist;
                _primPos[i] = primPos;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 start = Projectile.Center;
            Vector2 end = start + Projectile.velocity * Length;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(), targetHitbox.Size(),
                start, end, Projectile.scale * 12, ref collisionPoint);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        protected virtual float DistanceFunction(float completionRatio)
        {
            return 48 * completionRatio;
        }

        protected virtual float GlobalWidthFunction(float completionRatio)
        {
            return completionRatio;
        }

        protected virtual Color GlobalColorFunction(float completionRatio)
        {
            return Color.White;
        }

        protected virtual void DrawPrims(Vector2[] trailPos)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrims(_primPos);
            return false;
        }
    }
}
