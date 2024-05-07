using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets.Biomes;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Vanity.Gia;
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
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Items.Weapons.Thrown.Jugglers;
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
	//[AutoloadHead]
	[AutoloadBossHead]
	public class Delgrim : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;
		public const string ShopName = "Shop";
		public const string ShopName2 = "New Shop";
		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 11; // The amount of frames the NPC has

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
			NPC.width = 92;
			NPC.height = 84;
			NPC.aiStyle = -1;
			NPC.damage = 90;
			NPC.defense = 42;
			NPC.lifeMax = 1;
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
			NPC.frameCounter += 0.20f;
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
					return ModContent.Request<Texture2D>("Stellamod/NPCs/Town/Delgrim");

				if (npc.altTexture == 1)
					return ModContent.Request<Texture2D>("Stellamod/NPCs/Town/Delgrim_Head");

				return ModContent.Request<Texture2D>("Stellamod/NPCs/Town/Delgrim");
			}

			public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Stellamod/NPCs/Town/Delgrim_Head");
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
				new FlavorTextBestiaryInfoElement("A magical engineer huh?"),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Delgrim the eternal engineer.")
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
			chat.Add(Language.GetTextValue("You're chill aren't ya?"));
			chat.Add(Language.GetTextValue("Everyone comes in for the same stuff, come and go please."));
			chat.Add(Language.GetTextValue("Another visitor?"));
			chat.Add(Language.GetTextValue("What goes on in the world nowadays?"), 1.0);
			chat.Add(Language.GetTextValue("I don't mean trouble, but do as you please."), 1.0);


			NumberOfTimesTalkedTo++;
			if (NumberOfTimesTalkedTo >= 40)
			{
				//This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
				chat.Add(Language.GetTextValue("..."));
			}

			return chat; // chat is implicitly cast to a string.
		}
		public override void HitEffect(NPC.HitInfo hit)
		{
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.IceTorch);
			}
		}






		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				"Magical Engineer Delgrim",
				"Magical Engineer Delgrim"

			};
		}




		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button2 = Language.GetTextValue("LegacyInterface.28");
			button = "Old Tales";

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

				SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2"));

				//-----------------------------------------------------------------------------------------------	
				switch (Main.rand.Next(30))
				{
					case 0:
						Main.npcChatText = $"An old time there was a thriving capital, we used to call it the Harboring Morrow. It was a thriving city under the protection of Gothivia, Verlia, and Irradia. I used to be a member. Gothivia loved every one of us, Verlia would be off directing armies and Irradia well she spent time making electronics. ";

						break;

					case 1:
						Main.npcChatText = $"Irradia would make amazing tech, and I would come to every show she would host and watch her mechanic sword fights and spare parts, Verlia would sometimes participate and tear all of them apart. Verlia was just too good of a swordsman.";

						break;

					case 2:
						Main.npcChatText = $"Gothivia would always come down at 10 A.M in the morning to greet all the city members, it used to be the time of my life. She'd give us goodies and Rek would come by with his binding light to empower the underground and brighten things up. ";

						break;

					case 3:
						Main.npcChatText = $"Verlia and Gothivia are sisters, although they don't look it anymore, they used to be twins yet they couldn't be different from each other. Verlia was always rambunctious and outgoing while Gothivia was a calm and loving soul, the two got along.";

						break;

					case 4:
						Main.npcChatText = $"Of all the inventions Irradia made, by far my favorite was a rendition of Gothivia's Rek called Havoc. It was a magnificent beast. It held it's own against Verlia in the shows, if only I could go back to those days. ";

						break;

					case 5:
						Main.npcChatText = $"Verlia always would be training the army, the gintze army wasn't exactly the most reliable mainly because of Verlia's idiocity and lack of care for danger. The army would always charge head first without a care for defense. I wonder where the army was when we were defending against the Carian warriors and knights";

						break;

					case 6:
						Main.npcChatText = $"Some days I look back on the Harboring Morrow and wonder why we fell so hard. I remember BORDOC, an excellent blacksmith but too arrogant to read his ways. Irradia taught me some amazing things I carry with me today and all the people. the people.. ";

						break;

					case 7:
						Main.npcChatText = $"You know, I had loved Irradia, until we seperated through war our connection was magically I would say. Damn witches and their artistry. I don't hate magic, but its the reason I lost so much. That commander, I will never forget the smirk on that face. So called Fenix and her army of Carian knights. ";

						break;

					case 8:
						Main.npcChatText = $"One day in the Harboring Morrow, the most joyous of days, some collectors came back infected by some strange disease, as you may know now it's called the Virulent. An acid like corruption that eats at the soul. It tore our village apart like the black plague. It weakened us, right before a Carian army swept us away at our worst, we stood no chance.";

						break;

					case 9:
						Main.npcChatText = $"Verlia one day left the Morrow to search and make peace with the witches in the upcoming villages, she was by far the most magical out of the three of them, so she would stand a chance to help, when she came back, she was out of her mind.";

						break;

					case 10:
						Main.npcChatText = $"When Verlia came back from her excursion, she well, gave information that changed the course of our lives. Me being in a relationship with Irradia I was able to be let in on that information. She shared to us a tome from a small witch village. This would a fatal mistake.";

						break;

					case 11:
						Main.npcChatText = $"Verlia's excursion took place right when the corruption started occuring, she went to look for a way to heal the damaged. Gothivia would do her best to heal everyone but she was only one person in the biggest capital in terraria. ";

						break;

					case 12:
						Main.npcChatText = $"One harrowing day, a fox appears at the doorstep of Verlia and Gothivia's abode. I knew my eyes decieved me when I had seen the fox transform into a tall Nero, they are like human cats but I thought they'd been extinct for millenia! ";

						break;

					case 13:
						Main.npcChatText = $"When a fox arrived on the doorsteps of our beloved, The fox lady brings out what I can only think was a lantern and all I could make out inside that room was a giant blue flash and the screams of Gothivia. This was the last time anyone heard of Verlia.";

						break;

					case 14:
						Main.npcChatText = $"After what I call the 'blue flash,' everyone hurried to a sign of screaming in the middle of the night. We all come to see Gothivia crying with a note in her hand and the fox lady. She introduced herself as Fenix. ";

						break;

					case 15:
						Main.npcChatText = $"Fenix was a tall statured woman yet extremely menacing, at times a lot of us thought she may have been more powerful than Gothivia, but deep down Gothivia's suppressed feelings for her family would probably overcome that. ";

						break;

					case 16:
						Main.npcChatText = $"The illnesses started to rack up and since Verlia disappeared, our Harboring Morrow was not complete, there was nobody to keep our armies in check, Gothivia wasn't mentally there anymore and Irradia was trying her best to get everyone to calm down. It was a horrible time.";

						break;

					case 17:
						Main.npcChatText = $"Irradia did explain what happened to Verlia, but I thought it was just downright evil. Fenix trapped Verlia's soul inside a lantern and bounded it to a secret power for experimentation because she stole a book from their village. We'll never get to see her again will we?";

						break;

					case 18:
						Main.npcChatText = $"I think Irradia's way of coping to the loss of Verlia was teaching me now that Im starting to recollect. She may have used it as a way of reflecting, and I didn't see that she was hurting yet I was always there for her.";

						break;

					case 19:
						Main.npcChatText = $"I kept on living, hell I even got sick from the Virulent, yet I was Gothivia's last person to get healed from her. Rek eventually got infected too yet since havoc was electronic, he stayed normal. The creatures that lived the best were mostly metallic, its what remains of our old village.";

						break;

					case 20:
						Main.npcChatText = $"After the last healing in from Gothivia, I'd say about 90% of our village was wiped out from the Virulent, our soliders, our love, and will to live all started to fall apart. ";

						break;

					case 21:
						Main.npcChatText = $"I could recall me laying on a bed as I watched our city crumble, Irradia would stay by my side and we'd comfort each other as the world caved in for us.";

						break;

					case 22:
						Main.npcChatText = $"After a while of our suffering in the city Gothivia would leave to seek vengence and power to save her sister, leaving Irradia to be at the helm. Take a guess who arrived not so soon afterwards? Fenix, she came to take what was left of us.";

						break;

					case 23:
						Main.npcChatText = $"Fenix is like a scouraging raven looking for its next feast, and we were on the menu. I was forced to split from my love due to her sacrifices of being a leader. We haven't seen each other since, yet I know she's out there somewhere. No way would she loose to Fenix..";

						break;

					case 24:
						Main.npcChatText = $"The Great Departure, that's what I call the leaving from what was left in our city. As the Carian army approached, we hid through underground tunnels to escape, some of us, including me, haven't gone back to the surface. I was even invited to come to the Fable.";

						break;

					case 25:
						Main.npcChatText = $"Those who made it to the surface from the Great Departure reinvented the Harboring Morrow yet from all the messages from BORDOC, it isn't the same. They close their doors to everyone in fear and they look for Gothivia, she never said where she went.";

						break;

					case 26:
						Main.npcChatText = $"I was in tunnels underground for 10 of my years, now I lay here with all the corpses and electronics that I bear, I've travelled every part of this world except the surface, to ever find Fenix again would send me over my limit.";

						break;

					case 27:
						Main.npcChatText = $"You know, maybe I should have travelled with the rest of the members of the morrow, many of us split up in our escape, many died, hell, maybe I'm the only standing survivor, but I'm here.";

						break;
					
					case 28:
						Main.npcChatText = $"My creations that I make now are from my explorations, I will find Irradia again, I need to apologize, to say something. These are all for her anyway. If only I could have stopped this from getting this way, maybe I could have stayed against her will.";

						break;

					case 29:
						Main.npcChatText = $"Of all my years to live and ever to live, I will never understand things like Fenix, her torturous additude, why she went to such lengths to kill us, why the infection was so hard to fend off, and why I couldn't stay, these are all for you, Irradia. ";

						break;
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




		public override void AddShops()
		{
			var npcShop = new NPCShop(Type, ShopName)
			.Add(new Item(ItemID.WaterBolt) { shopCustomPrice = Item.buyPrice(gold: 5) })
			.Add<Hitme>()
			.Add<Hultinstein>()
			.Add<VillagersBroochA>()
			.Add<DriveConstruct>()
			.Add<ArmorDrive>()
			.Add<WeaponDrive>()
			.Add<BlankCard>()
			.Add<BlankCrossbow>()
			.Add<BlankSafunai>()
			.Add<BlankRune>()
			.Add<BlankBrooch>()
			.Add<BlankOrb>()
            .Add<BasicBaseball>(CustomConditions.PostSingularity)
            .Add<GunHolster>()
			.Add<Pulsing>()
			.Add<CogBomber>(Condition.Hardmode)
            .Add<TheTingler>(Condition.Hardmode)
            .Add<GearGutter>(Condition.Hardmode)
            .Add<DelgrimsHammer>(Condition.Hardmode)
            .Add(new Item(ItemID.Wire) { shopCustomPrice = Item.buyPrice(copper: 5) })
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