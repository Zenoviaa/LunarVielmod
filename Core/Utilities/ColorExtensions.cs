namespace Stellamod.Core.Utilities;

public static class ColorExtensions
{
    extension(Color color)
    {
        public Color Lighten(float pc)
        {
            return Color.Lerp(color, Color.White, pc);
        }

        public Color Darken(float pc)
        {
            return Color.Lerp(color, Color.Black, pc);
        }

       
    }
}