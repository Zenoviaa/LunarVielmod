

using Microsoft.Xna.Framework;

namespace Stellamod.Core.LunarLightingSystem
{
    /// <summary>
    /// Data structure representing a point light
    /// </summary>
    public struct PointLightData
    {
        public PointLightData(Color color, Vector2 position, float intensity, float radius)
        {
            this.color = color;
            this.position = position;
            this.intensity = intensity;
            this.radius = radius;
        }

        public Color color;
        public Vector2 position;
        public float intensity;
        public float radius;
    }
}
