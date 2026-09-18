using Stellamod.Common.QuestSystem;
using Stellamod.Content.Ammo;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Content.Areas.Tundra.Snow.AccsSN;
using Stellamod.Content.Currencies;
using Stellamod.Content.Quests.ZuiQuest;
using Stellamod.Content.Vanity.Witchen;
using Stellamod.Core;
using Stellamod.Items.Accessories;
using Stellamod.Items.Armors.Vanity.Nyxia;
using Stellamod.Items.Armors.Vanity.Solarian;
using Stellamod.Items.Quest.Zui;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.NPCs.Bosses.Zui;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Town
{
    public class Zui : VeilTownNPC
    {
        public int NumberOfTimesTalkedTo = 0;
        public const string ShopName = "Shop";
        public const string ShopName2 = "New Shop";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 4; // The amount of frames the NPC has
        }

        // Current frame
        public int frameCounter;
        // Current frame's progress
        public int frameTick;
        // Current state's timer
        public float timer;

        // AI counter
        public int counter;
        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Structures/WitchTown";
            spawner.spawnTileOffset = new Point(190, -20 - 38);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 54;
            NPC.height = 130;
            NPC.aiStyle = 0;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 2000;
            NPC.npcSlots = 0;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            SpawnAtPoint = true;
            HasTownDialogue = true;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.07f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        //This prevents the NPC from despawning
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
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "A traveller of the lands who may hold great power")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Zui the Traveller", "2"))
            });
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Zui The Traveller",
            };
        }

        private void Quest_NotCheckmarked()
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
            Main.npcChatText = LangText.Chat(this, "Special1");
            var entitySource = NPC.GetSource_GiftOrReward();
            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<EmptyFlowerBag>(), 1);
        }

        private void Quest_NotCheckmarkedHardmode()
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
            Main.npcChatText = LangText.Chat(this, "Special2");
            var entitySource = NPC.GetSource_GiftOrReward();
            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<EmptyCollectorsBag>(), 1);
        }

        private void Quest_1Complete()
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
            Main.npcChatText = LangText.Chat(this, "Special3");
            var entitySource = NPC.GetSource_GiftOrReward();
            if (Main.rand.NextBool(1))
            {
                Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.HealingPotion, 7);
            }

            if (Main.rand.NextBool(3))
            {
                Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.GreaterHealingPotion, 7);
            }

            if (Main.rand.NextBool(5))
            {
                Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.SuperHealingPotion, 5);
            }

            if (Main.rand.NextBool(1))
            {
                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RuinMedal>(), 9);
            }

            ZuiQuestSystem.CompleteQuest();
            if (ZuiQuestSystem.QuestsCompleted == 1)
            {


            }

            //   Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), 2);

            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedFlowerBag>());
            Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            //Setting all previous quests to be complete, so it's backwards compatible with the old version.

        }

        private void Quest_16Complete()
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
            Main.npcChatText = LangText.Chat(this, "Special4");
            var entitySource = NPC.GetSource_GiftOrReward();
            if (Main.rand.NextBool(1))
            {
                Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.HealingPotion, 10);
            }

            if (Main.rand.NextBool(3))
            {
                Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.GreaterHealingPotion, 15);
            }

            if (Main.rand.NextBool(5))
            {
                Main.LocalPlayer.QuickSpawnItem(entitySource, ItemID.SuperHealingPotion, 10);
            }

            if (Main.rand.NextBool(1))
            {
                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RuinMedal>(), 18);
                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<LiliumArrow>(), 250);
            }

            if (ZuiQuestSystem.QuestsCompleted == 15)
            {

                Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<FungalFlace>(), 1);

            }

            ZuiQuestSystem.CompleteQuest();
            //    Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<RippedFabric>(), Main.rand.Next(3));

            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedCollectorsBag>());
            Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            //Setting all previous quests to be complete, so it's backwards compatible with the old version.

        }
        private void Quest_3Complete()
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
            Main.npcChatText = LangText.Chat(this, "Special5");
            //Setting all previous quests to be complete, so it's backwards compatible with the old version.
            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedFlowerBag>());

            var entitySource = NPC.GetSource_GiftOrReward();
            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<TomeofRaining>(), 1);


            Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            ZuiQuestSystem.CompleteQuest();
        }

        private void Quest_6Complete()
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
            Main.npcChatText = LangText.Chat(this, "Special6");

            //Setting all previous quests to be complete, so it's backwards compatible with the old version.
            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedFlowerBag>());
            Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            ZuiQuestSystem.CompleteQuest();
        }
        private void Quest_10Complete()
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
            Main.npcChatText = LangText.Chat(this, "Special7");

            var entitySource = NPC.GetSource_GiftOrReward();
            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<Hookarama>(), 1);

            //Setting all previous quests to be complete, so it's backwards compatible with the old version.
            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedFlowerBag>());
            Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            ZuiQuestSystem.CompleteQuest();
        }

        private void Quest_20Complete()
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
            Main.npcChatText = LangText.Chat(this, "Special8");


            //Setting all previous quests to be complete, so it's backwards compatible with the old version.
            var entitySource = NPC.GetSource_GiftOrReward();
            Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<CarrotPatrol>(), 1);


            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedCollectorsBag>());
            Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();
            ZuiQuestSystem.CompleteQuest();
        }

        private void Quest_30Complete()
        {
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss2")); // Reforge/Anvil sound
            Main.npcChatText = LangText.Chat(this, "Special9");
            int DesertRuneItemIndex = Main.LocalPlayer.FindItem(ModContent.ItemType<CompletedCollectorsBag>());
            Main.LocalPlayer.inventory[DesertRuneItemIndex].TurnToAir();

            ZuiQuestSystem.CompleteQuest();
            if (ZuiQuestSystem.QuestsCompleted == 30)
            {

                var entitySource = NPC.GetSource_GiftOrReward();
                //	Main.LocalPlayer.QuickSpawnItem(entitySource, ModContent.ItemType<SirestiasToken>(), 1);

            }

            //Setting all previous quests to be complete, so it's backwards compatible with the old version.

        }

        private bool CompleteQuests()
        {
            Player player = Main.LocalPlayer;

            if (ZuiQuestSystem.QuestsCompleted == 2 && player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
            {
                Quest_3Complete();
                return true;
            }
            else if (ZuiQuestSystem.QuestsCompleted == 5 && player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
            {
                Quest_6Complete();
                return true;
            }
            else if (ZuiQuestSystem.QuestsCompleted == 9 && player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
            {
                Quest_10Complete();
                return true;
            }
            else if (ZuiQuestSystem.QuestsCompleted == 19 && player.HasItem(ModContent.ItemType<CompletedCollectorsBag>()))
            {
                Quest_20Complete();
                return true;
            }
            else if (ZuiQuestSystem.QuestsCompleted == 29 && player.HasItem(ModContent.ItemType<CompletedCollectorsBag>()))
            {
                Quest_30Complete();
                return true;
            }
            else if (player.HasItem(ModContent.ItemType<CompletedFlowerBag>()) && ZuiQuestSystem.QuestsCompleted != 29 && ZuiQuestSystem.QuestsCompleted != 19 && ZuiQuestSystem.QuestsCompleted != 9 && ZuiQuestSystem.QuestsCompleted != 5 && ZuiQuestSystem.QuestsCompleted != 2 && ZuiQuestSystem.QuestsCompleted < 10)
            {
                Quest_1Complete();
                return true;
            }

            else if (player.HasItem(ModContent.ItemType<CompletedCollectorsBag>()) && ZuiQuestSystem.QuestsCompleted != 29 && ZuiQuestSystem.QuestsCompleted != 19 && ZuiQuestSystem.QuestsCompleted != 9 && ZuiQuestSystem.QuestsCompleted != 5 && ZuiQuestSystem.QuestsCompleted != 2 && ZuiQuestSystem.QuestsCompleted >= 10)
            {
                Quest_16Complete();
                return true;
            }


            return false;
        }

        private void StartQuests()
        {
            Player player = Main.LocalPlayer;

            //Go through the list of quests in a specific order and see if any need to be started
            if (ZuiQuestSystem.QuestsCompleted < 10 && !player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
            {
                Quest_NotCheckmarked();
            }

            if (ZuiQuestSystem.QuestsCompleted >= 10 && ZuiQuestSystem.QuestsCompleted < 30 && !player.HasItem(ModContent.ItemType<CompletedFlowerBag>()))
            {
                Quest_NotCheckmarkedHardmode();
            }
            else if (ZuiQuestSystem.QuestsCompleted >= 30)
            {
                //All Quests completed
                Main.npcChatText = LangText.Chat(this, "Special10");
            }
        }
        public override void AI()
        {
            timer++;
            NPC.CheckActive();
            NPC.spriteDirection = NPC.direction;
            if (NPC.AnyNPCs(ModContent.NPCType<ZuiTheTraveller>()))
            {

                NPC.Kill();
            }
        }

        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
            //Set buttons
            buttons.Add(new Tuple<string, Action>("Talk", Talk));
            buttons.Add(new Tuple<string, Action>("Shop", OpenShop));



            portrait = "ZuiPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "ZuiOpenDialogue1";
        }

        public override void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {
            base.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            portrait = "ZuiPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "ZuiIdleChat1";


        }

        public override void SetQuestLine(List<Quest> quests)
        {
            base.SetQuestLine(quests);
            quests.Add(ModContent.GetInstance<CraftAtCauldron>());
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
            .Add(new Item(ItemID.Bottle) { shopCustomPrice = Item.buyPrice(copper: 50) })
            .Add(new Item(ItemID.JungleRose) { shopCustomPrice = Item.buyPrice(gold: 1) })
            .Add<IceClimbers>()
            //.Add<FloweredCard>()
            //.Add<ZenoviasPikpikGlove>()
            .Add<NyxiaHat>()
            .Add<NyxiaRobe>()
            .Add<NyxiaThighs>()
            .Add<SolarianHat>()
            .Add<SolarianChestplate>()
            .Add<SolarianPants>()
            .Add<PerfectionStaff>(ZuiQuestSystem.ShopCondition3)
            //	.Add<AquaCrystal>(ZuiQuestSystem.ShopCondition3)
            //.Add<OnionOfHeight>(ZuiQuestSystem.ShopCondition3)
            .Add(new Item(ItemID.NaturesGift) { shopCustomPrice = Item.buyPrice(gold: 1) }, (ZuiQuestSystem.ShopCondition3))
            .Add(new Item(ItemID.LuckyHorseshoe) { shopCustomPrice = Item.buyPrice(gold: 15) }, (ZuiQuestSystem.ShopCondition6))
            .Add(new Item(ItemID.CloudinaBalloon) { shopCustomPrice = Item.buyPrice(gold: 25) }, (ZuiQuestSystem.ShopCondition6))
            //{ shopCustomPrice = Item.buyPrice(platinum: 1) })

            //.Add<OnionOfUselessness>(ZuiQuestSystem.ShopCondition10)
            .Add(new Item(ItemID.BundleofBalloons) { shopCustomPrice = Item.buyPrice(gold: 65) }, (ZuiQuestSystem.ShopCondition10))
            .Add(new Item(ItemID.CobaltShield) { shopCustomPrice = Item.buyPrice(gold: 80) }, (ZuiQuestSystem.ShopCondition10))
            .Add(new Item(ItemID.Obsidian) { shopCustomPrice = Item.buyPrice(silver: 4) }, (ZuiQuestSystem.ShopCondition10))

            //	.Add<OnionOfSight>(ZuiQuestSystem.ShopCondition20)
            .Add<WitchenHat>(ZuiQuestSystem.ShopCondition20)
            .Add<WitchenRobe>(ZuiQuestSystem.ShopCondition20)
            .Add<WitchenPants>(ZuiQuestSystem.ShopCondition20)
            .Add<EckasectSire>(ZuiQuestSystem.ShopCondition20)

            .Add<ChromaCutter>(ZuiQuestSystem.ShopCondition30)
            //	.Add<OnionOfStrength>(ZuiQuestSystem.ShopCondition30)

            ;
            npcShop.Register(); // Name of this shop tab		
        }


    }
}