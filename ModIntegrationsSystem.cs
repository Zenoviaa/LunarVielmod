using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.StarrVeriplant;
using Stellamod.NPCs.Bosses.Sylia;
using System;
using Stellamod.NPCs.Bosses.Jack;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;
using Stellamod.NPCs.Bosses.STARBOMBER;
using Stellamod.NPCs.Bosses.Fenix;
using Stellamod.NPCs.Catacombs.Fire;
using Stellamod.NPCs.Catacombs.Fire.BlazingSerpent;
using Stellamod.NPCs.Catacombs.Trap.Cogwork;
using Stellamod.NPCs.Catacombs.Trap.Sparn;
using Stellamod.NPCs.Catacombs.Water.WaterJellyfish;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Stellamod.NPCs.Bosses.Zui;
using Stellamod.NPCs.Bosses.Niivi;
using Stellamod.NPCs.Bosses.GothiviaTheSun.REK;
using Stellamod.NPCs.Bosses.SupernovaFragment;
using Stellamod.Items.Consumables;

namespace Stellamod
{
    // Showcases using Mod.Call of other mods to facilitate mod integration/compatibility/support
    // Mod.Call is explained here https://github.com/tModLoader/tModLoader/wiki/Expert-Cross-Mod-Content#call-aka-modcall-intermediate
    // This only showcases one way to implement such integrations, you are free to explore your own options and other mods examples

    // You need to look for resources the mod developers provide regarding how they want you to add mod compatibility
    // This can be their homepage, workshop page, wiki, github, discord, other contacts etc.
    // If the mod is open source, you can visit its code distribution platform (usually GitHub) and look for "Call" in its Mod class
    public class ModIntegrationsSystem : ModSystem
	{
		Mod bossChecklistMod;
		public override void PostSetupContent()
		{
			// Most often, mods require you to use the PostSetupContent hook to call their methods. This guarantees various data is initialized and set up properly

			// Boss Checklist shows comprehensive information about bosses in its own UI. We can customize it:
			// https://forums.terraria.org/index.php?threads/.50668/
			DoBossChecklistIntegration();

			// We can integrate with other mods here by following the same pattern. Some modders may prefer a ModSystem for each mod they integrate with, or some other design.
		}

		private void DoJackIntegration()
		{          
			// The "LogBoss" method requires many parameters, defined separately below:
			// The name used for the title of the page
			string internalName = "Jack";

			// The NPC type of the boss
			int bossType = ModContent.NPCType<NPCs.Bosses.Jack.Jack>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 1.2f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedJackBoss;

			// If the boss should show up on the checklist in the first place and when (here, always)
			Func<bool> available = () => true;

			// "collectibles" like relic, trophy, mask, pet
			List<int> collection = new List<int>()
			{
				ModContent.ItemType<Items.Placeable.JackBossRel>(),
			};

			// The item used to summon the boss with (if available)
			int summonItem = ModContent.ItemType<Items.Consumables.WanderingEssence>();
            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Jack/JackBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // Information for the player so he knows how to encounter the boss
            // Ideally you'd have this text in the localization file, but screw that
			bossChecklistMod.Call(
                "LogMiniBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem,
					["customPortrait"] = customPortait
					// Other optional arguments as needed are inferred from the wiki
				}
			);
		}

		private void DoDaedusIntegration()
        {
			string internalName2 = "DaedustheForgotten";

			// The NPC type of the boss
			int bossType2 = ModContent.NPCType<DaedusR>();

			// Value inferred from boss progression, see the wiki for details
			float weight2 = 2.5f;

			// Used for tracking checklist progress
			Func<bool> downed2 = () => DownedBossSystem.downedDaedusBoss;

			// If the boss should show up on the checklist in the first place and when (here, always)
			Func<bool> available2 = () => true;

			// "collectibles" like relic, trophy, mask, pet
			List<int> collection2 = new List<int>()
			{
				ModContent.ItemType<Items.Placeable.DaedusBossRel>(),

			};
           
			Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/DaedusRework/DaedusBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // Information for the player so he knows how to encounter the boss
            //string spawnInfo2 = $"High at the fabled castle lies a forgotten guardian of Gothivia's ranks";
			bossChecklistMod.Call(
				"LogBoss",
				Mod,
				internalName2,
				weight2,
				downed2,
				bossType2,
				new Dictionary<string, object>()
				{
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}

		private void DoDreadmireIntegration()
        {
			string internalName3 = "BloodGoddessDreadmire";

			// The NPC type of the boss
			int bossType3 = ModContent.NPCType<NPCs.Bosses.DreadMire.DreadMireR>();

			// Value inferred from boss progression, see the wiki for details
			float weight3 = 2.6f;

			// Used for tracking checklist progress
			Func<bool> downed3 = () => DownedBossSystem.downedDreadBoss;

            int summonItem8 = ModContent.ItemType<DreadMedalion>();


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/DreadMire/DreadMireBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available3 = () => true;

			// "collectibles" like relic, trophy, mask, pet
			List<int> collection3 = new List<int>()
			{
				ModContent.ItemType<Items.Placeable.DreadBossRel>(),
			};


			// The boss does not have a custom despawn message, so we omit it

			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogBoss",
				Mod,
				internalName3,
				weight3,
				downed3,
				bossType3,
				new Dictionary<string, object>()
				{
					["spawnItems"]= summonItem8,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}

		private void DoCommanderGintziaIntegration()
        {
			string internalName4 = "CommanderGintzia";

			// The NPC type of the boss
			int bossType4 = ModContent.NPCType<NPCs.Event.Gintzearmy.BossGintze.CommanderGintzia>();

			// Value inferred from boss progression, see the wiki for details
			float weight4 = 0.2f;

			// Used for tracking checklist progress
			Func<bool> downed4 = () => DownedBossSystem.downedGintzlBoss;

			// If the boss should show up on the checklist in the first place and when (here, always)
			Func<bool> available4 = () => true;

			// "collectibles" like relic, trophy, mask, pet
			List<int> collection4 = new List<int>()
			{
				ModContent.ItemType<Items.Placeable.GintzeBossRel>(),

			};

			// The item used to summon the boss with (if available)
			int summonItem4 = ModContent.ItemType<Items.Consumables.WanderingEssence>();
           
			Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Event/Gintzearmy/BossGintze/GintziaPreview").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // Information for the player so he knows how to encounter the boss
			bossChecklistMod.Call(
                "LogMiniBoss",
				Mod,
				internalName4,
				weight4,
				downed4,
				bossType4,
				new Dictionary<string, object>()
				{
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}

		private void DoSunStalkerIntegration()
        {
			string internalName5 = "Sunstalker";

			// The NPC type of the boss
			int bossType5 = ModContent.NPCType<NPCs.Bosses.SunStalker.SunStalker>();

			// Value inferred from boss progression, see the wiki for details
			float weight5 = 0.4f;

			// Used for tracking checklist progress
			Func<bool> downed5 = () => DownedBossSystem.downedSunsBoss;

			// If the boss should show up on the checklist in the first place and when (here, always)
			Func<bool> available5 = () => true;

			// "collectibles" like relic, trophy, mask, pet
			List<int> collection5 = new List<int>()
			{
				ModContent.ItemType<Items.Placeable.SunsBossRel>(),

			};

			// The item used to summon the boss with (if available)
            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/SunStalker/SunStalkerBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // Information for the player so he knows how to encounter the boss

            // The boss does not have a custom despawn message, so we omit it

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
                "LogMiniBoss",
				Mod,
				internalName5,
				weight5,
				downed5,
				bossType5,
				new Dictionary<string, object>()
				{
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}



		private void DoNESTIntegration()
		{
			string internalName5 = "NestoftheAcidic";

			// The NPC type of the boss
			int bossType5 = ModContent.NPCType<NPCs.Bosses.INest.IrradiatedNest>();

			// Value inferred from boss progression, see the wiki for details
			float weight5 = 10.2f;

			// Used for tracking checklist progress
			Func<bool> downed5 = () => DownedBossSystem.downedNESTBoss;

			// If the boss should show up on the checklist in the first place and when (here, always)
			Func<bool> available5 = () => true;

			// "collectibles" like relic, trophy, mask, pet
			List<int> collection5 = new List<int>()
			{
				ModContent.ItemType<Items.Placeable.SunsBossRel>(),

			};

			// The item used to summon the boss with (if available)
			int summonItem5 = ModContent.ItemType<Items.Consumables.EDR>();


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/INest/IrradiatedNestBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };

            // Information for the player so he knows how to encounter the boss

            // The boss does not have a custom despawn message, so we omit it

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
                "LogMiniBoss",
				Mod,
				internalName5,
				weight5,
				downed5,
				bossType5,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem5,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}


		private void DoSingularityFragmentIntegration()
        {
			string internalName6 = "LumarSingularity";

            // The NPC type of the boss
            int bossType6 = ModContent.NPCType<NPCs.Bosses.singularityFragment.SingularityFragment>();


            // Value inferred from boss progression, see the wiki for details
            float weight6 = 3.4f;

			// Used for tracking checklist progress
			Func<bool> downed6 = () => DownedBossSystem.downedSOMBoss;

			// If the boss should show up on the checklist in the first place and when (here, always)
			Func<bool> available6 = () => true;

			// "collectibles" like relic, trophy, mask, pet
			List<int> collection6 = new List<int>()
			{
				ModContent.ItemType<Items.Placeable.SOMBossRel>(),

			};
          
			Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/singularityFragment/SingularityFragmentBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // The item used to summon the boss with (if available)
            int summonItem6 = ModContent.ItemType<Items.Consumables.VoidKey>();

			// Information for the player so he knows how to encounter the boss
			bossChecklistMod.Call(
				"LogBoss",
				Mod,
				internalName6,
				weight6,
				downed6,
				bossType6,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem6,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}

		private void DoVerliaIntegration()
        {
			string internalName7 = "VerliaoftheMoon";

			// The NPC type of the boss
			int bossType7 = ModContent.NPCType<NPCs.Bosses.Verlia.VerliaB>();

			// Value inferred from boss progression, see the wiki for details
			float weight7 = 5.4f;

			// Used for tracking checklist progress
			Func<bool> downed7 = () => DownedBossSystem.downedVeriBoss;

			// If the boss should show up on the checklist in the first place and when (here, always)
			Func<bool> available7 = () => true;

			// "collectibles" like relic, trophy, mask, pet
			List<int> collection7 = new List<int>()
			{
				ModContent.ItemType<Items.Placeable.VerliBossRel>(),

			};

            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Verlia/VerliaPreview").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };

            // The item used to summon the boss with (if available)
            int summonItem7 = ModContent.ItemType<Items.Consumables.MoonflameLantern>();

			// The boss does not have a custom despawn message, so we omit it

			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location

			bossChecklistMod.Call(
				"LogBoss",
				Mod,
				internalName7,
				weight7,
				downed7,
				bossType7,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem7,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}

		private void DoGothiviaIntegration()
        {

			string internalName8 = "GothiviatheSunGoddess";

			// The NPC type of the boss
			int bossType8 = ModContent.NPCType<NPCs.Bosses.GothiviaTheSun.GOS.GothiviaIyx>();

            // Value inferred from boss progression, see the wiki for details
            float weight8 = 18.4f;

            // Used for tracking checklist progress
            Func<bool> downed8 = () => DownedBossSystem.downedGothBoss;

			// If the boss should show up on the checklist in the first place and when (here, always)
			Func<bool> available8 = () => true;

			// "collectibles" like relic, trophy, mask, pet
			List<int> collection8 = new List<int>()
			{
				ModContent.ItemType<Items.Placeable.GothiviaBossRel>(),

			};

			// The item used to summon the boss with (if available)
			int summonItem8 = ModContent.ItemType<Items.Consumables.GothiviasSeal>();

            // Information for the player so he knows how to encounter the boss

            // The boss does not have a custom despawn message, so we omit it
            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/GothiviaBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };
            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogBoss",
				Mod,
				internalName8,
				weight8,
				downed8,
				bossType8,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem8,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}


        private void DoIrradiaIntegration()
        {

            string internalName8 = "IrradiaNHavoc";

            // The NPC type of the boss
            int bossType8 = ModContent.NPCType<NPCs.Bosses.IrradiaNHavoc.Irradia.Irradia>();

            // Value inferred from boss progression, see the wiki for details
            float weight8 = 8.3f;

            // Used for tracking checklist progress
            Func<bool> downed8 = () => DownedBossSystem.downedIrradiaBoss;

            // If the boss should show up on the checklist in the first place and when (here, always)
            Func<bool> available8 = () => true;

            // "collectibles" like relic, trophy, mask, pet
            List<int> collection8 = new List<int>()
            {
                ModContent.ItemType<Items.Placeable.IrradiaBossRel>(),

            };

            // The item used to summon the boss with (if available)
            int summonItem8 = ModContent.ItemType<Items.Materials.ManifestedBravery>();

            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/IrradiaNHavoc/Irradia/IrradiaBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };

            // Information for the player so he knows how to encounter the boss

            // The boss does not have a custom despawn message, so we omit it

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName8,
                weight8,
                downed8,
                bossType8,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = summonItem8,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }



        private void DoSyliaIntegration()
		{

			string internalName = nameof(Sylia);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<Sylia>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 11.8f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedSyliaBoss;
           
			Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Sylia/SyliaPreview").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogMiniBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}

		private void DoSTARIntegration()
		{

			string internalName = nameof(STARBOMBER);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<STARBOMBER>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 8.6f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedSTARBoss;

			int summonItem8 = ModContent.ItemType<Items.Consumables.CursedShard>();

            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/STARBOMBER/STARBOMBERPreview").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogMiniBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}

		private void DoFenixIntegration()
		{

			string internalName = nameof(Fenix);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<Fenix>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 15.6f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedFenixBoss;

			int summonItem8 = ModContent.ItemType<Items.Consumables.CursedShard>();


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Fenix/FenixPreview").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };

            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}



		private void DoZuiIntegration()
		{

			string internalName = nameof(ZuiTheTraveller);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<ZuiTheTraveller>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 12.9f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedZuiBoss;

			int summonItem8 = ModContent.ItemType<RadianceStone>();


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Zui/ZuiBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };


            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem8,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
			);
		}
		private void DoStoneGolemIntegration()
        {
			string internalName = nameof(StarrVeriplant);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<StarrVeriplant>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 0.1f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedStoneGolemBoss;



			int summonItem8 = ModContent.ItemType<Items.Consumables.CursedShard>();

			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
                "LogMiniBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
					// Other optional arguments as needed are inferred from the wiki
				}
			);
		}

		private void DoBlazingSerpentIntegration()
		{
			string internalName = nameof(BlazingSerpentHead);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<BlazingSerpentHead>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 7.2f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedBlazingSerpent;

			int summonItem8 = ModContent.ItemType<Items.Consumables.CursedShard>();

			Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
				Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Catacombs/Fire/BlazingSerpent/BlazingSerpentPreview").Value;
				Vector2 centered = new Vector2(
					rect.X + (rect.Width / 2) - (texture.Width / 2), 
					rect.Y + (rect.Height / 2) - (texture.Height / 2));
				spriteBatch.Draw(texture, centered, color);
			};

			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogMiniBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem8,
					["customPortrait"] = customPortait
					// Other optional arguments as needed are inferred from the wiki
				}
			);
		}

		private void DoCogworkIntegration()
		{
			string internalName = nameof(Cogwork);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<Cogwork>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 7.21f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedCogwork;

			int summonItem8 = ModContent.ItemType<Items.Consumables.CursedShard>();

			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogMiniBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem8,
					// Other optional arguments as needed are inferred from the wiki
				}
			);
		}

		private void DoWaterJellyfishIntegration()
		{
			string internalName = nameof(WaterJellyfish);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<WaterJellyfish>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 7.22f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedWaterJellyfish;


			int summonItem8 = ModContent.ItemType<Items.Consumables.CursedShard>();

			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogMiniBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem8,
					// Other optional arguments as needed are inferred from the wiki
				}
			);
		}
		private void DoSparnIntegration()
		{
			string internalName = nameof(Sparn);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<Sparn>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 7.23f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedSparn;

			int summonItem8 = ModContent.ItemType<Items.Consumables.CursedShard>();
			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogMiniBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem8,
					// Other optional arguments as needed are inferred from the wiki
				}
			);
		}

		private void DoPandorasFireBoxIntegration()
		{
			string internalName = nameof(PandorasFlamebox);

			// The NPC type of the boss
			int bossType = ModContent.NPCType<PandorasFlamebox>();

			// Value inferred from boss progression, see the wiki for details
			float weight = 7.24f;

			// Used for tracking checklist progress
			Func<bool> downed = () => DownedBossSystem.downedPandorasBox;

			int summonItem8 = ModContent.ItemType<Items.Consumables.CursedShard>();

			// By default, it draws the first frame of the boss, omit if you don't need custom drawing
			// But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
			bossChecklistMod.Call(
				"LogMiniBoss",
				Mod,
				internalName,
				weight,
				downed,
				bossType,
				new Dictionary<string, object>()
				{
					["spawnItems"] = summonItem8,
					// Other optional arguments as needed are inferred from the wiki
				}
			);
		}


		private void DoNiiviIntegration()
		{
            string internalName = nameof(Niivi);

            // The NPC type of the boss
            int bossType = ModContent.NPCType<Niivi>();

            // Value inferred from boss progression, see the wiki for details
            float weight = 18.2f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedNiiviBoss;


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/Niivi/NiiviPreview").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };


            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }

		private void DoRekIntegration()
		{
            string internalName = nameof(RekSnake);

            // The NPC type of the boss
            int bossType = ModContent.NPCType<RekSnake>();

            // Value inferred from boss progression, see the wiki for details
            float weight = 18.1f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedRekBoss;


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/REK/RekBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };


            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            bossChecklistMod.Call(
                "LogMiniBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }

		private void DoSupernovaFragmentIntegration()
		{
            string internalName = nameof(SupernovaFragment);

            // The NPC type of the boss
            int bossType = ModContent.NPCType<SupernovaFragment>();

            // Value inferred from boss progression, see the wiki for details
            float weight = 12.92f;

            // Used for tracking checklist progress
            Func<bool> downed = () => DownedBossSystem.downedSupernovaFragmentBoss;

            int summonItem8 = ModContent.ItemType<VoidalPassageway>();


            Action<SpriteBatch, Rectangle, Color> customPortait = (SpriteBatch spriteBatch, Rectangle rect, Color color) => {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/SupernovaFragment/SupernovaFragmentBestiary").Value;
                Vector2 centered = new Vector2(
                    rect.X + (rect.Width / 2) - (texture.Width / 2),
                    rect.Y + (rect.Height / 2) - (texture.Height / 2));
                spriteBatch.Draw(texture, centered, color);
            };


            // By default, it draws the first frame of the boss, omit if you don't need custom drawing
            // But we want to draw the bestiary texture instead, so we create the code for that to draw centered on the intended location
            bossChecklistMod.Call(
                "LogBoss",
                Mod,
                internalName,
                weight,
                downed,
                bossType,
                new Dictionary<string, object>()
                {
                    ["spawnItems"] = summonItem8,
                    ["customPortrait"] = customPortait
                    // Other optional arguments as needed are inferred from the wiki
                }
            );
        }

		private void DoBossChecklistIntegration()
		{
			// The mods homepage links to its own wiki where the calls are explained: https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-Log-Entry-Mod-Call
			// If we navigate the wiki, we can find the "LogBoss" method, which we want in this case
			// A feature of the call is that it will create an entry in the localization file of the specified NPC type for its spawn info, so make sure to visit the localization file after your mod runs once to edit it
			if (!ModLoader.TryGetMod("BossChecklist", out bossChecklistMod))
            {
				return;
			}
	
			// For some messages, mods might not have them at release, so we need to verify when the last iteration of the method variation was first added to the mod, in this case 1.6
			// Usually mods either provide that information themselves in some way, or it's found on the GitHub through commit history/blame
			if (bossChecklistMod.Version < new Version(1, 6))
			{
				return;
			}

			//Integrate the Bosses Here
			DoJackIntegration();
			DoDaedusIntegration();
			DoDreadmireIntegration();
			DoCommanderGintziaIntegration();
			DoSunStalkerIntegration();
			DoSingularityFragmentIntegration();
			DoVerliaIntegration();
			DoIrradiaIntegration();
			DoSyliaIntegration();
			DoStoneGolemIntegration();
			DoSTARIntegration();
			DoFenixIntegration();
			DoBlazingSerpentIntegration();
			DoCogworkIntegration();
			DoPandorasFireBoxIntegration();
			DoWaterJellyfishIntegration();
			DoSparnIntegration();
			DoZuiIntegration();
			DoNESTIntegration();
			DoNiiviIntegration();
			DoRekIntegration();
            DoGothiviaIntegration();
			DoSupernovaFragmentIntegration();
        }
	}
}