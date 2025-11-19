using Microsoft.Xna.Framework;
using Stellamod.Content.Dialogue;
using Stellamod.Core;
using Stellamod.Core.DialogueSystem;
using Stellamod.Core.TriggersSystem.Triggers;
using Stellamod.Helpers;
using Stellamod.NPCs;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Dock.BossesDK.Jiitas
{
    public class JiitasIdle : VeilTownNPC,
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
            base.SetDefaults();
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 32;
            NPC.height = 100;
            NPC.damage = 32;
            NPC.defense = 0;
            NPC.lifeMax = 1100;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.npcSlots = 10f;
            NPC.aiStyle = 0;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.dontTakeDamageFromHostiles = true;
            SpawnAtPoint = true;
            HasTownDialogue = true;
        }


        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
            button2 = Language.GetTextValue("LegacyInterface.28");
            button = LangText.Chat(this, "Button");
        }


        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.07f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Jiitas",
            };
        }

        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Struct/Overworld/TheDock";
            spawner.spawnTileOffset = new Point(67, -12);
        }

        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
            //Set buttons
            buttons.Add(new Tuple<string, Action>("Talk", Talk));

            portrait = "JiitasPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "ZuiOpenDialogue1";
        }

        public override void Talk()
        {
            base.Talk();
            OpenTalkOptions(
                ModContent.GetInstance<JiitasStartDialogue>());
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

        public override void AI()
        {
            DrawOffset = new Vector2(0, 8);
            NPC.spriteDirection = NPC.direction;
            if (NPC.AnyNPCs(ModContent.NPCType<Jiitas>()))
            {
                NPC.Kill();
            }
        }

        public bool CanSpawn()
        {
            return !NPC.AnyNPCs(ModContent.NPCType<Jiitas>()) && !DownedBossSystem.downedJiitasBoss;
        }
    }
}
