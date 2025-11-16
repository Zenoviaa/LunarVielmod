using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Core.InverseKinematics
{
    public class ArmatureV2
    {
        public float r1;
        public float r2;
        public float angle1;
        public float angle2;
        public void Solve(Vector2 p)
        {
            angle2 = MathF.Acos(((p.X * p.X + p.Y * p.Y) - r1 * r1 - r2 * r2) / (2 * r1 * r2));
        //    angle2 = -angle2;
            angle1 = MathF.Atan2(p.Y, p.X) - MathF.Atan(
                    (r2 * MathF.Sin(angle2)) / (r1 + r2 * MathF.Cos(angle2)));
          
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 rootPosition)
        {
            Vector2 screenPos = Main.screenPosition;
            Vector2 pos1 = angle1.ToRotationVector2() * r1;
            Vector2 pos2 = pos1 + angle2.ToRotationVector2() * r2;
            pos1 -= screenPos;
            pos2 -= screenPos;

            pos1 += rootPosition;
            pos2 += rootPosition;
            Primitives2D.DrawLine(spriteBatch, rootPosition - screenPos, pos1, Color.White);
            Primitives2D.DrawLine(spriteBatch, pos1, pos2, Color.White);
        }
    }
}
