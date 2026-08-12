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
    //ok, time to try gpu instancing...
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 3 };
    public override int GetPoolSize()
    {
        return 1_000;
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
        }
        Main.QueueMainThreadAction(UnloadBuffers);

    }

    public override void OnSpawn(ref BitDustParticleData particle)
    {
        //particle.gravity = 0.2f;
        //particle.innerColor = Vector4.One;
        //particle.outerColor = new Vector4(1f, 0f, 0f, 1f);
        particle.frameIndex = Main.rand.Next(3);
        particle.stretchScale = Vector2.One;
    }

    protected override void UpdateParticles()
    {
        base.UpdateParticles();
        FastParallel.For(0, _length, delegate (int start, int end, object context)
        {
            for (int i = start; i < end; i++)
            {
                ref BitDustParticleData particle = ref _particles[i];
                particle.timeLeft--;
                particle.Position += particle.Velocity;


                particle.Velocity.Y += 0.2f;
                
                if (particle.Scale.X < 0.1f)
                    particle.timeLeft = 0;
                particle.Rotation = particle.Velocity.ToRotation();
                particle.Scale *= 0.97f;
                particle.Velocity.Y += 0.2f;

                particle.Scale *= 0.97f;
                particle.color *= 0.99f;

                float stretchInterp = particle.Velocity.LengthSquared() / 25f;
                particle.stretchScale.X = MathHelper.Lerp(1f, 1.5f, stretchInterp);
                particle.stretchScale.Y = 1f;
                if (particle.Scale.X < 0.1f)
                    particle.timeLeft = 0;

                particle.timeLeft--;
                particle.Position += particle.Velocity;

                Vector2 collisionVelocity = Collision.TileCollision(particle.Position, particle.Velocity, 2, 2);
                if (particle.Velocity.X != collisionVelocity.X)
                    particle.Velocity.X = -collisionVelocity.X * 0.7f;
                if (particle.Velocity.Y != collisionVelocity.Y)
                    particle.Velocity.Y = -collisionVelocity.Y * 0.7f;
            }
        });

    }


    private void UpdateInstances()
    {
        var drawData = GetParticleFrame(0);
        float frameHeight = (float)drawData.frame.Height;
        float textureHeight = frameHeight * FrameData.FrameCount;
        float yTiling = frameHeight / textureHeight;
        for (int i = 0; i < _length; i++)
        {

            ref BitDustParticleData particle = ref _particles[i];
            ref BitDustInstanceData instance = ref _instances[i];
            Vector2 scale = particle.Scale * particle.stretchScale;
            instance.Transformation = new Vector4(scale.X, scale.Y, particle.Position.X, particle.Position.Y);
            instance.InnerColor = particle.innerColor;
            instance.OuterColor = particle.outerColor;
            instance.Color = particle.color;

            float yOffset = (particle.frameIndex * frameHeight) / textureHeight;
            instance.TilingOffsetRotation = new Vector3(yTiling, yOffset, particle.Rotation);


        }

        //  Main.NewText(_instances[0].Transformation);
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
        // shader.ScreenPos = Main.screenPosition;

        shader.SpriteTexture = _particleTextureAsset.Value;
        shader.Projection = TrailDrawer.WorldViewPoint2;

  
        shader.Effect.CurrentTechnique.Passes[0].Apply();
        graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2, _length);


        spriteBatch.Begin(SpritebatchParams.InWorldAndZoomed());
        //  base.Draw(spriteBatch, screenPos);
    }
    public override void Draw(SpriteBatch spriteBatch, ref BitDustParticleData particle)
    {

    }
}