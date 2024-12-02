using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal abstract class BaseShriekExplosionProjectile : ModProjectile
    {
        private float _duration;
        protected Vector2[] _primPos = new Vector2[32];
        protected Vector2[] _miniPrimPos = new Vector2[16];
        private ref float Countertimer => ref Projectile.ai[0];

        public override string Texture => TextureRegistry.EmptyTexture;
        public float Progress { get; private set; }
        public float NumMiniWisps { get; set; }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NumMiniWisps = 5;
            Projectile.width = Projectile.height = 1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            base.AI();
            Countertimer++;
            if (Countertimer == 1)
            {
                _duration = Projectile.timeLeft;
            }

            Progress = Countertimer / _duration;
            Vector2 velocity = Projectile.velocity;


            for (int i = 0; i < _primPos.Length; i++)
            {
                float f = i;
                float p = f / _primPos.Length;
                float dist = DistanceFunction(p);
                Vector2 primPos =
                    (Projectile.position + velocity * dist);
                _primPos[i] = primPos;
            }


            int numToReplace = (int)((float)_primPos.Length * Progress);
            for (int n = numToReplace; n < _primPos.Length; n++)
            {
                _primPos[n] = Vector2.Zero;
            }

        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        protected virtual float MovementFunction(float completionRatio)
        {
            return Easing.OutExpo(completionRatio) * 80;
        }

        protected virtual float DistanceFunction(float completionRatio)
        {
            return 48 * completionRatio;
        }

        protected virtual float WispRadiusFunction(float completionRatio)
        {
            return 64;
        }

        protected virtual void DrawPrims(Vector2[] trailPos)
        {

        }

        protected virtual void DrawMiniWispPrims(Vector2[] trailPos)
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            for (int i = 0; i < NumMiniWisps; i++)
            {
                float f = i;
                float mainProgress = f / NumMiniWisps;

                float rot = MathHelper.Lerp(-MathHelper.PiOver2, MathHelper.PiOver2, mainProgress);
                rot += MathHelper.Pi / NumMiniWisps / 2f;
                Vector2 velocityToMove = Projectile.velocity.RotatedBy(rot);
                _miniPrimPos[0] = Projectile.position;
                _miniPrimPos[1] = _miniPrimPos[0]
                    + velocityToMove * WispRadiusFunction(Progress);
                for (int j = 2; j < _miniPrimPos.Length; j++)
                {
                    float f2 = j;
                    float p2 = f2 / (float)_miniPrimPos.Length;
                    Vector2 vectorBetween = _miniPrimPos[j - 1] - _miniPrimPos[j - 2];
                    vectorBetween = vectorBetween.SafeNormalize(Vector2.Zero);
                    vectorBetween *= WispRadiusFunction(Progress);
                    Vector2 newOffset = Vector2.Lerp(vectorBetween,
                       -Vector2.UnitY * WispRadiusFunction(Progress), Easing.InCirc(p2) * Easing.OutCirc(Progress));
                    _miniPrimPos[j] = _miniPrimPos[j - 1] + newOffset;
                }

                DrawMiniWispPrims(_miniPrimPos);
            }
            DrawPrims(_primPos);
            return false;
        }
    }
}
