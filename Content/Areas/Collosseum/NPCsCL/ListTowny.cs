using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Content.Areas.Shop.AccShop;
using Stellamod.Content.Dialogue;
using Stellamod.Content.Vanity.RedFeatherHat;
using Stellamod.Core;
using Stellamod.Items.Insources;
using Stellamod.NPCs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.NPCsCL;

public class ListTowny : VeilTownNPC
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 1; // The amount of frames the NPC has
    }

    public override void SetDefaults()
    {
        // Sets NPC to be a Town NPC
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 38;
        NPC.height = 50;
        NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
        NPC.damage = 90;
        NPC.defense = 42;
        NPC.lifeMax = 200;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.knockBackResist = 0.5f;
        NPC.dontTakeDamageFromHostiles = true;
        SpawnAtPoint = true;
        HasTownDialogue = true;
        breathe = true;
    }

    public override void SetChatButtons(ref string button, ref string button2)
    {
        button2 = Language.GetTextValue("LegacyInterface.28");
        button = LangText.Chat(this, "Button");
    }

    public override void OnChatButtonClicked(bool firstButton, ref string shop)
    {
        if (!firstButton)
        {
            shop = "Shop";
        }
    }

    public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
    {
        spawner.structureToSpawnIn = "Structures/ListsHouse";
        spawner.spawnTileOffset = new Point(10, -5);
    }

    public override bool CheckActive()
    {
        return false;
    }

    public override void OpenTownDialogue(ref SpeechBoxTalkingParameters talkingParameters, List<Tuple<string, Action>> buttons)
    {
        base.OpenTownDialogue(ref talkingParameters, buttons);
        talkingParameters.profile = GooberDialoguePresets.List;
        //Set buttons
        buttons.Add(new("Talk", Talk));
        buttons.Add(new("Shop", OpenShop));

    }

    public override void Talk()
    {
        base.Talk();
        OpenTalkOptions(
            ModContent.GetInstance<ListUmDialogue>(),
            ModContent.GetInstance<ListWhyHereDialogue>(),
            ModContent.GetInstance<ListZuiDialogue>(),
            ModContent.GetInstance<ListAloneDialogue>());
    }

    public override void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
    {
        base.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
        portrait = "ListPortrait";
        timeBetweenTexts = 0.015f;
        talkingSound = SoundID.Item1;

        //This pulls from the new Dialogue localization
        text = "ZuiIdleChat1";
    }

    public override void AddShops()
    {
        var npcShop = new NPCShop(Type, "Shop")
        .Add(new Item(ModContent.ItemType<DesertMap>())
        {
            shopCustomPrice = 20,
            shopSpecialCurrency = Stellamod.MedalCurrencyID
        })
        .Add(new Item(ItemID.SandBoots)
        {
            shopCustomPrice = 20,
            shopSpecialCurrency = Stellamod.MedalCurrencyID
        })
        .Add(new Item(ModContent.ItemType<GreenCarpet>())
        {
            shopCustomPrice = 20,
            shopSpecialCurrency = Stellamod.MedalCurrencyID
        })
        .Add(new Item(ModContent.ItemType<WindingInsource>())
        {
            shopCustomPrice = 20,
            shopSpecialCurrency = Stellamod.MedalCurrencyID
        })
        .Add(new Item(ModContent.ItemType<PaperPaws>())
        {
            shopCustomPrice = 20,
            shopSpecialCurrency = Stellamod.MedalCurrencyID
        })
        .Add(new Item(ModContent.ItemType<TravelersBackpack>())
        {
            shopCustomPrice = 20,
            shopSpecialCurrency = Stellamod.MedalCurrencyID
        })
        .Add(new Item(ModContent.ItemType<RedFeatherHat>())
        {
            shopCustomPrice = 2,
            shopSpecialCurrency = Stellamod.MedalCurrencyID
        }); ;
        npcShop.Register(); // Name of this shop t
    }
}
