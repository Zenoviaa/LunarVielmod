
using Terraria;

namespace Stellamod.Common.Particles;


public interface IInstancedParticleCreator<ParticleData, StaticParticleData> 
    where ParticleData : struct, IParticleData
    where StaticParticleData : struct
{
    void CreateInstance(ref ParticleData particle, ref StaticParticleData instance);
}


public struct BitDustFactory : IInstancedParticleCreator<BitDustParticleData, BitDustInstanceData>
{
    public static readonly BitDustFactory Default = new BitDustFactory()
    {
        position = Vector2.Zero,
        color = Vector4.One,
        scale = Vector2.One,
        velocity = Vector2.Zero,
        innerColor = Vector4.One,
        outerColor  =  new Vector4(1f, 0f, 0f, 1f),
        timeLeft = 60,
        velocityPerTickMult = 1
    }; 
    
    public static readonly BitDustFactory SlowingOverTime = new BitDustFactory()
    {
        position = Vector2.Zero,
        color = Vector4.One,
        scale = Vector2.One,
        velocity = Vector2.Zero,
        innerColor = Vector4.One,
        outerColor = new Vector4(1f, 0f, 0f, 1f),
        timeLeft = 60,
        velocityPerTickMult = 0.95f
    };
    public Vector4 color;
    public Vector4 innerColor;
    public Vector4 outerColor;
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 scale;
    public float timeLeft;

    public float velocityPerTickMult;
    public void CreateInstance(ref BitDustParticleData particle, ref BitDustInstanceData instance)
    {
        //These get simulated
        particle.position = position;
        particle.velocity = velocity;
        particle.scale = scale;
        particle.timeLeft = timeLeft;
        particle.color = color;
        particle.stretchScale = Vector2.One;
        particle.rotation = 0;
        particle.frameIndex = Main.rand.Next(3);
        particle.velocityPerTickMult = velocityPerTickMult;

        //Static Data
        instance.OuterColor = outerColor;
        instance.InnerColor = innerColor;
    }
}
public struct BitDustParticleData() : IParticleData
{
    public static readonly BitDustParticleData Default = new BitDustParticleData()
    {
        position = Vector2.Zero,
        color = Vector4.One,
        scale = Vector2.One,
        velocity = Vector2.Zero,
        stretchScale = Vector2.One,
        timeLeft = 300,
        rotation = 0,
        velocityPerTickMult = 1
    };

    public Vector4 color;
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 scale;
    public Vector2 stretchScale;
    public int frameIndex;
    public float rotation;
    public float timeLeft;
    public float velocityPerTickMult;
    public bool IsActive
    {
        get
        {
            if (timeLeft <= 0)
                return false;
            if (scale.X <= 0.1f)
                return false;
            return true;
        }
    }
}
