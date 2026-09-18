namespace Stellamod.Core.Utilities;

public static class LunarColor
{
    static LunarColor()
    {
        AbyssWaterfall = Color.Lerp(Color.White, Color.Cyan, 0.75f);
        AbyssWaterfall = Color.Lerp(AbyssWaterfall, Color.Blue, 0.5f);
    }

    /// <summary>
    /// Used by the abyss waterfalls
    /// </summary>
    public static readonly Color AbyssWaterfall;
}
