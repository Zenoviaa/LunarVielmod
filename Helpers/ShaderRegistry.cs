using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.LoadingSystems;
using Stellamod.Skies;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    internal static class ShaderRegistry
    {
        private static List<IOrderedLoadable> _loadCache;
        public static string VampKnives_Basic_Trail => "VampKnives:BasicTrail";
        public static string VampKnives_Lightning_Trail => "VampKnives:LightningTrail";
        public static string VampKnives_Generic_Laser_Shader => "VampKnives:GenericLaserShader";
        public static string VampKnives_Light_Beam_Vertex_Shader => "VampKnives:LightBeamVertexShader";

        public static string VampKnives_Fire => "VampKnives:Fire";
        public static string StellamodFireWhiteShader => "VampKnives:FireWhite";


        private static string Silhouette_Shader => "Stellamod:SilhouetteShader";

        public static string Screen_Black => "Stellamod:Black";
        public static string Screen_Tint => "Stellamod:Tint";
        public static string Screen_NormalDistortion => "Stellamod:NormalDistortion";
        public static string Screen_Vignette => "Stellamod:Vignette";

        public static string Screen_Palette => "Stellamod:Palette";

        //SHADERING
        private static string GlowingDustShader => "Stellamod:GlowingDust";
        public static MiscShaderData MiscGlowingDust => GameShaders.Misc[GlowingDustShader];

        private static string FireWhitePixelShaderName => "Stellamod:FireWhitePixelShader";
        public static MiscShaderData MiscFireWhitePixelShader => GameShaders.Misc[FireWhitePixelShaderName];

        private static string TestPixelShaderName => "Stellamod:TestPixelShader";
        public static MiscShaderData MiscTestPixelShader => GameShaders.Misc[TestPixelShaderName];

        private static string SilShaderName => "Stellamod:SilShader";
        public static MiscShaderData MiscSilPixelShader => GameShaders.Misc[SilShaderName];

        private static string DistortionShaderName => "Stellamod:DistortionShader";
        public static MiscShaderData MiscDistortionShader => GameShaders.Misc[DistortionShaderName];

        public static AssetRepository Assets => Stellamod.Instance.Assets;

        private static void RegisterMiscShader(string name, string path, string pass)
        {
            Asset<Effect> miscShader = Assets.Request<Effect>(path, AssetRequestMode.ImmediateLoad);
            var miscShaderData = new MiscShaderData(miscShader, pass);
            GameShaders.Misc[name] = miscShaderData;
        }
        private static void RegisterScreenShader(string name, string path, EffectPriority effectPriority = EffectPriority.Medium)
        {
            Asset<Effect> paletteShader = Assets.Request<Effect>(path);
            Filters.Scene[name] = new Filter(new ScreenShaderData(paletteShader, "ScreenPass"), effectPriority);

        }
        public static void LoadShaders()
        {
            if (!Main.dedServ)
            {
                Filters.Scene["Stellamod:VeilSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
                SkyManager.Instance["Stellamod:VeilSky"] = new AuroranSky();

                Filters.Scene["Stellamod:GreenSunSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 1f, 0.3f).UseOpacity(0.275f), EffectPriority.VeryHigh);
                SkyManager.Instance["Stellamod:GreenSunSky"] = new GreenSunSky();
            }

            Ref<Effect> BasicTrailRef = new(Assets.Request<Effect>("Effects/Primitives/BasicTrailShader", AssetRequestMode.ImmediateLoad).Value);
            Ref<Effect> LightningTrailRef = new(Assets.Request<Effect>("Effects/Primitives/LightningTrailShader", AssetRequestMode.ImmediateLoad).Value);

            GameShaders.Misc[ShaderRegistry.VampKnives_Basic_Trail] = new MiscShaderData(BasicTrailRef, "TrailPass");
            GameShaders.Misc[ShaderRegistry.VampKnives_Lightning_Trail] = new MiscShaderData(LightningTrailRef, "TrailPass");

            Asset<Effect> shader2 = ModContent.Request<Effect>("Stellamod/Trails/SilhouetteShader", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc[ShaderRegistry.Silhouette_Shader] = new MiscShaderData(new Ref<Effect>(shader2.Value), "SilhouettePass");

            Ref<Effect> genericLaserShader = new(Assets.Request<Effect>("Effects/Primitives/GenericLaserShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc[ShaderRegistry.VampKnives_Generic_Laser_Shader] = new MiscShaderData(genericLaserShader, "TrailPass");

            Ref<Effect> LightBeamVertexShader = new(Assets.Request<Effect>("Effects/Primitives/LightBeamVertexShader", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc[ShaderRegistry.VampKnives_Light_Beam_Vertex_Shader] = new MiscShaderData(LightBeamVertexShader, "TrailPass");



            Ref<Effect> shadowflameShader = new(Assets.Request<Effect>("Effects/Primitives/Shadowflame", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc[ShaderRegistry.VampKnives_Fire] = new MiscShaderData(shadowflameShader, "TrailPass");

            Ref<Effect> whiteflameShader = new(Assets.Request<Effect>("Effects/Whiteflame", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc[ShaderRegistry.StellamodFireWhiteShader] = new MiscShaderData(whiteflameShader, "TrailPass");

            Asset<Effect> glowingDustShader = Assets.Request<Effect>("Effects/GlowingDust");
            GameShaders.Misc[ShaderRegistry.GlowingDustShader] = new MiscShaderData(glowingDustShader, "GlowingDustPass");

            Ref<Effect> SuperSimpleTrailRef = new(Assets.Request<Effect>("Effects/SimpleTrail", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["VampKnives:SuperSimpleTrail"] = new MiscShaderData(SuperSimpleTrailRef, "TrailPass");

            Ref<Effect> DaedusRobeRef = new(Assets.Request<Effect>("Effects/DaedusRobe", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["LunarVeil:DaedusRobe"] = new MiscShaderData(DaedusRobeRef, "PixelPass");

            Ref<Effect> lightningBoltRef = new(Assets.Request<Effect>("Effects/LightningBolt", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["LunarVeil:LightningBolt"] = new MiscShaderData(lightningBoltRef, "PrimitivesPass");

            Asset<Effect> blackShader = Assets.Request<Effect>("Effects/Black");
            Filters.Scene[ShaderRegistry.Screen_Black] = new Filter(new ScreenShaderData(blackShader, "BlackPass"), EffectPriority.Medium);

            Asset<Effect> tintShader = Assets.Request<Effect>("Effects/Tint");
            Filters.Scene[ShaderRegistry.Screen_Tint] = new Filter(new ScreenShaderData(tintShader, "ScreenPass"), EffectPriority.Medium);

            Asset<Effect> distortionShader = Assets.Request<Effect>("Effects/NormalDistortion");
            Filters.Scene[ShaderRegistry.Screen_NormalDistortion] = new Filter(new ScreenShaderData(distortionShader, "ScreenPass"), EffectPriority.Medium);

            Asset<Effect> vignetteShader = Assets.Request<Effect>("Effects/Vignette");
            Filters.Scene[ShaderRegistry.Screen_Vignette] = new Filter(new ScreenShaderData(vignetteShader, "ScreenPass"), EffectPriority.Medium);


            Asset<Effect> paletteShader = Assets.Request<Effect>("Effects/Palette");
            Filters.Scene[ShaderRegistry.Screen_Palette] = new Filter(new ScreenShaderData(paletteShader, "ScreenPass"), EffectPriority.Medium);

            Ref<Effect> gustArmorRef = new(Assets.Request<Effect>("Effects/GustArmor", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["LunarVeil:GustArmor"] = new MiscShaderData(gustArmorRef, "PixelPass");

            //Palette Shaders
            RegisterScreenShader("LunarVeil:PaletteAbyss", "Effects/Palette");
            RegisterScreenShader("LunarVeil:PaletteHell", "Effects/PaletteHell");
            RegisterScreenShader("LunarVeil:PaletteRoyalCapital", "Effects/PaletteRoyalCapital");
            RegisterScreenShader("LunarVeil:PaletteDungeon", "Effects/PaletteDungeon");
            RegisterScreenShader("LunarVeil:PaletteDesert", "Effects/PaletteDesert");
            RegisterScreenShader("LunarVeil:PaletteDesertTop", "Effects/PaletteDesertTop");
            RegisterScreenShader("LunarVeil:PaletteBloodCathedral", "Effects/PaletteBloodCathedral");
            RegisterScreenShader("LunarVeil:PaletteBloodHound", "Effects/PaletteBloodHound");
            RegisterScreenShader("LunarVeil:PaletteVirulent", "Effects/PaletteVirulent");
            RegisterScreenShader("LunarVeil:DarknessVignette", "Effects/DarknessVignette");
            RegisterScreenShader("LunarVeil:DarknessCurve", "Effects/DarknessCurve", EffectPriority.High);
            RegisterScreenShader("LunarVeil:Blur", "Effects/Blur", EffectPriority.High);

            Ref<Effect> skyRef = new(Assets.Request<Effect>("Effects/RoyalCapitalSky", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["LunarVeil:RoyalCapitalSky"] = new MiscShaderData(skyRef, "ScreenPass");

            Ref<Effect> starsRef = new(Assets.Request<Effect>("Effects/RoyalCapitalStars", AssetRequestMode.ImmediateLoad).Value);
            GameShaders.Misc["LunarVeil:RoyalCapitalStars"] = new MiscShaderData(starsRef, "ScreenPass");

            //White Flame Pixel Shader
            RegisterMiscShader(FireWhitePixelShaderName, "Effects/WhiteflamePixelShader", "TrailPass");

            //Test Shader (For Testing)
            RegisterMiscShader(TestPixelShaderName, "Effects/TestShader", "PixelPass");

            //Sil Shader
            RegisterMiscShader(SilShaderName, "Effects/SilShader", "PixelPass");

            //Distortion Shader
            RegisterMiscShader(DistortionShaderName, "Effects/NormalDistortion", "ScreenPass");

            RegisterMiscShader("LunarVeil:SimpleDistortion", "Effects/SimpleDistortion", "PixelPass");
            RegisterMiscShader("LunarVeil:SimpleMasking", "Effects/SimpleMasking", "PixelPass");

            Ref<Effect> lavaRef = new(Assets.Request<Effect>("Effects/Lava", AssetRequestMode.ImmediateLoad).Value);
            Filters.Scene["LunarVeil:Lava"] = new Filter(new ScreenShaderData(lavaRef, "PrimitivesPass"), EffectPriority.VeryHigh);
            Filters.Scene["LunarVeil:Lava"].Load();

            //Skies

            SkyManager.Instance["LunarVeil:RoyalCapitalSky"] = new RoyalCapitalSky();
            SkyManager.Instance["LunarVeil:RoyalCapitalSky"].Load();

            SkyManager.Instance["Stellamod:NaxtrinSky"] = new NaxtrinSky();
            SkyManager.Instance["Stellamod:NaxtrinSky"].Load();

            SkyManager.Instance["Stellamod:NaxtrinSky2"] = new NaxtrinSky2();
            SkyManager.Instance["Stellamod:NaxtrinSky2"].Load();

            SkyManager.Instance["Stellamod:AlcadSky"] = new NaxtrinSky3();
            SkyManager.Instance["Stellamod:AlcadSky"].Load();

            SkyManager.Instance["Stellamod:SyliaSky"] = new SyliaSky();
            SkyManager.Instance["Stellamod:SyliaSky"].Load();

            SkyManager.Instance["Stellamod:VillageSky"] = new VillageSky();
            SkyManager.Instance["Stellamod:VillageSky"].Load();
            LoadOrderedLoadables();
        }

        public static void LoadOrderedLoadables()
        {
            _loadCache = new List<IOrderedLoadable>();
            foreach (Type type in Stellamod.Instance.Code.GetTypes())
            {
                if (!type.IsAbstract && type.GetInterfaces().Contains(typeof(IOrderedLoadable)))
                {
                    object instance = Activator.CreateInstance(type);
                    _loadCache.Add(instance as IOrderedLoadable);
                }

                _loadCache.Sort((n, t) => n.Priority.CompareTo(t.Priority));
            }

            for (int k = 0; k < _loadCache.Count; k++)
            {
                _loadCache[k].Load();
            }
        }

        public static void UnloadOrderedLoadables()
        {
            if (_loadCache != null)
            {
                foreach (IOrderedLoadable loadable in _loadCache)
                {
                    loadable.Unload();
                }

                _loadCache = null;
            }
            else
            {
                //   Logger.Warn("load cache was null, IOrderedLoadable's may not have been unloaded...");
            }
        }
    }
}
