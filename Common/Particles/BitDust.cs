using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Common.Particles;


public struct CloudDustParticleData : IParticleData
{
    public static readonly CloudDustParticleData Default = new CloudDustParticleData
    {
        color = Color.White.ToVector4(),
        position = Vector2.Zero,
        velocity = Vector2.Zero,
        timeLeft = 120,
        rotation = 0,
        frameIndex = 0
    };

    public Vector4 color;
    public Vector2 position;
    public Vector2 velocity;
    public float timeLeft;
    public float rotation;
    public int frameIndex;
    public float originPoint;
    public bool IsActive
    {
        get
        {
            if (timeLeft < 0)
                return false;
            return true;
        }
    }
}

public struct CloudDustInstanceData : IVertexType
{
    private Vector4 _color;
    private Vector4 _transformation;
    private Vector3 _tilingOffsetRotation;
    public CloudDustInstanceData()
    {

    }

    public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
    (
        new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
        new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
        new VertexElement(32, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 2)
    );

    VertexDeclaration IVertexType.VertexDeclaration
    {
        get { return VertexDeclaration; }
    }

    public Vector4 Color
    {
        get { return _color; }
        set { _color = value; }
    }
    public Vector4 Transformation
    {
        get { return _transformation; }
        set { _transformation = value; }
    }
    public Vector3 TilingOffsetRotation
    {
        get
        {
            return _tilingOffsetRotation;
        }
        set
        {
            _tilingOffsetRotation = value;
        }
    }
}

public class CloudDust : ParticleUpdater<CloudDustParticleData>
{
    private GPUInstanceBuffer<CloudDustInstanceData> _gpuInstancedBuffer;
    public override ParticleFrameData FrameData => base.FrameData with { FrameCount = 1 };


    public Vector2 bottomLeft;
    public Vector2 bottomRight;
    public Vector2 topRight;
    public override int GetPoolSize()
    {
        //Perf tests were done with 1,000,000 max particles
        return 9_000;
    }

    public override void LoadSafe()
    {
        base.LoadSafe();
        _gpuInstancedBuffer = new GPUInstanceBuffer<CloudDustInstanceData>(_particles.Length, 36, 36);
        On_Main.DoDraw_WallsTilesNPCs += DrawWalls;
    }

    private void DrawWalls(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
    {
        Draw(Main.spriteBatch, Main.screenPosition);
        orig(self);
    }

    public override void UnloadSafe()
    {
        base.UnloadSafe();
        _gpuInstancedBuffer?.Dispose();
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
                ref var particle = ref _particles[i];
                particle.timeLeft--;

                float lerpValue = Utils.GetLerpValue(960, 0, particle.timeLeft, clamped: true);
                lerpValue += particle.originPoint;
                lerpValue %= 1.0f;
                Vector2 a = Vector2.Lerp(bottomLeft, bottomRight, lerpValue);
                Vector2 b = Vector2.Lerp(bottomRight, topRight, lerpValue);
                Vector2 c = Vector2.Lerp(a, b, lerpValue);
                c.Y += MathF.Sin(i * 0.2f) * 127;
                particle.position = c;
                particle.position.X += MathF.Sin(particle.timeLeft * 0.04f + Main.GlobalTimeWrappedHourly * 2) * 128;
                particle.position.X += MathHelper.Lerp(-100, 0, lerpValue);

                particle.rotation += 0.004f;
                
               // particle.position += particle.velocity;
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
                ref var particle = ref _particles[i];
                ref var instance = ref instances[i];
                Vector2 scale = Vector2.One ;
                Vector2 pos = particle.position;
                pos += Main.screenPosition;
                instance.Transformation = new Vector4(scale.X, scale.Y, pos.X, pos.Y);

                float tl = particle.timeLeft;
                float lerp = Utils.GetLerpValue(0, 980, tl, clamped: true);
                Vector4 cloudColor = particle.color;
                instance.Color = cloudColor;
                float yOffset = (particle.frameIndex * frameHeight) / textureHeight;

                //ytiling and yoffset are static and should be moved out of here
                //Then rotation becomes a separate variable
                instance.TilingOffsetRotation = new Vector3(yTiling, yOffset, particle.rotation);
            }
        });


        // watch.Stop();
        //  elapsedString = $"~{(float)watch.ElapsedTicks / 10000f}ms ::: Particle Count: {_length}";

    }
    private Vector2 GetScreenOffset(float scale)
    {
        //Apply an offset so the texture doesn't move when you're moving
        //This will wrap inside the shader
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        Vector2 screenoffset = Main.screenPosition * texelSize;
        screenoffset *= (1f / scale);
        return screenoffset;
    }
    private void DrawStars(SpriteBatch spriteBatch)
    {
        var starsTexture = TextureRegistry.StarNoise2;
        var noiseTexture = TextureRegistry.BlurryPerlinNoise2;
        MiscShaderData eff = GameShaders.Misc["LunarVeil:RoyalCapitalStars"];

        eff.Shader.Parameters["primaryTexture"].SetValue(starsTexture.Value);
        eff.Shader.Parameters["primaryTextureSize"].SetValue(starsTexture.Value.Size());
        eff.Shader.Parameters["resolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
        eff.Shader.Parameters["screenOffset"].SetValue(GetScreenOffset(scale: 1));
        eff.UseImage2(noiseTexture);
        eff.Shader.Parameters["parallax"].SetValue(Main.Camera.Center * 0.000025f);
        eff.Shader.Parameters["gradientFade"].SetValue(1f);
        eff.UseOpacity(1f);
        eff.Apply();

       
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, eff.Shader, Main.BackgroundViewMatrix.TransformationMatrix);
        spriteBatch.Draw(starsTexture.Value,
           new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
            null, Color.White * 0.3f);
        spriteBatch.Draw(starsTexture.Value,
   new Rectangle(127, 127, Main.screenWidth, Main.screenHeight),
    null, Color.White * 0.3f);



        /*
        spriteBatch.Draw(starsTexture.Value, 
            new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), 
            new Rectangle((int)-_parallax.X, (int)-_parallax.Y, Main.screenWidth, Main.screenHeight), Color.White * 0.3f);
        */
        spriteBatch.End();
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        if (_length <= 0)
            return;

        /*
        Rectangle rect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        SpritebatchDrawer squareDrawer = SpritebatchDrawer.FromTextureAsset(TextureAssets.MagicPixel, Vector2.Zero);
        squareDrawer.drawOrigin = Vector2.Zero;
        squareDrawer.dstRect = rect;
        spriteBatch.Draw(squareDrawer);*/
        spriteBatch.End();
      
        UpdateInstances();
        _gpuInstancedBuffer.PrepareForDrawing(spriteBatch.GraphicsDevice);
        InstancedParticleShader shader = InstancedParticleShader.Instance;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.BlendState = BlendState.Additive;

        shader.SpriteTexture = _particleTextureAsset.Value;
        shader.Projection = TrailDrawer.WorldViewPoint2;

        shader.Effect.CurrentTechnique.Passes[0].Apply();
        graphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2, _length);

        DrawStars(spriteBatch);
        spriteBatch.Begin(SpritebatchParams.InWorldAndZoomed());
    }

    public override void Draw(SpriteBatch spriteBatch, ref CloudDustParticleData particle)
    {

    }
}

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
