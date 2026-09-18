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

namespace Stellamod.Content.Areas.SpringHills.BossesSH.Minerva
{
    public class MinervaIdle : VeilTownNPC,
          INPCSpawnCondition
    {
        private int _frame;
        public override string Texture => this.GetType().DirectoryHere() + "/Minerva";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 46;
        }
        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 100;
            NPC.damage = 32;
            NPC.defense = 10;
            NPC.lifeMax = 1500;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.npcSlots = 10f;

            NPC.friendly = true; // NPC Will not attack player
            NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;

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
            dialogueSystem.StartDialogueSequence(ModContent.GetInstance<MinervaStartDialogue>());
        }


        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
            StartDialogue();
            Main.CloseNPCChatOrSign();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.1f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            if (_frame >= 10)
            {
                _frame = 0;
            }
            NPC.frame.Y = frameHeight * _frame;
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
                "Minerva",
            };
        }

        public override void AI()
        {
            NPC.spriteDirection = -NPC.direction;
            if (NPC.AnyNPCs(ModContent.NPCType<Minerva>()))
            {
                NPC.Kill();
            }
        }

        public bool CanSpawn()
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Minerva>()) && !DownedBossSystem.downedMinervaBoss;
        }
    }
}
