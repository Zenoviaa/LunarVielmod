using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public class PreGeneratedNoise : ModSystem
{
    private static float[,] _simplexNoise;
    private static float[,] _randomNumbers;
    public override void OnModLoad()
    {
        base.OnModLoad();

        float levels = 20;
        FastNoiseLite topFNL = new FastNoiseLite();
        topFNL.SetSeed(Main.rand.Next(0, int.MaxValue));
        topFNL.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        topFNL.SetFrequency(1f / levels);
        topFNL.SetDomainWarpAmp(10);
        topFNL.SetDomainWarpType(FastNoiseLite.DomainWarpType.OpenSimplex2);
        _simplexNoise = new float[(int)levels, (int)levels];
        for (int x = 0; x < levels; x++)
        {
            for (int y = 0; y < levels; y++)
            {
                _simplexNoise[x, y] = topFNL.GetNoise(x / levels, y / levels);
            }
        }

        _randomNumbers = new float[(int)levels, (int)levels];
        for (int x = 0; x < levels; x++)
        {
            for (int y = 0; y < levels; y++)
            {
                _randomNumbers[x, y] = Main.rand.NextFloat(0f, 1f);
            }
        }

    }

    public override void Unload()
    {
        base.Unload();
        _simplexNoise = null;
        _randomNumbers = null;
    }
    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
    }

    public static float SampleRand(int x, int y)
    {
        x %= _randomNumbers.GetLength(0);
        y %= _randomNumbers.GetLength(1);
        return _randomNumbers[x, y];
    }
    public static float SampleSimplexNoise(int x, int y)
    {
        x %= _simplexNoise.GetLength(0);
        y %= _simplexNoise.GetLength(1);
        return _simplexNoise[x, y];
    }
}
