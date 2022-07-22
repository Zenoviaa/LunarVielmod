using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using Terraria.DataStructures;
using System.Collections.Generic;
using ReLogic.Content;
using Terraria.ModLoader.IO;
using Stellamod.Dusts;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles;

namespace Stellamod.NPCs.Town
{
	// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
	[AutoloadHead]
	public class Gambit : ModNPC
	{
		public int NumberOfTimesTalkedTo = 0;

		public override void SetStaticDefaults()
		{
			// DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
			// DisplayName.SetDefault("Example Person");
			Main.npcFrameCount[Type] = 16; // The amount of frames the NPC has
			NPCID.Sets.DangerDetectRange[Type] = 700; // The amount of pixels away from the center of the npc that it tries to attack enemies.
			NPCID.Sets.AttackType[Type] = 0;
			NPCID.Sets.AttackTime[Type] = 90; // The amount of time it takes for the NPC's attack animation to be over once it starts.
			NPCID.Sets.AttackAverageChance[Type] = 30;
			NPCID.Sets.HatOffsetY[Type] = 4; // For when a party is active, the party hat spawns at a Y offset.

			// Influences how the NPC looks in the Bestiary
			NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
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
				.SetNPCAffection(NPCID.Demolitionist, AffectionLevel.Hate) // Hates living near the demolitionist.
			; // < Mind the semicolon!
		}
		public enum ActionState
		{

			Walk,
			Attack,
			Wait,
		}
		// Current state

		public ActionState State = ActionState.Walk;
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
			NPC.townNPC = true; // Sets NPC to be a Town NPC
			NPC.friendly = true; // NPC Will not attack player
			NPC.width = 18;
			NPC.height = 40;
			NPC.aiStyle = 7;
			NPC.damage = 90;
			NPC.defense = 42;
			NPC.lifeMax = 900;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath1;
			NPC.knockBackResist = 0.5f;

		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement("She loves those ruin medals huh?, so much so that she is now attracted to you :("),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement("Mods.StellaMod.Bestiary.Zielie")
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

			SpriteEffects effects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;


			// The rectangle we specify allows us to only cycle through specific parts of the texture by defining an area inside it

			// Using a rectangle to crop a texture can be imagined like this:
			// Every rectangle has an X, a Y, a Width, and a Height
			// Our X and Y values are the position on our texture where we start to sample from, using the top left corner as our origin
			// Our Width and Height values specify how big of an area we want to sample starting from X and Y
			Rectangle rect;

			switch (State)
			{
				case ActionState.Walk:
					rect = new Rectangle(0, 0 * 56, 40, 10 * 56);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 10, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Wait:
					rect = new Rectangle(0, 0 * 56, 40, 0 * 56);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 2000, 0, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
				case ActionState.Attack:
					rect = new Rectangle(0, 10 * 40, 40, 6 * 56);
					spriteBatch.Draw(texture, NPC.position - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 6, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
					break;
			}
			return false;

		}

		public override void HitEffect(int hitDirection, double damage)
		{
			int num = NPC.life > 0 ? 1 : 5;

			for (int k = 0; k < num; k++)
			{
				Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
			}
		}

		public override bool CanTownNPCSpawn(int numTownNPCs, int money)
		{ // Requirements for the town NPC to spawn.
			for (int k = 0; k < 255; k++)
			{
				Player player = Main.player[k];
				if (!player.active)
				{
					continue;
				}

				// Player has to have either an ExampleItem or an ExampleBlock in order for the NPC to spawn
				if (player.inventory.Any(item => item.type == ModContent.ItemType<Medal>()))
				{
					return true;
				}
			}

			return false;
		}

		

		public override ITownNPCProfile TownNPCProfile()
		{
			return new GambitPersonProfile();
		}

		public override List<string> SetNPCNameList()
		{
			return new List<string>() {
				"Zielie",
				"Zemmie",
				"Zeilie",
				"Zielie"
			};
		}

		
		

		public override void SetChatButtons(ref string button, ref string button2)
		{ // What the chat buttons are when you open up the chat UI
			button = Language.GetTextValue("LegacyInterface.28");
			button2 = "Gamble";
			if (Main.LocalPlayer.HasItem(ModContent.ItemType<Starrdew>()))
			{
				button = "Upgrade " + Lang.GetItemNameValue(ModContent.ItemType<Starrdew>());
			}
		}

		public override void OnChatButtonClicked(bool firstButton, ref bool shop)
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

				shop = true;
			}

		
		}

		public override void AI()
		{
			switch (State)
			{

				case ActionState.Walk:
					AIType = NPCID.Guide;
					counter++;
					Walk();
					break;

				case ActionState.Wait:
					AIType = NPCID.Guide;
					counter++;
					Wait();
					break;

				case ActionState.Attack:
					counter++;
					Attack();
					break;

				default:
					counter++;
					break;
			}




			Vector3 RGB = new(2.30f, 0.21f, 0.72f);
			// The multiplication here wasn't doing anything
			Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);

			//for (int j = 0; j < 2; j++)
			//{
			//	Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position, NPC.velocity * 0, ProjectileID.Spark, NPC.damage / 2, NPC.knockBackResist);
			//}
		}
		public void ResetTimers()
		{
			timer = 0;
			frameCounter = 0;
			frameTick = 0;
		}
		private void Attack()
        {
			timer++;

			if (timer == 5)
            {
				NPC.HealEffect(100, true);
				float speedX = NPC.velocity.X * Main.rand.NextFloat(.3f, .3f) + Main.rand.NextFloat(4f, 4f);
				float speedY = NPC.velocity.Y * Main.rand.Next(-1, -1) * 0.0f + Main.rand.Next(-4, -4) * 0f;
				Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedX , NPC.position.Y + speedY , speedX - 2 * 3, speedY - 1 * 2f, ModContent.ProjectileType<SineButterflyfriendly>(), (int)(15), 0f, 0, 0f, 0f);


				timer = 0;
            }
			if (NPC.life == NPC.lifeMax)
			{
				State = ActionState.Walk;
			}

		}

        private void Walk()
        {
			if (NPC.life != NPC.lifeMax)
			{
				State = ActionState.Attack;
				ResetTimers();
			}
			
			if (NPC.velocity.X == 0)
            {
				State = ActionState.Wait;
			}
		}

        private void Wait()
        {
			if (NPC.velocity.X != 0)
			{
				State = ActionState.Walk;
			}
		}

        public override void SetupShop(Chest shop, ref int nextSlot) {
			shop.item[nextSlot].SetDefaults(ModContent.ItemType<LenaSongPowder>());
		}
	 	

		
		
		 //	if (Main.moonPhase < 2) {
		 //		shop.item[nextSlot++].SetDefaults(ItemType<ExampleSword>());
		 //	}
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

	public class GambitPersonProfile : ITownNPCProfile
	{
		public int RollVariation() => 0;
		public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

		public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
		{
			if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
				return ModContent.Request<Texture2D>("Stellamod/NPCs/Gambit");

			if (npc.altTexture == 1)
				return ModContent.Request<Texture2D>("Stellamod/NPCs/Gambit_Party");

			return ModContent.Request<Texture2D>("Stellamod/NPCs/Gambit");
		}

		public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Stellamod/NPCs/Gambit_Head");
	}
}