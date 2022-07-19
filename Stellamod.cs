using Stellamod.WorldG;
using System;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using static Terraria.ModLoader.ModContent;
using ReLogic.Content;

namespace Stellamod
{
    public class Stellamod : Mod
    {
    


        public override void Load()
        {
            // ...other Load stuff goes here

            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("Stellamod/Effects/Shockwave", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();
            }
        }
    }
}