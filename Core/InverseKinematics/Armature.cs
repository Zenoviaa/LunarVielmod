using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Stellamod.Core.InverseKinematics
{
    public class Armature
    {
        public Segment[] segments;
        public Armature()
        {
            segments = new Segment[2];
            segments[0] = new Segment(new Vector2(300, 300), 100, 0);
            for (int i = 1; i < segments.Length; i++)
            {
                segments[i] = new Segment(segments[i - 1], 200, 0);
            }
        }
        public Vector2 oldTargetPosition;
        public float timer;

        public void IK(Vector2 rootPosition, Vector2 targetPosition)
        {
        
            //So the issue with this solver is that 
            //1. it doesn't actually find a solution, it just goes to the nearest possible point,
            //Even if there is no solution it'll go to the next best spot, which may be desired in some cases
            //But for the cast of STARBOMBER we probably only want real solutions

            //2. It goes to any solution if there is one, for STARBOMBER his legs aren't allowed to bend downward, so we don't want to solve for thos solutions specifically
            //Which means we need a REAL solver, lets read up
            int total = segments.Length;
            Segment end = segments[total - 1];
            end.Follow(targetPosition);
            end.Update();
            for (int i = total - 2; i >= 0; i--)
            {
                Segment segment = segments[i];
                segment.Follow(segments[i + 1]);
                segment.Update();
            }

            segments[0].SetA(rootPosition);
            for (int i = 1; i < total; i++)
            {
                segments[i].SetA(segments[i - 1].b);
            }

           
        }

        public Vector2 GetEndEffector()
        {
            return segments[segments.Length - 1].b;
        }
        public void FK(Vector2 rootPosition)
        {
            int total = segments.Length;
            Segment end = segments[total - 1];
            end.Update();
            for (int i = total - 2; i >= 0; i--)
            {
                Segment segment = segments[i];
                segment.Update();
            }

            segments[0].SetA(rootPosition);
            for (int i = 1; i < total; i++)
            {
                segments[i].SetA(segments[i - 1].b);
            }
        }

        public void SetDefaults()
        {
            for(int i = 0; i < segments.Length; i++)
            {
                Segment segment = segments[i];
                segment.angle = segment.rootDirection.ToRotation();
            }
        }
        public void LerpDefaults()
        {
            for (int i = 0; i < segments.Length; i++)
            {
                Segment segment = segments[i];
                segment.angle = MathHelper.Lerp(segment.angle, segment.rootDirection.ToRotation(), 0.1f);
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < segments.Length; i++)
            {
                segments[i].Draw(spriteBatch);
            }
        }
    }
}
