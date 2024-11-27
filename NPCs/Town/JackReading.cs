using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.JackTheScholar;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.NPCs.Town
{
    internal class JackReading : VeilTownNPC
    {
        private int _frame;
        public int NumberOfTimesTalkedTo = 0;
        public override string Texture => "Stellamod/NPCs/Bosses/JackTheScholar/JackTheScholar";
        public override void SetStaticDefaults()
        {
            // DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
            // DisplayName.SetDefault("Example Person");
            Main.npcFrameCount[Type] = 28; // The amount of frames the NPC has

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

            // Set Example Person's biome and neighbor preferences with the NPCHappiness hook. You can add happiness text and remarks with localization (See an example in ExampleMod/Localization/en-US.lang).
        }

        public override void SetPointSpawnerDefaults(ref NPCPointSpawner spawner)
        {
            spawner.structureToSpawnIn = "Struct/Huntria/FableBiomeFinal";
            spawner.spawnTileOffset = new Point(190, -70);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 54;
            NPC.height = 65;
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
            SpawnAtPoint = true;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);

            //Animation Speed
            NPC.frameCounter += 0.15f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }
            if (_frame >= 4f)
            {
                _frame = 0;
            }

            NPC.frame.Y = frameHeight * _frame;
        }

        public override bool CanChat()
        {
            return true;
        }

        //This prevents the NPC from despawning
        public override bool CheckActive()
        {
            return true;
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

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add(LangText.Chat(this, "Basic1"));


            NumberOfTimesTalkedTo++;
            if (NumberOfTimesTalkedTo >= 10)
            {
                //This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
                chat.Add(LangText.Chat(this, "Basic2"));
            }

            return chat; // chat is implicitly cast to a string.
        }


        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Jack the Scholar",
            };
        }


        public override void SetChatButtons(ref string button, ref string button2)
        {
            // What the chat buttons are when you open up the chat UI
           // button2 = Language.GetTextValue("LegacyInterface.28");
            button = LangText.Chat(this, "Button");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Main.NewText(LangText.Chat(this, "Challenge"), Color.Gold);
                    NPC npc = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.position.X, (int)NPC.position.Y,
                        ModContent.NPCType<JackTheScholar>());
                    npc.netUpdate = true;
                }
                else
                {
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        return;

                    StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI,
                        ModContent.NPCType<JackTheScholar>(), (int)NPC.position.X, (int)NPC.position.Y);
                }

                //Spawn Boss
                NPC.Kill();
            }
        }
    }
}
