using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Graphics.Light;

namespace Stellamod.Core.LunarLightingSystem
{
    public static class LightingAccessors
    {
        [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "_activeEngine")]
        public static extern ref ILightingEngine _activeEngine(Lighting canBeNull);
    }
}
