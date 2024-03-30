using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Thrown;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.NPCs.Town
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    //[AutoloadHead]
	public class Merena : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;
		public const string ShopName = "Shop";
		public const string ShopName2 = "New Shop";
		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 8; // The amount of frames the NPC has

			NPCID.Sets.ActsLikeTownNPC[Type] = true;

			//To reiterate, since this NPC isn't technically a town NPC, we need to tell the game that we still want this NPC to have a custom/randomized name when they spawn.
			//In order to do this, we simply make this hook return true, which will make the game call the TownNPCName method when spawning the NPC to determine the NPC's name.
			NPCID.Sets.SpawnsWithCustomName[Type] = true;
			NPCID.Sets.NoTownNPCHappiness[Type] = true;

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
			{
				Velocity = 1f, // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
				Direction = 1 // -1 is left and 1 is right. NPCs are drawn facing the left by default but ExamplePerson will be drawn facing the right
							  // Rotation = MathHelper.ToRadians(180) // You can also change the rotation of an NPC. Rotation is measured in radians
							  // If you want to see an example of manually modifying these when the NPC is drawn, see PreDraw
			};


			NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);

			// Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
		}

		// Current frame
		public int frameCounter;
		// Current frame's progress
		public int frameTick;
		// Current state's timer
		public float timer;

		// AI counter
		public int counter;
		public override void SetDefaults()
		{
			NPC.friendly = true; // NPC Will not attack player
			NPC.width = 62;
			NPC.height = 90;
			NPC.aiStyle = 0;
			NPC.damage = 90;
			NPC.defense = 42;
			NPC.lifeMax = 200;
			NPC.npcSlots = 0;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			NPC.dontTakeDamageFromHostiles = true;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.16f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}

		public override bool CanChat()
		{
			return true;
		}

        //This prevents the NPC from despawning
        public override bool CheckActive()
        {
            return false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement("Magic Magic MAGIC"),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Merena the bewitched sorcerer")
			});
		}

		// The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
		// Returning false will allow you to manually draw your NPC
		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			// This code slowly rotates the NPC in the bestiary
			// (simply checking NPC.IsABestiaryIconDummy and incrementing NPC.Rotation won't work here as it gets overridden by drawModifiers.Rotation each tick)
			if (NPCID.Sets.NPCBestiaryDrawOffset.TryGetValue(Type, out NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers))
			{
				drawModifiers.Rotation += 0.001f;

				// Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
				NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
				NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			}

			return true;

		}
		public override string GetChat()
		{
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int partyGirl = NPC.FindFirstNPC(NPCID.Clothier);
			if (partyGirl >= 0 && Main.rand.NextBool(4))
			{
				chat.Add(Language.GetTextValue("Funny enough the clothier used to come through here all the time for some of our amazing fabrics", Main.npc[partyGirl].GivenName));
			}
			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(Language.GetTextValue("I wonder what my sister Sylia is up to nowadays, do you know her?"));
			chat.Add(Language.GetTextValue("I'm gonna be the very best, like no one ever was... "));
			chat.Add(Language.GetTextValue("Fenix is the best queen there will ever be! She gives us all what we want :P"));
			chat.Add(Language.GetTextValue("This place is so calming, I can't stand it but I would give everything to be near Fenix"), 5.0);
			chat.Add(Language.GetTextValue("Aimacra seems pretty cool, she came here recently looking for some brooches"), 0.4);
			chat.Add(Language.GetTextValue("Hey uh could you do something for me? I have a little favor, just ask what it is!"), 0.1);
			chat.Add(Language.GetTextValue("I've always wanted to be the best witch there is! I've always been outshined by exiles though, including my sisters sadly."), 0.1);
			chat.Add(Language.GetTextValue("I really need to beat my sister in a battle someday."), 0.1);
			chat.Add(Language.GetTextValue("It's strange, the economic system here is almost like communism"), 0.1);
			chat.Add(Language.GetTextValue("Damn we have some cool shit here"), 0.1);
			chat.Add(Language.GetTextValue("No doubt about it but Fenix is truly the strongest witch in all of the lands, maybe even more powerful than the witch of light in the hallow, I don't like her."), 0.1);
			chat.Add(Language.GetTextValue("Hahh, I remember that goon Verlia, she's kind of dumb but shes at least powerful, I remember she got exiled for stealing tomes, I might need that from her."), 0.1);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 10)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(Language.GetTextValue("Can you go collect some runes for me? I'd love for you to get working."));
			}

			return chat; // chat is implicitly cast to a string.
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood);
			}
		}
	
		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				"Merena the Sorcerer",
				"Merena the Sorcerer"
				
			};
		}


		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button2 = Language.GetTextValue("LegacyInterface.28");
			button = "Merenas Quest";

		}

		private void SendQuestPacket()
		{
			Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.CompleteMerenaQuest,
				MerenaQuestSystem.KillVerliaCompleted,
				MerenaQuestSystem.ExploreMorrowedVillageCompleted,
				MerenaQuestSystem.Give100DustBagsCompleted,
				MerenaQuestSystem.MakeMagicPaperCompleted,
				MerenaQuestSystem.MakeTomeOfInfiniteSorceryCompleted).Send(-1);

        }

		private void Quest_VerliaStart()
        {
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss1")); // Reforge/Anvil sound
			Main.npcChatText = $"What are you standing there for, go kill Verlia! She's an enemy of the royal capital and she has a book I need lmao";
			var entitySource = NPC.GetSource_GiftOrReward();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<KillVerlia>(), 1);
		}


		private void Quest_VerliaComplete()
        {
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

			Main.npcChatText = $"Oh damn thanks! Next on the list I need you to steal an orb from a village in an underground morrowed village, the orb contains a magic unlike any other. I have no idea how it was manifested but it's needed for this tome.";

			int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<KillVerliaC>());
			var entitySource = NPC.GetSource_GiftOrReward();

			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ExploreMorrowedVillage>(), 1);
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), Main.rand.Next(20));
			//Setting all previous quests to be complete, so it's backwards compatible with the old version.
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.KillVerliaCompleted, -1);
			SendQuestPacket();
        }

		private void Quest_MorrowStart()
        {
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"Oh damn thanks! Next on the list I need you to steal an orb from a village in an underground morrowed village, the orb contains a magic unlike any other. I have no idea how it was manifested but it's needed for this tome.";
			var entitySource = NPC.GetSource_GiftOrReward();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ExploreMorrowedVillage>(), 1);

		}

		private void Quest_MorrowComplete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

			Main.npcChatText = $"Woa, the energy is pouring out of this one with seamless orange stripes! How did you even get your hands on this?? Either way thanks, now I just need 100 dust bags, it helps with the brewery.";

			int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<ExploreMorrowedVillageC>());
			var entitySource = NPC.GetSource_GiftOrReward();

			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Give100DustBags>(), 1);
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), Main.rand.Next(20));
			//Setting all previous quests to be complete, so it's backwards compatible with the old version.
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.KillVerliaCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.ExploreMorrowedVillageCompleted, -1);
			SendQuestPacket();
        }

		private void Quest_DustBagsStart()
        {
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"Woa, the energy is pouring out of this one with seamless orange stripes! How did you even get your hands on this?? Either way thanks, now I just need 100 dust bags, it helps with the brewery.";
			var entitySource = NPC.GetSource_GiftOrReward();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Give100DustBags>(), 1);
			
		}

		private void Quest_DustBagsComplete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

			Main.npcChatText = $"Neat neat, that shouldn't have been too bad for you I think. Next I need some magical paper, there are magical creatures all over the world of hardmode who drop these, most of them being rare and unique creatures, go get em'!";

			int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<Give100DustBagsC>());
			var entitySource = NPC.GetSource_GiftOrReward();

			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<MakeMagicPaper>(), 1);
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), Main.rand.Next(20));
			//Setting all previous quests to be complete, so it's backwards compatible with the old version.
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.KillVerliaCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.ExploreMorrowedVillageCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.Give100DustBagsCompleted, -1);
            SendQuestPacket();
        }

		private void Quest_MagicPaperStart()
        {
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"Neat neat, that shouldn't have been too bad for you I think. Next I need some magical paper, there are magical creatures all over the world of hardmode who drop these, most of them being rare and unique creatures, go get em'!";
			var entitySource = NPC.GetSource_GiftOrReward();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<MakeMagicPaper>(), 1);
			
		}
		
		private void Quest_MagicPaperComplete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

			Main.npcChatText = $"OHH Great lmao. Ok we have one more thing we need to do. Legend has it an old thief of this Royal Capital stole an extremely special Carian tome, they stay deep underground hidden far away underneath the abyss. Even if the rumors arent true I'd love for you to find this scroll, it may take years...";

			int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<MakeMagicPaperC>());
			var entitySource = NPC.GetSource_GiftOrReward();

			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<MakeUltimateScroll>(), 1);
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), Main.rand.Next(20));
			//Setting all previous quests to be complete, so it's backwards compatible with the old version.
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.KillVerliaCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.ExploreMorrowedVillageCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.Give100DustBagsCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.MakeMagicPaperCompleted, -1);
            SendQuestPacket();
        }

		private void Quest_TomeStart()
        {
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"OHH Great lmao. Ok we have one more thing we need to do. Legend has it an old thief of this Royal Capital stole an extremely special Carian tome, they stay deep underground hidden far away underneath the abyss. Even if the rumors arent true I'd love for you to find this scroll, it may take years...";
			var entitySource = NPC.GetSource_GiftOrReward();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<MakeUltimateScroll>(), 1);
		}

		private void Quest_TomeComplete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"THANK YOU THANK YOU THANK YOU, omg this is the best day of my life! I never knew this actually existed! Were the rumors true??! dsfjhnbhfribdhs- Nevermind who cares anymore, we can both be the best mages ever! I open my shop to you and here, a token of my graditude. ";
			var entitySource = NPC.GetSource_GiftOrReward();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), Main.rand.Next(100));
			//Setting all previous quests to be complete, so it's backwards compatible with the old version.
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.KillVerliaCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.ExploreMorrowedVillageCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.Give100DustBagsCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.MakeMagicPaperCompleted, -1);
			NPC.SetEventFlagCleared(ref MerenaQuestSystem.MakeTomeOfInfiniteSorceryCompleted, -1);
            SendQuestPacket();
        }


		private bool CompleteQuests()
		{
			Player player = Main.LocalPlayer;
	
			if (player.HasItem(ModContent.ItemType<KillVerliaC>()))
			{
				Quest_VerliaComplete();
				return true;
			} 
			else if (player.HasItem(ModContent.ItemType<ExploreMorrowedVillageC>()))
			{
				Quest_MorrowComplete();
				return true;
			}
			else if (player.HasItem(ModContent.ItemType<Give100DustBagsC>()))
			{
				Quest_DustBagsComplete();
				return true;
			}
			else if (player.HasItem(ModContent.ItemType<MakeMagicPaperC>()))
			{
				Quest_MagicPaperComplete();
				return true;
			}
			else if (player.HasItem(ModContent.ItemType<TomeOfInfiniteSorcery>()))
			{
				Quest_TomeComplete();
				return true;
			}

			return false;
		}

		private void StartQuests()
        {
			Player player = Main.LocalPlayer;

			//Go through the list of quests in a specific order and see if any need to be started
			if (!MerenaQuestSystem.KillVerliaCompleted)
			{
				if (!player.HasItem(ModContent.ItemType<KillVerlia>()))
				{
					Quest_VerliaStart();
				}
			}
			else if (!MerenaQuestSystem.ExploreMorrowedVillageCompleted)
			{
				if (!player.HasItem(ModContent.ItemType<ExploreMorrowedVillage>()))
				{
					Quest_MorrowStart();
				}
			}
			else if (!MerenaQuestSystem.Give100DustBagsCompleted)
			{
				if (!player.HasItem(ModContent.ItemType<Give100DustBags>()))
				{
					Quest_DustBagsStart();
				}
			}
			else if (!MerenaQuestSystem.MakeMagicPaperCompleted)
			{
				if (!player.HasItem(ModContent.ItemType<MakeMagicPaper>()))
				{
					Quest_MagicPaperStart();
				}
			}
			else if (!MerenaQuestSystem.MakeTomeOfInfiniteSorceryCompleted)
			{
				if (!player.HasItem(ModContent.ItemType<MakeUltimateScroll>()))
				{
					Quest_TomeStart();
				}
			}
			else
			{
				//All Quests completed
				Main.npcChatText = $"Hey, I have nothing else for you to do! Thanks for all of your help, have you checked out my shop yet?";
			}
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (!firstButton)
			{
                shop = ShopName;
			}

			if (firstButton)
			{
				//We need to complete current quests before trying to start them otherwise it will break
				bool hasCompletedQuest = CompleteQuests();
                if (!hasCompletedQuest)
                {
					StartQuests();
				}
			
				//Leaving this here incase ya need the text at some point, but it probably won't ever be needed.
				/*
				if (!Main.LocalPlayer.HasItem(ModContent.ItemType<TomeOfInfiniteSorcery>()) || !Main.LocalPlayer.HasItem(ModContent.ItemType<MakeMagicPaperC>()) || !Main.LocalPlayer.HasItem(ModContent.ItemType<Give100DustBagsC>()) || !Main.LocalPlayer.HasItem(ModContent.ItemType<KillVerliaC>()) || !Main.LocalPlayer.HasItem(ModContent.ItemType<ExploreMorrowedVillageC>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
					Main.npcChatText = $"Hey you wanna do a quest? I'll open up my expansive magic shop for you if you do.. I have quite some great goodies in store but I want to become the best witch in all of the Lunar Veil, and I want you to help me make a tome, but first I need you to kill Verlia first for me.";

					var entitySource = NPC.GetSource_GiftOrReward();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<KillVerlia>(), 1);
				}
				*/
			}
		}


		public void ResetTimers()
		{
			timer = 0;
			frameCounter = 0;
			frameTick = 0;
		}

		public override void ModifyActiveShop(string shopName, Item[] items)
		{
			foreach (Item item in items)
			{
				// Skip 'air' items and null items.
				if (item == null || item.type == ItemID.None)
				{
					continue;
				}

				// If NPC is shimmered then reduce all prices by 50%.
				if (NPC.IsShimmerVariant)
				{
					int value = item.shopCustomPrice ?? item.value;
					item.shopCustomPrice = value / 2;
				}
			}
		}


		public override void AddShops()
		{
			var npcShop = new NPCShop(Type, ShopName)
			.Add(new Item(ItemID.Book) { shopCustomPrice = Item.buyPrice(copper: 7) })
            .Add(new Item(ItemID.FallenStar) { shopCustomPrice = Item.buyPrice(silver: 5) })
            .Add(new Item(ItemID.AbigailsFlower) { shopCustomPrice = Item.buyPrice(gold: 1) })
			.Add(new Item(ModContent.ItemType<BurnedCarianTome>()))
			.Add<WickofSorcery>(MerenaQuestSystem.ShopConditionTome)
			.Add<WickofSorcery>(Condition.PlayerCarriesItem(ModContent.ItemType<SewingKit>()))//{ shopCustomPrice = Item.buyPrice(platinum: 1) })
			.Add<PearlescentScrap>(MerenaQuestSystem.ShopConditionKillVerlia)
			.Add<LostScrap>(MerenaQuestSystem.ShopConditionKillVerlia)// { shopCustomPrice = Item.buyPrice(silver: 50) })
            .Add<AlcadBomb>(MerenaQuestSystem.ShopConditionExploreMorrowedVillage)
            .Add<Hyua>(MerenaQuestSystem.ShopConditionExploreMorrowedVillage) //{ shopCustomPrice = Item.buyPrice(silver: 10) })
			.Add<BlossomingScissor>(MerenaQuestSystem.ShopConditionGive100DustBags)
			.Add<Bagitem>(MerenaQuestSystem.ShopConditionGive100DustBags)//{ shopCustomPrice = Item.buyPrice(platinum: 1) })
			.Add<AlcadThrowingCards>(MerenaQuestSystem.ShopConditionMakeMagicPaper)//{ shopCustomPrice = Item.buyPrice(silver: 10) })
			.Add<AlcaricMush>(MerenaQuestSystem.ShopConditionTome); //{ shopCustomPrice = Item.buyPrice(gold: 2) })
			npcShop.Register(); // Name of this shop tab		
		}

		public override void AI()
		{
			timer++;
            NPC.CheckActive();
            NPC.spriteDirection = NPC.direction;
		}
	}
}