using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;

namespace Stellamod.Common.Bases
{
    public class CircleSwingStyle : BaseSwingStyle
    {
        public float startSwingRotOffset;
        public float endSwingRotOffset;
        public float swingDistance;
        public bool spinCenter;
        public float spinCenterOffset;
        public override void AI()
        {
            Vector3 RGB = new Vector3(1.28f, 0f, 1.28f);
            float multiplier = 0.2f;
            RGB *= multiplier;

            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            int dir = (int)Projectile.ai[1];

            float lerpValue = SwingProjectile.Timer / SwingProjectile.GetSwingTime(swingTime);

            //Smooth it some more
            float swingProgress = lerpValue;

            // the actual rotation it should have
            float targetRotation = Projectile.velocity.ToRotation();

            //How wide is the swing, in radians
            float start = targetRotation + startSwingRotOffset;
            float end = targetRotation + endSwingRotOffset;
            SwingProjectile.uneasedLerpValue = lerpValue;
            swingProgress = easingFunc(swingProgress);
            SwingProjectile._smoothedLerpValue = swingProgress;
            PlaySwingSound(swingProgress);

            // current rotation obv
            // angle lerp causes some weird things here, so just use a normal lerp
            float rotation = dir == 1 ? MathHelper.Lerp(start, end, swingProgress) : MathHelper.Lerp(end, start, swingProgress);

            // offsetted cuz sword sprite
            Vector2 position = Owner.RotatedRelativePoint(Owner.MountedCenter);
            Vector2 baseHoldOffset = rotation.ToRotationVector2() * SwingProjectile.holdOffset;
            Vector2 extraHoldOffset = rotation.ToRotationVector2() * swingDistance;
            position += baseHoldOffset + extraHoldOffset;
            Projectile.Center = position;
            Projectile.rotation = (position - Owner.Center).ToRotation() + MathHelper.PiOver4;
            if (spinCenter)
            {
                Projectile.Center -= rotation.ToRotationVector2() * SwingProjectile.holdOffset;
                Projectile.Center -= rotation.ToRotationVector2() * swingDistance;
                Projectile.Center -= rotation.ToRotationVector2() * spinCenterOffset;
            }

            //Calculate Trail Points

            Vector2[] points = new Vector2[ProjectileID.Sets.TrailCacheLength[Projectile.type]];
            for (int i = 0; i < points.Length; i++)
            {
                float l = points.Length;
                //Lerp between the points
                float progressOnTrail = i / l;

                //Calculate starting lerp value
                float startTrailLerpValue = MathHelper.Clamp(lerpValue - SwingProjectile.trailStartOffset, 0, 1);
                float startTrailProgress = startTrailLerpValue;
                startTrailProgress = easingFunc(startTrailLerpValue);

                //Calculate ending lerp value
                float endTrailLerpValue = lerpValue;
                float endTrailProgress = endTrailLerpValue;
                endTrailProgress = easingFunc(endTrailLerpValue);

                //Lerp in between points
                float smoothedTrailProgress = MathHelper.Lerp(startTrailProgress, endTrailProgress, progressOnTrail);
                float rot = dir == 1 ? MathHelper.Lerp(start, end, smoothedTrailProgress) : MathHelper.Lerp(end, start, smoothedTrailProgress);

                Vector2 pos = Owner.RotatedRelativePoint(Owner.MountedCenter);
                pos += rot.ToRotationVector2() * (SwingProjectile.GetFramingSize());
       
                if (spinCenter)
                {
                    Vector2 d = (Owner.RotatedRelativePoint(Owner.MountedCenter) - pos).SafeNormalize(Vector2.Zero);
                    pos += d * spinCenterOffset;
                }
                points[i] = pos - SwingProjectile.GetFramingSize() / 2;
            }
            SwingProjectile._trailPoints = points;
        }
    }
}
