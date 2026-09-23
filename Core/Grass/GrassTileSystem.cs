using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Grass;

/// <summary>
/// Contains the data for grass profiles
/// </summary>
public class GrassTileSystem : ModSystem
{
    private static Queue<(int, GrassProfile)> _pairs = new();
    public static GrassProfile[] Profiles;

    public override void Load()
    {
        base.Load();
        _pairs.Clear();
    }
    public override void Unload()
    {
        base.Unload();
        Profiles = null;
    }

    public override void ResizeArrays()
    {
        base.ResizeArrays();

    }

    public override void PostAddRecipes()
    {
        base.PostAddRecipes();
        //We can set ids in here I think
        Profiles = ModContent.GetContent<GrassProfile>().ToArray();
        for (int i = 0; i < Profiles.Length; i++)
            Profiles[i].type = i;
        while (_pairs.Count > 0)
        {
            var p = _pairs.Dequeue();
            TileID.Sets.MarshyGrass[p.Item1] = p.Item2.type;
        }
    }
    public static void RegisterGrassyTile<T>(int type) where T : GrassProfile
    {
        _pairs.Enqueue((type, ModContent.GetInstance<T>()));
    }
}
