using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Igniter;
using Stellamod.Items.Armors.Vanity.Gia;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Whips;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Stellamod.Buffs;
using Stellamod.Items.Accessories.Brooches;

namespace Stellamod.NPCs.Town
{
	// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
	[AutoloadBossHead]
	public class Gia : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;
		public const string ShopName = "Shop";
	
		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 25; // The amount of frames the NPC has

			NPCID.Sets.ExtraFramesCount[Type] = 9; // Generally for Town NPCs, but this is how the NPC does extra things such as sitting in a chair and talking to other NPCs.
			NPCID.Sets.AttackFrameCount[Type] = 4;
			NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the npc that it tries to attack enemies.
			NPCID.Sets.AttackType[Type] = 0;
			NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.


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
			// NOTE: The following code uses chaining - a style that works due to the fact that the SetXAffection methods return the same NPCHappiness instance they're called on.
			NPC.Happiness
				.SetBiomeAffection<OceanBiome>(AffectionLevel.Like) // Example Person prefers the forest.
				.SetBiomeAffection<DesertBiome>(AffectionLevel.Dislike) // Example Person dislikes the snow.
				.SetBiomeAffection<SnowBiome>(AffectionLevel.Love) // Example Person likes the Example Surface Biome
				.SetNPCAffection(NPCID.PartyGirl, AffectionLevel.Love) // Loves living near the dryad.
				.SetNPCAffection(NPCID.Stylist, AffectionLevel.Like) // Likes living near the guide.
				.SetNPCAffection(NPCID.Merchant, AffectionLevel.Dislike) // Dislikes living near the merchant.
				.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate); // Hates living near the demolitionist.

	
			NPCDebuffImmunityData debuffData = new NPCDebuffImmunityData
			{
				SpecificallyImmuneTo = new int[] {
					BuffID.Poisoned,
					BuffID.Burning,
					BuffID.Ichor,
					BuffID.Frostburn,
					BuffID.Confused // Most NPCs have this
				}
			};
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.OnFire3] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Frostburn2] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Ichor] = true;
			NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<AcidFlame>()] = true;
			// < Mind the semicolon!
		}
		
		// Current state

		
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
			  // Sets NPC to be a Town NPC
			NPC.friendly = true; // NPC Will not attack player
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.damage = 90;
			NPC.defense = 420;
			NPC.lifeMax = 5000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			AnimationType = NPCID.Guide;
			NPC.dontTakeDamage = true;

			NPC.BossBar = Main.BigBossProgressBar.NeverValid;
		}


		//This prevents the NPC from despawning
		public override bool CheckActive()
		{
			return false;
		}
		//public override bool CanTownNPCSpawn(int numTownNPCs)
		//{
		//	return AlcadSpawnSystem.TownedGia;
		//}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Wow you done messed up this time with the Virulent spill huh.")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Gia the Scientist", "2"))
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
		public override bool CanChat()
		{
			return true;
		}
		public override string GetChat()
		{
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int partyGirl = NPC.FindFirstNPC(NPCID.Steampunker);
			if (partyGirl >= 0 && Main.rand.NextBool(4))
			{
				chat.Add(LangText.Chat(this, "Basic1", Main.npc[partyGirl].GivenName));
			}
			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(LangText.Chat(this, "Basic2"));
			chat.Add(LangText.Chat(this, "Basic3"));
			chat.Add(LangText.Chat(this, "Basic4"));
			chat.Add(LangText.Chat(this, "Basic5"), 5.0);
			chat.Add(LangText.Chat(this, "Basic6"), 0.4);
			chat.Add(LangText.Chat(this, "Basic7"), 0.1);
			chat.Add(LangText.Chat(this, "Basic8"), 0.1);
			chat.Add(LangText.Chat(this, "Basic9"), 0.1);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 10)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(LangText.Chat(this, "Basic10"));
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

	

		

		public override ITownNPCProfile TownNPCProfile()
		{
			return new GiaPersonProfile();
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				"Gia",
				"Gia",
				"Gia",
				"Gia",
				"Gia"
			};
		}

		
		

		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = LangText.Chat(this, "Button2");
			
		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (firstButton)
			{
				// We want 3 different functionalities for chat buttons, so we use HasItem to change button 1 between a shop and upgrade action.

				//if (Main.LocalPlayer.HasItem(ItemID.HiveBackpack))
				//{
				//	SoundEngine.PlaySound(SoundID.Item37); // Reforge/Anvil sound

				//	Main.npcChatText = $"I upgraded your {Lang.GetItemNameValue(ItemID.HiveBackpack)} to a {Lang.GetItemNameValue(ModContent.ItemType<WaspNest>())}";

				//	int hiveBackpackItemIndex = Main.LocalPlayer.FindItem(ItemID.HiveBackpack);
				//	var entitySource = NPC.GetSource_GiftOrReward();

				//	Main.LocalPlayer.inventory[hiveBackpackItemIndex].TurnToAir();
				//	Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WaspNest>());

				//	return;
				//}

				shop = ShopName;
			}

			if (!firstButton)
            {
				
					Player player = Main.LocalPlayer;
					WeightedRandom<string> chat = new WeightedRandom<string>();
				
				
				
				//-----------------------------------------------------------------------------------------------
				if (Main.LocalPlayer.HasItem(ModContent.ItemType<DesertRuneI>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss1")); // Reforge/Anvil sound

					Main.npcChatText = LangText.Chat(this, "Special1");

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<DesertRuneI>());
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
                    Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 2);
                    switch (Main.rand.Next(7))
					{


						case 0:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 10);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Bagitem>(), 3);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.DesertFossil, 100);
							break;
						case 1:
					

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.DesertFossil, 500);
							break;
						case 2:
						
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<DesertWhip>());
			
							break;

						case 3:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.AntlionMandible, 25);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CondensedDirt>(), 250);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FrileOre>(), 50);

							break;

						case 4:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ArncharChunk>(), 50);

							break;

						case 5:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.LifeCrystal, 2);

							break;

						case 6:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 10);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.SandstorminaBottle, 1);

							break;

						case 7:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Dunderbustion>(), 1);
						

							break;
					}

					

					return;



				}



				if (Main.LocalPlayer.HasItem(ModContent.ItemType<SkyRuneI>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss1")); // Reforge/Anvil sound

					Main.npcChatText = LangText.Chat(this, "Special2");

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<SkyRuneI>());
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
                    Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 3);
                    switch (Main.rand.Next(7))
					{


						case 0:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 10);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Bagitem>(), 3);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Feather, 10);
							break;
						case 1:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Feather, 15);
							break;
						case 2:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<DivineSniper>());

							break;

						case 3:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.AntlionMandible, 25);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CondensedDirt>(), 250);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FrileOre>(), 50);

							break;

						case 4:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<SkyrageShasher>());

							break;

						case 5:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CloudBow>());

							break;

						case 6:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 10);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.CloudinaBalloon, 1);

							break;
					}



					return;



				}


				if (Main.LocalPlayer.HasItem(ModContent.ItemType<IceRuneI>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss1")); // Reforge/Anvil sound

					Main.npcChatText = LangText.Chat(this, "Special3");

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<IceRuneI>());
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
                    Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 1);
                    switch (Main.rand.Next(10))
					{


						case 0:
						
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Bagitem>(), 3);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.IceBlade, 1);
							break;
						case 1:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.FrostDaggerfish, 200);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.SnowballCannon, 1);
							break;
						case 2:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.FrostDaggerfish, 200);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Parendine>(), 1);

							break;

						case 3:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.FrostDaggerfish, 200);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.IceBlock, 25);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CondensedDirt>(), 250);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FrileOre>(), 50);

							break;

						case 4:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.FrostDaggerfish, 200);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<TheFirstAurora>());

							break;

						case 5:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.FrostDaggerfish, 200);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FrileOre>(), 100);

							break;

						case 6:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.BlizzardinaBottle, 1);

							break;
						case 7:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.IceSkates, 1);
							break;

						case 8:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<TwilitTome>(), 1);
							break;

						case 9:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FrileBroochA>(), 1);
							break;

						case 10:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Cryal>(), 1);
							break;
					}



					return;



				}




				if (Main.LocalPlayer.HasItem(ModContent.ItemType<OverworldRuneI>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss1")); // Reforge/Anvil sound

					Main.npcChatText = LangText.Chat(this, "Special4");

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<OverworldRuneI>());
					var entitySource = NPC.GetSource_GiftOrReward();
					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 1);
					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();

					switch (Main.rand.Next(16))
					{


						case 0:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 50);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.IronOre, 100);
							break;
						case 1:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.EmptyBucket, 9);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Bomb, 9);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.LifeCrystal, 1);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.HerbBag, 2);
							break;
						case 2:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.LifeCrystal, 3);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Silk, 5);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Gel, 300);

							break;

						case 3:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.ChainKnife);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CondensedDirt>(), 250);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FrileOre>(), 50);

							break;

						case 4:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ArncharChunk>(), 50);

							break;

						case 5:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.LifeCrystal, 2);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Bagitem>(), 3);
							break;

						case 6:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.HermesBoots, 1);

							break;

						case 7:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.CloudinaBalloon, 1);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<AlcadizScrap>(), 5);

							break;

						case 8:
			
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<VerianOre>(), 50);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Aglet, 1);

							break;


						


						case 9:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<AlcadizScrap>(), 5);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.BandofStarpower, 1);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Bagitem>(), 3);
							break;

						case 10:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Cinderscrap>(), 50);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<wowgun>(), 1);

							break;

						case 11:

				
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Ivythorn>(), 150);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CondensedDirt>(), 150);

							break;

						case 12:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<GildedStaff>(), 1);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Bagitem>(), 3);

							break;

						case 13:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CanOfLeaves>(), 1);


							break;
						case 14:
				
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ViolinStick>(), 1);


							break;

						case 15:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<HornedNail>(), 1);


							break;
					}



					return;



				}









				if (Main.LocalPlayer.HasItem(ModContent.ItemType<OceanRuneI>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss1")); // Reforge/Anvil sound

					Main.npcChatText = LangText.Chat(this, "Special5");

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<OceanRuneI>());
					var entitySource = NPC.GetSource_GiftOrReward();
                    Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 2);
                    Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
					switch (Main.rand.Next(17))
					{
						case 0:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Bagitem>(), 3);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Coral, 100);
							break;

						case 1:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.SharkFin, 5);
							break;

						case 2:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<OceansTrinket>());
							break;

						case 3:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Seashell, 25);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CondensedDirt>(), 250);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FrileOre>(), 50);
							break;

						case 4:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<OceanScroll>(), 1);
							break;

						case 5:
		
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.LifeCrystal, 2);
							break;

						case 6:
							
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.TsunamiInABottle, 1);
							break;

						case 7:
							if (Main.hardMode)
							{
								Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<BubbleBlasher>(), 1);
							}
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.MasterBait, 10);
							break;

						case 8:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Sextant, 1);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.SharkFin, 5);
							break;

						case 9:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.WeatherRadio, 1);
							break;

						case 10:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.BlackInk, 3);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<TorrentialLance>(), 1);
							break;

						case 11:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<OceanicExtenderPowder>(), 1);
							break;

						case 12:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<AquaticHealingNecklace>(), 1);
							break;

						case 13:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<AquaCrystal>(), 1);
							break;

						case 14:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CoralBand>(), 1);
							break;

						case 15:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ShellSheid>(), 1);
							break;

						case 16:
                            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<PalmThrower>());
                            break;
					}



					return;



				}






				if (Main.LocalPlayer.HasItem(ModContent.ItemType<JungleRuneI>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss1")); // Reforge/Anvil sound

					Main.npcChatText = LangText.Chat(this, "Special6");

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<JungleRuneI>());
					var entitySource = NPC.GetSource_GiftOrReward();
                    Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 2);
                    Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
					switch (Main.rand.Next(16))
					{


						case 0:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.IronOre, 100);
							break;
						case 1:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.EmptyBucket, 9);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Bomb, 9);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.LifeCrystal, 1);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.HerbBag, 2);
							break;
						case 2:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.LifeCrystal, 3);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Silk, 5);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Stinger, 30);

							break;

						case 3:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 50);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<VerstiDance>());
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FrileOre>(), 150);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Bagitem>(), 3);

							break;

						case 4:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ArncharChunk>(), 50);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.Stinger, 30);
							break;

						case 5:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.LifeCrystal, 2);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Bagitem>(), 3);
							break;

						case 6:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.FlowerBoots, 1);

							break;

						case 7:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.JungleRose, 1);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<AlcadizScrap>(), 5);

							break;

						case 8:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<VerianOre>(), 50);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.AnkletoftheWind, 1);

							break;





						case 9:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
						
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<AlcadizScrap>(), 5);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.JungleSpores, 30);

							break;

						case 10:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Cinderscrap>(), 50);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.JungleSpores, 30);

							break;

						case 11:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mushroom>(), 15);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Ivythorn>(), 150);
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CondensedDirt>(), 150);

							break;

						case 12:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.JungleSpores, 10);

							break;

						case 13:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<SwordOfTheJungleGoddess>(), 1);


							break;
						case 14:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ViolinStick>(), 1);


							break;

						case 15:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<MahoganyStorm>(), 1);


							break;
					}



					return;



				}
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


			.Add<EDR>(Condition.DownedMechBossAny)
			.Add(new Item(ModContent.ItemType<GiaWig>()) { shopCustomPrice = Item.buyPrice(gold: 10) })
			.Add(new Item(ModContent.ItemType<GiaSuit>()) { shopCustomPrice = Item.buyPrice(gold: 10) })
			.Add(new Item(ModContent.ItemType<GiaPants>()) { shopCustomPrice = Item.buyPrice(gold: 10) })
			.Add(new Item(ModContent.ItemType<DustedSilk>()) { shopCustomPrice = Item.buyPrice(silver: 10) })
			.Add(new Item(ItemID.WormFood) { shopCustomPrice = Item.buyPrice(gold: 5) })
			.Add(new Item(ItemID.BloodySpine) { shopCustomPrice = Item.buyPrice(gold: 5) })
			.Add(new Item(ItemID.BloodMoonStarter) { shopCustomPrice = Item.buyPrice(gold: 10)})
			.Add(new Item(ItemID.SuspiciousLookingEye) { shopCustomPrice = Item.buyPrice(gold: 1) })
			.Add(new Item(ItemID.SlimeCrown) { shopCustomPrice = Item.buyPrice(gold: 1) })
			.Add(new Item(ItemID.GuideVoodooDoll) { shopCustomPrice = Item.buyPrice(gold: 10) })
			.Add(new Item(ItemID.GoblinBattleStandard) { shopCustomPrice = Item.buyPrice(gold: 10) })
				;
				
				
			
				npcShop.Register(); // Name of this shop tab
		}





	






	
				//	else if (Main.moonPhase < 4) {
				// shop.item[nextSlot++].SetDefaults(ItemType<ExampleGun>());
				//		shop.item[nextSlot].SetDefaults(ItemType<ExampleBullet>());
				//	}
				//	else if (Main.moonPhase < 6) {
				// shop.item[nextSlot++].SetDefaults(ItemType<ExampleStaff>());
				// 	}
				//
				// 	// todo: Here is an example of how your npc can sell items from other mods.
				// 	// var modSummonersAssociation = ModLoader.TryGetMod("SummonersAssociation");
				// 	// if (ModLoader.TryGetMod("SummonersAssociation", out Mod modSummonersAssociation)) {
				// 	// 	shop.item[nextSlot].SetDefaults(modSummonersAssociation.ItemType("BloodTalisman"));
				// 	// 	nextSlot++;
				// 	// }
				//
				// 	// if (!Main.LocalPlayer.GetModPlayer<ExamplePlayer>().examplePersonGiftReceived && GetInstance<ExampleConfigServer>().ExamplePersonFreeGiftList != null) {
				// 	// 	foreach (var item in GetInstance<ExampleConfigServer>().ExamplePersonFreeGiftList) {
				// 	// 		if (Item.IsUnloaded) continue;
				// 	// 		shop.item[nextSlot].SetDefaults(Item.Type);
				// 	// 		shop.item[nextSlot].shopCustomPrice = 0;
				// 	// 		shop.item[nextSlot].GetGlobalItem<ExampleInstancedGlobalItem>().examplePersonFreeGift = true;
				// 	// 		nextSlot++;
				// 	// 		//TODO: Have tModLoader handle index issues.
				// 	// 	}
				// 	// }
				// }







			}


public class GiaPersonProfile : ITownNPCProfile
	{
		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return ModContent.Request<Texture2D>("Stellamod/NPCs/Town/Gia");

			if (npc.altTexture == 1)
				return ModContent.Request<Texture2D>("Stellamod/NPCs/Town/Gia_Party");

			return ModContent.Request<Texture2D>("Stellamod/NPCs/Town/Gia");
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Stellamod/NPCs/Town/Gia_Head");
	}
}