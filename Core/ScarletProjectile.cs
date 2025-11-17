using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using static Stellamod.Tiles.SpecialDecorativeWall;

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

        public void DrawAfterImageEffect(SpriteBatch spriteBatch, Texture2D texture, Rectangle drawFrame, Vector2 drawOrigin, Vector2 drawScale, SpriteEffects spriteEffects, Color startColor, float intensity)
        {
            for (int i = 0; i < TrailCacheLength; i++)
            {
                Vector2 oldPos = OldCenterPos[i];
                Vector2 oldDrawPos = oldPos - Main.screenPosition;
                float f = i;
                float interpolant = f / (float)Projectile.oldPos.Length;
                Color fadeColor = Color.Lerp(startColor, Color.Transparent, interpolant) * intensity;
                spriteBatch.Draw(texture, oldDrawPos, drawFrame, fadeColor, OldCenterRot[i], drawOrigin, drawScale, spriteEffects, 0f);
            }
        }
    }
}
