using Microsoft.Xna.Framework;
using Stellamod.Content.Areas.Abyss.BossesAB.VerlianSingularity;
using Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine;
using Stellamod.Core.DialogueSystem;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Dialogue
{
    public class BishinineStart : BaseDialogue
    {
        public override int GetLength()
        {
            return 8;
        }

        public override void OnComplete()
        {
            base.OnComplete();
       
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int index = NPC.FindFirstNPC(ModContent.NPCType<BishinineIdle>());
                if (index == -1)
                    return;
                Vector2 position = Main.npc[index].position;
                int x = (int)position.X;
                int y = (int)position.Y;    
                int npcID = NPC.NewNPC(new EntitySource_TileBreak(x, y), x, y, ModContent.NPCType<Bishinine>());
                Main.npc[npcID].netUpdate = true;
            }
            else
            {
                int index = NPC.FindFirstNPC(ModContent.NPCType<BishinineIdle>());
                if (index == -1)
                    return;

                Vector2 position = Main.npc[index].position;
                MultiplayerHelper.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI, 
                    ModContent.NPCType<Bishinine>(), (int)position.X, (int)position.Y);
            }
        }
    }
}
