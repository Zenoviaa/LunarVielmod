using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    /// <summary>
    /// Base class for all projectiles in the mod that has a few extra variables and functions
    /// </summary>
    public abstract class ScarletProjectile : ModProjectile
    {
        private Vector2[] _oldCenterPos;
        private float[] _oldCenterRot;
        public Player Owner => Main.player[Projectile.owner];
        public Vector2[] OldCenterPos
        {
            get
            {
                if (_oldCenterPos == null)
                    _oldCenterPos = new Vector2[TrailCacheLength];
                return _oldCenterPos;
            }
            private set
            {
                _oldCenterPos = value;
            }
        }
        public float[] OldCenterRot
        {
            get
            {
                if(_oldCenterRot == null)
                    _oldCenterRot = new float[TrailCacheLength];
                return _oldCenterRot;
            }
            private set
            {
                _oldCenterRot = value;
            }
        }
        public int TrailCacheLength;
        public override void AI()
        {
            base.AI();
            if(TrailCacheLength > 0)
            {
                for (int i = TrailCacheLength - 1; i > 0; i--)
                {
                    OldCenterPos[i] = OldCenterPos[i - 1];
                    OldCenterRot[i] = OldCenterRot[i - 1];
                }
                OldCenterPos[0] = Projectile.Center;
                OldCenterRot[0] = Projectile.rotation;
            }

        }
    }
}
