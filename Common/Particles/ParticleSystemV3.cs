using ReLogic.Content;
using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Particles;

//Gonna include the glow colors as an extra thingy

public struct BitDustInstanceData : IVertexType
{
    private Vector4 _color;
    private Vector4 _innerColor;
    private Vector4 _outerColor;
    private Vector4 _transformation;
    private Vector3 _tilingOffsetRotation;
    public BitDustInstanceData()
    {

    }

    public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
    (
        new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
        new VertexElement(16, VertexElementFormat.Vector4, VertexElementUsage.Color, 1),
        new VertexElement(32, VertexElementFormat.Vector4, VertexElementUsage.Color, 2),
        new VertexElement(48, VertexElementFormat.Vector4, VertexElementUsage.TextureCoordinate, 1),
        new VertexElement(64, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 2)
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
    public Vector4 InnerColor
    {
        get { return _innerColor; }
        set { _innerColor = value; }
    }
    public Vector4 OuterColor
    {
        get { return _outerColor; }
        set { _outerColor = value; }
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

//Helper methods for spawning particles
[Autoload(Side = ModSide.Client)]
public sealed class Particles : ModSystem
{
    private static List<IParticleUpdater> _particleUpdaters;
    public static BitDust BitDust;
    public override void Load()
    {
        base.Load();
        On_Main.DrawDust += DrawParticles;
        BitDust = new();
        _particleUpdaters = new List<IParticleUpdater>();
        _particleUpdaters.Add(BitDust);
        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            if (_particleUpdaters[i] is ILoadable loadable)
            {
                loadable.Load(Mod);
            }
        }
    }

    public override void Unload()
    {
        base.Unload();
        if (_particleUpdaters == null)
            return;

        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            if (_particleUpdaters[i] is ILoadable loadable)
            {
                loadable.Unload();
            }
        }
        _particleUpdaters.Clear();
        _particleUpdaters = null;
    }

    private void DrawParticles(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            IParticleUpdater particleUpdater = _particleUpdaters[i];
            if (particleUpdater.PixelationDrawLayer != DrawLayer.None)
            {
                PixelationManager.QueueSpritebatchDrawAction(_particleUpdaters[i].Draw, particleUpdater.PixelationDrawLayer);
            }
            else
            {
                _particleUpdaters[i].Draw(Main.spriteBatch, Main.screenPosition);
            }
        }
    }

    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();


        for (int i = 0; i < _particleUpdaters.Count; i++)
        {
            _particleUpdaters[i].Update();
        }

        /*
        if (Main.mouseLeft)
        {
            for(int i = 0; i < 128; i++)
            {
                BitDust.Spawn(BitDustParticleData.Default with
                {
                    Position = Main.MouseWorld,
                    timeLeft = 120,
                    Scale = new Vector2(1),
                    Velocity = Main.rand.NextVector2Circular(8, 8),
                    innerColor = Main.DiscoColor.ToVector4(),
                });
            }

        }*/

    }

    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        //Just for testing the atlas
    }
}


public record struct ParticleFrameData(string Texture, int FrameCount)
{
    public static ParticleFrameData Create(Type type, int frameCount)
    {
        return new ParticleFrameData($"{type.Namespace}.{type.Name}".Replace('.', '/'), frameCount);
    }
}

public interface IParticleUpdater
{
    ParticleFrameData FrameData { get; }
    DrawLayer PixelationDrawLayer { get; }
    void Update();
    void Draw(SpriteBatch spriteBatch, Vector2 screenPos);
}

//TODO: replace particle texture assets with a single atlas and have it read from that.
//Probably having an external atlas generating tool

[Autoload(false)]
public abstract class ParticleUpdater<T> : IParticleUpdater, ILoadable
    where T : struct, IParticleData
{

    protected int _length;
    private T _dummyParticle;
    protected readonly T[] _particles;
    protected Asset<Texture2D> _particleTextureAsset;
    public ParticleUpdater()
    {
        _particles = new T[GetPoolSize()];

    }
    public virtual void Load(Mod mod)
    {

    }
  

    public virtual void Unload()
    {
        _particleTextureAsset?.Dispose();
        _particleTextureAsset = null;
    }

    public virtual ParticleFrameData FrameData
    {
        get
        {
            return ParticleFrameData.Create(this.GetType(), 1);
        }
    }

    public virtual (Texture2D texture, Rectangle frame) GetParticleFrame(in int frameIndex)
    {
        var frameData = FrameData;
        int frameHeight = _particleTextureAsset.Height() / frameData.FrameCount;
        Rectangle frame = new Rectangle(0, frameIndex * frameHeight, _particleTextureAsset.Width(), frameHeight);
        return (_particleTextureAsset.Value, frame);
    }

    public virtual DrawLayer PixelationDrawLayer => DrawLayer.OverNPCsAdditive;
    public virtual int GetPoolSize() => 200;
    public virtual void Update()
    {
    //    var watch = Stopwatch.StartNew();
        var frameData = FrameData;
        _particleTextureAsset ??= ModContent.Request<Texture2D>(frameData.Texture);
        /*
        FastParallel.For(0, _length, delegate (int start, int end, object context)
        {

            for (int i = start; i < end; i++)
            {
                ref T particleData = ref _particles[i];
                Update(ref particleData);
                if (!particleData.IsActive)
                {
                    KillParticle(i);
                    i--;
                }
            }
        });*/

       
        for (int i = 0; i < _length; i++)
        {
            ref T particleData = ref _particles[i];
            Update(ref particleData);
            if (!particleData.IsActive)
            {
                KillParticle(i);
                i--;
            }
        }
  //      watch.Stop();
   //     Main.NewText($"~{(float)watch.ElapsedTicks/10000f}ms ::: Particle Count: {_length}");

        //  Main.NewText(_length);
    }

    public ref T Spawn(in T particleData)
    {
        //If too many particles just return a reference to one that's not being used or drawn to the screen
        //That way we don't interrupt anything that's happening
        if (_length >= _particles.Length)
            return ref _dummyParticle;

        int index = _length;
        _length++;
        _particles[index] = particleData;
        OnSpawn(ref _particles[index]);
        return ref _particles[index];
    }

    public void KillParticle(in int index)
    {
        //Swap with the last active particle and set the data to default
        //Order does not matter for when they get updated, so we can do it like this :)
        _particles[index] = _particles[_length - 1];
        _particles[_length - 1] = default;
        _length--;
    }

    public abstract void OnSpawn(ref T particle);
    public abstract void Update(ref T particle);
    public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            Draw(spriteBatch, ref particle);
        }
    }

    public abstract void Draw(SpriteBatch spriteBatch, ref T particle);

}

public interface IParticleData
{
    bool IsActive { get; }
}

public class BitDustProfile
{
    public Color innerColor;
    public Color outerColor;
    public float gravity;
    public bool fast;
    public bool superFast;
}
public struct BitDustParticleData() : IParticleData
{
    public static readonly BitDustParticleData Default = new BitDustParticleData()
    {
        Position = Vector2.Zero,
        color = Vector4.One,
        innerColor = Vector4.One,
        outerColor = Vector4.One,
        Scale = Vector2.One,
        Velocity = Vector2.Zero,
        stretchScale = Vector2.One,
        timeLeft = 300,
        gravity = 0.2f,
        Rotation = 0
    };

    public Vector4 color;
    public Vector4 innerColor;
    public Vector4 outerColor;
    public Vector2 Position;
    public Vector2 Velocity;
    public Vector2 Scale;
    public Vector2 stretchScale;
    public int frameIndex;
    public float Rotation;
    public float timeLeft;
    public float gravity;
    public float dampening;
    public bool fast;
    public bool superFast;
    public bool noTileCollide;

    public bool IsActive => timeLeft > 0;
}
public class BitDustShader : CrystalShader<BitDustShader>
{
    private EffectParameter _spriteTextureParam;
    private EffectParameter _projectionParam;
    public Matrix Projection
    {
        set
        {
            _projectionParam = Effect.Parameters["projection"];
            _projectionParam.SetValue(value);
        }
    }
    public Texture2D SpriteTexture
    {
        set
        {
            _spriteTextureParam = Effect.Parameters["spriteTexture"];
            _spriteTextureParam.SetValue(value);
        }
    }
    public Vector2 ScreenPos
    {
        set
        {
            Effect.Parameters["screenPosition"].SetValue(value);
        }
    }
}

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
        return 20_000;
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
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        _instanceBuffer?.Dispose();
    }

    public override void OnSpawn(ref BitDustParticleData particle)
    {
        //particle.gravity = 0.2f;
        //particle.innerColor = Vector4.One;
        //particle.outerColor = new Vector4(1f, 0f, 0f, 1f);
        particle.frameIndex = Main.rand.Next(3);
        particle.stretchScale = Vector2.One;
    }

    public override void Update(ref BitDustParticleData particle)
    {
        particle.Velocity.Y += particle.gravity;
        particle.Velocity *= 1.0f - particle.dampening;
        particle.Rotation = particle.Velocity.ToRotation();
        particle.Scale *= 0.97f;
        if (particle.fast)
            particle.Scale *= 0.98f;
        if (particle.superFast)
        {
            particle.Velocity *= 0.9f;
            particle.Scale *= 0.94f;
        }
        particle.color *= 0.99f;

        float stretchInterp = particle.Velocity.LengthSquared() / 25f;
        particle.stretchScale.X = MathHelper.Lerp(1f, 1.5f, stretchInterp);
        particle.stretchScale.Y = 1f;
        if (particle.Scale.X < 0.1f)
            particle.timeLeft = 0;
        Lighting.AddLight(particle.Position, new Vector3(particle.color.X, particle.color.Y, particle.color.Z));
        particle.timeLeft--;
        particle.Position += particle.Velocity;
       // Main.NewText(_length);
        //Bouncing
        if (particle.noTileCollide)
            return;

        Vector2 collisionVelocity = Collision.TileCollision(particle.Position, particle.Velocity, 2, 2);
        if (particle.Velocity.X != collisionVelocity.X)
            particle.Velocity.X = -collisionVelocity.X * 0.7f;
        if (particle.Velocity.Y != collisionVelocity.Y)
            particle.Velocity.Y = -collisionVelocity.Y * 0.7f;
    }

    public override void Update()
    {
        base.Update();
        //Update instance data after particle data updates

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