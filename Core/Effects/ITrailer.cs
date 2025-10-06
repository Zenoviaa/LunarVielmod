using Microsoft.Xna.Framework;

namespace Stellamod.Core.Effects
{
    public interface ITrailer
    {
        delegate float GetTrailWidth(float interpolant);
        delegate Color GetTrailColor(float interpolant);
        Shader Shader { get; set; }
        GetTrailWidth TrailWidthFunction { get; set; }
        GetTrailColor TrailColorFunction { get; set; }
        void SetTrailingValues(float interpolant);
        void DrawTrail(ref Color lightColor, Vector2[] trailCache);
    }
}
