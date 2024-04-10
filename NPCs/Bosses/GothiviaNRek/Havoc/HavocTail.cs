using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaNRek.Havoc
{
    internal class HavocTail : ModNPC
    {
        private NPC FrontConnectedSegment
        {
            get
            {
                int npcIndex = (int)NPC.ai[0];
                NPC npc = Main.npc[npcIndex];
                return npc;
            }
            set
            {
                NPC.ai[0] = value.whoAmI;
            }
        }

        public override void SetDefaults()
        {
            NPC.width = 136;
            NPC.height = 58;
            NPC.lifeMax = 1000;
            NPC.damage = 100;
            NPC.boss = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.aiStyle = -1;
            NPC.knockBackResist = 0f;
        }


        public override void AI()
        {
            NPC.Center = Vector2.Lerp(NPC.Center, FrontConnectedSegment.Center, 0.5f);
        }
    }
}
