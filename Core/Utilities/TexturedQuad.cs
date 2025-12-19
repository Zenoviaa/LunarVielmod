using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Terraria;

namespace Stellamod.Core.Utilities
{
    public class TexturedQuad
    {
        public TexturedQuad()
        {
            vertices = new VertexPositionColorTexture[4];
            indices = new short[6];

            //Triangle 1
            indices[0] = 0;
            indices[1] = 2;
            indices[2] = 3;

            //Triangle 2
            indices[3] = 0;
            indices[4] = 1;
            indices[5] = 3;
        }

        public readonly VertexPositionColorTexture[] vertices;
        public readonly short[] indices;
        public void DrawWithShader(BaseShader shader)
        {
            shader.ApplyPasses();
            Draw();
        }
        public void Draw()
        {
            GraphicsDevice graphicsDevice = Main.spriteBatch.GraphicsDevice;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, 2);
        }

        public void SetColor(Color color)
        {
            for(int i = 0; i < vertices.Length; i++)
            {
                ref VertexPositionColorTexture vertex = ref vertices[i];
                vertex.Color = color;
            }
        }

        public void CalculateCenterVertices(Vector2 center, float length, float width, float rotation = 0)
        {
            Vector2 topLeftOffset = new Vector2(-length / 2f, -width / 2f);
            Vector2 bottomLeftOffset = new Vector2(-length / 2f, width / 2f);
            Vector2 topRightOffset = topLeftOffset + new Vector2(length, -width / 2f);
            Vector2 bottomRightOffset = bottomLeftOffset + new Vector2(length, width / 2f);

            topLeftOffset = topLeftOffset.RotatedBy(rotation);
            bottomLeftOffset = bottomLeftOffset.RotatedBy(rotation);
            topRightOffset = topRightOffset.RotatedBy(rotation);
            bottomRightOffset = bottomRightOffset.RotatedBy(rotation);

            Vector2 topLeft = center + topLeftOffset;
            Vector2 bottomLeft = center + bottomLeftOffset;
            Vector2 topRight = center + topRightOffset;
            Vector2 bottomRight = center + bottomRightOffset;


            vertices[0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), Color.White, new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(topRight, 0), Color.White, new Vector2(1, 0));

            vertices[2] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), Color.White, new Vector2(0, 1));
            vertices[3] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), Color.White, new Vector2(1, 1));
        }

        public void CalculateVertices(Vector2 position, Vector2 direction, float length, float width)
        {            //Triangle 1

            Vector2 topLeftOffset = new Vector2(0, -width / 2f);
            Vector2 bottomLeftOffset = new Vector2(0, width / 2f);
            Vector2 topRightOffset = topLeftOffset + new Vector2(length, 0);
            Vector2 bottomRightOffset = bottomLeftOffset + new Vector2(length, 0);

            float rotation = direction.ToRotation();
            topLeftOffset = topLeftOffset.RotatedBy(rotation);
            bottomLeftOffset = bottomLeftOffset.RotatedBy(rotation);
            topRightOffset = topRightOffset.RotatedBy(rotation);
            bottomRightOffset = bottomRightOffset.RotatedBy(rotation);

            Vector2 topLeft = position + topLeftOffset;
            Vector2 bottomLeft = position + bottomLeftOffset;
            Vector2 topRight = position + topRightOffset;
            Vector2 bottomRight = position + bottomRightOffset;

            vertices[0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), Color.White, new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(topRight, 0), Color.White, new Vector2(1, 0));

            vertices[2] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), Color.White, new Vector2(0, 1));
            vertices[3] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), Color.White, new Vector2(1, 1));
        }
    }
}
