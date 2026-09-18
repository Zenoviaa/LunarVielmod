using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.QuestSystem;
using Stellamod.Content.Quests.OldManQuest;
using Stellamod.Core;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Melee.Swords;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Thrown;
using System;
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

    public class OldMan : VeilTownNPC
    {
        public int NumberOfTimesTalkedTo = 0;
        public const string ShopName = "Shop";
        public const string ShopName2 = "New Shop";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 30;
        }

        public override void SetDefaults()
        {
            // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 74;
            NPC.height = 56;
            NPC.aiStyle = 0;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 200;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamageFromHostiles = true;
            SpawnAtPoint = true;
            HasTownDialogue = true;
        }

        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Struct/Overworld/TheDock";
            spawner.spawnTileOffset = new Point(125, -15);
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

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.VortexPillar,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Freezing to death")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Veldris the assassin", "2"))
            });
        }


        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Veiizal the Hunter",
            };
        }

        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
            //Set buttons
            buttons.Add(new Tuple<string, Action>("Talk", Talk));
            buttons.Add(new Tuple<string, Action>("Shop", OpenShop));

            portrait = "GardenerWilly";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "ZuiOpenDialogue1";
        }

        public override void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {
            base.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            portrait = "GardenerWilly";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "ZuiIdleChat1";
        }

        public override void SetQuestLine(List<Quest> quests)
        {
            base.SetQuestLine(quests);
            quests.Add(ModContent.GetInstance<CollectFlowersI>());
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
            .Add<AssassinsRecharge>(Condition.DownedGolem);
            npcShop.Register(); // Name of this shop t
        }
    }
}