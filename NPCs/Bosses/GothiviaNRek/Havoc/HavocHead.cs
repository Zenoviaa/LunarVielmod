
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaNRek.Havoc
{
    internal class HavocHead : ModNPC
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

        float Timer;
        public override void SetDefaults()
        {
            NPC.width = 170;
            NPC.height = 90;
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
            Timer++;
            if(Timer == 1 && StellaMultiplayer.IsHost)
            {
                NPC segmentFront = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.position, ModContent.NPCType<HavocSegmentFront>());
                NPC segmentMiddle = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.position, ModContent.NPCType<HavocSegmentMiddle>());
                NPC segmentBack = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.position, ModContent.NPCType<HavocSegmentBack>());
                NPC segmentTail = NPC.NewNPCDirect(NPC.GetSource_FromThis(), NPC.position, ModContent.NPCType<HavocTail>());

                //Connect head segment
                BackConnectedSegment = segmentFront;

                //Connect front segment

                //Back
                segmentFront.ai[0] = segmentMiddle.whoAmI;

                //Front
                segmentFront.ai[1] = NPC.whoAmI;

                //Connect middle segment
                //Back
                segmentMiddle.ai[0] = segmentBack.whoAmI;

                //Front
                segmentMiddle.ai[1] = segmentMiddle.whoAmI;

                //Connect back segment
                segmentBack.ai[0] = segmentTail.whoAmI;
                segmentBack.ai[1] = segmentBack.whoAmI;

                //Connect tail segment
                segmentTail.ai[0] = segmentBack.whoAmI;
                NPC.netUpdate = true;
            }

            NPC.velocity = Vector2.UnitX * 4;
        }
    }
}
