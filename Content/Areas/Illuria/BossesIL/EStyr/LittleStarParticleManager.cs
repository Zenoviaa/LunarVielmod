using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Threading;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    /// <summary>
    /// Manages the little particles that orbit around the singularity, lots of little particles
    /// </summary>
    public class LittleStarParticleManager
    {
        private float _timer;
        private readonly VertexPositionColor[] _particleVertexBufferArr;
        private readonly Vector2[] _particleOldPos;
        private readonly Vector2[] _trailWidths;
        private readonly FastNoiseLite _fastNoise;
        private readonly float[] _noiseValues;
        public LittleStarParticleManager(int particleCount, int trailLength)
        {
            _fastNoise = new FastNoiseLite();
            ParticleCount = particleCount;
            TrailLength = trailLength;

            //Calculate the number of vertices that we'll need to draw the tornado
            //This should be equal to the particle count times the trail length times the nubmer of vertices per point
            int verticesPerPosition = 6;
            int vertexCount = particleCount * trailLength * verticesPerPosition;
            _particleVertexBufferArr = new VertexPositionColor[vertexCount];
            _particleOldPos = new Vector2[particleCount * trailLength];

            //We can pre calculate the uv floats since it's always the same
            //We increase the trail length by 1 here because in the trailing functionwe need to get the next point, this last position is basically just a duplicate
            _trailWidths = new Vector2[trailLength + 1];
            for (int i = 0; i < _trailWidths.Length; i++)
            {
                float ratio = (float)i / (float)trailLength;
                _trailWidths[i] = GetTrailWidth(ratio) * Vector2.One;
            }


            //calculate noise values of each particle
            _noiseValues = new float[particleCount];
            for (int n = 0; n < _noiseValues.Length; n++)
            {
                _noiseValues[n] = _fastNoise.GetNoise(n, n) * 0.5f + 0.5f;
                _noiseValues[n] *= 0.5f;
            }
        }

        public readonly int ParticleCount;
        public readonly int TrailLength;
        public float xOvalRadius;
        public float yOvalRadius;


        /// <summary>
        /// Calculate the position of the particle at specific a timestep
        /// </summary>
        /// <param name="time"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector2 CalculateParticlePosition(float time, int index)
        {
            const float revolutionTime = 100f;

            //Calculate the rotation offset for this particle
            const float maxRadiansOffset = MathHelper.TwoPi;

            float particleRatio = (float)index / (float)TrailLength;
            float particleRadiansOffset = particleRatio * maxRadiansOffset;
            float timeRadians = time / revolutionTime * MathHelper.TwoPi;
            float rotationRadians = particleRadiansOffset + timeRadians;


            //Calculate the initial position of the particle

            float off = index * 0.1f;
            float x = 200f;
            if (index > ParticleCount / 2)
            {
                x *= 3;
            }
            float xRadius = x + ExtraMath.Osc(-500, 500, 0, off);
            float yRadius = ExtraMath.Osc(-150f, 0f, 1, off) + ExtraMath.Osc(-500f, 500f, 0f, offset: off);
            Vector3 initialPosition = new Vector3(xRadius, yRadius / 2f, yRadius);

            //Create the rotation matrix and Rotate the particle
            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(new Vector3(1, 1, 0.25f), rotationRadians);
            Vector3 rotatedPosition = Vector3.Transform(initialPosition, rotationMatrix);
            Vector2 flatPosition = new Vector2(rotatedPosition.X, rotatedPosition.Y);
            return flatPosition;
        }


        public Vector2 GetRotation(int particleIndex, int index)
        {
            Vector2 prev;
            Vector2 next;

            /*
            Vector2 prev = CalculateParticlePosition(time - 1, index);
            Vector2 next = CalculateParticlePosition(time + 1, index);
            */



            if (index > 0 && index < TrailLength - 1)
            {
                //Read from the old pos array
                int oldPosIndex = particleIndex * TrailLength + index;
                next = _particleOldPos[oldPosIndex];
                prev = _particleOldPos[oldPosIndex + 1];
                return Vector2.Normalize(next - prev).RotatedBy(MathHelper.Pi / 2);
            }
            else
            {
                return Vector2.One;
            }

        }

        private void SimulateParticles()
        {
            float numPoints = TrailLength;
            int numVerticesPerParticle = TrailLength * 6;

            _fastNoise.SetFrequency(2);
            //Shift our position array backward
            FastParallel.For(0, ParticleCount, delegate (int start, int end, object context)
            {
                for (int i = start; i < end; i++)
                {
                    for (int j = TrailLength - 1; j > 0; j--)
                    {
                        int oldPosIndex = i * TrailLength + j;
                        _particleOldPos[oldPosIndex] = _particleOldPos[oldPosIndex - 1];
                    }
                }
            });

            //Simulate all of our particles
            FastParallel.For(0, ParticleCount, delegate (int start, int end, object context)
            {
                for (int i = start; i < end; i++)
                {
                    //Fast noise returns a value between -1 and 1, so we're normalizing it to 0-1 for the lerp function
                    float noiseColorInterpolant = _noiseValues[i];

                    //Width multiplier for the trail
                    float widthMultiplier = 1f;
                    if (noiseColorInterpolant > 0.4f)
                        widthMultiplier *= 8;
                    Color black = Color.Black;
                    for (int j = 0; j < TrailLength; j++)
                    {
                        //Substract to get the previous frames of the particle
                        float timeStep = _timer - j;

                        //Now we have the position of the particle at this specific time step
                        Vector2 currentPosition;
                        Vector2 prevPosition;

                        if (j > 0 && j < TrailLength - 1)
                        {
                            //Read from the old pos array
                            int oldPosIndex = i * TrailLength + j;
                            currentPosition = _particleOldPos[oldPosIndex];
                            prevPosition = _particleOldPos[oldPosIndex + 1];
                        }
                        else
                        {
                            currentPosition = CalculateParticlePosition(timeStep, i);
                            prevPosition = CalculateParticlePosition(timeStep - 1, i);
                            _particleOldPos[i * TrailLength] = currentPosition;
                        }

                        //Calculate the widths
                        Vector2 width = _trailWidths[j] * widthMultiplier;
                        Vector2 width2 = _trailWidths[j + 1] * widthMultiplier;

                        //Calculate the rotation offsets
                        Vector2 off1 = GetRotation(i, j) * width;
                        Vector2 off2 = GetRotation(i, j + 1) * width2;

                        Color col1 = Color.White;
                        Color col2 = Color.White;

                        col1 = Color.Lerp(col1, black, noiseColorInterpolant);
                        col2 = Color.Lerp(col2, black, noiseColorInterpolant);

                        //Apply camera offset
                        currentPosition += Main.Camera.Center;
                        prevPosition += Main.Camera.Center;

                        //Calcualte the index of the vertices
                        int primIndex = i * numVerticesPerParticle + j * 6;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(currentPosition + off1, 0f), col1);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(currentPosition - off1, 0f), col1);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(prevPosition + off2, 0f), col2);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(prevPosition + off2, 0f), col2);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(prevPosition - off2, 0f), col2);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(currentPosition - off1, 0f), col1);
                    }
                }
            });
        }

        public void Update()
        {
            _timer++;
            SimulateParticles();
        }

        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(0.66f, 0, completionRatio);
        }

        public void Draw()
        {
            var particleShader = TileShadowShader.Instance;
            particleShader.ApplyPasses();

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.DrawUserPrimitives(
              PrimitiveType.TriangleList, _particleVertexBufferArr, 0, _particleVertexBufferArr.Length / 3);

        }
    }
}
