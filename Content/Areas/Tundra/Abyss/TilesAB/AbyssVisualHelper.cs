using Terraria;

namespace Stellamod.Content.Areas.Tundra.Abyss.TilesAB;

public class AbyssVisualHelper
{
    /// <summary>
    /// Sets:
    /// tileLighted: true
    /// tileBlockLight: false
    /// tileFrameImportant: true
    /// tileNoAttach: true
    /// tileLavaDeath: true
    /// </summary>
    /// <param name="type"></param>
    public static void DefaultToGlowingPlant(in int type)
    {
        Main.tileLighted[type] = true;
        Main.tileBlockLight[type] = false;
        Main.tileFrameImportant[type] = true;
        Main.tileNoAttach[type] = true;
        Main.tileLavaDeath[type] = true;
    }

    /// <summary>
    /// Modifies light to the color of a glowing abyss plant, which is a nice shade of blue/purple
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    public static void AbyssPlantModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        const float factor = 3;
        r = .154f * factor;
        g = .177f * factor;
        b = .255f * factor;
    }
}
