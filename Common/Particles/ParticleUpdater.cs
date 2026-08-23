using ReLogic.Content;
using Stellamod.Core.Pixelation;
using System;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Particles;

//TODO: replace particle texture assets with a single atlas and have it read from that.
//Probably having an external atlas generating tool

[Autoload(false)]
public abstract class ParticleUpdater<ParticleStructType> : 
    IParticleUpdater, 
    ILoadable
    where ParticleStructType : struct, IParticleData
{

    protected int _length;
    protected ParticleStructType _dummyParticle;
    protected readonly ParticleStructType[] _particles;
    protected Asset<Texture2D> _particleTextureAsset;
    public ParticleUpdater()
    {
        _particles = new ParticleStructType[GetPoolSize()];
        elapsedString = string.Empty;
    }

    public string elapsedString;

    public virtual void LoadSafe()
    {

    }
    public virtual void UnloadSafe()
    {

    }
    public  void Load(Mod mod)
    {
        Main.QueueMainThreadAction(() =>
        {
            _particleTextureAsset = ModContent.Request<Texture2D>(FrameData.Texture);
            LoadSafe();
        });

    }
    public void Unload()
    {
        Main.QueueMainThreadAction(() =>
        {
            //idk if i have to null out the particle asset
            _particleTextureAsset?.Dispose();
            _particleTextureAsset = null;
            UnloadSafe();
        });
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
        //This function can be changed to use an atlas later
        var frameData = FrameData;
        int frameHeight = _particleTextureAsset.Height() / frameData.FrameCount;
        Rectangle frame = new Rectangle(0, frameIndex * frameHeight, _particleTextureAsset.Width(), frameHeight);
        return (_particleTextureAsset.Value, frame);
    }

    public virtual DrawLayer PixelationDrawLayer => DrawLayer.OverNPCsAdditive;
    public virtual int GetPoolSize() => 200;
    public void Update()
    {
        //var watch = Stopwatch.StartNew();
        UpdateParticles();
        for (int i = 0; i < _length; i++)
        {
            ref ParticleStructType particleData = ref _particles[i];
            if (!particleData.IsActive)
            {
                KillParticle(i);
                i--;
            }
        }
        //watch.Stop();
        //elapsedString = $"~{(float)watch.ElapsedTicks / 10000f}ms ::: Particle Count: {_length}";
    }

    protected virtual void UpdateParticles() { }

    public ref ParticleStructType Spawn(in ParticleStructType particleData)
    {
        //If too many particles just return a reference to one that's not being used or drawn to the screen
        //That way we don't interrupt anything that's happening
        if (_length >= _particles.Length)
            return ref _dummyParticle;

        int index = _length;
        _length++;
        _particles[index] = particleData;
        OnSpawn(ref _particles[index], index);
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

    public virtual void OnSpawn(ref ParticleStructType particle, in int index) { }
    public virtual void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        for (int i = 0; i < _length; i++)
        {
            ref var particle = ref _particles[i];
            Draw(spriteBatch, ref particle);
        }
    }

    public abstract void Draw(SpriteBatch spriteBatch, ref ParticleStructType particle);

}
