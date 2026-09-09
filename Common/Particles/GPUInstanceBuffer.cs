using System;
using Terraria;

namespace Stellamod.Common.Particles;

public class GPUInstanceBuffer<InstanceData> : IDisposable
    where InstanceData : struct, IVertexType

{
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private VertexBuffer _instanceBuffer;
    public GPUInstanceBuffer(int length, int frameWidth, int frameHeight)
    {
        //Prepare buffers for GPU Instancing
        var vertices = new VertexPositionTexture[4];

        float halfWidth = frameWidth * 0.5f;
        float halfHeight = frameHeight * 0.5f;
        vertices[0] = new VertexPositionTexture(new Vector3(-halfWidth, -halfHeight, 0), new Vector2(0, 0));
        vertices[1] = new VertexPositionTexture(new Vector3(halfWidth, -halfHeight, 0), new Vector2(1, 0));
        vertices[2] = new VertexPositionTexture(new Vector3(-halfWidth, halfHeight, 0), new Vector2(0, 1));
        vertices[3] = new VertexPositionTexture(new Vector3(halfWidth, halfHeight, 0), new Vector2(1, 1));

        _vertexBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(VertexPositionTexture), 4, BufferUsage.WriteOnly);
        _vertexBuffer.SetData<VertexPositionTexture>(vertices);

        _indexBuffer = new IndexBuffer(Main.graphics.GraphicsDevice, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
        _indexBuffer.SetData(new ushort[] {
                    0, 2, 3,
                    0, 1, 3
                });

        _instanceBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(InstanceData),
            length, BufferUsage.WriteOnly);

        instances = new InstanceData[length];

        bindings = new VertexBufferBinding[2];
        bindings[0] = new VertexBufferBinding(_vertexBuffer);
        bindings[1] = new VertexBufferBinding(_instanceBuffer, 0, 1);
    }

    public VertexBufferBinding[] bindings;
    public InstanceData[] instances;

    public void Dispose()
    {
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        _instanceBuffer?.Dispose();
    }

    public void PrepareForDrawing(GraphicsDevice graphicsDevice)
    {
        _instanceBuffer.SetData(instances);
        graphicsDevice.SetVertexBuffers(bindings);
        graphicsDevice.Indices = _indexBuffer;
    }
}
