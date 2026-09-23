using Microsoft.Xna.Framework;
using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Content.Areas.Shop.AccShop;
using Stellamod.Content.Dialogue;
using Stellamod.Content.Vanity.AcademyOutfit;
using Stellamod.Core;
using Stellamod.Helpers;
using Stellamod.Items.Insources;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Town;

// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.

public class Rysa : VeilTownNPC
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
        NPC.friendly = true;
        NPC.width = 38;
        NPC.height = 50;
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

    public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
    {
        spawner.structureToSpawnIn = "Structures/Rysahouse";
        spawner.spawnTileOffset = new Point(24, -11);
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

    public override void OpenTownDialogue(ref SpeechBoxTalkingParameters talkingParameters, List<Tuple<string, Action>> buttons)
    {
        base.OpenTownDialogue(ref talkingParameters, buttons);
        talkingParameters.profile = GooberDialoguePresets.Rysa;
        //Set buttons
        buttons.Add(new Tuple<string, Action>("Talk", Talk));
        buttons.Add(new Tuple<string, Action>("Shop", OpenShop));
    }

    public override void Talk()
    {
        base.Talk();
        OpenTalkOptions(
            ModContent.GetInstance<RysaGotAnythingDialogue>(), 
            ModContent.GetInstance<RysaLivingDialogue>());
    }

    public override void AddShops()
    {
        var npcShop = new NPCShop(Type, ShopName)
         .Add(new Item(ModContent.ItemType<AcademyOutfitHead>())
         {
             shopCustomPrice = 2,
             shopSpecialCurrency = Stellamod.MedalCurrencyID
         })
        .Add(new Item(ModContent.ItemType<AcademyOutfitRobe>())
        {
            shopCustomPrice = 2,
            shopSpecialCurrency = Stellamod.MedalCurrencyID
        })
        .Add(new Item(ModContent.ItemType<AcademyOutfitLegs>())
        {
            shopCustomPrice = 2,
            shopSpecialCurrency = Stellamod.MedalCurrencyID
        });
        npcShop.Register();
    }
}