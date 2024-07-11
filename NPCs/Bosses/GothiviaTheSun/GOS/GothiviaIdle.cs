using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets.Biomes;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Vanity.Gia;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Items.Placeable;
using Stellamod.Items.Quest.Merena;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Melee.Safunais;
using Stellamod.Items.Weapons.PowdersItem;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.Items.Weapons.Whips;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.Fenix;
using Stellamod.NPCs.Bosses.GothiviaTheSun.GOS;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS
{
    // [AutoloadHead] and NPC.townNPC are extremely important and absolutely both necessary for any Town NPC to work at all.
    [AutoloadBossHead]
    public class GothiviaIdle : ModNPC
    {
        public enum ActionState
        {








            //--------------------------------------------
            ReallyStartGoth,
            IdleGoth,
            StartGoth,
            Dichotamy,
            Archery,
            BoostBounce1,
            BoostBounce2,
            BoostBounce3,
            SunExplosionCharge1,
            SunExplosionCharge2,
            Suns1,
            Suns2,
            BonfireLeft,
            BonfireRight,
            TheZoomer,
            Kick,
            ExplodeOut,
            StandCuss,
            Desperation,
            Invisible,




        }
        private ActionState _state = ActionState.IdleGoth;

        public int NumberOfTimesTalkedTo = 0;
        public const string ShopName = "Shop";
        public const string ShopName2 = "New Shop";
        public override void SetStaticDefaults()
        {
            // DisplayName automatically assigned from localization files, but the commented line below is the normal approach.
            // DisplayName.SetDefault("Example Person");
            Main.npcFrameCount[Type] = 61; // The amount of frames the NPC has

            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            //To reiterate, since this NPC isn't technically a town NPC, we need to tell the game that we still want this NPC to have a custom/randomized name when they spawn.
            //In order to do this, we simply make this hook return true, which will make the game call the TownNPCName method when spawning the NPC to determine the NPC's name.
            NPCID.Sets.SpawnsWithCustomName[Type] = true;


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





            ; // < Mind the semicolon!
        }
        public bool ThreeQ = false;
        public bool FourQ = false;
        public bool NoWings = false;

        // Current state
        public float squish = 0f;
        private int _wingFrameCounter;
        private int _wingFrameTick;

        // Current frame
        public int frameCounter;
        // Current frame's progress
        public int frameTick;
        // Current state's timer
        public float timer;

        // AI counter
        public int counter;
        public override void SetDefaults()
        {
            // Sets NPC to be a Town NPC
            NPC.friendly = true; // NPC Will not attack player
            NPC.width = 106;
            NPC.height = 92;
            NPC.aiStyle = -1;
            NPC.damage = 90;
            NPC.defense = 42;
            NPC.lifeMax = 9000;
            NPC.knockBackResist = 0.5f;
            NPC.npcSlots = 0;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
        }
        public override void FindFrame(int frameHeight)
        {
            /*
            NPC.frameCounter += 1f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;*/
        }

        public override bool CanChat()
        {
            return true;
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
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundJungle,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "You sense a strange godly prescence coming from Gothivia")),

				// You can add multiple elements if you really wanted to
				// You can also use localization keys (see Localization/en-US.lang)
				new FlavorTextBestiaryInfoElement(LangText.Bestiary(this, "???", "2"))
            });
        }
        public override ITownNPCProfile TownNPCProfile()
        {
            return new GothiviaPersonProfie();
        }

        public class GothiviaPersonProfie : ITownNPCProfile
        {
            public int RollVariation() => 0;
            public string GetNameForVariant(NPC npc) => npc.getNewNPCName();

            public Asset<Texture2D> GetTextureNPCShouldUse(NPC npc)
            {
                if (npc.IsABestiaryIconDummy && !npc.ForcePartyHatOn)
                    return ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/GothiviaIdle");

                if (npc.altTexture == 1)
                    return ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/GothiviaIdle_Head");

                return ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/GothiviaIdle");
            }

            public int GetHeadTextureIndex(NPC npc) => ModContent.GetModHeadSlot("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/GothiviaIdle_Head");
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();

            int partyGirl = NPC.FindFirstNPC(NPCID.Steampunker);

            // These are things that the NPC has a chance of telling you when you talk to it.
            chat.Add("...");
            chat.Add(LangText.Chat(this, "Basic1"));
            chat.Add(LangText.Chat(this, "Basic2"));
            chat.Add(LangText.Chat(this, "Basic3"), 1.0);
            chat.Add(LangText.Chat(this, "Basic4"), 1.0);


            NumberOfTimesTalkedTo++;
            if (NumberOfTimesTalkedTo >= 10)
            {
                //This counter is linked to a single instance of the NPC, so if ExamplePerson is killed, the counter will reset.
                chat.Add("...");
            }

            return chat; // chat is implicitly cast to a string.
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>() {
                "Gothivia The Enraged",
                "Gothivia The Enraged"

            };
        }

        // The PreDraw hook is useful for drawing things before our sprite is drawn or running code before the sprite is drawn
        // Returning false will allow you to manually draw your NPC
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {

            Vector2 size = new Vector2(166, 96);


            Player player = Main.player[NPC.target];
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects effects = SpriteEffects.None;

            if (ThreeQ && player.Center.X > NPC.Center.X)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            if (FourQ)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            Rectangle rect;


            Vector2 drawPosition = NPC.Center - screenPos;
            Vector2 origin = new Vector2(83, 48);
            Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/Gwings4Q").Value;
            int wingFrameSpeed = 1;
            int wingFrameCount = 60;
            spriteBatch.Draw(syliaWingsTexture, drawPosition,
                syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
                drawColor, NPC.rotation, origin, 2f, effects, 0f);

            ///Animation Stuff for Verlia
            /// 1 - 2 Summon Start
            /// 3 - 7 Summon Idle / Idle
            /// 8 - 11 Summon down
            /// 12 - 19 Hold UP
            /// 20 - 30 Sword UP
            /// 31 - 35 Sword Slash Simple
            /// 36 - 45 Hold Sword
            /// 46 - 67 Barrage 
            /// 68 - 75 Explode
            /// 76 - 80 Appear
            /// 133 width
            /// 92 height


            ///Animation Stuff for Veribloom
            /// 1 = Idle
            /// 2 = Blank
            /// 2 - 8 Appear Pulse
            /// 9 - 19 Pulse Buff Att
            /// 20 - 26 Disappear Pulse
            /// 27 - 33 Appear Winding
            /// 34 - 38 Wind Up
            /// 39 - 45 Dash
            /// 46 - 52 Slam Appear
            /// 53 - 58 Slam
            /// 59 - 64 Spin
            /// 80 width
            /// 89 height
            /// 

            /// 1 = Idle
            /// 1 - 4 Jump Startup
            /// 5 - 8 Jump
            /// 9 - 12 land
            /// 13 - 29 Doublestart
            /// 30 - 42 Tiptoe



            switch (_state)
            {










                //------------------------ Gothivia




                case ActionState.ReallyStartGoth:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 7, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;


                case ActionState.IdleGoth:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 7, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.StartGoth:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 7, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.Desperation:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.Suns1:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.Suns2:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.SunExplosionCharge1:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.SunExplosionCharge2:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.Invisible:
                    rect = new(0, 54 * 96, 166, 1 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.BonfireLeft:
                    rect = new(0, 54 * 96, 166, 1 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.BonfireRight:
                    rect = new(0, 54 * 96, 166, 1 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.TheZoomer:
                    rect = new(0, 55 * 96, 166, 1 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 400, 1, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.BoostBounce1:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.BoostBounce2:
                    rect = new(0, 57 * 96, 166, 4 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 4, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.BoostBounce3:
                    rect = new(0, 38 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.Kick:
                    rect = new(0, 38 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 25, 7, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.StandCuss:
                    rect = new(0, 57 * 96, 166, 4 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 4, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.Dichotamy:
                    rect = new(0, 1 * 96, 166, 14 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 14, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.Archery:
                    rect = new(0, 24 * 96, 166, 13 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 13, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;

                case ActionState.ExplodeOut:
                    rect = new(0, 46 * 96, 166, 9 * 96);
                    spriteBatch.Draw(texture, NPC.Center - screenPos, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 9, rect), drawColor, NPC.rotation, NPC.frame.Size() / 2, 2f, effects, 0f);
                    break;
            }


            return false;
        }


        public override void SetChatButtons(ref string button, ref string button2)
        { // What the chat buttons are when you open up the chat UI
            button = LangText.Chat(this, "Button");

        }
        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {


            //-----------------------------------------------------------------------------------------------

            if (firstButton)
            {
                if (DownedBossSystem.downedRekBoss)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y - 5,
                            ModContent.NPCType<StartGoth>());
                    }
                    else
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                            return;

                        StellaMultiplayer.SpawnBossFromClient((byte)Main.LocalPlayer.whoAmI,
                            ModContent.NPCType<StartGoth>(), (int)NPC.Center.X, (int)NPC.Center.Y - 5);
                    }
                }
                else
                {
                    Main.npcChatText = LangText.Chat(this, "Special1");
                }
            }
        }


        public override void HitEffect(NPC.HitInfo hit)
        {
            int num = NPC.life > 0 ? 1 : 5;
            for (int k = 0; k < num; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.GreenBlood);
            }
        }


        public override void AI()
        {
            NPC.TargetClosest();
            NPC.spriteDirection = NPC.direction;
            Player target = Main.player[NPC.target];

            if (NPC.AnyNPCs(ModContent.NPCType<GothiviaIyx>()))
            {
                NPC.Kill();
            }

            Vector3 RGB = new(2.30f, 2.21f, 2.72f);
            Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
        }
    }
}