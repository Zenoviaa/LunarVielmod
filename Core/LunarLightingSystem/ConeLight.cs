using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Core.LunarLightingSystem
{
    public interface ILight
    {
        void RayCast(Vector2 position, Vector2 direction, float edgeLightWidth, float distance);
        void Draw();
    }

    public class PointLight : ILight
    {
        private LegacyPointLight _pointLight;
        public PointLight()
        {
            _pointLight = new LegacyPointLight(Vector2.Zero, Color.White, 1, 100, 1800);
        }
        public void RayCast(Vector2 position, Vector2 direction, float edgeLightWidth, float distance)
        {
            _pointLight.position = position;
            _pointLight.radius = distance;
            _pointLight.lightNormal = Vector2.Zero;
            _pointLight.threshold = 0f;
            _pointLight.Update();
        }

        public void Draw()
        {
            _pointLight.DrawLight();
            _pointLight.DrawShadow();
        }
    }

    public class ConeLight : ILight
    {
        public Color lightColor;
        private VertexPositionColorTexture[] _vertices;
        private LegacyPointLight _pointLight;
        public ConeLight()
        {
            lightColor = Color.White;
            _vertices = new VertexPositionColorTexture[12];
            _pointLight = new LegacyPointLight(Vector2.Zero, Color.White, 1, 100, 1800);
        }


        public void RayCast(Vector2 position, Vector2 direction, float edgeLightWidth, float distance)
        {


            float edgeLightRadius = edgeLightWidth / 2f;
            float castMultiplier = 0.1f;
            float edgeColorMultiplier = 0f;
            Vector2 start = position;
            Vector2 end = start + direction * distance;

            //First Quad
            Vector2 topRightVertex = end - direction.RotatedBy(MathHelper.PiOver2) * edgeLightRadius;
            Vector2 bottomRightVertex = end;

            Vector2 topLeftVertex = start - direction.RotatedBy(MathHelper.PiOver2) * edgeLightRadius * castMultiplier;
            Vector2 bottomLeftVertex = start;

            _vertices[0] = new VertexPositionColorTexture(new Vector3(topLeftVertex, 0), lightColor, new Vector2(1, 1));
            _vertices[1] = new VertexPositionColorTexture(new Vector3(bottomLeftVertex, 0), lightColor, new Vector2(1, 0));
            _vertices[2] = new VertexPositionColorTexture(new Vector3(bottomRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(0, 0));

            _vertices[3] = new VertexPositionColorTexture(new Vector3(topLeftVertex, 0), lightColor, new Vector2(1, 1));
            _vertices[4] = new VertexPositionColorTexture(new Vector3(topRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(0, 1));
            _vertices[5] = new VertexPositionColorTexture(new Vector3(bottomRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(0, 0));

            //Second Quad
            topRightVertex = end;
            bottomRightVertex = end + direction.RotatedBy(MathHelper.PiOver2) * edgeLightRadius;

            topLeftVertex = start;
            bottomLeftVertex = start + direction.RotatedBy(MathHelper.PiOver2) * edgeLightRadius * castMultiplier;

            _vertices[6] = new VertexPositionColorTexture(new Vector3(topLeftVertex, 0), lightColor, new Vector2(0, 0));
            _vertices[7] = new VertexPositionColorTexture(new Vector3(bottomLeftVertex, 0), lightColor, new Vector2(0, 1));
            _vertices[8] = new VertexPositionColorTexture(new Vector3(bottomRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(1, 1));

            _vertices[9] = new VertexPositionColorTexture(new Vector3(topLeftVertex, 0), lightColor, new Vector2(0, 0));
            _vertices[10] = new VertexPositionColorTexture(new Vector3(topRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(1, 0));
            _vertices[11] = new VertexPositionColorTexture(new Vector3(bottomRightVertex, 0), lightColor * edgeColorMultiplier, new Vector2(1, 1));


            _pointLight.position = start;
            _pointLight.radius = distance;
            _pointLight.lightNormal = direction;
            _pointLight.threshold = 0.9f;
            _pointLight.Update();
        }

        public void Draw()
        {
            var shader = LanternShader.Instance;
            shader.Apply();
            foreach (var pass in shader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            GraphicsHelpers.SaveGraphicsDeviceState();

            graphicsDevice.RasterizerState.CullMode = CullMode.None;
            graphicsDevice.BlendState = BlendState.Additive;
            graphicsDevice.DrawUserPrimitives(
              PrimitiveType.TriangleList, _vertices, 0, _vertices.Length / 3);

            GraphicsHelpers.RestoreGraphicsDeviceState();

            _pointLight.DrawShadow();
        }
    }
}
