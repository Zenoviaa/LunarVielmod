using Microsoft.Xna.Framework;
using Stellamod.Content.Areas.Dock.BossesDK.Jiitas;
using Stellamod.Core.DialogueSystem;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Dialogue
{
    public class JiitasStartDialogue : BaseDialogue
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CloseOnComplete = true;
        }

        public override int GetLength()
        {
            return 5;
        }

        public override void OnComplete()
        {
            base.OnComplete();

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int index = NPC.FindFirstNPC(ModContent.NPCType<JiitasIdle>());
                if (index == -1)
                    return;
                Vector2 position = Main.npc[index].position;
                int x = (int)position.X;
                int y = (int)position.Y;
                int npcID = NPC.NewNPC(new EntitySource_TileBreak(x, y), x, y, ModContent.NPCType<Jiitas>());
                Main.npc[npcID].netUpdate = true;
            }
            else
            {
                int index = NPC.FindFirstNPC(ModContent.NPCType<JiitasIdle>());
                if (index == -1)
                    return;

                Vector2 position = Main.npc[index].position;
                MultiplayerHelper.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI,
                    ModContent.NPCType<Jiitas>(), (int)position.X, (int)position.Y);
            }
        }
    }
}
