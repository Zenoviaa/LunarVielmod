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
using Stellamod.Items.Materials;
using Stellamod.Helpers;
using Terraria.GameContent.UI;

namespace Stellamod
{
    public class Stellamod : Mod
    {


        public static int MedalCurrencyID;
        public override void Load()
        {
            // ...other Load stuff goes here
            MedalCurrencyID = CustomCurrencyManager.RegisterCurrency(new Helpers.Medals(ModContent.ItemType<Medal>(), 999L, "Ruin medals"));


            if (Main.netMode != NetmodeID.Server)
            {
                Ref<Effect> screenRef = new Ref<Effect>(ModContent.Request<Effect>("Stellamod/Effects/Shockwave", AssetRequestMode.ImmediateLoad).Value); // The path to the compiled shader file.
                Filters.Scene["Shockwave"] = new Filter(new ScreenShaderData(screenRef, "Shockwave"), EffectPriority.VeryHigh);
                Filters.Scene["Shockwave"].Load();
            }
        }


        public override void Unload()
        {
            // The Unload() methods can be used for unloading/disposing/clearing special objects, unsubscribing from events, or for undoing some of your mod's actions.
            // Be sure to always write unloading code when there is a chance of some of your mod's objects being kept present inside the vanilla assembly.
            // The most common reason for that to happen comes from using events, NOT counting On.* and IL.* code-injection namespaces.
            // If you subscribe to an event - be sure to eventually unsubscribe from it.

            // NOTE: When writing unload code - be sure use 'defensive programming'. Or, in other words, you should always assume that everything in the mod you're unloading might've not even been initialized yet.
            // NOTE: There is rarely a need to null-out values of static fields, since TML aims to completely dispose mod assemblies in-between mod reloads.
        }
    }
}