using System;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public class LightingOptimization : ModSystem
    {
        public override void PreUpdateWorld()
        {
            base.PreUpdateWorld();
      
            var client = ModContent.GetInstance<LunarVeilClientConfig>();
            if (client.NoLightingEveryFrameOverride)
            {
                Main.LightingEveryFrame = false;
            }
        }
    }
}
