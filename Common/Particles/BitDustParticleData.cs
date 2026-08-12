namespace Stellamod.Common.Particles;

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
    public bool IsActive => timeLeft > 0;
}
