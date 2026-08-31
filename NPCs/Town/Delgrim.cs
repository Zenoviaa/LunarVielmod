using Stellamod.Common.QuestSystem;
using Stellamod.Content.Areas.Junkyard.WeaponsJY;
using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Quests.DelgrimQuest;
using Stellamod.Core;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Melee.Greatswords;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.UI.CellConverterSystem;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Town
{
    public class Delgrim : VeilTownNPC
    {
        public int NumberOfTimesTalkedTo = 0;
        public const string ShopName = "Shop";
        public const string ShopName2 = "New Shop";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 11;
        }

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
            HasTownDialogue = true;
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




        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A magical engineer huh?")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Delgrim the eternal engineer.", "2"))
            });
        }



        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Magical Engineer Delgrim"
            };
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
            //.Add(new Item(ItemID.WaterBolt) { shopCustomPrice = Item.buyPrice(gold: 40) })
            .Add<GunHolster>()
            .Add<Pulsing>()

            .Add<Hitme>()
            .Add<CogBomber>(Condition.Hardmode)
            .Add<TheTingler>(Condition.Hardmode)
            .Add<GearGutter>(Condition.Hardmode)
            .Add<DelgrimsHammer>(Condition.Hardmode)
            .Add(new Item(ItemID.Wire) { shopCustomPrice = Item.buyPrice(copper: 5) })
            ;
            npcShop.Register(); // Name of this shop tab		
        }

        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
            //Set buttons
            buttons.Add(new Tuple<string, Action>("Shop", OpenShop));
            buttons.Add(new Tuple<string, Action>("CellConverter", OpenCellConverter));

            //Delgrim Portrait
            text = "TestDialogue";
            portrait = "DelgrimPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;
        }

        private void OpenCellConverter()
        {
            Main.CloseNPCChatOrSign();
            Main.playerInventory = true;
            CellConverterUISystem uiSystem = ModContent.GetInstance<CellConverterUISystem>();
            uiSystem.CellConverterPos = NPC.Center;
            uiSystem.OpenUI();

        }
        public override void SetQuestLine(List<Quest> quests)
        {
            base.SetQuestLine(quests);
            quests.Add(ModContent.GetInstance<MysteriousPlacesI>());
        }
    }
}