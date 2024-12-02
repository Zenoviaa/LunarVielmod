using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal abstract class BaseSpikeAuraExplosionProjectile : ModProjectile
    {
        private float _duration;
        protected Vector2[] _primPos = new Vector2[32];
        private ref float Countertimer => ref Projectile.ai[0];

        public override string Texture => TextureRegistry.EmptyTexture;
        public float Progress { get; private set; }

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
            float r = RadiusFunction(Progress);
            for (int i = 0; i < _primPos.Length; i++)
            {
                float f = i;
                float length = _primPos.Length;
                float progress = f / length;
                float offset = progress * MathHelper.TwoPi * 2;
                Vector2 rotatedOffset = Vector2.UnitY.RotatedBy(offset + (Countertimer / 20f)).RotatedByRandom(MathHelper.PiOver4 / 24f);
                Vector2 rotatedVector = (rotatedOffset * r * VectorHelper.Osc(0.9f, 1f, 9));
                if (i % 2 == 0)
                {
                    _primPos[i] = rotatedVector * 0.5f + Projectile.position;
                }
                else
                {
                    _primPos[i] = rotatedVector + Projectile.position;
                }
            }
        }

        protected virtual float RadiusFunction(float completionRatio)
        {
            return 64;
        }

        protected virtual float BeamWidthFunction(float completionRatio)
        {
            return 64;
        }

        protected virtual Color BeamColorFunction(float completionRatio)
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
