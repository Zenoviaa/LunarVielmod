using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Core.Utilities
{
    /// <summary>
    /// Represents a verlet point, with a position and old position
    /// </summary>
    public struct VerletPoint
    {
        public Vector2 position;
        public Vector2 oldPosition;
        public bool pinned;
    }

    /// <summary>
    /// A connection of points simulated with verlet integration, great for rope physics
    /// </summary>
    public class VerletChain
    {
        public VerletPoint[] points;
        public VerletChain(int numPoints, Vector2 initialPosition, Vector2 initialDirection)
        {
            points = new VerletPoint[numPoints];
            for (int i = 0; i < numPoints; i++)
            {
                ref VerletPoint point = ref points[i];
                point.position = initialPosition + initialDirection * i;
                point.oldPosition = point.position;
            }
            gravity = 0.5f;
            segmentLength = 2;
            subdivisionCount = 1;
        }

        public VerletChain(Vector2 startPosition, Vector2 endPosition, float pointLength)
        {
            List<VerletPoint> points = new List<VerletPoint>();
            float distance = Vector2.Distance(startPosition, endPosition);
            int numPoints = (int)MathF.Floor(distance / pointLength / 2f) * 2;


            for(int i = 0; i < numPoints; i++)
            {
                VerletPoint point = new VerletPoint();
                float completionRatio = (float)i / (float)numPoints;
                point.position = Vector2.Lerp(startPosition, endPosition, completionRatio);
                point.oldPosition = point.position;
                points.Add(point);
            }
            gravity = 0.5f;
            segmentLength = pointLength;
            subdivisionCount = 5;
            this.points = points.ToArray();
        }

        public int pointRadius = 4;
        public float gravity;
        public float segmentLength;
        public int subdivisionCount;
        public bool noTileCollide;
        public Vector2 externalForces;
        public void Update()
        {
            UpdateVelocities();

            for (int i = 0; i < subdivisionCount; i++)
            {
        
                UpdateConstraints();

            }
         
        }

        private void UpdateVelocities()
        {
            //Loops over all of our points, calculate the velocity and apply
            for (int i = 0; i < points.Length; i++)
            {
                ref VerletPoint point = ref points[i];
                if (point.pinned)
                    continue;

                Vector2 velocity = point.position - point.oldPosition;
                velocity.Y += gravity;
                velocity += externalForces;
                point.oldPosition = point.position;

                //Interact with tiles, the tile collision function returns an inverse velocity I think?
                //If it doesn't we can just invert it lol
                if (!noTileCollide)
                {
                    Vector2 collisionVelocity = Collision.TileCollision(point.position, velocity, pointRadius, pointRadius);
                    point.position += collisionVelocity;

                }

            }

        }
        private void UpdateConstraints()
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                ref VerletPoint p1 = ref points[i];
                ref VerletPoint p2 = ref points[i + 1];

                float dx = p2.position.X - p1.position.X;
                float dy = p2.position.Y - p1.position.Y;
                float distance = MathF.Sqrt(dx * dx + dy * dy);
                if(distance > segmentLength)
                {
                    float difference = segmentLength - distance;
                    float percent = difference / distance / 2f;
                    float offsetX = dx * percent;
                    float offsetY = dy * percent;


                    p1.position.X -= offsetX;
                    p1.position.Y -= offsetY;

                    p2.position.X += offsetX;
                    p2.position.Y += offsetY;
                }

            }
        }

        public void FillArr(Vector2[] linePoints)
        {
            for(int i = 0; i < linePoints.Length; i++)
            {
                linePoints[i] = points[i].position;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < points.Length; i++)
            {
                VerletPoint point = points[i];
                Primitives2D.DrawCircle(spriteBatch, point.position - Main.screenPosition, 16, 16, Color.Red);
            }
        }
    }

}
