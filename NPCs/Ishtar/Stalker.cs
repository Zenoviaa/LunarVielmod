using Stellamod.Assets.Biomes;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Thrown.Jugglers;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Ishtar
{
    internal class Stalker : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 16;
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 14;
            NPC.lifeMax = 450;
            NPC.damage = 90;
            AIType = NPCID.BoneLee;
            NPC.aiStyle = NPCAIStyleID.Fighter;
            NPC.HitSound = SoundID.NPCHit29;
            NPC.DeathSound = SoundID.NPCDeath32;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<IshtarBiome>().Type };
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.6f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.InModBiome<IshtarBiome>())
            {
                return 0.3f;
            }

            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<IshtarCandle>(), 20, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FlinchMachine>(), 12, 1, 1));
        }
    }
}
