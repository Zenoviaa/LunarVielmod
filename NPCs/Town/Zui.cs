using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Common.QuestSystem;
using Stellamod.Content.Ammo;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Content.Areas.Tundra.Snow.AccsSN;
using Stellamod.Content.Currencies;
using Stellamod.Content.Dialogue;
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

        public override void OpenTownDialogue(ref SpeechBoxTalkingParameters talkingParameters, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref talkingParameters, buttons);
            talkingParameters.profile = GooberDialoguePresets.Zui;

            //Set buttons
            buttons.Add(new Tuple<string, Action>("Talk", GoobeR));
            buttons.Add(new Tuple<string, Action>("Shop", OpenShop));
        }

        private void GoobeR()
        {
            OpenTalkOptions(
ModContent.GetInstance<VerliaHappenedDialogue>(),
ModContent.GetInstance<VerliaFamilyDialogue>(),
ModContent.GetInstance<VerliaWingsDialogue>());
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