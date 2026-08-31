using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core;
using Stellamod.Helpers;

using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.NPCs.Town
{
    public class Sirestias : VeilTownNPC
    {
        public int NumberOfTimesTalkedTo = 0;
        public const string ShopName = "Shop";
        public const string ShopName2 = "New Shop";
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 60;
        }

        public override void SetDefaults()
        {
            // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 48;
            NPC.height = 79;
            NPC.aiStyle = -1;
            NPC.damage = 9000;
            NPC.defense = 69;
            NPC.lifeMax = 200000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamage = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            SpawnAtPoint = true;
            HasTownDialogue = true;
        }


        //This prevents the NPC from despawning
        public override bool CheckActive()
        {
            return false;
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.5f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Structures/WitchTown";
            spawner.spawnTileOffset = new Point(150, -35 - 38);
        }


    
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // We can use AddRange instead of calling Add multiple times in order to add multiple items at once
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "Your eternal bonding with this individual resonates with everyone throughout!")),
				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(NPC.FullName)
            });
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Sirestias",
            };
        }

        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
            button2 = LangText.Chat(this, "Button");
            button = LangText.Chat(this, "Button2");

        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (!firstButton)
            {

                Player player = Main.LocalPlayer;
                WeightedRandom<string> chat = new WeightedRandom<string>();



                //-----------------------------------------------------------------------------------------------
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Bliss1"));

                if (!DownedBossSystem.downedGintzlBoss)
                {
                    Main.npcChatText = LangText.Chat(this, "Downed1");


                }
                if (DownedBossSystem.downedGintzlBoss)
                {

                    Main.npcChatText = LangText.Chat(this, "Downed2");


                    if (DownedBossSystem.downedSunsBoss)
                    {

                        Main.npcChatText = LangText.Chat(this, "Downed3");



                    }


                    if (DownedBossSystem.downedJackBoss)
                    {

                        Main.npcChatText = LangText.Chat(this, "Downed4");



                    }

                    if (DownedBossSystem.downedJackBoss && DownedBossSystem.downedSunsBoss)
                    {

                        Main.npcChatText = LangText.Chat(this, "Downed5");


                        if (DownedBossSystem.downedDaedusBoss)
                        {

                            Main.npcChatText = LangText.Chat(this, "Downed6");




                            if (DownedBossSystem.downedDreadBoss)
                            {

                                Main.npcChatText = LangText.Chat(this, "Downed7");

                                if (DownedBossSystem.downedSOMBoss)
                                {

                                    Main.npcChatText = LangText.Chat(this, "Downed8");




                                    if (DownedBossSystem.downedVeriBoss)
                                    {

                                        Main.npcChatText = LangText.Chat(this, "Downed9");


                                        if (Main.hardMode)
                                        {
                                            Main.npcChatText = LangText.Chat(this, "Downed10");



                                            if (DownedBossSystem.downedIrradiaBoss)
                                            {

                                                Main.npcChatText = LangText.Chat(this, "Downed11");



                                                if (DownedBossSystem.downedSTARBoss)
                                                {

                                                    Main.npcChatText = LangText.Chat(this, "Downed12");



                                                    if (DownedBossSystem.downedSyliaBoss)
                                                    {

                                                        Main.npcChatText = LangText.Chat(this, "Downed13");

                                                        if (DownedBossSystem.downedZuiBoss)
                                                        {

                                                            Main.npcChatText = LangText.Chat(this, "Downed14");

                                                            if (DownedBossSystem.downedSupernovaFragmentBoss)
                                                            {

                                                                Main.npcChatText = LangText.Chat(this, "Downed15");



                                                                if (DownedBossSystem.downedFenixBoss)
                                                                {

                                                                    Main.npcChatText = LangText.Chat(this, "Downed16");


                                                                    if (DownedBossSystem.downedRekBoss)
                                                                    {

                                                                        Main.npcChatText = LangText.Chat(this, "Downed17");


                                                                        if (DownedBossSystem.downedNiiviBoss)
                                                                        {

                                                                            Main.npcChatText = LangText.Chat(this, "Downed18");



                                                                            if (DownedBossSystem.downedGothBoss)
                                                                            {

                                                                                Main.npcChatText = LangText.Chat(this, "Downed19");





                                                                            }

                                                                        }


                                                                    }




                                                                }



                                                            }


                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                    }

                    return;



                }

























            }

            if (firstButton)
            {


            }
        }

        public override void OpenTownDialogue(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound, List<Tuple<string, Action>> buttons)
        {
            base.OpenTownDialogue(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound, buttons);
            buttons.Add(new Tuple<string, Action>("Talk", Talk));

            portrait = "SirestiasPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "ZuiOpenDialogue1";
        }

        public override void IdleChat(ref string text, ref string portrait, ref float timeBetweenTexts, ref SoundStyle? talkingSound)
        {
            base.IdleChat(ref text, ref portrait, ref timeBetweenTexts, ref talkingSound);
            portrait = "SirestiasPortrait";
            timeBetweenTexts = 0.015f;
            talkingSound = SoundID.Item1;

            //This pulls from the new Dialogue localization
            text = "ZuiIdleChat1";
        }

    }
}