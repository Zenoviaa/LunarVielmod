using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Morrow
{
    public class EmeraldBeetle : BaseBeetleNPC
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
			NPC.damage = 30;
			NPC.defense = 10;
			NPC.lifeMax = 270;
			NPC.noGravity = true;
			NPC.value = 90f;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneJungle)
            {

                return SpawnCondition.OverworldDay.Chance * 0.04f;

            }
            return SpawnCondition.OverworldNight.Chance * 0f;
        }
 
		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
		
			npcLoot.Add(ItemDropRule.Common(ItemID.Emerald, 2, 1, 4));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), 5, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), 2, 1, 5));
            npcLoot.Add(ItemDropRule.Common(ItemID.Silk, 1, 1, 7));
			npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<MorrowChestKey>(), 2, 1, 1));

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