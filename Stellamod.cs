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

            SkyManager.Instance["Stellamod:CloudySky"] = new CloudySky();
            SkyManager.Instance["Stellamod:CloudySky"].Load();
            Filters.Scene["Stellamod:CloudySky"] = new Filter((new ScreenShaderData("FilterMiniTower")).UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
            Instance = this;
        }
    }
}

