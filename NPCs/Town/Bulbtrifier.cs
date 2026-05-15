using Stellamod.Content.Bar.Drinks;
using Stellamod.Content.Dialogue;
using Stellamod.Core;
using Stellamod.Helpers;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Town;

public class Bulbtrifier : VeilTownNPC
{
    public const string ShopName = "Shop";
    public override void SetStaticDefaults()
    {
        // DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
        // DisplayName.SetDefault("Example Person");
        Main.npcFrameCount[Type] = 1; // The amount of frames the NPC has

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
    }

    public override void SetDefaults()
    {
        // Sets NPC to be a Town NPC
        breathe = true;
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 38;
        NPC.height = 38;
        NPC.aiStyle = -1;
        NPC.damage = 90;
        NPC.defense = 42;
        NPC.lifeMax = 200;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        NPC.dontTakeDamageFromHostiles = true;
        HasTownDialogue = true;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    { // What the chat buttons are when you open up the chat UI
        button2 = Language.GetTextValue("LegacyInterface.28");
        button = LangText.Chat(this, "Button");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        if (!firstButton)
        {
            shop = ShopName;
        }
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
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Rysa", "2"))
        });
    }

    public override List<string> SetNPCNameList()
    {
        return new List<string>() {
            "Bulbtrifier",
        };
    }

    public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
    {
        base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
        //Set buttons
        buttons.Add(new Tuple<string, Action>("Talk", Talk));
        buttons.Add(new Tuple<string, Action>("Shop", OpenShop));

        portrait = "BulbtrifierPortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "ZuiOpenDialogue1";
    }

    public override void Talk()
    {
        base.Talk();
        OpenTalkOptions(
            ModContent.GetInstance<BulbtrifierHiDialogue>(),
            ModContent.GetInstance<BulbtrifierWhoDialogue>(),
            ModContent.GetInstance<BulbtrifierHowMuchDialogue>());
    }

    public override void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
    {
        base.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
        portrait = "BulbtrifierPortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "ZuiIdleChat1";
    }

    public override void ModifyActiveShop(string shopName, Item[] items)
    {
        base.ModifyActiveShop(shopName, items);
        int index = 0;
        for(int i = 0; i < items.Length; i++)
        {
            items[i] = new Item();
            items[i].TurnToAir();
        }

        foreach (Item item in DrinkShopSystem.items)
        {
            items[index++] = item;
        }
    }

    public override void AddShops()
    {
        var npcShop = new NPCShop(Type, ShopName);
        npcShop.Register(); // Name of this shop t
    }
}