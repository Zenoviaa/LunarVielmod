using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Core.LunarLightingSystem
{
    public class ConeLight
    {
        public Color lightColor;
        private VertexPositionColor[] _vertices;
        public ConeLight()
        {
            _vertices = new VertexPositionColor[3]; 
        }

        public void RayCast(Vector2 position, Vector2 direction, float radians, float distance)
        {
            Vector2 start = position;

            float halfRadians = radians / 2f;
            Vector2 vel1 = direction.RotatedBy(halfRadians) * distance;
            Vector2 vel2 = direction.RotatedBy(-halfRadians) * distance;

            Vector2 point1 = CollisionHelper.RayCast(start, vel1, distance);
            Vector2 point2 = CollisionHelper.RayCast(start, vel2, distance);

            _vertices[0] = new VertexPositionColor(new Vector3(start, 0), lightColor);
            _vertices[1] = new VertexPositionColor(new Vector3(point1, 0), lightColor * 0);
            _vertices[2] = new VertexPositionColor(new Vector3(point2, 0), lightColor * 0);
        }

        public void Draw()
        {
            var shader = TileShadowShader.Instance;
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
        }
    }
}
