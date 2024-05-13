using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Brooches;
using Stellamod.Items.Armors.Vanity.Gothivia;
using Stellamod.Items.Consumables;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Weapons.Igniters;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using Stellamod.Items.Weapons.Thrown;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.GothiviaNRek.Reks;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc.Projectiles;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles;
using Stellamod.NPCs.Bosses.Verlia.Projectiles;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS
{
    [AutoloadBossHead] // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
    public class GothiviaIyx : ModNPC
    {
        private bool _resetTimers;
        public enum ActionState
        {


        
            
            CallHavoc,
            StartIrr,
            Blastout,
            FallingBlast,
            HideIrr,
            STARTNODES,
            STARTAXE,
            STARTSPIKE,
            STARTLASER,
            ReallyStartIrr,
            LandIrr,



            //--------------------------------------------
            ReallyStartGoth,
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
        private ActionState _state = ActionState.ReallyStartGoth;
        // Current state
        public int Jumpin = 0;
        public ActionState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                if (StellaMultiplayer.IsHost)
                    NPC.netUpdate = true;
            }
        }

        // Current frame
        public int frameCounter;
        // Current frame's progress
        public int frameTick;
        // Current state's timer
        public float timer;

        // AI counter
        public int counter;

        public bool ThreeQ = false;
        public bool FourQ = false;
        public bool NoWings = false;

        public int rippleCount = 20;
        public int rippleSize = 5;
        public int rippleSpeed = 15;
        public float distortStrength = 300f;
        public float GothiviaStartPosTime;
        public Vector2 GothiviaStartPos;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Verlia of The Moon");

            Main.npcFrameCount[Type] = 61;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            // Influences how the NPC looks in the Bestiary

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                PortraitScale = 0.8f, // Portrait refers to the full picture when clicking on the icon in the bestiary
                PortraitPositionYOverride = 0f,
            };

            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.Size = new Vector2(44, 80);
            NPC.damage = 1;
            NPC.defense = 130;
            NPC.lifeMax = 190000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.value = Item.buyPrice(gold: 99);
            NPC.boss = true;
            NPC.npcSlots = 10f;
            NPC.scale = 1f;
            NPC.BossBar = ModContent.GetInstance<IrradiaBossBar>();

            // Take up open spawn slots, preventing random NPCs from spawning during the fight

            // Don't set immunities like this as of 1.4:
            // NPC.buffImmune[BuffID.Confused] = true;
            // immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            // Custom boss bar

            // The following code assigns a music track to the boss in a simple way.
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Gothivia");
            }
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("Empress of the Green sun and nature. Everything empowering and living falls under her reign.")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write((float)_state);
            writer.Write(_resetTimers);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            _state = (ActionState)reader.ReadSingle();
            _resetTimers = reader.ReadBoolean();
        }

        bool axed = false;
        bool p2 = false;

        public float squish = 0f;
        private int _wingFrameCounter;
        private int _wingFrameTick;

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 20; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.CoralTorch, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Player player = Main.player[NPC.target];
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteEffects effects = SpriteEffects.None;

            if (player.Center.X > NPC.Center.X)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Rectangle rect;
            originalHitbox = new Vector2(NPC.width / 100, NPC.height / 2) + new Vector2(120, 50);

            if (player.Center.X > NPC.Center.X)
            {
                effects = SpriteEffects.FlipHorizontally;
            }

            Color drawColors = NPC.GetAlpha(Color.White);
            ;


            if (ThreeQ && !FourQ && !NoWings)
            {
                Vector2 drawPosition = NPC.Center - screenPos;
                Vector2 origin = new Vector2(144, 82);
                Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/Gwings3Q").Value;
                int wingFrameSpeed = 1;
                int wingFrameCount = 60;
                spriteBatch.Draw(syliaWingsTexture, drawPosition,
                    syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
                    drawColor, 0f, origin, 1f, effects, 0f);

            }


            if (FourQ && !ThreeQ && !NoWings)
            {
                Vector2 drawPosition = NPC.Center - screenPos;
                Vector2 origin = new Vector2(144, 82);
                Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/Gwings4Q").Value;
                int wingFrameSpeed = 1;
                int wingFrameCount = 60;
                spriteBatch.Draw(syliaWingsTexture, drawPosition,
                    syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
                    drawColor, 0f, origin, 1f, effects, 0f);

            }

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



            switch (State)
            {
                

                

            




                case ActionState.ReallyStartIrr:
                    rect = new(0, 32 * 146, 206, 1 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.StartIrr:
                    rect = new(0, 32 * 146, 206, 1 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.HideIrr:
                    rect = new(0, 32 * 146, 206, 1 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.Blastout:
                    rect = new(0, 18 * 146, 206, 5 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 5, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.FallingBlast:
                    rect = new(0, 24 * 146, 206, 1 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.STARTNODES:
                    rect = new(0, 32 * 146, 206, 1 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.STARTAXE:
                    rect = new(0, 32 * 146, 206, 1 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.STARTLASER:
                    rect = new(0, 32 * 146, 206, 1 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.STARTSPIKE:
                    rect = new(0, 32 * 146, 206, 1 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 1, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.CallHavoc:
                    rect = new(0, 1 * 146, 206, 17 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 4, 17, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;

                case ActionState.LandIrr:
                    rect = new(0, 26 * 146, 206, 5 * 146);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 5, rect), drawColor, 0f, Vector2.Zero, 1f, effects, 0f);
                    break;










                //------------------------ Gothivia




                case ActionState.ReallyStartGoth:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter,  ref frameTick, 7, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;



                case ActionState.StartGoth:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter,  ref frameTick, 7, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.Desperation:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.Suns1:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.Suns2:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.SunExplosionCharge1:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.SunExplosionCharge2:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.Invisible:
                    rect = new(0, 55 * 96, 166, 1 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.BonfireLeft:
                    rect = new(0, 55 * 96, 166, 1 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.BonfireRight:
                    rect = new(0, 55 * 96, 166, 1 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 200, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.TheZoomer:
                    rect = new(0, 56 * 96, 166, 1 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 400, 1, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.BoostBounce1:
                    rect = new(0, 16 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.BoostBounce2:
                    rect = new(0, 57 * 96, 166, 4 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 7, 4, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.BoostBounce3:
                    rect = new(0, 38 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter,  ref frameTick, 7, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.Kick:
                    rect = new(0, 38 * 96, 166, 7 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter,  ref frameTick, 7, 7, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.StandCuss:
                    rect = new(0, 57 * 96, 166, 4 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 4, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.Dichotamy:
                    rect = new(0, 1 * 96, 166, 14 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 14, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.Archery:
                    rect = new(0, 24 * 96, 166, 13 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 5, 13, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;

                case ActionState.ExplodeOut:
                    rect = new(0, 46 * 96, 166, 9 * 96);
                    spriteBatch.Draw(texture, NPC.position - screenPos - originalHitbox, texture.AnimationFrame(ref frameCounter, ref frameTick, 8, 9, rect), drawColor, 0f, Vector2.Zero, 2f, effects, 0f);
                    break;
            }


            return false;
        }

        //Custom function so that I don't have to copy and paste the same thing in FindFrame
        int bee = 220;
        private Vector2 originalHitbox;
        //int Timer2 = 0;
        float timert = 0;
        public float Spawner = 0;

        public bool Elect = false;
        public override void AI()
        {
            p2 = NPC.life < NPC.lifeMax * 0.5f;
            bee--;
            //Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 10f);
            NPC.damage = 0;
            GothiviaStartPosTime++;

            if (GothiviaStartPosTime <= 1)
            {

                GothiviaStartPos = NPC.position;

            }

           




            if (bee == 0)
            {
                bee = 220;
            }

            Vector3 RGB = new(2.30f, 0.21f, 0.72f);
            Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
            NPC.spriteDirection = NPC.direction;

            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget)
                {               // If the targeted player is dead, flee
                    NPC.velocity.Y += 0.5f;
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;
                    // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                    NPC.EncourageDespawn(1);
                }
            }

            FinishResetTimers();
            switch (State)
            {
                //Gothivia Stuff here



                case ActionState.ReallyStartGoth:
                    NPC.damage = 0;
                    counter++;
                    FourQ = true;
                    ReallyIdleGoth();
                    NPC.aiStyle = -1;
                    break;




                //----------- Irradia stuff under

                case ActionState.ReallyStartIrr:
                    NPC.damage = 0;
                    counter++;

                    ReallyIdleIrr();
                    NPC.aiStyle = -1;
                    break;


                case ActionState.StartIrr:
                    NPC.damage = 0;
                    counter++;

                    StartIrr();
                    NPC.aiStyle = -1;
                    break;


                case ActionState.Blastout:
                    NPC.damage = 0;
                    counter++;

                    Blastout();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.CallHavoc:
                    NPC.damage = 0;
                    counter++;

                    CallingHavoc();
                    NPC.aiStyle = -1;
                    break;


                case ActionState.FallingBlast:
                    NPC.damage = 0;
                    counter++;
                    NPC.velocity.Y *= 1.01f;
                    NPC.velocity.X *= 0.96f;
                    NPC.aiStyle = -1;
                    NPC.noTileCollide = false;
                    if (NPC.velocity.Y == 0)
                    {

                        State = ActionState.LandIrr;
                        frameCounter = 0;
                        frameTick = 0;
                    }
                    // You dont need to do anything here
                    break;


                case ActionState.LandIrr:
                    NPC.damage = 0;
                    counter++;

                    LandIrr();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.STARTLASER:
                    NPC.damage = 0;
                    counter++;

                    LASERIRR();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.STARTAXE:
                    NPC.damage = 0;
                    counter++;

                    AXEIRR();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.STARTSPIKE:
                    NPC.damage = 0;
                    counter++;

                    SPIKEIRR();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.STARTNODES:
                    NPC.damage = 0;
                    counter++;

                    ELECTRICIRR();
                    NPC.aiStyle = -1;
                    break;
            }
        }






        //-------------- Goth AI stuff

        private void ReallyIdleGoth()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;

            if (timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {

                }
            }



            if (timer == 60)
            {
                ResetTimers();
                State = ActionState.StartGoth;
            }
        }


        private void IdleGoth()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;

            if (timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {

                }
            }
           
            if (timer < 50)
            {
                NPC.velocity.Y -= 0.07f;
            }


            if (timer == 60)
            {
                ResetTimers();
                switch (Main.rand.Next(3))
                {
                    case 0:
                        State = ActionState.Dichotamy;
                        break;

                    case 1:
                        State = ActionState.BoostBounce1;
                        break;

                    case 2:
                        State = ActionState.Archery;

                        break;


                }
            }
        }




        private void Dichotamy()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;

            if (timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {

                }
            }

            if (timer == 80)
            {
                if (StellaMultiplayer.IsHost)
                {

                }
            }


            if (timer == 60)
            {
                ResetTimers();
                switch (Main.rand.Next(3))
                {
                    case 0:
                        State = ActionState.Dichotamy;
                        break;

                    case 1:
                        State = ActionState.BoostBounce1;
                        break;

                    case 2:
                        State = ActionState.Archery;

                        break;


                }
            }
        }














        // ----------------- Irradia AI
        private void ReallyIdleIrr()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;

            if (timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {

                }
            }



            if (timer == 2)
            {
                ResetTimers();
                State = ActionState.StartIrr;
            }
        }

        private void StartIrr()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;


            if (timer == 50)
            {
                ResetTimers();
                State = ActionState.CallHavoc;
            }
        }

        private void CallingHavoc()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player player = Main.player[NPC.target];
            if (timer == 1)
            {
                if (Elect)
                {
                    if (StellaMultiplayer.IsHost)
                    {
                        float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                        float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, 4f);
                        float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), player.position.X, player.position.Y, speedXa * 0, speedYa * 0,
                            ModContent.ProjectileType<IrradiaElectricBoxConnectorProj>(), 24, 0f, Owner: Main.myPlayer);

                    }

                    Elect = false;
                }



               
            }


            if (timer == 68)
            {
                ResetTimers();
                State = ActionState.Blastout;
            }
        }

        private void Blastout()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;

            if (timer == 45)
            {
                if (Jumpin < 1)
                {
                    switch (Main.rand.Next(6))
                    {
                        case 0:
                            NPC.velocity.Y -= 6.0f;
                            NPC.velocity.X -= 18;

                            Jumpin = 2;
                            break;

                        case 1:
                            NPC.velocity.Y -= 5.0f;
                            NPC.velocity.X -= 18;


                            Jumpin = 1;
                            break;

                        case 2:
                            NPC.velocity.Y -= 10.0f;
                            NPC.velocity.X -= 18;

                            Jumpin = 3;

                            break;


                        case 3:
                            NPC.velocity.Y -= 6.0f;
                            NPC.velocity.X += 18;

                            Jumpin = 5;
                            break;

                        case 4:
                            NPC.velocity.Y -= 5.0f;
                            NPC.velocity.X += 18;


                            Jumpin = 4;
                            break;

                        case 5:
                            NPC.velocity.Y -= 10.0f;
                            NPC.velocity.X += 18;

                            Jumpin = 6;

                            break;
                    }



                }





                if (Jumpin == 1)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            NPC.velocity.Y -= 6.0f;
                            NPC.velocity.X += 20;

                            Jumpin = 2;
                            break;

                        case 1:
                            NPC.velocity.Y -= 5.0f;
                            NPC.velocity.X += 18;


                            Jumpin = 0;
                            break;

                        case 2:
                            NPC.velocity.Y -= 10.0f;
                            NPC.velocity.X += 30;

                            Jumpin = 6;

                            break;
                    }



                }

                if (Jumpin == 2)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            NPC.velocity.Y += 9.0f;
                            NPC.velocity.X += 30;

                            Jumpin = 1;
                            break;

                        case 1:
                            NPC.velocity.Y += 6.0f;
                            NPC.velocity.X += 18;


                            Jumpin = 0;
                            break;

                        case 2:
                            NPC.velocity.Y += 1.0f;
                            NPC.velocity.X += 30;

                            Jumpin = 4;

                            break;
                    }



                }

                if (Jumpin == 3)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            NPC.velocity.Y += 9.0f;
                            NPC.velocity.X += 20;

                            Jumpin = 1;
                            break;

                        case 1:
                            NPC.velocity.Y -= 2f;
                            NPC.velocity.X += 30;


                            Jumpin = 6;
                            break;

                        case 2:
                            NPC.velocity.Y += 3.0f;
                            NPC.velocity.X += 30;

                            Jumpin = 5;

                            break;
                    }



                }

                if (Jumpin == 4)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            NPC.velocity.Y -= 12.0f;
                            NPC.velocity.X -= 0;

                            Jumpin = 5;
                            break;

                        case 1:
                            NPC.velocity.Y -= 5.0f;
                            NPC.velocity.X -= 30;


                            Jumpin = 0;
                            break;

                        case 2:
                            NPC.velocity.Y -= 20.0f;
                            NPC.velocity.X -= 30;

                            Jumpin = 3;

                            break;
                    }



                }

                if (Jumpin == 5)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            NPC.velocity.Y += 9.0f;
                            NPC.velocity.X -= 0;

                            Jumpin = 4;
                            break;

                        case 1:
                            NPC.velocity.Y += 12.0f;
                            NPC.velocity.X -= 20;


                            Jumpin = 0;
                            break;

                        case 2:
                            NPC.velocity.Y += 4.0f;
                            NPC.velocity.X -= 30;

                            Jumpin = 1;

                            break;
                    }



                }



                if (Jumpin == 6)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            NPC.velocity.Y += 18.0f;
                            NPC.velocity.X -= 0;

                            Jumpin = 4;
                            break;

                        case 1:
                            NPC.velocity.Y += 0f;
                            NPC.velocity.X -= 30;


                            Jumpin = 3;
                            break;

                        case 2:
                            NPC.velocity.Y += 3.0f;
                            NPC.velocity.X -= 30;

                            Jumpin = 2;

                            break;
                    }



                }








            }


            if (timer == 50)
            {
                ResetTimers();
                State = ActionState.FallingBlast;
            }
        }

        private void LandIrr()
        {
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            // Maybe a land effect or projectile?
            // 
            float ai1 = NPC.whoAmI;
            NPC.velocity.X *= 0;
            timer++;
            if (timer == 49)
            {

                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXb, NPC.position.Y - 250, speedXb - 2 * 2, speedYb - 2 * 2,
                        ModContent.ProjectileType<IrradiaBuilds>(), 1, 0f, Main.myPlayer, 0f, ai1);

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BuildingSomething"), NPC.position);
                }

            }


            if (timer == 50)
            {

                switch (Main.rand.Next(6))
                {
                    case 0:
                        State = ActionState.STARTAXE;
                        break;

                    case 1:
                        State = ActionState.STARTLASER;
                        break;

                    case 2:
                        State = ActionState.STARTNODES;

                        break;

                    case 3:
                        State = ActionState.STARTSPIKE;

                        break;

                    case 4:
                        State = ActionState.STARTNODES;

                        break;

                    case 5:
                        State = ActionState.STARTSPIKE;

                        break;
                }
                ResetTimers();

            }
        }


        private void LASERIRR()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            if (timer == 60)
            {
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                    float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(4f, 4f);
                    float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 0.3f, speedYa - 1 * 0,
                        ModContent.ProjectileType<IrradiaLaserBoxProj>(), 39, 0f, Owner: Main.myPlayer);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXb * 0.3f, speedYa - 1 * 0,
                        ModContent.ProjectileType<IrradiaLaserBoxProj>(), 39, 0f, Owner: Main.myPlayer);
                }
            }






            if (timer == 240)
            {


                ResetTimers();
                State = ActionState.StartIrr;

            }
        }


        private void AXEIRR()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            if (timer == 1)
            {
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                    float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, 4f);
                    float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.position.X + speedXa, NPC.position.Y + speedYa, speedXa * 0.5f, speedYa - 1 * 0,
                        ModContent.ProjectileType<IrradiaAxeProj>(), 26, 0f, Owner: Main.myPlayer);

                }
            }

            if (timer == 420)
            {


                ResetTimers();
                State = ActionState.StartIrr;

            }
        }
        public int recharge = 0;
        private void SPIKEIRR()
        {
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            timer++;

            recharge++;

            if (recharge == 120)
            {
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                    float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, 4f);
                    float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), player.position.X, player.position.Y, speedXa * 0, speedYa * 0,
                        ModContent.ProjectileType<Noder>(), 1, 0f, Owner: Main.myPlayer);

                }

                recharge = 0;
            }

            if (timer == 241)
            {

                recharge = 0;
                ResetTimers();
                State = ActionState.StartIrr;

            }
        }


        private void ELECTRICIRR()
        {
            NPC.spriteDirection = NPC.direction;
            Player player = Main.player[NPC.target];
            timer++;

            recharge++;

            if (recharge == 40)
            {
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, -4f);
                    float speedXa = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(-4f, 4f);
                    float speedYa = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), player.position.X, player.position.Y, speedXa * 0, speedYa * 0,
                        ModContent.ProjectileType<NoderElectric>(), 1, 0f, Owner: Main.myPlayer);

                }

                recharge = 0;
            }


            Elect = true;
            if (timer == 240)
            {

                recharge = 0;
                ResetTimers();
                State = ActionState.StartIrr;

            }
        }


        


       

       

        

       

       









        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Do NOT misuse the ModifyNPCLoot and OnKill hooks: the former is only used for registering drops, the latter for everything else

            // Add the treasure bag using ItemDropRule.BossBag (automatically checks for expert mode)
            //	npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<MinionBossBag>()));




            // ItemDropRule.MasterModeCommonDrop for the relic

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 13, 25));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<ManifestedLove>(), 1, 1, 1));
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<GothiviaBag>()));
            // ItemDropRule.MasterModeDropOnAllPlayers for the pet
            //npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<MinionBossPetItem>(), 4));

            // All our drops here are based on "not expert", meaning we use .OnSuccess() to add them into the rule, which then gets added
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.OneFromOptions(1,
                ModContent.ItemType<BurningGBroochA>(),
                ModContent.ItemType<GothiviasCard>(),
                ModContent.ItemType<BurnBlast>(),
                ModContent.ItemType<WeddingDay>()));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Plate>(), minimumDropped: 200, maximumDropped: 1300));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), minimumDropped: 4, maximumDropped: 55));

            // Notice we use notExpertRule.OnSuccess instead of npcLoot.Add so it only applies in normal mode
            // Boss masks are spawned with 1/7 chance
            //notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<MinionBossMask>(), 7));

            // This part is not required for a boss and is just showcasing some advanced stuff you can do with drop rules to control how items spawn
            // We make 12-15 ExampleItems spawn randomly in all directions, like the lunar pillar fragments. Hereby we need the DropOneByOne rule,
            // which requires these parameters to be defined
            //int itemType = ModContent.ItemType<Gambit>();
            //var parameters = new DropOneByOne.Parameters()
            //{
            //	ChanceNumerator = 1,
            //	ChanceDenominator = 1,
            //	MinimumStackPerChunkBase = 1,
            //	MaximumStackPerChunkBase = 1,
            //	MinimumItemDropsCount = 1,
            //	MaximumItemDropsCount = 3,
            //};

            //notExpertRule.OnSuccess(new DropOneByOne(itemType, parameters));

            // Finally add the leading rule
            npcLoot.Add(notExpertRule);
        }
        private void FinishResetTimers()
        {
            if (_resetTimers)
            {
                timer = 0;
                frameCounter = 0;
                frameTick = 0;
                _resetTimers = false;
            }
        }

        public void ResetTimers()
        {
            if (StellaMultiplayer.IsHost)
            {
                _resetTimers = true;
                NPC.netUpdate = true;
            }
        }

        public override void OnKill()
        {

            if (Main.netMode != NetmodeID.Server && Terraria.Graphics.Effects.Filters.Scene["Shockwave"].IsActive())
            {
                Terraria.Graphics.Effects.Filters.Scene["Shockwave"].Deactivate();
            }
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedIrradiaBoss, -1);
        }

    }
}
