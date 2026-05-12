using Stellamod.Common.Shaders;
using Terraria;

namespace Stellamod.Core.Utilities;

public class Quad<T>
    where T : struct, IVertexType
{
    public Quad()
    {
        vertices = new T[4];
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
    public readonly T[] vertices;
    public readonly short[] indices;
    public void Draw(BaseShader shader)
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
}
