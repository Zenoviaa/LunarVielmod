using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Diagnostics;
using Terraria;

namespace Stellamod.Core.InverseKinematics
{
    public class Segment
    {
        public 
            Segment(Vector2 position, float length, float angle)
        {
            a = position;
            this.length = length;
            this.angle = angle;
            CalculateB();
        }

        public Segment(Segment parent, float length, float angle)
        {
            a = parent.b;
            this.length = length;
            this.angle = angle;
            angle = rootDirection.ToRotation();
            CalculateB();
        }

        public Vector2 a;
        public Vector2 b;
        public float length;
        public float angle;
        public Vector2 rootDirection;
        public float rangeOfMotion;
        public float oldAngle;
        public float GetDefaultAngle()
        {
            return rootDirection.ToRotation();
        }

        public void SetA(Vector2 pos)
        {
            a = pos;
            CalculateB();
        }

        public void CalculateB()
        {
            float dx = length * MathF.Cos(angle);
            float dy = length * MathF.Sin(angle);
            b = a + new Vector2(dx, dy);
        }

        public void Follow(Segment child)
        {
            Vector2 target = child.a;
            Follow(target);
        }

        public void Follow(Vector2 target)
        {
            Vector2 direction = target - a;
            float newAngle = MathF.Atan2(direction.Y, direction.X);
            if(rangeOfMotion != 0)
            {
                //Check if the new angle is within our range of motion, we won't move to it if it isn't
                direction.Normalize();
                float dp = Vector2.Dot(direction, rootDirection);
                angle = newAngle;
            } else
            {
                angle = newAngle;
            }


 
            direction *= length;
            direction *= -1;
            a = target + direction;
            oldAngle = angle;
        }

        public void Update()
        {
            CalculateB();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Primitives2D.DrawLine(spriteBatch, a - Main.screenPosition, b - Main.screenPosition, Color.White);
        }
    }
}
