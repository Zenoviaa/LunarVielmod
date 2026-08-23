namespace Stellamod.Common.Particles;

public sealed class ParticleBuffer<ParticleStructType> 
    where ParticleStructType : struct, IParticleData
{

    public int length;
    protected ParticleStructType _dummyParticle;
    public readonly ParticleStructType[] _particles;
    public ParticleBuffer(int poolSize)
    {
        _particles = new ParticleStructType[poolSize];
    }

    public bool HasAnyParticles => length > 0;

    public void CheckForAndKillParticles()
    {
        //var watch = Stopwatch.StartNew();
        for (int i = 0; i < length; i++)
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

    public ref ParticleStructType Spawn(in ParticleStructType particleData)
    {
        //If too many particles just return a reference to one that's not being used or drawn to the screen
        //That way we don't interrupt anything that's happening
        if (length >= _particles.Length)
            return ref _dummyParticle;

        int index = length;
        length++;
        _particles[index] = particleData;
        return ref _particles[index];
    }

    public void KillParticle(in int index)
    {
        //Swap with the last active particle and set the data to default
        //Order does not matter for when they get updated, so we can do it like this :)
        _particles[index] = _particles[length - 1];
        _particles[length - 1] = default;
        length--;
    }
}
