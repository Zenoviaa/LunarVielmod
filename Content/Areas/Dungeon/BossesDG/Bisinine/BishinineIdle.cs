using Stellamod.Common.DialogueTowning;
using Stellamod.Common.GooberDialogue;
using Stellamod.Content.Dialogue;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.TriggersSystem.Triggers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dungeon.BossesDG.Bisinine
{

    public class BishinineIdle : VeilTownNPC,
        INPCSpawnCondition
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 5;
        }

        public override void SetDefaults()
        {
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 54;
            NPC.height = 106;
            NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 2000;
            NPC.npcSlots = 0;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            HasTownDialogue = true;
            OnlyInteract = true;
        }

        private void StartDialogue()
        {
            DialogueSystemV2 dialogueSystem = ModContent.GetInstance<DialogueSystemV2>();
            dialogueSystem.StartDialogueSequence(ModContent.GetInstance<BishinineStart>());
        }
        public override void OpenTownDialogue(ref SpeechBoxTalkingParameters talkingParameters, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref talkingParameters, buttons);
            talkingParameters.profile = GooberDialoguePresets.Bishinine;
            StartDialogue();
            Main.CloseNPCChatOrSign();
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

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Bishinine",
            };
        }

        public override void AI()
        {
            DrawOffset = new Vector2(0, 8);
            NPC.spriteDirection = -NPC.direction;
            if (NPC.AnyNPCs(ModContent.NPCType<Bishinine>()))
            {
                NPC.active = false;
            }
        }

        public bool CanSpawn()
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Bishinine>()) && !DownedBossSystem.downedBishinineBoss;
        }
    }
}
