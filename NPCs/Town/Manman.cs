using Stellamod.Common.ArmorShop.UI;
using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Core;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Town
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.

    public class Manman : VeilTownNPC
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
            NPC.height = 94;
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
            spawner.structureToSpawnIn = "Structures/WitchTown";
            spawner.spawnTileOffset = new Point(28, -20 - 38);
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

        public override bool CanChat()
        {
            return true;
        }

        public override void OpenTownDialogue(ref SpeechBoxTalkingParameters talkingParameters, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref talkingParameters, buttons);
            talkingParameters.profile = GooberDialoguePresets.ManMan;
            //Set buttons
            buttons.Add(new("Talk", Talk));
            buttons.Add(new("ArmorShop", OpenArmorShop));
        }
        private void OpenArmorShop()
        {
            Main.CloseNPCChatOrSign();
            CloseTownDialogue();
            Main.playerInventory = true;
            ArmorShopSystem uiSystem = ModContent.GetInstance<ArmorShopSystem>();
            uiSystem.OpenUI();

        }
    }
}