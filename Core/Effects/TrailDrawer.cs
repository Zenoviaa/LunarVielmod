using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria;
using Stellamod.Core.Helpers.Math;

namespace Stellamod.Core.Effects
{
    internal abstract class TrailDrawer : ITrailer
    {

        public static Matrix WorldViewPoint
        {
            get
            {
                GraphicsDevice graphics = Main.graphics.GraphicsDevice;
                Vector2 screenZoom = Main.GameViewMatrix.Zoom;
                int width = graphics.Viewport.Width;
                int height = graphics.Viewport.Height;

                var zoom = Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up) *
                    Matrix.CreateTranslation(width / 2f, height / -2f, 0) *
                    Matrix.CreateRotationZ(MathHelper.Pi) * Matrix.CreateScale(screenZoom.X, screenZoom.Y, 1f);
                var projection = Matrix.CreateOrthographic(width, height, 0, 1000);
                return zoom * projection;
            }
        }

        public static Matrix WorldViewPoint2
        {
            get
            {
                Vector3 screenPosition = new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0);
                Matrix world = Matrix.CreateTranslation(-screenPosition);
                Matrix view = Main.GameViewMatrix.TransformationMatrix;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);
                return world * view * projection;
            }
        }   
        
        /// <summary>
        /// Value that tells how much progress has been made on whatever is using this trail.
        /// Useful for it the trail is supposed to gradually change over time.
        /// </summary>
        public float Interpolant { get; set; }
        public Shader Shader { get; set; }
        public void SetTrailingValues(float interpolant)
        {
            Interpolant = interpolant;
        }

        public virtual float GetTrailWidth(float t)
        {
            return EasingFunction.QuadraticBump(t) * 16;
        }

        public virtual Color GetTrailColor(float t)
        {
            return Color.Lerp(Color.White, Color.White, t);
        }

        private void CalculateVerticesTris(Vector2[] trailingPoints, List<VertexPositionColorTexture> vertices)
        {

            for (int i = 0; i < trailingPoints.Length - 1; i++)
            {
                float uv = i / (float)trailingPoints.Length;
                float uv2 = (i + 1) / (float)trailingPoints.Length;
                Vector2 width = GetTrailWidth(uv) * Vector2.One;
                Vector2 width2 = GetTrailWidth(uv2) * Vector2.One;
                Vector2 pos1 = trailingPoints[i];
                Vector2 pos2 = trailingPoints[i + 1];

                Vector2 off1 = ExtraMath.GetRotation(trailingPoints, i) * width;
                Vector2 off2 = ExtraMath.GetRotation(trailingPoints, i + 1) * width2;

                Color col1 = GetTrailColor(uv);
                Color col2 = GetTrailColor(uv2);
                float uvAdd = 0;
                float uvMultiplier = 1;
                float coord1 = 0;
                float coord2 = 1;
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 + off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord2)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 + off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord1)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos2 - off2, 0f), col2, new Vector2((uv2 + uvAdd) * uvMultiplier, coord2)));
                vertices.Add(new VertexPositionColorTexture(new Vector3(pos1 - off1, 0f), col1, new Vector2((uv + uvAdd) * uvMultiplier, coord2)));
            }
        }

        private List<VertexPositionColorTexture> CalculateVertices(Vector2[] oldPos, Vector2? offset = null)
        {
            Vector2 o = offset == null ? Vector2.Zero : (Vector2)offset;
            var vertices = new List<VertexPositionColorTexture>();
            oldPos = ExtraMath.RemoveZeros(oldPos, o);
            ExtraMath.LerpTrailPoints(oldPos, out Vector2[] trailingPoints);
            CalculateVerticesTris(trailingPoints, vertices);
            return vertices;
        }

        private void DrawPrimsTriangles(List<VertexPositionColorTexture> vertices, Shader shader)
        {
            if (vertices.Count % 6 != 0 || vertices.Count <= 3)
                return;

            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            BlendState originalBlendState = graphicsDevice.BlendState;
            CullMode oldCullMode = graphicsDevice.RasterizerState.CullMode;
            SamplerState originalSamplerState = graphicsDevice.SamplerStates[0];

            graphicsDevice.RasterizerState.CullMode = CullMode.None;

            if (shader != null)
            {
                graphicsDevice.BlendState = shader.BlendState;
                graphicsDevice.SamplerStates[0] = shader.SamplerState;
            }
            foreach (var pass in shader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }
            graphicsDevice.DrawUserPrimitives(
              PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }

        public void DrawTrail(Vector2[] trailCache)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Shader.ApplyToEffect();
            var vertices = CalculateVertices(trailCache);
            DrawPrimsTriangles(vertices, Shader);
        }
    }
}
