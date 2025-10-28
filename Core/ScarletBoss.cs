using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.HealthbarSystem;
using Stellamod.Core.TitleSystem;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public abstract class ScarletBoss : ModNPC
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.boss = true;
        }
        public string Texture_BossIcon => Texture + "_BossIcon";
        public string Texture_BossBar => Texture + "_BossBar";

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if (Main.netMode == NetmodeID.Server)
                return;

            ModContent.GetInstance<BossHealthbarSystem>().Add(this);
        }

        public virtual bool CanFight()
        {
            return true;
        }

        public void ShowNamePlate()
        {
            //UI can't run on the server
            if (Main.netMode == NetmodeID.Server)
                return;

            TitleCardUISystem uiSystem = ModContent.GetInstance<TitleCardUISystem>();
            uiSystem.OpenUI(DisplayName.Value, 7);
            uiSystem.titleUIState.titleCardUI.LineTexture = ModContent.Request<Texture2D>(TitleCardUISystem.RootTexturePath + "UnderlineBiome");
        }
    }
}
