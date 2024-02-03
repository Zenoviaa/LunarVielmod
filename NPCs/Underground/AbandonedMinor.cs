using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Underground
{
    internal class AbandonedMinor : ModNPC
    {
        private int _lastDirection;
        private float _waitTimer;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
        }

        public override void SetDefaults()
        {
            NPC.width = 34;
            NPC.height = 40;
            NPC.aiStyle = 3;
            NPC.damage = 42;
            NPC.defense = 8;
            NPC.lifeMax = 170;
            NPC.HitSound = SoundID.NPCHit48;
            NPC.DeathSound = SoundID.DD2_SkeletonDeath;
            NPC.value = 563f;
            NPC.knockBackResist = .45f;
            NPC.aiStyle = 3;
            AIType = NPCID.SnowFlinx;
        }

        public override void AI()
        {
     
            _waitTimer++;
            if (_waitTimer < 120)
            {
                NPC.velocity.X *= 0;
                NPC.spriteDirection = _lastDirection;
            }
            else
            {
                NPC.spriteDirection = -NPC.direction;
                _lastDirection = NPC.spriteDirection;
            }

            if(_waitTimer >= 400)
            {
                _waitTimer = 0;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if(_waitTimer < 120)
            {
                //Idle
                NPC.frameCounter += 0.2f;
                NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                if (NPC.frameCounter >= 6)
                    NPC.frameCounter = 0;
                int frame = (int)NPC.frameCounter;
                NPC.frame.Y = frame * frameHeight;
            }
            else
            {
                //Moving
                NPC.frameCounter += 0.2f;
                NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                if (NPC.frameCounter < 6)
                    NPC.frameCounter = 6;
                int frame = (int)NPC.frameCounter;
                NPC.frame.Y = frame * frameHeight;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            //You can't be in the surface and underground at the same time so this should work
            //0.05f should make it 20 less common than normal spawns.
            return (SpawnCondition.Cavern.Chance * 0.75f);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.SpelunkerGlowstick, minimumDropped: 1, maximumDropped: 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.Coal, minimumDropped: 1, maximumDropped: 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.MiningPants, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.MiningShirt, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 3));
            npcLoot.Add(ItemDropRule.Common(ItemID.MiningHelmet, chanceDenominator: 20, minimumDropped: 1, maximumDropped: 3));
        }
    }
}
