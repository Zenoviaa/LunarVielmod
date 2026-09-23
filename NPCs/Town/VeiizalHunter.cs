using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Common.QuestSystem;
using Stellamod.Content.Quests.VeiizalQuest;
using Stellamod.Core;
using Stellamod.Items.Weapons.Melee.Swords;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Thrown;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Town;

// [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.

public class VeiizalHunter : VeilTownNPC
{
    public int NumberOfTimesTalkedTo = 0;
    public const string ShopName = "Shop";
    public const string ShopName2 = "New Shop";
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.npcFrameCount[Type] = 30; // The amount of frames the NPC has
    }

    public override void SetDefaults()
    {
        // Sets NPC to be a Town NPC
        NPC.friendly = true; // NPC Will not attack player
        NPC.width = 74;
        NPC.height = 56;
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
    }

    public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
    {
        spawner.structureToSpawnIn = "Struct/Overworld/VeizalManor";
        spawner.spawnTileOffset = new Point(83, -18);
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
    public override void OpenTownDialogue(ref SpeechBoxTalkingParameters talkingParameters, List<Tuple<string, Action>> buttons)
    {
        base.OpenTownDialogue(ref talkingParameters, buttons);
        talkingParameters.profile = GooberDialoguePresets.Veizal;
        //Set buttons
        buttons.Add(new Tuple<string, Action>("Talk", Talk));
        buttons.Add(new Tuple<string, Action>("Shop", OpenShop));

    }

    public override void SetQuestLine(List<Quest> quests)
    {
        base.SetQuestLine(quests);
        quests.Add(ModContent.GetInstance<HuntI>());
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