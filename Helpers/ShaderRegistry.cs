using ReLogic.Content;
using Stellamod.Core.Skies;
using Stellamod.Skies;
using System.Collections.Generic;
using System.IO;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Helpers
{
    public static class ShaderRegistry
    {
        public static string Screen_Black => "Stellamod:Black";
        public static string Screen_Tint => "Stellamod:Tint";
        public static string Screen_NormalDistortion => "Stellamod:NormalDistortion";
        public static string Screen_Vignette => "Stellamod:Vignette";
        private static string GlowingDustShader => "Stellamod:GlowingDust";
        private static string FireWhitePixelShaderName => "Stellamod:FireWhitePixelShader";
        public static MiscShaderData MiscFireWhitePixelShader => GameShaders.Misc[FireWhitePixelShaderName];
        private static string SilShaderName => "Stellamod:SilShader";
        public static MiscShaderData MiscSilPixelShader => GameShaders.Misc[SilShaderName];

        private static string DistortionShaderName => "Stellamod:DistortionShader";
        public static AssetRepository Assets => Stellamod.Instance.Assets;
        public static MiscShaderData GradientShader => GameShaders.Misc["CrystalMoon:Gradient"];
        public static MiscShaderData CloudsShader => GameShaders.Misc["CrystalMoon:Clouds"];
        public static MiscShaderData CloudsFrontShader => GameShaders.Misc["CrystalMoon:CloudsFront"];
        public static MiscShaderData NightCloudsShader => GameShaders.Misc["CrystalMoon:NightClouds"];
        public static MiscShaderData CloudsDesertShader => GameShaders.Misc["CrystalMoon:CloudsDesert"];
        public static MiscShaderData CloudsDesertNightShader => GameShaders.Misc["CrystalMoon:CloudsDesertNight"];
        public static List<string> ScreenShaders;
        private static void RegisterMiscShader(string name, string path, string pass)
        {
            Asset<Effect> miscShader = Assets.Request<Effect>(path);
            var miscShaderData = new MiscShaderData(miscShader, pass);
            GameShaders.Misc[name] = miscShaderData;
        }
        private static void RegisterMiscCrystalShader(string name, string pass)
        {
            string assetPath = $"Effects/CrystalShaders/{name}";
            Asset<Effect> miscShader = Assets.Request<Effect>(assetPath);
            GameShaders.Misc[$"CrystalMoon:{name}"] = new MiscShaderData(miscShader, pass);
        }
        private static void RegisterScreenShader(string name, string path, EffectPriority effectPriority = EffectPriority.Medium)
        {
            var mod = Stellamod.Instance;
            if (!mod.FileExists(path + ".xnb") && !mod.FileExists(path + ".fxc"))
                return;
            Asset<Effect> paletteShader = Assets.Request<Effect>(path, AssetRequestMode.AsyncLoad);
            Filters.Scene[name] = new Filter(new ScreenShaderData(paletteShader, "ScreenPass"), effectPriority);
            ScreenShaders.Add(name);
        }

        public static void LoadShaders()
        {
            ScreenShaders = new List<string>();

            Asset<Effect> DaedusRobeRef = Assets.Request<Effect>("Effects/DaedusRobe");
            GameShaders.Misc["LunarVeil:DaedusRobe"] = new MiscShaderData(DaedusRobeRef, "PixelPass");

            Asset<Effect> lightningBoltRef = Assets.Request<Effect>("Effects/LightningBolt");
            GameShaders.Misc["LunarVeil:LightningBolt"] = new MiscShaderData(lightningBoltRef, "PrimitivesPass");

            Asset<Effect> blackShader = Assets.Request<Effect>("Effects/Black");
            Filters.Scene[ShaderRegistry.Screen_Black] = new Filter(new ScreenShaderData(blackShader, "BlackPass"), EffectPriority.Medium);

            Asset<Effect> tintShader = Assets.Request<Effect>("Effects/Tint");
            Filters.Scene[ShaderRegistry.Screen_Tint] = new Filter(new ScreenShaderData(tintShader, "ScreenPass"), EffectPriority.Medium);

            Asset<Effect> distortionShader = Assets.Request<Effect>("Effects/NormalDistortion");
            Filters.Scene[ShaderRegistry.Screen_NormalDistortion] = new Filter(new ScreenShaderData(distortionShader, "ScreenPass"), EffectPriority.Medium);

            Asset<Effect> vignetteShader = Assets.Request<Effect>("Effects/Vignette");
            Filters.Scene[ShaderRegistry.Screen_Vignette] = new Filter(new ScreenShaderData(vignetteShader, "ScreenPass"), EffectPriority.Medium);

            //Palette Shaders

            RegisterScreenShader("LunarVeil:DarknessVignette", "Effects/DarknessVignette");
            RegisterScreenShader("LunarVeil:DarknessCurve", "Effects/DarknessCurve", EffectPriority.High);
            RegisterScreenShader("LunarVeil:Blur", "Effects/Blur", EffectPriority.High);
            RegisterScreenShader("LunarVeil:BlackWhite", "Effects/BlackWhite");
            RegisterScreenShader("LunarVeil:BlackSea", "Effects/BlackSea");
            RegisterScreenShader("LunarVeil:DomainExpansion", "Effects/DomainExpansion");
            RegisterScreenShader("LunarVeil:Invert", "Effects/Invert");
            RegisterScreenShader("LunarVeil:DarkSmear", "Effects/DarkSmear");
            RegisterScreenShader("LunarVeil:PetalStorm", "Effects/PetalStorm");
            RegisterScreenShader("LunarVeil:SuperShockwave", "Effects/SuperShockwave");
            RegisterScreenShader("LunarVeil:WorldDepthGradient", "Effects/WorldDepthGradient");
            RegisterScreenShader("LunarVeil:Rippler", "Effects/Rippler");
            RegisterScreenShader("LunarVeil:FlameWinds", "Effects/GothinFlames/FlameWinds");

            Mod mod = Stellamod.Instance;
            foreach (var file in mod.GetFileNames())
            {
                if (file.Contains(".pal"))
                {
                    string fileName = new FileInfo(file).Name;
                    RegisterScreenShader($"LunarVeil:{fileName}", $"Effects/Palettes/{fileName.Replace(".pal", "")}", EffectPriority.Low);
                }
            }

            Asset<Effect> skyRef = Assets.Request<Effect>("Effects/RoyalCapitalSky");
            GameShaders.Misc["LunarVeil:RoyalCapitalSky"] = new MiscShaderData(skyRef, "ScreenPass");

            Asset<Effect> starsRef = Assets.Request<Effect>("Effects/RoyalCapitalStars");
            GameShaders.Misc["LunarVeil:RoyalCapitalStars"] = new MiscShaderData(starsRef, "ScreenPass");

            RegisterMiscShader("LunarVeil:SunShadow", "Effects/SunShadow", "ScreenPass");
            RegisterMiscShader("LunarVeil:SunBlur", "Effects/SunBlur", "ScreenPass");
            RegisterMiscShader("LunarVeil:MoonWaters", "Effects/MoonWaters", "P0");
            RegisterMiscShader("LunarVeil:SingularReflection", "Effects/SingularReflection", "P0");

 
            //Sil Shader
            RegisterMiscShader(SilShaderName, "Effects/SilShader", "PixelPass");

            //Distortion Shader
            RegisterMiscShader(DistortionShaderName, "Effects/NormalDistortion", "ScreenPass");

            RegisterMiscShader("LunarVeil:SimpleDistortion", "Effects/SimpleDistortion", "PixelPass");
            RegisterMiscShader("LunarVeil:SimpleMasking", "Effects/SimpleMasking", "PixelPass");

            //Skies
            SkyManager.Instance["LunarVeil:RoyalCapitalSky"] = new RoyalCapitalSky();
            SkyManager.Instance["LunarVeil:RoyalCapitalSky"].Load();

            SkyManager.Instance["LunarVeil:DarkspaceSky"] = new RoyalCapitalSky();
            SkyManager.Instance["LunarVeil:DarkspaceSky"].Load();

            SkyManager.Instance["Stellamod:NaxtrinSky"] = new NaxtrinSky();
            SkyManager.Instance["Stellamod:NaxtrinSky"].Load();

            SkyManager.Instance["Stellamod:AlcadSky"] = new NaxtrinSky3();
            SkyManager.Instance["Stellamod:AlcadSky"].Load();

            RegisterMiscCrystalShader("Clouds", "ScreenPass");
            RegisterMiscCrystalShader("CloudsFront", "ScreenPass");
            RegisterMiscCrystalShader("NightClouds", "ScreenPass");
            RegisterMiscCrystalShader("CloudsDesert", "ScreenPass");
            RegisterMiscCrystalShader("CloudsDesertNight", "ScreenPass");
            RegisterMiscCrystalShader("Gradient", "ScreenPass");

            //Crystal Moon Skies
            SkyManager.Instance["CrystalMoon:CloudySky"] = new CloudySky();
            SkyManager.Instance["CrystalMoon:CloudySky"].Load();
            Filters.Scene["CrystalMoon:CloudySky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);

            SkyManager.Instance["CrystalMoon:DesertSky"] = new DesertSky();
            SkyManager.Instance["CrystalMoon:DesertSky"].Load();
            Filters.Scene["CrystalMoon:DesertSky"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
        }

    }
}
