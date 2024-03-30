using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets.Biomes;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Vanity.Gia;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
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

	public class Veldris : ModNPC
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
			NPC.width = 74;
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
				new FlavorTextBestiaryInfoElement("Freezing to death"),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Veldris the assassin")
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
			chat.Add(Language.GetTextValue("Who's next on this list to kill?"));
			chat.Add(Language.GetTextValue("How are you doing? "));
			chat.Add(Language.GetTextValue("I need to gamble a bit more today"));
			chat.Add(Language.GetTextValue("That Sirestias woman creeps me out."), 5.0);
			chat.Add(Language.GetTextValue("I gotta get to the Royal Capital some day. Maybe they can empower my weapons."), 2.0);
			chat.Add(Language.GetTextValue("So coldddd"), 0.1);
			chat.Add(Language.GetTextValue("Hey you, come buy some stuff from me!"), 0.1);
			chat.Add(Language.GetTextValue("Im starting to warm up this winter"), 0.1);
			chat.Add(Language.GetTextValue("Everyone and all this damn god talk, it makes me crazy"), 0.1);
			chat.Add(Language.GetTextValue("Interestingly, I don't know why those sisters all are disconnected, it seems like they're always hiding something"), 0.1);
			chat.Add(Language.GetTextValue("I wish I was someone important"), 0.1);
			chat.Add(Language.GetTextValue("Sometimes I wanna lead an army you know? I just wanna see Veiizal again"), 0.1);

			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 20)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(Language.GetTextValue("You gonna buy something lad?"));
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
				"Veldris the Calm Assassin",
				"Veldris the Calm Assassin"

			};
		}




		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button2 = Language.GetTextValue("LegacyInterface.28");
			button = "Talk";

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

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2"));

				//-----------------------------------------------------------------------------------------------	
				switch (Main.rand.Next(10))
				{
					case 0:
						Main.npcChatText = $"You doing good? I mean it's a hellhole out here. Sometimes I even start to wonder what brings you here. I'm just some lonesome seller, just be sure not to get on someone's deadlist or else I'll be seeing ya. ";

						break;

					case 1:
						Main.npcChatText = $"Everytime I hear someone talk about gods I want to just puke in their faces, like ew. I'm kind of glad that Sirestias woman is getting rid of them. Her and Fenix don't seem all too power driven. Fenix is just revenge driven, and I can respect that.";

						break;

					case 2:
						Main.npcChatText = $"If my weapons we're ever used, they better be in the right hands. My tools were crafted with some Luminull and some special metals, they better hold together- I had to steal some stuff from that Illuria place";

						break;

					case 3:
						Main.npcChatText = $"I'm just gonna rant about this Illuria place honestly. WHY IS THERE A DRAGON JUST ROAMING. Like I thought those went extinct with the the virulent- oh I meant acid. I really just needed some scales but oh noo the watcher of society is there. ";

						break;

					case 4:
						Main.npcChatText = $"Back in my day building this house was my masterpiece, Fenix really gathered up an army just to build this cathedral here and that temple below just to trap some harlet who took her stuff. Down right evil though on Fenix's behalf, maybe overkill. ";

						break;

					case 5:
						Main.npcChatText = $"Some knights visited me the other day talking of this 'peace and formality' and I couldn't take it so I went to the Lunar tree, which for some reason they live on and I stole some fragments and some luminull? It seems pretty powerful and is probably related to Lumi in some way.";

						break;

					case 6:
						Main.npcChatText = $"I love myself commissions, just sayingg if you wanna commission me I'm all available, the last person I went to kill was some goofy guy named Rallad, some girl named Sylia asked me if I could do it for her since she didnt want to be seen, took forever to find him though. ";

						break;

					case 7:
						Main.npcChatText = $"Interestingly enough me and Sylia have fun sometimes, shes pretty nice once you get to know her, sadly she stays away from her sister Merena and the rest of the capital, she's pretty chill. Kind of wanna travel with her though. ";

						break;

					case 8:
						Main.npcChatText = $"The small joys of life comes from not having your house destroyed by some malevolent gods throwing down their trap cards when youre trying to sleep. It makes me annoyed when I have to peek out my window to make sure the black hole isn't going to hit my house.";

						break;

					case 9:
						Main.npcChatText = $"You knowww, I was named Veldris by my friend, I always grew up without a name since my parents died by a stupid raging black hole guy, name was Sepsis I believe? He killed off so much 30 years ago yet I remember it like yesterday.";

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
			.Add(new Item(ItemID.Mace) { shopCustomPrice = Item.buyPrice(gold: 5) })
			.Add<AssassinsDischarge>()
			.Add<AssassinsKnife>()
			.Add<AssassinsShuriken>()
			.Add<AssassinsSlash>()
			.Add(new Item(ItemID.ThrowingKnife) { shopCustomPrice = Item.buyPrice(copper: 5) })
			.Add(new Item(ItemID.Shuriken) { shopCustomPrice = Item.buyPrice(copper: 5) })
			.Add(new Item(ItemID.BorealWood) { shopCustomPrice = Item.buyPrice(copper: 7) })
			.Add(new Item(ItemID.ApplePie) { shopCustomPrice = Item.buyPrice(silver: 50) })
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





}