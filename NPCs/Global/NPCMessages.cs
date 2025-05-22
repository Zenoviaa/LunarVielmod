using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.GothiviaNRek.Gothivia;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Irradia;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Global
{
    internal class NPCMessages : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if(npc.type == NPCID.Plantera && !NPC.downedPlantBoss)
            {
                string text = "Zui has something to offer you...";
                Color color = Color.Gold;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkText nText = NetworkText.FromLiteral(text);
                    ChatHelper.BroadcastChatMessage(nText, color);
                }
                else
                {
                    Main.NewText(text, color.R, color.G, color.B);
                }
            }

            if (npc.type == NPCID.Plantera && !NPC.downedPlantBoss)
            {
                string text = "The Lunar tree rumbles...";
                Color color = Color.LightGreen;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkText nText = NetworkText.FromLiteral(text);
                    ChatHelper.BroadcastChatMessage(nText, color);
                }
                else
                {
                    Main.NewText(text, color.R, color.G, color.B);
                }
            }

            if(npc.type == NPCID.WallofFlesh && !Main.hardMode)
            {
                string text = "The Abysm and Virulent stir...";
                Color color = Color.Red;
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkText nText = NetworkText.FromLiteral(text);
                    ChatHelper.BroadcastChatMessage(nText, color);
                }
                else
                {
                    Main.NewText(text, color.R, color.G, color.B);
                }
            } 
           
            if (npc.type == NPCID.EyeofCthulhu && !NPC.downedBoss1)
            {
                string text = "Azurewrath's dreams alcadize the sky.";
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkText nText = NetworkText.FromLiteral(text);
                    ChatHelper.BroadcastChatMessage(nText, new Color(234, 96, 114));
                }
                else
                {
                    Main.NewText(text, 234, 96, 114);
                }
            }

            if (npc.type == ModContent.NPCType<Irradia>() && !DownedBossSystem.downedIrradiaBoss)
            {
                string text = "The toxic reunion begins! Gothivia's love bleeds through as the sun burns with everlasting hue.";
                if (Main.netMode == NetmodeID.Server)
                {
                    NetworkText nText = NetworkText.FromLiteral(text);
                    ChatHelper.BroadcastChatMessage(nText, new Color(234, 296, 114));
                }
                else
                {
                    Main.NewText(text, 234, 96, 114);
                }
            }

            base.OnKill(npc);
        }
    }
}
