using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Skies;
using System.IO;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod
{
    public class TestPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if (Main.netMode == NetmodeID.Server)
                return;
            if (!SkyManager.Instance["Stellamod:CloudySky"].IsActive())
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("Stellamod:CloudySky", targetCenter);
            }
            if (!SkyManager.Instance["Stellamod:DesertSky"].IsActive())
            {
                Vector2 targetCenter = Player.Center;
                SkyManager.Instance.Activate("Stellamod:DesertSky", targetCenter);
            }
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

            Asset<Effect> gradient = Assets.Request<Effect>("fx/Gradient", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc["Stellamod:Gradient"] = new MiscShaderData(gradient, "ScreenPass");

            SkyManager.Instance["Stellamod:CloudySky"] = new CloudySky();
            SkyManager.Instance["Stellamod:CloudySky"].Load();
            Filters.Scene["Stellamod:CloudySky"] = new Filter((new ScreenShaderData("FilterMiniTower")).UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);

            SkyManager.Instance["Stellamod:DesertSky"] = new DesertSky();
            SkyManager.Instance["Stellamod:DesertSky"].Load();
            Filters.Scene["Stellamod:DesertSky"] = new Filter((new ScreenShaderData("FilterMiniTower")).UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);

            Instance = this;
        }
    }
}

