namespace Stellamod.Assets.ContentReader.Aseprite;

public static class AseBlending
{
    // Source: https://gist.github.com/NoelFB/778d190e5d17f1b86ebf39325346fcc5
    // Copied from Aseprite's source code:
    // https://github.com/aseprite/aseprite/blob/master/src/doc/blend_funcs.cpp

    private static int MUL_UN8(int a, int b)
    {
        var t = (a * b) + 0x80;
        return (((t >> 8) + t) >> 8);
    }

    public delegate void Blend(ref Color dest, Color src, byte opacity);
    public static Blend[] BlendModes = new Blend[]
    {
        //TODO: Implement other blend modes
            // 0 - NORMAL
            (ref Color dest, Color src, byte opacity) =>
            {
                int r, g, b, a;

                if (dest.A == 0)
                {
                    r = src.R;
                    g = src.G;
                    b = src.B;
                }
                else if (src.A == 0)
                {
                    r = dest.R;
                    g = dest.G;
                    b = dest.B;
                }
                else
                {
                    r = (dest.R + MUL_UN8((src.R - dest.R), opacity));
                    g = (dest.G + MUL_UN8((src.G - dest.G), opacity));
                    b = (dest.B + MUL_UN8((src.B - dest.B), opacity));
                }

                a = (dest.A + MUL_UN8((src.A - dest.A), opacity));
                if (a == 0)
                    r = g = b = 0;

                dest.R = (byte)r;
                dest.G = (byte)g;
                dest.B = (byte)b;
                dest.A = (byte)a;
            }
    };
}
