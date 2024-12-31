using Microsoft.Xna.Framework;
using Stellamod.Items.Placeable;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Morrow
{
    public class AmethystBeetle : BaseBeetleNPC
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
			NPC.lifeMax = 90;
			NPC.noGravity = true;
			NPC.value = 90f;
			NPC.noTileCollide = false;
			NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.Player.ZoneCorrupt)
			{
				return SpawnCondition.OverworldDay.Chance * 0.5f;
			}
			return SpawnCondition.OverworldNight.Chance * 0f;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			npcLoot.Add(ItemDropRule.Common(ItemID.Amethyst, 3, 1, 3));
			npcLoot.Add(ItemDropRule.Common(ItemID.Silk, 1, 1, 7));
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