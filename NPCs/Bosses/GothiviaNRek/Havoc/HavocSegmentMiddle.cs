using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaNRek.Havoc
{
    internal class HavocSegmentMiddle : ModNPC
    {
        private NPC BackConnectedSegment
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

        private NPC FrontConnectedSegment
        {
            get
            {
                int npcIndex = (int)NPC.ai[1];
                NPC npc = Main.npc[npcIndex];
                return npc;
            }
            set
            {
                NPC.ai[1] = value.whoAmI;
            }
        }

        public override void SetDefaults()
        {
            NPC.width = 54;
            NPC.height = 74;
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
            NPC.Center = Vector2.Lerp(BackConnectedSegment.Center, FrontConnectedSegment.Center, 0.5f);
        }
    }
}
