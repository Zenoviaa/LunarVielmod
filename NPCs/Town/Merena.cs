using Stellamod.Content.Armors.Alsis;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core;
using Stellamod.Helpers;
using Stellamod.Items.Weapons.Mage;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Town
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    //[AutoloadHead]
    public class Merena : VeilTownNPC
    {
        public int NumberOfTimesTalkedTo = 0;
        public const string ShopName = "Shop";
        public const string ShopName2 = "New Shop";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 8; 
        }

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
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 62;
            NPC.height = 90;
            NPC.aiStyle = 0;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 200;
            NPC.npcSlots = 0;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamageFromHostiles = true;
            SpawnAtPoint = true;
        }

        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Struct/Alcad/RoyalCapital3";
            spawner.spawnTileOffset = new Point(506, -13);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.16f;
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
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Magic Magic MAGIC")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Merena the bewitched sorcerer", "2"))
            });
        }



        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Merena the Sorcerer"
            };
        }

        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
            .Add(new Item(ItemID.Book) { shopCustomPrice = Item.buyPrice(copper: 50) })
            .Add(new Item(ItemID.FallenStar) { shopCustomPrice = Item.buyPrice(silver: 75) })
            .Add(new Item(ItemID.AbigailsFlower) { shopCustomPrice = Item.buyPrice(gold: 1) })
            .Add(new Item(ModContent.ItemType<BurnedCarianTome>()))
            .Add<AlcadBomb>(MerenaQuestSystem.ShopConditionKillVerlia) //{ shopCustomPrice = Item.buyPrice(silver: 10) })//{ shopCustomPrice = Item.buyPrice(platinum: 1) })
            .Add<PearlescentScrap>(MerenaQuestSystem.ShopConditionKillVerlia)
            .Add<AlsisMask>(MerenaQuestSystem.ShopConditionTome)
            .Add<AlsisChestplate>(MerenaQuestSystem.ShopConditionTome)
            .Add<AlsisMask>(MerenaQuestSystem.ShopConditionTome)//{ shopCustomPrice = Item.buyPrice(platinum: 1) })//{ shopCustomPrice = Item.buyPrice(silver: 10) })
            .Add<AlcaricMush>(MerenaQuestSystem.ShopConditionTome); //{ shopCustomPrice = Item.buyPrice(gold: 2) })
            npcShop.Register(); // Name of this shop tab		
        }

        public override void AI()
        {
            NPC.spriteDirection = NPC.direction;
        }
    }
}