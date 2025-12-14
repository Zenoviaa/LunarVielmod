using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Core.TailSystem
{
    public struct TailPoint
    {
        public Vector2 position;
        public Vector2 oldPosition;
        public float amplitude;
        public float frequency;
    }
    public class TailSimulation
    {
        private float _time;
        public TailSimulation(int numPoints, float totalLength)
        {
            this.numPoints = numPoints;
            this.totalLength = totalLength;
            this.points = new TailPoint[numPoints];
            this.baseAmplitude = 16;
            this.baseFrequency = 1;
        }

        public readonly int numPoints;
        public readonly float totalLength;
        public readonly TailPoint[] points;

        public float baseAmplitude;
        public float baseFrequency;
        public Vector2 rootPosition;
        public Vector2 initialDirection;
        public Vector2 gravity;
        public void Update()
        {
            _time += 0.1f;
            if (_time >= MathHelper.TwoPi)
                _time = 0f;
            float time = _time;
            float segmentLength = (float)totalLength / (float)numPoints;
            float twoSegmentLength = segmentLength * 2;
            for (int i = 0; i < points.Length; i++)
            {
                ref TailPoint point = ref points[i];
                float completionRatio = (float)i / (float)numPoints;
                point.frequency = MathHelper.Lerp(baseFrequency, baseFrequency * 3f, completionRatio);
                point.amplitude = MathHelper.Lerp(baseAmplitude * 0.15f, baseAmplitude, completionRatio);
            }
                
            for (int i = 0; i < points.Length; i++)
            {
                ref TailPoint point = ref points[i];
                float height = point.amplitude * MathF.Sin(point.frequency * MathHelper.TwoPi + time);


                float weight = (float)i / (float)numPoints;
                Vector2 gravityModifier = gravity * weight;

                float offset = i * totalLength / numPoints;
                Vector2 tailPoint = new Vector2(-offset, height);
                tailPoint = tailPoint.RotatedBy(initialDirection.ToRotation());
                tailPoint += gravityModifier;

                Vector2 newPosition = rootPosition + tailPoint;
                Vector2 diff = newPosition - point.oldPosition;
 
                point.oldPosition = point.position;

                Vector2 smoothedDiff = diff * MathHelper.SmoothStep(1.0f, 0.01f, weight);
                point.position += smoothedDiff;

                float distance = diff.Length();
                if (distance > twoSegmentLength)
                {
                    point.position = newPosition;
                } 
            }
        }

        public void FillArr(Vector2[] positions)
        {
            for(int i = 0; i < positions.Length; i++)
            {
                positions[i] = points[i].position;
            }
        }
    }
}
