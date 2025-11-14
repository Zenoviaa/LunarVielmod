using Microsoft.Xna.Framework;
using Stellamod.Content.Dialogue;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.TriggersSystem.Triggers;
using Stellamod.Helpers;
using Stellamod.UI.CauldronSystem;
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
            NPCID.Sets.TrailCacheLength[NPC.type] = 5;
            NPCID.Sets.TrailingMode[Type] = 3;
            Main.npcFrameCount[NPC.type] = 5;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.ActsLikeTownNPC[Type] = true;
            NPCID.Sets.SpawnsWithCustomName[Type] = true;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 54;
            NPC.height = 106;
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
            HasTownDialogue = true;
            OnlyInteract = true;
        }

        private void StartDialogue()
        {
            DialogueSystemV2 dialogueSystem = ModContent.GetInstance<DialogueSystemV2>();
            dialogueSystem.StartDialogueSequence(ModContent.GetInstance<BishinineStart>());
        }


        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
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
                "Bishinine",
            };
        }

        public override void AI()
        {
            DrawOffset = new Vector2(0, 8);
            NPC.spriteDirection = -NPC.direction;
            if (NPC.AnyNPCs(ModContent.NPCType<Bishinine>()))
            {
                NPC.Kill();
            }
        }

        public bool CanSpawn()
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Bishinine>()) && !DownedBossSystem.downedBishinineBoss;
        }
    }
}
