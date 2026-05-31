using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Stellamod.Core.InverseKinematics
{
    public class Armature
    {
        public Segment[] segments;
        public Armature()
        {
            segments = new Segment[2];
            segments[0] = new Segment(new Vector2(300, 300), 158, 0);
            for (int i = 1; i < segments.Length; i++)
            {
                segments[i] = new Segment(segments[i - 1], 158, 0);
            }
        }
        public Armature(int numSegments, int segmentLength)
        {
            segments = new Segment[numSegments];
            Vector2 pos = new Vector2(300, 300);
            segments[0] = new Segment(pos, segmentLength, 0);
            for (int i = 1; i < segments.Length; i++)
            {
                segments[i] = new Segment(segments[i - 1], segmentLength, 0);
                segments[i].SetA(pos + new Vector2(0, i * 10));
            }
        }
        public Vector2 oldTargetPosition;
        public float timer;

        private void ResolveInner(int index)
        {

            ref Segment s2 = ref segments[index - 1];
            ref Segment s1 = ref segments[index];

            ref Vector2 p2 = ref s2.a;//ref points[index - 1];
            ref Vector2 p1 = ref s1.a;
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            float distance = MathF.Sqrt(dx * dx + dy * dy);

            //Calculating one direction is one way to get around the bounciness the other verlet integration implementation has
            //This looks a lot cleaner and way less stiff
            if (distance > s1.length)
            {
                float difference = s1.length - distance;
                float percent = difference / distance;
                float offsetX = dx * percent;
                float offsetY = dy * percent;
                p1.X -= offsetX;
                p1.Y -= offsetY;
            }
        }

        public void ResolveBackToRoot()
        {
            for (int i = segments.Length - 1; i >= 1; i--)
            {
                ResolveInner(i);
            }
        }


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
        public void Draw(SpriteBatch spriteBatch, Texture2D texture, Color drawColor)
        {
            for (int i = 0; i < segments.Length; i++)
            {

                var segment = segments[i];
                Draw(spriteBatch, texture, segment.a, segment.angle, drawColor);
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

      
            //Draw Leg
            Draw(spriteBatch, legTexture, legSegment.a, legSegment.angle, drawColor);     
            
            //Draw Foot
            DrawCenetered(spriteBatch, footTexture, legSegment.b, MathHelper.PiOver2, drawColor);

            //Draw Knee
            DrawCenetered(spriteBatch, kneeTexture, thighSegment.b, thighSegment.angle, drawColor);
            DrawCenetered(spriteBatch, kneeTexture, thighSegment.a, thighSegment.angle, drawColor);
        }
        public void DrawLikeLegOutlines(SpriteBatch spriteBatch, Texture2D[] textures, Color drawColor)
        {
            float outlineOffset = 2;
            DrawLeg(spriteBatch, textures, drawColor, Vector2.UnitX * outlineOffset);
            DrawLeg(spriteBatch, textures, drawColor, -Vector2.UnitX * outlineOffset);
            DrawLeg(spriteBatch, textures, drawColor, Vector2.UnitY * outlineOffset);
            DrawLeg(spriteBatch, textures, drawColor, -Vector2.UnitY * outlineOffset);
        }

        private void DrawLeg(SpriteBatch spriteBatch, Texture2D[] textures, Color drawColor, Vector2 withOffset)
        {
            Texture2D thighTexture = textures[0];
            Texture2D kneeTexture = textures[1];
            Texture2D legTexture = textures[2];
            Texture2D footTexture = textures[3];


            Segment legSegment = segments[segments.Length - 1];
            Segment thighSegment = segments[0];

            //Draw Thight
            Draw(spriteBatch, thighTexture, thighSegment.a + withOffset, thighSegment.angle, drawColor);


            //Draw Leg
            Draw(spriteBatch, legTexture, legSegment.a + withOffset, legSegment.angle, drawColor);

            //Draw Foot
            DrawCenetered(spriteBatch, footTexture, legSegment.b + withOffset, MathHelper.PiOver2, drawColor);

            //Draw Knee
            DrawCenetered(spriteBatch, kneeTexture, thighSegment.b + withOffset, thighSegment.angle, drawColor);
            DrawCenetered(spriteBatch, kneeTexture, thighSegment.a + withOffset, thighSegment.angle, drawColor);
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
