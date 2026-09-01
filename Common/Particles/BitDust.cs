using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
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

public class BitDust : ParticleUpdater<BitDustParticleData>
{
    private GPUInstanceBuffer<BitDustInstanceData> _gpuInstancedBuffer;
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 3 };
    public override int GetPoolSize()
    {
        //Perf tests were done with 1,000,000 max particles
        return 2_000;
    }

    public override void LoadSafe()
    {
        base.LoadSafe();
        _gpuInstancedBuffer = new GPUInstanceBuffer<BitDustInstanceData>(_particles.Length, 64, 64);
        On_Main.DrawDust += DrawParticles;
    }
    public override void UnloadSafe()
    {
        base.UnloadSafe();
        _gpuInstancedBuffer?.Dispose();
    }

    public ref BitDustParticleData Spawn(in BitDustFactory factory)
    {
        if (_length >= _particles.Length)
            return ref _dummyParticle;

        int index = _length;
        _length++;
        factory.CreateInstance(ref _particles[index], ref _gpuInstancedBuffer.instances[index]);
        OnSpawn(ref _particles[index], index);
        return ref _particles[index];
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        if (_length <= 0)
            return;
        FastParallel.For(0, _length, delegate (int start, int end, object context)
        {
            for (int i = start; i < end; i++)
            {
                ref BitDustParticleData particle = ref _particles[i];
                particle.velocity.Y += 0.05f;
                particle.velocity *= particle.velocityPerTickMult;

                particle.timeLeft--;
                particle.position += particle.velocity;

                particle.rotation = particle.velocity.ToRotation();
                particle.scale *= 0.97f;
                particle.color *= 0.99f;

                float stretchInterp = particle.velocity.LengthSquared() / 25f;
                particle.stretchScale.X = MathHelper.Lerp(1f, 1.5f, stretchInterp);
                particle.stretchScale.Y = 1f;

                //This gets expensive kinda quickly, so probably should just have a separate particle system that doesn't collide with tiles at all and one that does
                //Trying to minimize branching as much as possible
                //Perf tests were done without this tile collision check
                /*
     Vector2 collisionVelocity = Collision.TileCollision(particle.position, particle.velocity, 2, 2);
     if (particle.velocity.X != collisionVelocity.X)
         particle.velocity.X = -collisionVelocity.X * 0.7f;
     if (particle.velocity.Y != collisionVelocity.Y)
         particle.velocity.Y = -collisionVelocity.Y * 0.7f;*/
            }
        });


    }


    private void UpdateInstances()
    {
        //  var watch = Stopwatch.StartNew();
        var drawData = GetParticleFrame(0);
        float frameHeight = drawData.frame.Height;
        float textureHeight = frameHeight * FrameData.FrameCount;
        float yTiling = frameHeight / textureHeight;
        var instances = _gpuInstancedBuffer.instances;
        FastParallel.For(0, _length, delegate (int start, int end, object context)
        {
            for (int i = start; i < end; i++)
            {
                ref BitDustParticleData particle = ref _particles[i];
                ref BitDustInstanceData instance = ref instances[i];
                Vector2 scale = particle.scale * particle.stretchScale;
                instance.Transformation = new Vector4(scale.X, scale.Y, particle.position.X, particle.position.Y);
                instance.Color = particle.color;
                float yOffset = (particle.frameIndex * frameHeight) / textureHeight;

                //ytiling and yoffset are static and should be moved out of here
                //Then rotation becomes a separate variable
                instance.TilingOffsetRotation = new Vector3(yTiling, yOffset, particle.rotation);
            }
        });


        // watch.Stop();
        //  elapsedString = $"~{(float)watch.ElapsedTicks / 10000f}ms ::: Particle Count: {_length}";

    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        PixelationManager.QueueSpritebatchDrawAction(Draw, PixelationDrawLayer);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        if (_length <= 0)
            return;

        spriteBatch.End();
        UpdateInstances();
        _gpuInstancedBuffer.PrepareForDrawing(spriteBatch.GraphicsDevice);
        BitDustShader shader = BitDustShader.Instance;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.BlendState = BlendState.Additive;


        shader.SpriteTexture = _particleTextureAsset.Value;
        shader.Projection = TrailDrawer.WorldViewPoint2;


        shader.Effect.CurrentTechnique.Passes[0].Apply();
        graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2, _length);


        spriteBatch.Begin(SpritebatchParams.InWorldAndZoomed());
    }
    public override void Draw(SpriteBatch spriteBatch, ref BitDustParticleData particle)
    {

    }
}
