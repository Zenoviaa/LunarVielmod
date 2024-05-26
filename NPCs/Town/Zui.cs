﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets.Biomes;
using Stellamod.Items.Accessories;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Quest.Zui;
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
using Stellamod.Items.Armors.Witchen;
using Stellamod.Items.Consumables;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Armors.Vanity.Solarian;
using Stellamod.Items.Armors.Vanity.Azalean;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.Zui;
using Stellamod.Items.Armors.Vanity.Nyxia;
using Terraria.DataStructures;
using Stellamod.Items.Ammo;
using Stellamod.Items.Weapons.Ranged.GunSwapping;

namespace Stellamod.NPCs.Town
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    // [AutoloadHead]
	public class Zui : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;
		public const string ShopName = "Shop";
		public const string ShopName2 = "New Shop";
		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 4; // The amount of frames the NPC has

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
			NPC.width = 54;
			NPC.height = 65;
			NPC.aiStyle = 0;
			NPC.damage = 90;
			NPC.defense = 42;
			NPC.lifeMax = 2000;
			NPC.npcSlots = 0;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			NPC.dontTakeDamageFromHostiles = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
        }

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.07f;
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
				new FlavorTextBestiaryInfoElement("A traveller of the lands who may hold great power"),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Zui the Traveller")
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

			int partyGirl = NPC.FindFirstNPC(NPCID.Dryad);
			if (partyGirl >= 0 && Main.rand.NextBool(4))
			{
				chat.Add(Language.GetTextValue("The dryad is pretty fun after all, she's pretty sweet and helps us with the gardening!", Main.npc[partyGirl].GivenName));
			}
			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(Language.GetTextValue("We've had to kick a few witches out of this town you know, its quite a shame.."));
			chat.Add(Language.GetTextValue("I keep going out to find more plants but somehow you're always here when I return. "));
			chat.Add(Language.GetTextValue("I'm trying to find a way to stop the corruption and virulent from spreading but I have no fix currently."));
			chat.Add(Language.GetTextValue("Hey love, you know you could be collecting stuff for me!"), 1.0);
			chat.Add(Language.GetTextValue("Sirestias? Oh um yeahh we go pretty far back."), 0.4);
			chat.Add(Language.GetTextValue("Aimacra is cool too, this is my first time meeting her lol"), 0.5);
			chat.Add(Language.GetTextValue("I wouldn't say I'm too powerful, afterall I a just a traveller"), 0.1);
			chat.Add(Language.GetTextValue("Sometimes I wonder what they did in Vodhome after we all left. I really have to catch up with Sirestias and the others."), 0.1);
			chat.Add(Language.GetTextValue("Are you here with Sirestias?"), 0.1);
			chat.Add(Language.GetTextValue("All these plants are so cool! I did just arrive here before Sirestias, there was some commotion and I think people here are trying to disrupt time and power."), 0.5);
			chat.Add(Language.GetTextValue("Sirestias is a balancer, same goes for me but I like to just collect things."), 0.1);
			chat.Add(Language.GetTextValue("Hey if you collect some items for me I can give you some stuff!"), 2.0);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 10)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(Language.GetTextValue("Hey can you go collect for me?"));
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
				"Zui The Traveller",
				"Zui The Traveller"
				
			};
		}


		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button2 = Language.GetTextValue("LegacyInterface.28");
			button = "Zui's Quests";

		}

        private void Quest_NotCheckmarked()
        {
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"Hi hii! Thanks for asking! Could you fetch me some plants, you can use this bag! Just fill it up with some different types of plants and I'll give you some stuff afterwards!";
			var entitySource = NPC.GetSource_GiftOrReward();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<EmptyFlowerBag>(), 1);
		}

		private void Quest_NotCheckmarkedHardmode()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"Hehe I'm gonna up the difficulty of these, I'm gonna need some bigger plants, so you'll need to fill up this bag! Thanks!";
			var entitySource = NPC.GetSource_GiftOrReward();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<EmptyCollectorsBag>(), 1);
		}

		private void Quest_1Complete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"Nice nice, I'll take these, could you fetch some more for me!  ";
			var entitySource = NPC.GetSource_GiftOrReward();
			if (Main.rand.NextBool(1))
			{
				Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.HealingPotion, 7);
			}

			if (Main.rand.NextBool(3))
			{
				Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.GreaterHealingPotion, 7);
			}

			if (Main.rand.NextBool(5))
			{
				Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.SuperHealingPotion, 5);
			}

			if (Main.rand.NextBool(1))
			{
				Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Medal>(), 9);
			}

            ZuiQuestSystem.CompleteQuest();
            if (ZuiQuestSystem.QuestsCompleted == 1)
			{

				Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CanOfLeaves>(), 1);

			}

            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 2);

            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedFlowerBag>());
			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
			//Setting all previous quests to be complete, so it's backwards compatible with the old version.

		}

		private void Quest_16Complete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"Nice nice, I'll take these, could you fetch some more for me!  ";
			var entitySource = NPC.GetSource_GiftOrReward();
			if (Main.rand.NextBool(1))
			{
				Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.HealingPotion, 10);
			}

			if (Main.rand.NextBool(3))
			{
				Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.GreaterHealingPotion, 15);
			}

			if (Main.rand.NextBool(5))
			{
				Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.SuperHealingPotion, 10);
			}

			if (Main.rand.NextBool(1))
			{
				Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Medal>(), 18);
				Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<LiliumArrow>(), 250);
			}

			if (ZuiQuestSystem.QuestsCompleted == 15)
			{
				
				Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FungalFlace>(), 1);

			}

            ZuiQuestSystem.CompleteQuest();
            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), Main.rand.Next(3));

			int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedCollectorsBag>());
			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
			//Setting all previous quests to be complete, so it's backwards compatible with the old version.

		}
		private void Quest_3Complete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"Ok ok, 3 quests is kind of good, thanks for caring a lot about this! I've got more items in my shop for you, bring me 3 more and I'll get more for ya!";
			//Setting all previous quests to be complete, so it's backwards compatible with the old version.
            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedFlowerBag>());

			var entitySource = NPC.GetSource_GiftOrReward();
			Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<TomeofRaining>(), 1);


			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            ZuiQuestSystem.CompleteQuest();
        }

		private void Quest_6Complete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"6 Quests is actually kind of crazy, I've opened up with a few more items for you if you want! Bring me 4 more ;p";

			//Setting all previous quests to be complete, so it's backwards compatible with the old version.
            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedFlowerBag>());
			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            ZuiQuestSystem.CompleteQuest();
        }
		private void Quest_10Complete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"Damn, 10 quests? You're on a role you know! I've got some more items for you in my shop actually, I went travelling and Sirestias gave me a few items! I'll give you some more items if you can do 10 more!";

            var entitySource = NPC.GetSource_GiftOrReward();
            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Hookarama>(), 1);

            //Setting all previous quests to be complete, so it's backwards compatible with the old version.
            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedFlowerBag>());
			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            ZuiQuestSystem.CompleteQuest();
        }

		private void Quest_20Complete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"You have finished 20 quests for me! Thank you! I've got more items in my shop because of it too! If you want more from me bring me ten more and maybe we can have some fun! ";


			//Setting all previous quests to be complete, so it's backwards compatible with the old version.
            var entitySource = NPC.GetSource_GiftOrReward();
            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CarrotPatrol>(), 1);


            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedCollectorsBag>());
			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            ZuiQuestSystem.CompleteQuest();
        }

		private void Quest_30Complete()
		{
			SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
			Main.npcChatText = $"My graditude is of the utmost thanks, in return you can have anything in my shop! And I'll help you out personally sometime if you need me ;p ";
			int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedCollectorsBag>());
			Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();

            ZuiQuestSystem.CompleteQuest();
            if (ZuiQuestSystem.QuestsCompleted == 30)
			{

				var entitySource = NPC.GetSource_GiftOrReward();
				Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<SirestiasToken>(), 1);
            
            }

            //Setting all previous quests to be complete, so it's backwards compatible with the old version.

        }

		private bool CompleteQuests()
		{
			Player player = Main.LocalPlayer;
	
			if (ZuiQuestSystem.QuestsCompleted == 2 && player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
			{
				Quest_3Complete();
				return true;
			} 
			else if (ZuiQuestSystem.QuestsCompleted == 5 && player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
			{
				Quest_6Complete();
				return true;
			}
			else if (ZuiQuestSystem.QuestsCompleted == 9 && player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
			{
				Quest_10Complete();
				return true;
			}
			else if (ZuiQuestSystem.QuestsCompleted == 19 && player.HasItem(ModContent.ItemType<CompletedCollectorsBag>()))
			{
				Quest_20Complete();
				return true;
			}
			else if (ZuiQuestSystem.QuestsCompleted == 29 && player.HasItem(ModContent.ItemType<CompletedCollectorsBag>()))
			{
				Quest_30Complete();
				return true;
			}
			else if (player.HasItem(ModContent.ItemType<CompletedFlowerBag>()) && ZuiQuestSystem.QuestsCompleted != 29 && ZuiQuestSystem.QuestsCompleted != 19 && ZuiQuestSystem.QuestsCompleted != 9 && ZuiQuestSystem.QuestsCompleted != 5 && ZuiQuestSystem.QuestsCompleted != 2  && ZuiQuestSystem.QuestsCompleted < 10)
			{
				Quest_1Complete();
				return true;
			}

			else if (player.HasItem(ModContent.ItemType<CompletedCollectorsBag>()) && ZuiQuestSystem.QuestsCompleted != 29 && ZuiQuestSystem.QuestsCompleted != 19 && ZuiQuestSystem.QuestsCompleted != 9 && ZuiQuestSystem.QuestsCompleted != 5 && ZuiQuestSystem.QuestsCompleted != 2  && ZuiQuestSystem.QuestsCompleted >= 10)
			{
				Quest_16Complete();
				return true;
			}


			return false;
		}

		private void StartQuests()
        {
			Player player = Main.LocalPlayer;

			//Go through the list of quests in a specific order and see if any need to be started
			if (ZuiQuestSystem.QuestsCompleted < 10 && !player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
			{
				Quest_NotCheckmarked();
			}

			if (ZuiQuestSystem.QuestsCompleted >= 10 && ZuiQuestSystem.QuestsCompleted < 30 && !player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
			{
				Quest_NotCheckmarkedHardmode();
			}
			else if (ZuiQuestSystem.QuestsCompleted >= 30)
			{
				//All Quests completed
				Main.npcChatText = $"Hiya! I think that's all the things I need, you can get some stuff in my shop but thanks babe!";
			}
		}
		public override void AI()
		{
			timer++;
			NPC.CheckActive();
			NPC.spriteDirection = NPC.direction;
			if (NPC.AnyNPCs(ModContent.NPCType<ZuiTheTraveller>()))
			{

				NPC.Kill();
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
			.Add<RadianceStone>(Condition.DownedPlantera)
			.Add(new Item(ItemID.Bottle) { shopCustomPrice = Item.buyPrice(copper: 50) })
			.Add(new Item(ItemID.JungleRose) { shopCustomPrice = Item.buyPrice(gold: 1) })
			.Add<IceClimbers>()
			.Add<FloweredCard>()
			.Add<ZenoviasPikpikGlove>()
			.Add<NyxiaHat>()
			.Add<NyxiaRobe>()
			.Add<NyxiaThighs>()
			.Add<SolarianHat>()
			.Add<SolarianChestplate>()
			.Add<SolarianPants>()
			.Add<AzaleanHat>()
			.Add<AzaleanChestplate>()
			.Add<AzaleanLeggings>()

			.Add<PerfectionStaff>(ZuiQuestSystem.ShopCondition3)
			.Add<AquaCrystal>(ZuiQuestSystem.ShopCondition3)
			.Add<Morrowshroom>(ZuiQuestSystem.ShopCondition3)
			.Add<SunClaw>(ZuiQuestSystem.ShopCondition3)
			.Add<OnionOfHeight>(ZuiQuestSystem.ShopCondition3)
			.Add(new Item(ItemID.NaturesGift) { shopCustomPrice = Item.buyPrice(gold: 1) }, (ZuiQuestSystem.ShopCondition3))


			.Add<DriveConstruct>(ZuiQuestSystem.ShopCondition6)
			.Add(new Item(ItemID.LuckyHorseshoe) { shopCustomPrice = Item.buyPrice(gold: 15) }, (ZuiQuestSystem.ShopCondition6))
			.Add(new Item(ItemID.CloudinaBalloon) { shopCustomPrice = Item.buyPrice(gold: 25) }, (ZuiQuestSystem.ShopCondition6))
			.Add(new Item(ItemID.Gladius) { shopCustomPrice = Item.buyPrice(gold: 5) }, (ZuiQuestSystem.ShopCondition6))
			//{ shopCustomPrice = Item.buyPrice(platinum: 1) })

			.Add<OnionOfUselessness>(ZuiQuestSystem.ShopCondition10)
			.Add(new Item(ItemID.BundleofBalloons) { shopCustomPrice = Item.buyPrice(gold: 65) }, (ZuiQuestSystem.ShopCondition10))
			.Add(new Item(ItemID.CobaltShield) { shopCustomPrice = Item.buyPrice(gold: 80) }, (ZuiQuestSystem.ShopCondition10))
			.Add(new Item(ItemID.Obsidian) { shopCustomPrice = Item.buyPrice(silver: 4) }, (ZuiQuestSystem.ShopCondition10))

			.Add<OnionOfSight>(ZuiQuestSystem.ShopCondition20)
			.Add<WitchenHat>(ZuiQuestSystem.ShopCondition20)
			.Add<WitchenRobe>(ZuiQuestSystem.ShopCondition20)
			.Add<WitchenPants>(ZuiQuestSystem.ShopCondition20)
			.Add<EckasectSire>(ZuiQuestSystem.ShopCondition20)
            .Add<TornCarianPage>(ZuiQuestSystem.ShopCondition20)

            .Add<ChromaCutter>(ZuiQuestSystem.ShopCondition30)
			.Add<OnionOfStrength>(ZuiQuestSystem.ShopCondition30)
			.Add<ZuiCard>(ZuiQuestSystem.ShopCondition30)
			.Add<FocusingCrystal>(ZuiQuestSystem.ShopCondition30)
			;
			npcShop.Register(); // Name of this shop tab		
		}

		
	}
}