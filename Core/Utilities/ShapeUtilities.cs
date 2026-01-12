using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Core.Utilities
{
    public struct Line
    {
        public Line(Vector2 a, Vector2 b)
        {
            this.a = a;
            this.b = b;
        }
        public Vector2 a;
        public Vector2 b;
    }

    public static class ShapeUtilities
    {

        public static List<Line> GetLines(Vector2[] shapePoints, float detectLineChangeThreshold = 0.5f)
        {
            List<Line> lines = new List<Line>();
            Vector2 lineStart = shapePoints[0];
            Vector2 lastMovement = shapePoints[1] - shapePoints[0];
            lastMovement = lastMovement.SafeNormalize(Vector2.Zero);

            for (int s = 2; s < shapePoints.Length; s++)
            {
                Vector2 movement = shapePoints[s] - shapePoints[s - 1];
                movement = movement.SafeNormalize(Vector2.Zero);
                float dp = Vector2.Dot(lastMovement, movement);
                float threshold = detectLineChangeThreshold;
                if (dp < threshold || s == shapePoints.Length - 1)
                {
                    Line line = new Line(lineStart, shapePoints[s]);
                    lines.Add(line);
                    lineStart = shapePoints[s];
                    lastMovement = movement;
                }

            }
            return lines;
        }


        public static float CountAngles(List<Line> lines, float targetAngle, float marginOfError)
        {
            float numMatches = 0;
            float minAngle = targetAngle - marginOfError;
            float maxAngle = targetAngle + marginOfError;
            for (int i = 0; i < lines.Count; i++)
            {
                Line prevLine;
                if (i == 0)
                {
                    prevLine = lines[lines.Count - 1];
                }
                else
                {
                    prevLine = lines[i - 1];

                }

                Line line = lines[i];

                //Calculate the angle between the lines
                float angle1 = (line.b - line.a).ToRotation();
                float angle2 = (prevLine.b - prevLine.a).ToRotation();
                float diff = MathF.Abs(angle2 - angle1);

                //If the angle difference is too big, just invert the angle so we get the other side, and only check for 90 degree angles
                float angleDiff = diff > MathHelper.Pi ? MathHelper.TwoPi - diff : diff;

                //This conversion is technically unnecessary, we could just use radians it's just easier to wrap my head around this lol
                float diffDegrees = angleDiff * 180 / MathF.PI;
          //      Main.NewText(diffDegrees);
                if (diffDegrees >= minAngle && diffDegrees <= maxAngle)
                {
                    numMatches++;
                }
            }
            return numMatches;
        }
    }
}
