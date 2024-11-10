using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Particles;
using Stellamod.Utilis;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Morrow
{
    public class RubyBeetle : BaseBeetleNPC
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ruby Beetle");
            Main.npcFrameCount[NPC.type] = 6;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 20;
            NPC.defense = 10;
            NPC.lifeMax = 66;
            NPC.noGravity = true;
            NPC.value = 90f;
            NPC.noTileCollide = false;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0 && Main.netMode != NetmodeID.Server)
            {
                var entitySource = NPC.GetSource_FromThis();
                int Gore1 = ModContent.Find<ModGore>("Stellamod/RBeetle1").Type;
                int Gore2 = ModContent.Find<ModGore>("Stellamod/RBeetle2").Type;
                int Gore3 = ModContent.Find<ModGore>("Stellamod/RBeetle3").Type;
                int Gore4 = ModContent.Find<ModGore>("Stellamod/RBeetle4").Type;
                int Gore5 = ModContent.Find<ModGore>("Stellamod/RBeetle5").Type;
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Gore1);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Gore2);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Gore3);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Gore4);
                Gore.NewGore(entitySource, NPC.position, NPC.velocity, Gore5);
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                    ParticleManager.NewParticle(NPC.Center, speed * 4, ParticleManager.NewInstance<FlameParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            //Multiply by overworld chance so it doesn't spawn during vanilla events and such
            float spawnChance = SpawnCondition.Overworld.Chance * (spawnInfo.Player.ZoneFable() ? 1.6f : 0f);
            return spawnChance;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ItemID.Ruby, 2, 1, 4));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), 3, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), chanceDenominator: 1, minimumDropped: 2, maximumDropped: 4));
            npcLoot.Add(ItemDropRule.Common(ItemID.Silk, 3, 1, 7));
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.22f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
    }
}