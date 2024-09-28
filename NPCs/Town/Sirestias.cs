using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets.Biomes;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Vanity.Gia;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Items.Placeable;
using Stellamod.Items.Quest.BORDOC;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Mage.Stein;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Melee.Greatswords.INY;
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Summon.Orbs;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Items.Weapons.Thrown.Jugglers;
using Stellamod.Items.Weapons.Whips;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace Stellamod.NPCs.Town
{
	// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
	//[AutoloadHead]
	[AutoloadBossHead]
	public class Sirestias : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;
		public const string ShopName = "Shop";
		public const string ShopName2 = "New Shop";
		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 60; // The amount of frames the NPC has

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





			; // < Mind the semicolon!
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
			NPC.width = 48;
			NPC.height = 39;
			NPC.aiStyle = -1;
			NPC.damage = 9000;
			NPC.defense = 69;
			NPC.lifeMax = 200000;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			NPC.dontTakeDamage = true;
			NPC.BossBar = Main.BigBossProgressBar.NeverValid;
		}


		//This prevents the NPC from despawning
		public override bool CheckActive()
		{
			return false;
		}

		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.5f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}



		public override ITownNPCProfile TownNPCProfile()
		{
			return new DelgrimPersonProfile();
		}

		public class DelgrimPersonProfile : ITownNPCProfile
		{
			public int RollVariation() => 0;
			public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

			public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
			{
				if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
					return ModContent.Request<Texture2D>("Stellamod/NPCs/Town/Sirestias");

				if (npc.altTexture == 1)
					return ModContent.Request<Texture2D>("Stellamod/NPCs/Town/Sirestias_Head");

				return ModContent.Request<Texture2D>("Stellamod/NPCs/Town/Sirestias");
			}

			public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Stellamod/NPCs/Town/Sirestias_Head");
		}

		public override bool CanChat()
		{
			return true;
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Your eternal bonding with this individual resonates with everyone throughout!")),
				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(NPC.FullName)
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


				// Replace the existing NPCBestiaryDrawModifiers with our new one with an adjusted rotation
				NPCID.Sets.NPCBestiaryDrawOffset.Remove(Type);
				NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
			}

			return true;

		}
		public override string GetChat()
		{
			WeightedRandom<string> chat = new WeightedRandom<string>();

			int partyGirl = NPC.FindFirstNPC(NPCID.Steampunker);

			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(LangText.Chat(this, "Basic1"));
			chat.Add(LangText.Chat(this, "Basic2"));
			chat.Add(LangText.Chat(this, "Basic3"));
			chat.Add(LangText.Chat(this, "Basic4"));
			chat.Add(LangText.Chat(this, "Basic5"));


			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 40)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add("...");
			}

			return chat; // chat is implicitly cast to a string.
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldCoin);
			}
		}






		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				"Sirestias",
				"Sirestias"

			};
		}




		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button2 = LangText.Chat(this, "Button");
			button = LangText.Chat(this,  "Button2");

		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (!firstButton)
			{

				Player player = Main.LocalPlayer;
				WeightedRandom<string> chat = new WeightedRandom<string>();



				//-----------------------------------------------------------------------------------------------
				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss1"));

				if (!DownedBossSystem.downedGintzlBoss)
				{
					Main.npcChatText = LangText.Chat(this, "Downed1");


				}
				if (DownedBossSystem.downedGintzlBoss)
				{

					Main.npcChatText = LangText.Chat(this, "Downed2");


					if (DownedBossSystem.downedSunsBoss)
					{

						Main.npcChatText = LangText.Chat(this, "Downed3");



					}


					if (DownedBossSystem.downedJackBoss)
					{

						Main.npcChatText = LangText.Chat(this, "Downed4");



					}

					if (DownedBossSystem.downedJackBoss && DownedBossSystem.downedSunsBoss)
					{

						Main.npcChatText = LangText.Chat(this, "Downed5");


						if (DownedBossSystem.downedDaedusBoss)
						{

							Main.npcChatText = LangText.Chat(this, "Downed6");




							if (DownedBossSystem.downedDreadBoss)
							{

								Main.npcChatText = LangText.Chat(this, "Downed7");

								if (DownedBossSystem.downedSOMBoss)
								{

									Main.npcChatText = LangText.Chat(this, "Downed8");




									if (DownedBossSystem.downedVeriBoss)
									{

										Main.npcChatText = LangText.Chat(this, "Downed9");


										if (Main.hardMode)
										{
											Main.npcChatText = LangText.Chat(this, "Downed10");



											if (DownedBossSystem.downedIrradiaBoss)
											{

												Main.npcChatText = LangText.Chat(this, "Downed11");



												if (DownedBossSystem.downedSTARBoss)
												{

													Main.npcChatText = LangText.Chat(this, "Downed12");



													if (DownedBossSystem.downedSyliaBoss)
													{

														Main.npcChatText = LangText.Chat(this, "Downed13");

														if (DownedBossSystem.downedZuiBoss)
														{

															Main.npcChatText = LangText.Chat(this, "Downed14");

                                                            if (DownedBossSystem.downedSupernovaFragmentBoss)
                                                            {

                                                                Main.npcChatText = LangText.Chat(this, "Downed15");



                                                                if (DownedBossSystem.downedFenixBoss)
                                                                {

                                                                    Main.npcChatText = LangText.Chat(this, "Downed16");


                                                                    if (DownedBossSystem.downedRekBoss)
                                                                    {

                                                                        Main.npcChatText = LangText.Chat(this, "Downed17");


                                                                        if (DownedBossSystem.downedNiiviBoss)
                                                                        {

                                                                            Main.npcChatText = LangText.Chat(this, "Downed18");



                                                                            if (DownedBossSystem.downedGothBoss)
                                                                            {

                                                                                Main.npcChatText = LangText.Chat(this, "Downed19");





                                                                            }

                                                                        }


                                                                    }


                                                                   

                                                                }



                                                            }

                                                           
														}
													}

												}
											}
										}
									}
								}

							}
						}
					}

					return;



				}

























			}

			if (firstButton)
			{

				Player player = Main.LocalPlayer;
				WeightedRandom<string> chat = new WeightedRandom<string>();

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2"));



				if (Main.LocalPlayer.HasItem(ModContent.ItemType<CompletionIdol>()) && DownedBossSystem.downedDreadBoss)
				{

					Main.npcChatText = LangText.Chat(this, "Special1");

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletionIdol>());
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();

					CombatText.NewText(NPC.getRect(), Color.White, LangText.Chat(this, "Special2"), true, false);
					Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 16f);
					for (int i = 0; i < 4; i++)
					{
						Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.SilverCoin);
					}
					for (int i = 0; i < 14; i++)
					{

						Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Silver, 1f).noGravity = true;
					}

					for (int i = 0; i < 14; i++)
					{
						Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Silver, 1f).noGravity = true;
					}

					Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<SirestiasToken>(), 1);

					if (DownedBossSystem.downedSupernovaFragmentBoss)
					{

                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<SineSire>(), 1);

                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 30);


                                break;

                            case 1:
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RavestBlast>(), 1);
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 30);
                                break;

                            case 2:
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<IshNYire>(), 1);
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 30);
                                break;

                        }
                    }
					else if (DownedBossSystem.downedIrradiaBoss)
					{

						switch (Main.rand.Next(3))
						{
							case 0:
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 15);
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<StickyCards>(), 1);
								break;

							case 1:
								Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Mordred>(), 1);
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 15);
                                break;

							case 2:
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<SirestiasMask>(), 1);
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 15);
                                break;

						}
					}
					else if (DownedBossSystem.downedDreadBoss)
					{

                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Maelstrom>(), 1);
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 15);

                                break;

                            case 1:
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ReflectionSeeker>(), 1);
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 15);

                                break;

                            case 2:
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<SirestiasToken>(), 1);
                                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 15);
                                Main.npcChatText = LangText.Chat(this, "Special3");
                                break;

                        }


					}
						

					








					
                    else
					{


						Main.npcChatText = LangText.Chat(this, "Special4");
					}





				}
				

				// Reforge/Anvil sound







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





}