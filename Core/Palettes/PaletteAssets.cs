using ReLogic.Content;
using Stellamod.Assets.ContentReader.Pal;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Core.Palettes;

[Autoload(Side = ModSide.Client)]
public class PaletteAssets : ModSystem
{
    private readonly static Dictionary<string, Asset<Palette>> _paletteAssetLookup = new();
    public const string ABYSSWATER = "AbyssWater";
    public const string ABYSS = "Abyss";
    public const string PERFECT = "Perfect";
    public const string AEGISLAV = "Aegislav";
    public const string BlackHurricane = "BlackHurricane";
    public const string BLOODHOUND = "BloodHound";
    public const string DESERT = "Desert";
    public const string DESERTTOP = "DesertTop";
    public const string DUNGEON = "Dungeon";
    public const string FABLE = "Fable";
    public const string FIRESTORM = "FireStorm";
    public const string HELL = "Hell";
    public const string ILLURIANMISTYDUNGEON = "IllurianMistyDungeon";
    public const string MISTYDUNGEON = "MistyDungeon";
    public const string MOONSPIRALTOWER = "MoonspiralTower";
    public const string ROYALCAPITAL = "RoyalCapital";
    public const string RUSTY = "Rusty";
    public const string SANGUINESINGULARITY = "SanguineSingularity";
    public const string VILEPIPESNGARDEN = "VilepipesNGarden";
    public const string WITCHTOWN = "Witchtown";
    public const string FIREBREATH = "FireBreath";
    public override void Unload()
    {
        base.Unload();
        _paletteAssetLookup.Clear();
    }

    public static Asset<Palette> FromPaletteFile(string paletteName)
    {
        if (_paletteAssetLookup.ContainsKey(paletteName))
            return _paletteAssetLookup[paletteName];
        _paletteAssetLookup.Add(paletteName, ModContent.Request<Palette>(typeof(PaletteAssets).DirectoryHere() + $"/{paletteName}"));
        return _paletteAssetLookup[paletteName];
    }
}