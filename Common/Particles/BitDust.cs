using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Particles;


public class BitDust : ParticleUpdater<BitDustParticleData>
{
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private VertexBuffer _instanceBuffer;
    private BitDustInstanceData[] _instances;
    private VertexBufferBinding[] _bindings;

    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 3 };
    public override int GetPoolSize()
    {
        //Perf tests were done with 1,000,000 max particles
        return 2_000;
    }

    public override void Load(Mod mod)
    {
        base.Load(mod);
        void LoadBuffers()
        {
            //Prepare buffers for GPU Instancing
            var vertices = new VertexPositionTexture[4];

            float halfWidth = 64 * 0.5f;
            float halfHeight = 64 * 0.5f;
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

            _instanceBuffer = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(BitDustInstanceData),
                _particles.Length, BufferUsage.WriteOnly);

            _instances = new BitDustInstanceData[_particles.Length];

            _bindings = new VertexBufferBinding[2];
            _bindings[0] = new VertexBufferBinding(_vertexBuffer);
            _bindings[1] = new VertexBufferBinding(_instanceBuffer, 0, 1);
        }
        Main.QueueMainThreadAction(LoadBuffers);
    }


    public override void Unload()
    {
        base.Unload();
        void UnloadBuffers()
        {
            _vertexBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _instanceBuffer?.Dispose();
            _vertexBuffer = null;
            _indexBuffer = null;
            _instanceBuffer = null;
        }
        Main.QueueMainThreadAction(UnloadBuffers);

    }


    public ref BitDustParticleData Spawn(in BitDustFactory factory)
    {
        if (_length >= _particles.Length)
            return ref _dummyParticle;

        int index = _length;
        _length++;
        factory.CreateInstance(ref _particles[index], ref _instances[index]);
        OnSpawn(ref _particles[index], index);
        return ref _particles[index];
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        FastParallel.For(0, _length, delegate (int start, int end, object context)
        {
            for (int i = start; i < end; i++)
            {
                ref BitDustParticleData particle = ref _particles[i];
                particle.velocity.Y += 0.2f;

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
        var drawData = GetParticleFrame(0);
        float frameHeight = drawData.frame.Height;
        float textureHeight = frameHeight * FrameData.FrameCount;
        float yTiling = frameHeight / textureHeight;
        for (int i = 0; i < _length; i++)
        {
            ref BitDustParticleData particle = ref _particles[i];
            ref BitDustInstanceData instance = ref _instances[i];
            Vector2 scale = particle.scale * particle.stretchScale;
            instance.Transformation = new Vector4(scale.X, scale.Y, particle.position.X, particle.position.Y);
            instance.Color = particle.color;
            float yOffset = (particle.frameIndex * frameHeight) / textureHeight;

            //ytiling and yoffset are static and should be moved out of here
            //Then rotation becomes a separate variable
            instance.TilingOffsetRotation = new Vector3(yTiling, yOffset, particle.rotation);
        }

        _instanceBuffer.SetData(_instances);
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        if (_length <= 0)
            return;

        spriteBatch.End();
        UpdateInstances();


        BitDustShader shader = BitDustShader.Instance;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.BlendState = BlendState.Additive;
        graphicsDevice.SetVertexBuffers(_bindings);
        graphicsDevice.Indices = _indexBuffer;

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