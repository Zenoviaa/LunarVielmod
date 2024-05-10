using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets.Biomes;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Vanity.Gia;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Placeable;
using Stellamod.Items.Quest.BORDOC;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Mage.Stein;
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

	public class Ordin : ModNPC
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
			NPC.width = 103;
			NPC.height = 116;
			NPC.aiStyle = -1;
			NPC.damage = 90;
			NPC.defense = 42;
			NPC.lifeMax = 200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;
			NPC.dontTakeDamageFromHostiles = true;
			NPC.scale = 2;


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
				new FlavorTextBestiaryInfoElement("Steaming from the depths"),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Ordin, The New Monarch")
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

			int partyGirl = NPC.FindFirstNPC(NPCID.Steampunker);
			if (partyGirl >= 0 && Main.rand.NextBool(4))
			{
				chat.Add(Language.GetTextValue("Shes got some great cogs", Main.npc[partyGirl].GivenName));
			}
			// These are things that the NPC has a chance of telling you when you talk to it.
			chat.Add(Language.GetTextValue("BRMMMM"));
			chat.Add(Language.GetTextValue("Everyone always uses me but never asked how I'm able to talk.. "));
			chat.Add(Language.GetTextValue("I don't care I need to gamble"));
			chat.Add(Language.GetTextValue("My armors are the best in the lands"), 5.0);
			chat.Add(Language.GetTextValue("Sirestias and Aimacra game by recently and they laughed at me because they thought my shop was useless"), 2.0);
			chat.Add(Language.GetTextValue("Im always feeling hot!"), 0.1);
			chat.Add(Language.GetTextValue("Heh, nobody is as good as me"), 0.1);
			chat.Add(Language.GetTextValue("Burning Burning, heat and heat"), 0.1);
			chat.Add(Language.GetTextValue("I'm inpenetrable"), 0.1);
			chat.Add(Language.GetTextValue("Damn we have some hot shit here"), 0.1);
			chat.Add(Language.GetTextValue("Only I know that Gothivia has so many barriers between her and the rest of the world, nobody will be able to kill her."), 0.1);
			chat.Add(Language.GetTextValue("Those idiot gintze have made a horrible decision, now they can't use my armors I made for them."), 0.1);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 20)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(Language.GetTextValue("KRMMMKTEYYYMMMM BRMMMMM"));
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






		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				"Ordin, Illurian Monarch",
                "Ordin, Illurian Monarch"

            };
		}




		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button2 = Language.GetTextValue("LegacyInterface.28");
			button = "Reminisce of Sigfried";

		}

		public override void OnChatButtonClicked(bool firstButton, ref string shop)
		{
			if (!firstButton)
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

			if (firstButton)
			{

				Player player = Main.LocalPlayer;
				WeightedRandom<string> chat = new WeightedRandom<string>();



				//-----------------------------------------------------------------------------------------------	




				switch (Main.rand.Next(10))
				{
					case 0:
						SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound

						Main.npcChatText = $"Thy is so resilient, unhand me from these this trecherous nagging and take take this for thy shall not be astute to your prescence no longer.";

						var entitySource = NPC.GetSource_GiftOrReward();
						Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<SigfriedsPhotoAlbum>(), 1);



						break;

					case 1:
						Main.npcChatText = $"To be one of the forth or forward, the lands that I rest in reek of falsification of hypocrisy and power.";

						break;

					case 2:
						Main.npcChatText = $"Sigfried was once a young fellow, thy brother in one's eye's and a villian in anothers'.";

						break;

					case 3:
						Main.npcChatText = $"Our sacred castle falls ill to such burning. Our wishes have been grafted under the curse of Sigfried's unveiling.";

						break;

					case 4:
						Main.npcChatText = $"Fall away, dear brother ";

						break;

					case 5:
						Main.npcChatText = $"Mark thy words with caution. Ereshkigal is a loveless harlet who took the life of the brother of many. Yet thy fulfills their prophecy among lovers, hence she is not a false goddess.";

						break;

					case 6:
						Main.npcChatText = $"All our goals are put to rest following the lands of the veil. Finish all those who have wronged our beloved world.";

						break;

					case 7:
						Main.npcChatText = $"Manifest their souls and bring them to my company. You shall be rewarded until the rekoning begs for your place to be silenced. ";

						break;

					case 8:
						Main.npcChatText = $"Niivi, the protector of the lands of the veil, yet curse all upon thy brother to be selfish and steal among those of power and wealth. He was greedy and in his expense payed the lovers' prison.";

						break;

					case 9:
						Main.npcChatText = $"Now I sit ill waiting for the lands of the veil to change.";

						break;




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
				
			}
		}




		public override void AddShops()
		{


			Player player = Main.LocalPlayer;



			var npcShop = new NPCShop(Type, ShopName)





					.Add(new Item(ModContent.ItemType<Helios>())
					{
						shopCustomPrice = 1,
						shopSpecialCurrency = Stellamod.MOCCurrencyID // omit this line if shopCustomPrice should be in regular coins.
					});

					

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





}