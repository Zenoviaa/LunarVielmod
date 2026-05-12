using Terraria;

namespace Stellamod.Core.Utilities;

public static class VectorExtensions
{
    public static Vector2 Resize(this Vector2 vector, float newLength)
    {
        return vector.SafeNormalize(Vector2.Zero) * newLength;
    }
}
