using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    [Autoload(Side = ModSide.Client)]
    public class LunarLightingEngineEdit : ModSystem
    {
        private static LunarLightingEngine _lightingEngine;
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (config.UseLunarLightingEngine)
            {
                _lightingEngine ??= new();
                LightingAccessors._activeEngine(null) = _lightingEngine;
            }
            else
            {
                ref ILightingEngine lightingEngine = ref LightingAccessors._activeEngine(null);
                if (lightingEngine == _lightingEngine)
                {
                    lightingEngine = new LightingEngine();
                    lightingEngine.Rebuild();
                }
            }
        }
    }
}
