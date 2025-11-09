

using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Core.LunarLightingSystem
{
    public struct PointLight
    {
        public Vector2 position;
        public Vector3 color; 
        public float intensity;
        public float radius;
        public int extraRenders;
        public bool faint;
        public Vector2 directionOverride;
        public TileShadow tileShadow;
    }
}
