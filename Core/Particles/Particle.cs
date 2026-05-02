using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Particles;

/// <summary>
/// Base class for a particle, the generic parameter should just be the type of the particle, since it will automatically pre-initialize a pool for you
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Particle<T> : BaseParticle
    where T : BaseParticle, new()
{
    private static int _lastIndex;
    private static T[] _pool;
    private static int _poolSize;
    public virtual int GetPoolSize() => 300;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _poolSize = GetPoolSize();
        InitializePool();

    }

    public override void Unload()
    {
        base.Unload();
        _pool = null;
    }

    protected void BounceOnTileCollide()
    {
        Vector2 collisionVelocity = Collision.TileCollision(Center, Velocity, 2, 2);
        if (Velocity.X != collisionVelocity.X)
            Velocity.X = -collisionVelocity.X;
        if (Velocity.Y != collisionVelocity.Y)
            Velocity.Y = -collisionVelocity.Y;
    }

    private void InitializePool()
    {
        int maxParticleCount = GetPoolSize();
        _pool = new T[maxParticleCount];
        for (int i = 0; i < maxParticleCount; i++)
        {
            _pool[i] = new T();
        }
    }

    private static T GetParticle()
    {
        int oldest = _lastIndex;
        int particleCount = _poolSize;
        for (int i = 0; i < particleCount; i++)
        {
            _lastIndex++;
            _lastIndex = _lastIndex % _pool.Length;
            T particle = _pool[_lastIndex];
            if (!particle.active)
                return particle;
        }

        return _pool[oldest];
    }

    private static void SetParticleDefaults(T t)
    {
        t.fadeIn = 0;
        t.drawInUI = false;
        t.hasParent = false;
    }
    public static T Spawn(Vector2 position, Vector2 velocity, Color? color = null, float Scale = 1f)
    {
        T particle = GetParticle();

        //Don't do anyth of this other stuff cause the server doesn't need to simulate particles
        if (Main.netMode == NetmodeID.Server)
            return particle;

        SetParticleDefaults(particle);
        particle.active = true;
        particle.color = color.HasValue ? color.Value : Color.White;
        particle.parent = null;

        particle.Center = position;
        particle.Velocity = velocity;
        particle.Scale = Scale;

        particle.OnSpawn();
        ParticleSystemV2.AddParticle(particle);
        return particle;
    }
    public static T SpawnInAlphaLayer(Vector2 position, Vector2 velocity, Color? color = null, float Scale = 1f)
    {
        T particle = GetParticle();

        //Don't do anyth of this other stuff cause the server doesn't need to simulate particles
        if (Main.netMode == NetmodeID.Server)
            return particle;

        SetParticleDefaults(particle);
        particle.active = true;
        particle.color = color.HasValue ? color.Value : Color.White;
        particle.parent = null;
        particle.Center = position;
        particle.Velocity = velocity;
        particle.Scale = Scale;

        particle.OnSpawn();
        ParticleSystemV2.AddAlphaBlendedParticle(particle);
        return particle;
    }
    public static T SpawnInUI(Vector2 position, Vector2 velocity, Color? color = null, float Scale = 1f)
    {
        T particle = GetParticle();

        //Don't do anyth of this other stuff cause the server doesn't need to simulate particles
        if (Main.netMode == NetmodeID.Server)
            return particle;

        SetParticleDefaults(particle);
        particle.active = true;
        particle.color = color.HasValue ? color.Value : Color.White;
        particle.parent = null;
        particle.Center = position;
        particle.Velocity = velocity;
        particle.Scale = Scale;
        particle.drawInUI = true;

        particle.OnSpawn();
        ParticleSystemV2.AddUIParticle(particle);
        return particle;
    }
}
