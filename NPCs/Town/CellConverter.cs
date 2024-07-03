using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets.Biomes;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Items;
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
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Items.Weapons.Whips;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
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

	public class CellConverter : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;
		public const string ShopName = "Shop";
		public const string ShopName2 = "New Shop";
		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 30; // The amount of frames the NPC has

			NPCID.Sets.ActsLikeTownNPC[Type] = true;

			//To reiterate, since this NPC isn't technically a town NPC, we need to tell the game that we still want this NPC to have a custom/randomized name when they spawn.
			//In order to do this, we simply make this hook return true, which will make the game call the TownNPCName method when spawning the NPC to determine the NPC's name.
			NPCID.Sets.SpawnsWithCustomName[Type] = true;


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
			NPC.width = 63;
			NPC.height = 56;
			NPC.aiStyle = -1;
			NPC.damage = 90;
			NPC.defense = 42;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			NPC.dontTakeDamageFromHostiles = true;


		}
		public override void FindFrame(int frameHeight)
		{
			NPC.frameCounter += 0.50f;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			int frame = (int)NPC.frameCounter;
			NPC.frame.Y = frame * frameHeight;
		}



        public override bool CheckActive()
        {
			return false;
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Burning in a magical laboratory")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "The Cell converter", "2"))
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


			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(LangText.Chat(this, "Basic1"));
			chat.Add(LangText.Chat(this, "Basic2"));
			chat.Add(LangText.Chat(this, "Basic3"));

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 20)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(LangText.Chat(this, "Basic4"));
			}

			return chat; // chat is implicitly cast to a string.
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GoldFlame);
			}
		}





		int summonItem = ModContent.ItemType<Items.Consumables.ScrapToken>();
		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				"Cell Converter",
				"Cell Converter"

			};
		}




		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button = LangText.Chat(this, "Button");

		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{	
			if (firstButton)
			{
				Player player = Main.LocalPlayer;
				WeightedRandom<string> chat = new WeightedRandom<string>();

				//Item converting system
				if (player.HasItem(ModContent.ItemType<SirestiasToken>()) 
					&& player.HeldItem.ModItem is ClassSwapItem swapItem)
				{
					//Swap the item
					player.HeldItem.DamageType = swapItem.AlternateClass;
					swapItem.IsSwapped = true;
					swapItem.SetClassSwappedDefaults();

					//Delete the item
                    int itemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<SirestiasToken>());
                    Main.LocalPlayer.inventory[itemIndex].TurnToAir();
					NetMessage.SendData(MessageID.SyncItem);

					//Effects
                    SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Converted"));
					CombatText.NewText(NPC.getRect(), Color.White, LangText.Chat(this, "Special0"), true, false);
					Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(NPC.Center, 1024f, 16f);
                    for (int i = 0; i < 4; i++)
                    {
						Dust.NewDust(NPC.Center, NPC.width, NPC.height, DustID.Electric);
                    }
                    for (int i = 0; i < 14; i++)
                    {

                        Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
                    }

                    for (int i = 0; i < 14; i++)
                    {
                        Dust.NewDustPerfect(NPC.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
                    }
                } 
				else if (Main.LocalPlayer.HasItem(ModContent.ItemType<ScrapToken>()))
				{
					SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Converted")); // Reforge/Anvil sound

					Main.npcChatText = LangText.Chat(this, "Special1");

					int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<ScrapToken>());
					var entitySource = NPC.GetSource_GiftOrReward();

					Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
					switch (Main.rand.Next(20))
					{


						case 0:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<BrokenTech>(), 4);
							break;
						case 1:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<UnknownCircuitry>(), 10);
							break;
						case 2:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<BrokenTech>(), 1);

							break;

						case 3:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<DriveConstruct>(), 4);

							break;

						case 4:

							CombatText.NewText(NPC.getRect(), Color.White, LangText.Chat(this, "Special2"), true, false);

							break;

						case 5:

							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ArmorDrive>(), 1);

							break;

						case 6:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WeaponDrive>(), 2);

							break;

						case 7:
							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WeaponDrive>(), 2);

							break;
						case 8:

							CombatText.NewText(NPC.getRect(), Color.White, LangText.Chat(this, "Special3"), true, false);

							break;

						case 9:

							CombatText.NewText(NPC.getRect(), Color.White, LangText.Chat(this, "Special4"), true, false);

							break;

						case 10:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<DriveConstruct>(), 9);
							break;

						case 11:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<BrokenTech>(), 6);
							break;

						case 12:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<DriveConstruct>(), 2);
							break;

						case 13:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<UnknownCircuitry>(), 1);
							break;

						case 14:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<BrokenTech>(), 1);
							break;

						case 15:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<UnknownCircuitry>(), 1);
							break;

						case 16:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<ElectricCrystal>(), 1);
							break;

						case 17:


							Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<WeaponDrive>(), 3);
							break;

						case 18:

							CombatText.NewText(NPC.getRect(), Color.White, LangText.Chat(this, "Special4"), true, false);

							break;

						case 19:

							CombatText.NewText(NPC.getRect(), Color.White, LangText.Chat(this, "Special4"), true, false);

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