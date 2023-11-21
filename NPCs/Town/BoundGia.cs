using Microsoft.Xna.Framework;
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
			bool npcAlreadyExists = false;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if(npc.type == ModContent.NPCType<BoundGia>() || npc.type == ModContent.NPCType<Gia>())
                {
					npcAlreadyExists = true;
					break;
                }
			}

			//Don't spawn the npc if it already exists
			if (npcAlreadyExists)
            {
				return 0f;
			}

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
			CombatText.NewText(NPC.getRect(), Color.White, "Im being taken away help!", true, false);
			base.OnKill();
		}
		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				// Sets the description of this NPC that is listed in the bestiary.
				new FlavorTextBestiaryInfoElement("A bound scientist lurking in the Virulent")
			});
		}


		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => false;

		public override string GetChat() => "Oh goodness thank you for saving me. I dont know how long I was tied up... I got stranded down here by a bunch of scouts I presume are from the morrow. I really caused a mess this time but I am really glad you saved me. D'you have a place to stay?";


		public void Rescue()
		{
			NPC.Transform(ModContent.NPCType<Gia>());
			NPC.dontTakeDamage = false;
		}



	}



}