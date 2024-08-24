using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Skies;
using Stellamod.Water;
using System.Collections.Generic;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using System.Linq;

namespace Stellamod
{
    public class TestPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Main.netMode == NetmodeID.Server)
                return;
            /*
            if (!SkyManager.Instance["Stellamod:CloudySky"].IsActive())
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("Stellamod:CloudySky", targetCenter);
            }
            if (!SkyManager.Instance["Stellamod:DesertSky"].IsActive())
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("Stellamod:DesertSky", targetCenter);
            }*/
        }
    }

    public class Stellamod : Mod
    {
        public Stellamod()
        {
            Instance = this;

        }


        public static Stellamod Instance;
        public override void Load()
        {
            Asset<Effect> miscShader = Assets.Request<Effect>("fx/Clouds", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc["Stellamod:Clouds"] = new MiscShaderData(miscShader, "ScreenPass");

            Asset<Effect> miscShader2 = Assets.Request<Effect>("fx/CloudsFront", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc["Stellamod:CloudsFront"] = new MiscShaderData(miscShader2, "ScreenPass");
           
            Asset<Effect> miscShader3 = Assets.Request<Effect>("fx/NightClouds", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc["Stellamod:NightClouds"] = new MiscShaderData(miscShader3, "ScreenPass");

            Asset<Effect> miscShader4 = Assets.Request<Effect>("fx/CloudsDesert", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc["Stellamod:CloudsDesert"] = new MiscShaderData(miscShader4, "ScreenPass");

            Asset<Effect> miscShader5 = Assets.Request<Effect>("fx/CloudsDesertNight", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc["Stellamod:CloudsDesertNight"] = new MiscShaderData(miscShader5, "ScreenPass");

            var miscShader6 = new Ref<Effect>(Instance.Assets.Request<Effect>("fx/Water", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["Stellamod:Water"] = new Filter(new ScreenShaderData(miscShader6, "PrimitivesPass"), EffectPriority.High);
            Filters.Scene["Stellamod:Water"].Load();

            Asset<Effect> gradient = Assets.Request<Effect>("fx/Gradient", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc["Stellamod:Gradient"] = new MiscShaderData(gradient, "ScreenPass");

            SkyManager.Instance["Stellamod:CloudySky"] = new CloudySky();
            SkyManager.Instance["Stellamod:CloudySky"].Load();
            Filters.Scene["Stellamod:CloudySky"] = new Filter((new ScreenShaderData("FilterMiniTower")).UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);

            SkyManager.Instance["Stellamod:DesertSky"] = new DesertSky();
            SkyManager.Instance["Stellamod:DesertSky"].Load();
            Filters.Scene["Stellamod:DesertSky"] = new Filter((new ScreenShaderData("FilterMiniTower")).UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
            LoadOrderedLoadables();
            Instance = this;
        }
        private List<IOrderedLoadable> loadCache;
        void LoadOrderedLoadables()
        {
            loadCache = new List<IOrderedLoadable>();

            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IOrderedLoadable)))
                {
                    object instance = Activator.CreateInstance(type);
                    loadCache.Add(instance as IOrderedLoadable);
                }

                loadCache.Sort((n, t) => n.Priority.CompareTo(t.Priority));
            }

            for (int k = 0; k < loadCache.Count; k++)
            {
                loadCache[k].Load();
       //         SetLoadingText("Loading " + loadCache[k].GetType().Name);
            }

        //    recipeGroupCache = new List<IRecipeGroup>();
        /*
            foreach (Type type in Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IRecipeGroup)))
                {
                    object instance = Activator.CreateInstance(type);
                    recipeGroupCache.Add(instance as IRecipeGroup);
                }

                recipeGroupCache.Sort((n, t) => n.Priority > t.Priority ? 1 : -1);
            }*/
        }

        void UnloadOrderedLoadables()
        {
            if (loadCache != null)
            {
                foreach (IOrderedLoadable loadable in loadCache)
                {
                    loadable.Unload();
                }

                loadCache = null;
            }
            else
            {
                Logger.Warn("load cache was null, IOrderedLoadable's may not have been unloaded...");
            }

        }
        public override void Unload()
        {
            UnloadOrderedLoadables();
        }
    }
}

