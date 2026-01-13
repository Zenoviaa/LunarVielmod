using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
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

        public void CalculatePerspectiveCenterVertices(Vector2 center, float length, float width, float rotation = 0, float perspectiveRotation = 0)
        {
            Vector3 topLeftOffset = new Vector3(0.15f, -1, -1);
            Vector3 bottomLeftOffset = new Vector3(0.15f, 1, -1);
            Vector3 topRightOffset = new Vector3(0, -1, 1);
            Vector3 bottomRightOffset = new Vector3(0, 1, 1);

            //Rotate around the center pivot, considering the Z axis
            Vector2 axis = rotation.ToRotationVector2();
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0.15f), perspectiveRotation);
            Matrix rotationMatrix = Matrix.CreateFromQuaternion(quaternion);

            topLeftOffset = Vector3.Transform(topLeftOffset, rotationMatrix);
            bottomLeftOffset = Vector3.Transform(bottomLeftOffset, rotationMatrix);
            topRightOffset = Vector3.Transform(topRightOffset, rotationMatrix);
            bottomRightOffset = Vector3.Transform(bottomRightOffset, rotationMatrix);


            float halfLength = length * 0.5f;
            float halfWidth = width * 0.5f;

            Vector2 halfSize = new Vector2(halfLength, halfWidth);


            Vector2 tl = new Vector2(topLeftOffset.X, topLeftOffset.Y).RotatedBy(rotation);
            Vector2 bl = new Vector2(bottomLeftOffset.X, bottomLeftOffset.Y).RotatedBy(rotation);
            Vector2 tr = new Vector2(topRightOffset.X, topRightOffset.Y).RotatedBy(rotation);
            Vector2 br = new Vector2(bottomRightOffset.X, bottomRightOffset.Y).RotatedBy(rotation);


            Vector2 topLeft = center + tl * halfSize;
            Vector2 bottomLeft = center + bl * halfSize;
            Vector2 topRight = center + tr * halfSize;
            Vector2 bottomRight = center + br * halfSize;





            vertices[0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), Color.White, new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(topRight, 0), Color.White, new Vector2(1, 0));

            vertices[2] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), Color.White, new Vector2(0, 1));
            vertices[3] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), Color.White, new Vector2(1, 1));
        }
        public void CalculatePerspectiveCenterVertices2(Vector2 center, float length, float width, float rotation = 0, float perspectiveRotation = 0)
        {
            float altAxis = 0.05f;
            Vector3 topLeftOffset = new Vector3(altAxis, -1, -1);
            Vector3 bottomLeftOffset = new Vector3(altAxis, 1, -1);
            Vector3 topRightOffset = new Vector3(0, -1, 1);
            Vector3 bottomRightOffset = new Vector3(0, 1, 1);

            //Rotate around the center pivot, considering the Z axis
            Vector2 axis = rotation.ToRotationVector2();
            Quaternion quaternion = Quaternion.CreateFromAxisAngle(new Vector3(1, 0, altAxis), perspectiveRotation);
            Matrix rotationMatrix = Matrix.CreateFromQuaternion(quaternion);

            topLeftOffset = Vector3.Transform(topLeftOffset, rotationMatrix);
            bottomLeftOffset = Vector3.Transform(bottomLeftOffset, rotationMatrix);
            topRightOffset = Vector3.Transform(topRightOffset, rotationMatrix);
            bottomRightOffset = Vector3.Transform(bottomRightOffset, rotationMatrix);


            float halfLength = length * 0.5f;
            float halfWidth = width * 0.5f;

            Vector2 halfSize = new Vector2(halfLength, halfWidth);


            Vector2 tl = new Vector2(topLeftOffset.X, topLeftOffset.Y).RotatedBy(rotation);
            Vector2 bl = new Vector2(bottomLeftOffset.X, bottomLeftOffset.Y).RotatedBy(rotation);
            Vector2 tr = new Vector2(topRightOffset.X, topRightOffset.Y).RotatedBy(rotation);
            Vector2 br = new Vector2(bottomRightOffset.X, bottomRightOffset.Y).RotatedBy(rotation);


            Vector2 topLeft = center + tl * halfSize;
            Vector2 bottomLeft = center + bl * halfSize;
            Vector2 topRight = center + tr * halfSize;
            Vector2 bottomRight = center + br * halfSize;





            vertices[0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), Color.White, new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(topRight, 0), Color.White, new Vector2(1, 0));

            vertices[2] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), Color.White, new Vector2(0, 1));
            vertices[3] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), Color.White, new Vector2(1, 1));
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

            
            //Rotate around the center pivot



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

            vertices[0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), Color.White, new Vector2(1, 1));
            vertices[1] = new VertexPositionColorTexture(new Vector3(topRight, 0), Color.White, new Vector2(0, 1));

            vertices[2] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), Color.White, new Vector2(1, 0));
            vertices[3] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), Color.White, new Vector2(0, 0));
        }
    }
}
