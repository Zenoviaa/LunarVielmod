using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Utilis;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace Stellamod.NPCs.Town
{
    // This ModNPC serves as an example of a completely custom AI.
    public class BoundGia : ModNPC
	{
		// Our texture is 36x36 with 2 pixels of padding vertically, so 38 is the vertical spacing.
		// These are for our benefit and the numbers could easily be used directly in the code below, but this is how we keep code organized.
		private enum Frame
		{
			first,
			second,
			third,
			fourth
		}
		private enum ActionState
		{
			Ambient,
			Amb2,
			Amb3,
			Amb4
		}


		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("???");
			Main.npcFrameCount[NPC.type] = 1; // make sure to set this for your modnpcs.
			NPCID.Sets.TownCritter[NPC.type] = true;
			// Specify the debuffs it is immune to

		}

		public bool Towned = false;
		public override void SetDefaults()
		{

			NPC.friendly = true;
			NPC.townNPC = true;
			NPC.dontTakeDamage = true;
			NPC.width = 32;
			NPC.height = 48;
			NPC.aiStyle = 0;
			NPC.damage = 0;
			NPC.defense = 25;
			NPC.DeathSound = new SoundStyle($"Stellamod/Assets/Sounds/ItemHarvested");  // The sound the NPC will make when it dies.
			NPC.knockBackResist = 0f;
			NPC.rarity = 1;
			NPC.value = 250f; // How many copper coins the NPC will drop when killed.
	
			NPC.friendly = true;
			NPC.lifeMax = 250;
			NPC.dontTakeDamageFromHostiles = true;
			
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (AlcadSpawnSystem.TownedGia || NPC.AnyNPCs(ModContent.NPCType<BoundGia>()) || NPC.AnyNPCs(ModContent.NPCType<Gia>()))
				return 0f;

			if (spawnInfo.Player.ZoneRockLayerHeight)
			{
				return spawnInfo.Player.ZoneAcid() ? 0.1f : 0f;
			}

			if (spawnInfo.Player.ZoneOverworldHeight)
			{
				return spawnInfo.Player.ZoneAcid() ? 0.2f : 0f;
			}

			return SpawnCondition.Cavern.Chance * 0f;
		}

		// Our AI here makes our NPC sit waiting for a player to enter range, jumps to attack, flutter mid-fall to stay afloat a little longer, then falls to the ground. Note that animation should happen in FindFrame
		public override void AI()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.homeless = false;
				NPC.homeTileX = -1;
				NPC.homeTileY = -1;
				NPC.netUpdate = true;
			}

			if (NPC.wet)
			{
				NPC.life = 250;
			}
			foreach (var player in Main.player)
			{
				if (!player.active) continue;
				if (player.talkNPC == NPC.whoAmI)
				{
					Rescue();
					return;
				}
			}
		}


		public override void OnKill()
		{
			CombatText.NewText(NPC.getRect(), Color.White, LangText.Misc("BoundGia"), true, false);
			base.OnKill();
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A bound scientist lurking in the Virulent"))
			});
		}


		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override string GetChat() => LangText.Chat(this, "Basic");


		public void Rescue()
		{
			NPC.Transform(ModContent.NPCType<Gia>());
			NPC.dontTakeDamage = false;
		}
	}
}