namespace Stellamod.Core.Utilities;

public readonly partial record struct TriColorPalette(Color primaryColor, Color secondaryColor, Color accent)
{
    public static readonly TriColorPalette Foresty = new TriColorPalette(Color.LightGreen, Color.DarkGreen, Color.RosyBrown);
    public static readonly TriColorPalette Moonly = new TriColorPalette(Color.SkyBlue, Color.Aqua, Color.Violet);
}
