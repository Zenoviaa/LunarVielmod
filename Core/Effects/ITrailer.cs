using Microsoft.Xna.Framework;

namespace Stellamod.Core.Effects
{
    internal interface ITrailer
    {
        void SetTrailingValues(float interpolant);
        void DrawTrail(ref Color lightColor, Vector2[] trailCache);
    }
}
