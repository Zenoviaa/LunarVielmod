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

        public void DrawLikeLeg(SpriteBatch spriteBatch, Texture2D[] textures, Color drawColor)
        {
            Texture2D thighTexture = textures[0];
            Texture2D kneeTexture = textures[1];
            Texture2D legTexture = textures[2];
            Texture2D footTexture = textures[3];


            Segment legSegment = segments[segments.Length - 1];
            Segment thighSegment = segments[0];

            //Draw Thight
            Draw(spriteBatch, thighTexture, thighSegment.a, thighSegment.angle, drawColor);

            //Draw Foot
            DrawCenetered(spriteBatch, footTexture, legSegment.b, 0, drawColor);

            //Draw Leg
            Draw(spriteBatch, legTexture, legSegment.a, legSegment.angle, drawColor);

            //Draw Knee
            DrawCenetered(spriteBatch, kneeTexture, thighSegment.b, thighSegment.angle, drawColor);
        }


        public void DrawCenetered(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, float angle, Color drawColor)
        {
            Vector2 drawPosition = position - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = Vector2.One;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, angle, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Vector2 position, float angle, Color drawColor)
        {
            Vector2 drawPosition = position - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(0f, texture.Height / 2f);
            Vector2 drawScale = Vector2.One;
            spriteBatch.Draw(texture, drawPosition, null, drawColor, angle, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }
}
