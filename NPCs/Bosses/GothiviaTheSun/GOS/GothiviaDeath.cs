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
using Stellamod.NPCs.Bosses.Fenix.Projectiles;
using Stellamod.NPCs.Bosses.GothiviaNRek.Reks;
using Stellamod.NPCs.Bosses.GothiviaTheSun.GOS.Projectiles;
using Stellamod.NPCs.Bosses.GothiviaTheSun.REK;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Havoc.Projectiles;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Irradia;
using Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles;
using Stellamod.NPCs.Bosses.Verlia.Projectiles;
using Stellamod.NPCs.Bosses.Zui.Projectiles;
using Stellamod.Projectiles.Visual;
using Stellamod.Trails;
using Stellamod.UI.Dialogue;
using Stellamod.UI.Systems;
using Stellamod.Utilis;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.NPCs.Bosses.GothiviaTheSun.GOS
{
    // This attribute looks for a texture called "ClassName_Head_Boss" and automatically registers it as the NPC boss head ic
    

    public class GothiviaDeath : ModNPC
    {
        private bool _resetTimers;
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
        Vector2 dashDirection = Vector2.Zero;
        float dashDistance = 0f;

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(dashDirection);
            writer.Write(dashDistance);
            writer.Write((float)State);
            writer.Write(Spawner);
            writer.Write(_resetTimers);

        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            dashDirection = reader.ReadVector2();
            dashDistance = reader.ReadSingle();
            State = (ActionState)reader.ReadSingle();
            Spawner = reader.ReadSingle();
            _resetTimers = reader.ReadBoolean();
        }
    
        public int NumberOfTimesTalkedTo = 0;
        public const string ShopName = "Shop";
        public const string ShopName2 = "New Shop";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Verlia of The Moon");

            Main.npcFrameCount[Type] = 61;
            NPCID.Sets.TrailCacheLength[NPC.type] = 32;
            NPCID.Sets.TrailingMode[NPC.type] = 3;

            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            // Automatically group with other bosses
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
        }

        private Vector2 FigureEightStartCenter;
        public override void SetDefaults()
        {
            NPC.Size = new Vector2(44, 80);
            NPC.damage = 1;
            NPC.friendly = true;
            NPC.defense = 110;
            NPC.lifeMax = 240000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.scale = 1f;
            NPC.knockBackResist = 0.5f;
            NPC.dontTakeDamage = true;
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;


            // Take up open spawn slots, preventing random NPCs from spawning during the fight

            // Don't set immunities like this as of 1.4:
            // NPC.buffImmune[BuffID.Confused] = true;
            // immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            // Custom boss bar

            // The following code assigns a music track to the boss in a simple way.
         
        }
        public override bool CheckActive()
        {
            return false;
        }

        public override bool CanChat()
        {
            return false;
        }

       

       

        bool axed = false;
        bool p2 = false;

        public float squish = 0f;
        private int _wingFrameCounter;
        private int _wingFrameTick;    

        float ChargeTrailOpacity;
        bool DrawChargeTrail;
        bool TrailedOrange;
        public Color ColorFunctionCharge(float completionRatio)
        {
            if (!DrawChargeTrail)
            {
                ChargeTrailOpacity -= 0.05f;
                if (ChargeTrailOpacity <= 0)
                    ChargeTrailOpacity = 0;
            }
            else
            {
                ChargeTrailOpacity += 0.05f;
                if (ChargeTrailOpacity >= 1)
                    ChargeTrailOpacity = 1;
            }

            Color color = Color.Lerp(Color.Turquoise, Color.RoyalBlue, completionRatio);

            if (TrailedOrange)
            {
                color = Color.Lerp(Color.Orange, Color.DarkGoldenrod, completionRatio);
            }
            return color * ChargeTrailOpacity * (1f - completionRatio);
        }

        public float WidthFunctionCharge(float completionRatio)
        {
            return (((NPC.width * NPC.scale) * 2) / 0.75f * (1f - completionRatio)) * 1.5f;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {


            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunctionCharge, ColorFunctionCharge, TrailRegistry.FireVertexShader);
            }

            TrailRegistry.FireVertexShader.SetShaderTexture(TrailRegistry.WaterTrail);
            Vector2 size = new Vector2(166, 96);
            TrailDrawer.DrawPrims(NPC.oldPos, size * 0.5f - screenPos, 155);


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
            originalHitbox = new Vector2(NPC.width / 100, NPC.height / 2) + new Vector2(120, 50);



            Color drawColors = NPC.GetAlpha(Color.White);
            ;


            if (ThreeQ && !FourQ && !NoWings)
            {
                Vector2 drawPosition = NPC.Center - screenPos;
                Vector2 origin = new Vector2(83, 48);
                Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/Gwings3Q").Value;
                int wingFrameSpeed = 1;
                int wingFrameCount = 60;
                spriteBatch.Draw(syliaWingsTexture, drawPosition,
                    syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
                    drawColor, NPC.rotation, origin, 2f, effects, 0f);

            }


            if (FourQ && !ThreeQ && !NoWings)
            {
                Vector2 drawPosition = NPC.Center - screenPos;
                Vector2 origin = new Vector2(83, 48);
                Texture2D syliaWingsTexture = ModContent.Request<Texture2D>("Stellamod/NPCs/Bosses/GothiviaTheSun/GOS/Gwings4Q").Value;
                int wingFrameSpeed = 1;
                int wingFrameCount = 60;
                spriteBatch.Draw(syliaWingsTexture, drawPosition,
                    syliaWingsTexture.AnimationFrame(ref _wingFrameCounter, ref _wingFrameTick, wingFrameSpeed, wingFrameCount, true),
                    drawColor, NPC.rotation, origin, 2f, effects, 0f);

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

        //Custom function so that I don't have to copy and paste the same thing in FindFrame
        int bee = 220;
        private Vector2 originalHitbox;
        //int Timer2 = 0;
        float timert = 0;
        int Arrows = 0;
        public float Spawner = 0;

        public bool DM = false;
        public bool Elect = false;
        public override void AI()
        {

            NPC.CheckActive();

            timer++;
            if (timer == 1)
            {
                DialogueSystem dialogueSystem = ModContent.GetInstance<DialogueSystem>();

                //2. Create a new instance of your dialogue
                GothiviaBeatDialogue exampleDialogue = new GothiviaBeatDialogue();

                //3. Start it
                dialogueSystem.StartDialogue(exampleDialogue);

                Main.LocalPlayer.GetModPlayer<MyPlayer>().FocusOn(base.NPC.Center, 4f);
                CombatText.NewText(NPC.getRect(), Color.Gold, LangText.Misc("GothiviaDeath.1"), true, false);
            }



            Player player = Main.player[NPC.target];
            bool expertMode = Main.expertMode;
            if (!NPC.HasPlayerTarget)
            {
                NPC.TargetClosest(false);
                Player player1 = Main.player[NPC.target];

                if (!NPC.HasPlayerTarget || NPC.Distance(player1.Center) > 3000f)
                {
                    return;
                }
            }
            Player playerT = Main.player[NPC.target];
            int distance = (int)(NPC.Center - playerT.Center).Length();

            if (distance > 3000f || playerT.dead)
            {
                NPC.ai[0] = 0;
                NPC.ai[3]++;
                NPC.position.Y = player.Center.Y + -800;
                if (NPC.ai[3] >= 80)
                {
                    NPC.active = false;
                }
            }


            if (DM)
            {
                if (NPC.ai[2] == 0)
                {
                    NPC.ai[2] = 10;
                }
                p2 = NPC.life < NPC.lifeMax * 0.5f;
                Main.GraveyardVisualIntensity = 0.4f;
                if (NPC.ai[2] == 10)
                {
                    if (NPC.alpha >= 0)
                    {
                        NPC.alpha = 0;
                    }
                    NPC.ai[0]++;
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Dust dust = Dust.NewDustDirect(NPC.Center, NPC.width, NPC.height, DustID.CoralTorch);
                        dust.velocity *= -1f;
                        dust.scale *= .8f;
                        dust.noGravity = true;
                        Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                        vector2_1.Normalize();
                        Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                        dust.velocity = vector2_2;
                        vector2_2.Normalize();
                        Vector2 vector2_3 = vector2_2 * 34f;
                        dust.position = NPC.Center - vector2_3;
                        NPC.netUpdate = true;
                    }
                    if (NPC.ai[0] == 110)
                    {
                        CombatText.NewText(NPC.getRect(), Color.Gold, LangText.Misc("GothiviaDeath.2"), true, false);
                        var EntitySource = NPC.GetSource_Death();
                        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.NPC.Center, 2048f, 90f);
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Crysalizer4"), NPC.position);
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(base.NPC.Center, DustID.GoldCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
                        }
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(base.NPC.Center, DustID.CoralTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                        }
                        for (int i = 0; i < 14; i++)
                        {
                            Dust.NewDustPerfect(base.NPC.Center, DustID.GoldFlame, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = false;
                        }
                        for (int j = 0; j < 26; j++)
                        {

                            int a = Gore.NewGore(EntitySource, new Vector2(NPC.Center.X + Main.rand.Next(-10, 10), NPC.Center.Y + Main.rand.Next(-10, 10)), NPC.velocity, 911);
                            Main.gore[a].timeLeft = 20;
                            Main.gore[a].scale = Main.rand.NextFloat(.5f, 1f);
                        }
                        for (int i = 0; i < 40; i++)
                        {
                            Dust.NewDustPerfect(base.NPC.Center, DustID.GoldFlame, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
                        }

                      //  int Gore2 = ModContent.Find<ModGore>("Stellamod/ZuiHat").Type;
                      //  Gore.NewGore(EntitySource, NPC.position, NPC.velocity, Gore2);
                       // Utilities.NewProjectileBetter(NPC.Center.X, NPC.Center.Y, 0, 0, ModContent.ProjectileType<ZuiSpawnEffect>(), 0, 0f, -1, 0, NPC.whoAmI);

                        NPC.active = false;
                    }
                }

            }

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

            Vector3 RGB = new(2.30f, 2.21f, 2.72f);
            Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);
            NPC.spriteDirection = NPC.direction;

            FinishResetTimers();
            switch (State)
            {
                //Gothivia Stuff here

                case ActionState.IdleGoth:
                    NPC.damage = 0;
                    counter++;
                    FourQ = true;
                    TrailedOrange = false;
                    ThreeQ = false;
                    NoWings = false;
                    NPC.noGravity = true;
                    NPC.aiStyle = -1;
                    break;

                case ActionState.ReallyStartGoth:
                    NPC.damage = 0;
                    counter++;
                    FourQ = true;
                    TrailedOrange = false;
                    ThreeQ = false;
                    NoWings = false;
                    ReallyIdleGoth();
                    NPC.noGravity = true;
                    NPC.aiStyle = -1;
                    break;

                case ActionState.StartGoth:
                    NPC.damage = 0;
                    counter++;
                    FourQ = true;
                    DrawChargeTrail = false;
                    TrailedOrange = false;
                    ThreeQ = false;
                    IdleGoth();
                    NPC.noGravity = true;
                    NoWings = false;
                    NPC.velocity.Y *= 0.96f;
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Dichotamy:
                    NPC.damage = 0;
                    counter++;
                    ThreeQ = true;
                    TrailedOrange = false;
                    FourQ = false;
                    NPC.noGravity = true;
                    NoWings = false;
                    NPC.velocity.Y *= 0.96f;
                    Dichotamy();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Archery:
                    NPC.damage = 0;
                    counter++;
                    ThreeQ = true;
                    TrailedOrange = false;
                    FourQ = false;
                    NPC.noGravity = true;
                    NoWings = false;
                    NPC.velocity.Y *= 0.96f;
                    Archery();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.BoostBounce1:
                    NPC.damage = 600;
                    counter++;
                    TrailedOrange = false;
                    ThreeQ = false;
                    FourQ = true;
                    DrawChargeTrail = false;
                    NoWings = false;
                    NPC.noGravity = true;
                    NPC.velocity *= 0.96f;
                    BoostBoom1();
                    NPC.aiStyle = -1;
                    break;



                case ActionState.BoostBounce2:
                    NPC.damage = 600;
                    counter++;
                    ThreeQ = true;
                    FourQ = false;
                    NoWings = false;
                    NPC.velocity *= 0.96f;
                    BoostBoom2();
                    NPC.aiStyle = -1;
                    break;


                case ActionState.BoostBounce3:
                    NPC.damage = 600;
                    counter++;
                    ThreeQ = true;
                    FourQ = false;
                    NoWings = false;
                    NPC.velocity *= 0.8f;
                    BoostBoom3();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.TheZoomer:
                    counter++;
                    ThreeQ = true;
                    FourQ = false;
                    DrawChargeTrail = true;
                    NPC.velocity *= 0.9f;
                    NPC.noGravity = true;
                    TrailedOrange = true;
                    TheY();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Kick:
                    NPC.damage = 0;
                    counter++;
                    ThreeQ = true;
                    FourQ = false;
                    DrawChargeTrail = true;
                    NoWings = false;
                    NPC.noGravity = true;
                    NPC.velocity *= 0.96f;
                    Wangler();
                    NPC.aiStyle = -1;
                    break;


                case ActionState.SunExplosionCharge1:
                    NPC.damage = 0;
                    counter++;
                    ThreeQ = false;
                    FourQ = true;
                    DrawChargeTrail = false;
                    NoWings = false;
                    TrailedOrange = false;
                    NPC.velocity *= 0.96f;
                    SunchargeGreen();
                    NPC.aiStyle = -1;
                    break;


                case ActionState.SunExplosionCharge2:
                    NPC.damage = 0;
                    counter++;
                    ThreeQ = false;
                    FourQ = true;
                    DrawChargeTrail = false;
                    NoWings = false;
                    TrailedOrange = false;
                    NPC.velocity *= 0.96f;
                    SunchargeOrange();
                    NPC.aiStyle = -1;
                    break;


                case ActionState.Suns2:
                    NPC.damage = 0;
                    counter++;
                    ThreeQ = false;
                    FourQ = true;
                    DrawChargeTrail = false;
                    NoWings = false;
                    NPC.velocity *= 0.96f;
                    OrangeSuns();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Suns1:
                    NPC.damage = 0;
                    counter++;
                    ThreeQ = false;
                    FourQ = true;
                    DrawChargeTrail = false;
                    NoWings = false;
                    NPC.velocity *= 0.96f;
                    GreenSuns();
                    NPC.aiStyle = -1;
                    break;


                case ActionState.BonfireLeft:
                    NPC.damage = 0;
                    counter++;
                    ThreeQ = false;
                    FourQ = false;
                    DrawChargeTrail = true;
                    NoWings = true;
                    NPC.noGravity = false;

                    BonfireGreen();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.BonfireRight:
                    NPC.damage = 0;
                    counter++;
                    ThreeQ = false;
                    FourQ = false;
                    DrawChargeTrail = true;
                    NoWings = true;

                    NPC.noGravity = false;
                    BonfireOrange();
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
            Player target = Main.player[NPC.target];
            if (timer == 1 && NPC.HasValidTarget)
            {

                Vector2 targetCenter = target.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(312, 0);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                NPC.netUpdate = true;

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }



            if (timer < 50)
            {
                NPC.velocity.Y -= 0.08f;
            }


            if (timer == 60)
            {
                NPC.velocity.Y *= 0;
                ResetTimers();
                switch (Main.rand.Next(3))
                {
                    case 0:
                        State = ActionState.Dichotamy;
                        break;

                    case 1:
                        State = ActionState.Dichotamy;
                        break;

                    case 2:
                        State = ActionState.Dichotamy;

                        break;


                }
            }
        }
        private void TheY()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player target = Main.player[NPC.target];
            float ai1 = NPC.whoAmI;

            FigureEightStartCenter = Vector2.Lerp(FigureEightStartCenter, target.Center, 0.07f);


            if (timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BindingBless1") { Pitch = Main.rand.NextFloat(-3f, 3f) }, NPC.Center);

                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<GothBlastExplosionProj>(), 24, 0f, Main.myPlayer, 0f, ai1);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<BlinkingStar>(), 24, 0f, Main.myPlayer, 0f, ai1);
                }




            }
            if (timer < 50)
            {
                NPC.damage = 0;
            }

            if (timer < 50 && NPC.HasValidTarget)
            {


                Vector2 targetCenter = target.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, 256);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                NPC.netUpdate = true;

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
            if (timer == 90)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WavingGoth2") { Pitch = Main.rand.NextFloat(-3f, 3f) }, NPC.Center);
            }

            if (timer > 90 && timer < 470)
            {
                NPC.damage = 1600;
                NPC.rotation = NPC.velocity.ToRotation();
                float movementSpeed = 40;
                float size = 812;
                float figureEightSpeed = 0.06f;

                float t = timer * figureEightSpeed;
                float scale = 2 / (3 - MathF.Cos(2 * t));

                scale *= size;
                float x = scale * MathF.Cos(t);
                float y = scale * MathF.Sin(2 * t) / 2;

                Vector2 targetCenter = FigureEightStartCenter + new Vector2(x, y);
                Vector2 targetVelocity = NPC.Center.DirectionTo(targetCenter) * movementSpeed;
                float distance = Vector2.Distance(NPC.Center, targetCenter);
                if (distance < movementSpeed)
                {
                    targetVelocity = NPC.Center.DirectionTo(targetCenter) * distance;
                }
                NPC.velocity = targetVelocity;
            }

            if (timer == 510)
            {
                NPC.velocity *= 0.2f;
                ResetTimers();
                switch (Main.rand.Next(2))
                {
                    case 0:
                        State = ActionState.SunExplosionCharge1;
                        break;

                    case 1:
                        State = ActionState.SunExplosionCharge2;
                        break;

                }
                NPC.rotation = 0;
            }
        }
        private void BonfireGreen()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player target = Main.player[NPC.target];
            NPC.TargetClosest();
            float ai1 = NPC.whoAmI;
            if (timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothReact") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<SwirlingKick>(), 20, 0f, Main.myPlayer, 0f, ai1);

                    //     Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

                }


                if (StellaMultiplayer.IsHost)
                {
                    int distanceY = Main.rand.Next(-900, -600);
                    int distanceYa = Main.rand.Next(-100, -100);
                    NPC.position.X = target.Center.X + distanceY;
                    NPC.position.Y = target.Center.Y + distanceYa;
                    NPC.netUpdate = true;

                }
            }
            float speed = 20;




            if (timer > 110)
            {
                NPC.velocity.Y += 0.5f;
            }

            if (timer < 10)
            {


                if (StellaMultiplayer.IsHost)
                {
                    int distance = Main.rand.Next(4, 4);
                    NPC.ai[3] = Main.rand.Next(1);
                    NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (target.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }

                ShakeModSystem.Shake = 3;
            }


            if (timer > 110)
            {


                NPC.velocity *= 0.8f;
            }
            if (timer == 110)
            {
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<GreenSunsBoomProj>(), 24, 0f, Main.myPlayer, 0f, ai1);

                    //     Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

                }
                ShakeModSystem.Shake = 12;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothCarmody") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CarmodyGreen>());

                    }

                }

            }

            if (timer == 150)
            {
                NPC.velocity *= 0.3f;
                ResetTimers();
                State = ActionState.BoostBounce1;
            }
        }


        private void BonfireOrange()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player target = Main.player[NPC.target];
            NPC.TargetClosest();
            float ai1 = NPC.whoAmI;
            if (timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothReact") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<SwirlingKick>(), 24, 0f, Main.myPlayer, 0f, ai1);

                    //     Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

                }


                if (StellaMultiplayer.IsHost)
                {
                    int distanceY = Main.rand.Next(600, 900);
                    int distanceYa = Main.rand.Next(-100, -100);
                    NPC.position.X = target.Center.X + distanceY;
                    NPC.position.Y = target.Center.Y + distanceYa;
                    NPC.netUpdate = true;

                }
            }
            float speed = 20;




            if (timer > 110)
            {
                NPC.velocity.Y += 0.5f;
            }

            if (timer < 10)
            {


                if (StellaMultiplayer.IsHost)
                {
                    int distance = Main.rand.Next(4, 4);
                    NPC.ai[3] = Main.rand.Next(1);
                    NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (target.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }

                ShakeModSystem.Shake = 3;
            }


            if (timer > 110)
            {


                NPC.velocity *= 0.8f;
            }
            if (timer == 110)
            {
                ShakeModSystem.Shake = 12;
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<SunsBoomProj>(), 24, 0f, Main.myPlayer, 0f, ai1);

                    //     Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothCarmody") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Carmody>());

                    }

                }

            }

            if (timer == 150)
            {
                NPC.velocity *= 0.3f;
                ResetTimers();
                State = ActionState.BoostBounce1;
            }
        }


        private void BoostBoom1()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player target = Main.player[NPC.target];
            float ai1 = NPC.whoAmI;
            if (timer == 2)
            {
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<GothCircleShrink>(), 24, 0f, Main.myPlayer, 0f, ai1);

                    //     Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

                }

            }

            if (timer < 50 && NPC.HasValidTarget)
            {
                Vector2 targetCenter = target.Center;
                Vector2 targetHoverCenter = targetCenter + new Vector2(0, -300);
                NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                NPC.netUpdate = true;

                float hoverSpeed = 5;
                float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
            }
            float speed = 1;
            if (NPC.life < NPC.lifeMax / 2)
            {
                speed = 18f;
            }
            if (NPC.life > NPC.lifeMax / 2)
            {
                speed = 16f;
            }

            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
            if (timer == 51)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothKickSlap") { Pitch = Main.rand.NextFloat(-5f, 2f) }, NPC.Center);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RazorClash") { Pitch = Main.rand.NextFloat(-5f, 1f) }, NPC.Center);

                switch (Main.rand.Next(2))
                {
                    case 0:
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<KickboomBurn>(), 700, 0f, Main.myPlayer, 0f, ai1);
                        break;

                    case 1:
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<KickboomSun>(), 700, 0f, Main.myPlayer, 0f, ai1);
                        break;


                }


            }

            if (timer > 50 && timer < 56)
            {


                if (StellaMultiplayer.IsHost)
                {
                    int distance = Main.rand.Next(4, 4);
                    NPC.ai[3] = Main.rand.Next(1);
                    NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (target.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }

                ShakeModSystem.Shake = 3;
            }

            if (timer == 85)
            {
                NPC.velocity *= 0.3f;
                ResetTimers();
                State = ActionState.BoostBounce2;
            }
        }

        private void BoostBoom2()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player target = Main.player[NPC.target];
            float ai1 = NPC.whoAmI;

            float speed = 1;
            if (NPC.life < NPC.lifeMax / 2)
            {
                speed = 20f;
            }
            if (NPC.life > NPC.lifeMax / 2)
            {
                speed = 20f;
            }

            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
            if (timer == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothKickSlap") { Pitch = Main.rand.NextFloat(-5f, 2f) }, NPC.Center);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RazorClash") { Pitch = Main.rand.NextFloat(-5f, 1f) }, NPC.Center);
                switch (Main.rand.Next(2))
                {
                    case 0:
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<KickboomBurn>(), 700, 0f, Main.myPlayer, 0f, ai1);
                        break;

                    case 1:
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<KickboomSun>(), 700, 0f, Main.myPlayer, 0f, ai1);
                        break;


                }


            }

            if (timer < 5)
            {


                if (StellaMultiplayer.IsHost)
                {
                    int distance = Main.rand.Next(4, 4);
                    NPC.ai[3] = Main.rand.Next(1);
                    NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (target.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }

                ShakeModSystem.Shake = 3;
            }

            if (timer == 45)
            {
                NPC.velocity *= 0.3f;
                ResetTimers();
                State = ActionState.BoostBounce3;
            }
        }

        private void BoostBoom3()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player target = Main.player[NPC.target];
            float ai1 = NPC.whoAmI;

            float speed = 1;
            if (NPC.life < NPC.lifeMax / 2)
            {
                speed = 24f;
            }
            if (NPC.life > NPC.lifeMax / 2)
            {
                speed = 20f;
            }

            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
            if (timer == 1)
            {

                ShakeModSystem.Shake = 8;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothKickSlap") { Pitch = Main.rand.NextFloat(-5f, 2f) }, NPC.Center);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RazorClash") { Pitch = Main.rand.NextFloat(-5f, 1f) }, NPC.Center);
                switch (Main.rand.Next(2))
                {
                    case 0:
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<GothBlastExplosionProj2>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<KickboomBurn>(), 700, 0f, Main.myPlayer, 0f, ai1);
                        break;

                    case 1:
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<GothBlastExplosionProj>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<KickboomSun>(), 700, 0f, Main.myPlayer, 0f, ai1);
                        break;


                }


            }

            if (timer < 10)
            {


                if (StellaMultiplayer.IsHost)
                {
                    int distance = Main.rand.Next(4, 4);
                    NPC.ai[3] = Main.rand.Next(1);
                    NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (target.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }


            }



            if (timer == 45)
            {
                if (NPC.life > NPC.lifeMax / 2)
                {
                    NPC.velocity *= 0.2f;
                    ResetTimers();
                    State = ActionState.StartGoth;
                }

                if (NPC.life < NPC.lifeMax / 2)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            State = ActionState.StartGoth;
                            break;

                        case 1:
                            State = ActionState.TheZoomer;
                            break;

                        case 2:
                            State = ActionState.TheZoomer;
                            break;
                    }

                }
            }
        }


        // Do bonfire into Boomboost




















        public int Wanger = 0;
        public int Wtimes = 0;

        private void Wangler()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player target = Main.player[NPC.target];
            float ai1 = NPC.whoAmI;
            if (timer == 2)
            {
                switch (Main.rand.Next(4))
                {
                    case 0:
                        Wanger = 1;
                        break;

                    case 1:
                        Wanger = 2;
                        break;

                    case 2:
                        Wanger = 3;
                        break;

                    case 3:
                        Wanger = 4;
                        break;
                }



            }
            float speed = 1;

            if (NPC.life < NPC.lifeMax / 2)
            {
                speed = 24f;
            }
            if (NPC.life > NPC.lifeMax / 2)
            {
                speed = 22f;
            }


            if (timer < 15 && timer > 3)
            {

                if (timer == 10)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BindingBless1") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                }
                if (Wanger == 1)
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 targetHoverCenter = targetCenter + new Vector2(0, -300);
                    NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                    NPC.netUpdate = true;

                    float hoverSpeed = 5;
                    float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
                }

                if (Wanger == 2)
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 targetHoverCenter = targetCenter + new Vector2(0, 300);
                    NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                    NPC.netUpdate = true;

                    float hoverSpeed = 5;
                    float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
                }

                if (Wanger == 3)
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 targetHoverCenter = targetCenter + new Vector2(300, 0);
                    NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                    NPC.netUpdate = true;

                    float hoverSpeed = 5;
                    float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
                }

                if (Wanger == 4)
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 targetHoverCenter = targetCenter + new Vector2(-300, 0);
                    NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                    NPC.netUpdate = true;

                    float hoverSpeed = 5;
                    float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
                }
            }

            if (timer > 15 && timer < 70)
            {

                if (timer == 25)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BlindingBless2") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                }

                if (Wanger == 1)
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 targetHoverCenter = targetCenter + new Vector2(-300, 0);
                    NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                    NPC.netUpdate = true;

                    float hoverSpeed = 5;
                    float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
                }

                if (Wanger == 2)
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 targetHoverCenter = targetCenter + new Vector2(300, 0);
                    NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                    NPC.netUpdate = true;

                    float hoverSpeed = 5;
                    float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
                }

                if (Wanger == 3)
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 targetHoverCenter = targetCenter + new Vector2(0, -450);
                    NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                    NPC.netUpdate = true;

                    float hoverSpeed = 5;
                    float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
                }

                if (Wanger == 4)
                {
                    Vector2 targetCenter = target.Center;
                    Vector2 targetHoverCenter = targetCenter + new Vector2(0, 450);
                    NPC.Center = Vector2.Lerp(NPC.Center, targetHoverCenter, 0.25f);
                    NPC.netUpdate = true;

                    float hoverSpeed = 5;
                    float yVelocity = VectorHelper.Osc(1, -1, hoverSpeed);
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, yVelocity), 0.2f);
                }
            }


            Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

            if (timer == 24)
            {
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

                }
            }

            if (timer > 70 && timer < 82)
            {


                if (timer == 71)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/RazorWing") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);

                    if (StellaMultiplayer.IsHost)
                    {

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X, direction.Y, ModContent.ProjectileType<WingRazor>(), 700, 0f, Main.myPlayer, 0f, ai1);
                    }
                }


                if (StellaMultiplayer.IsHost)
                {
                    int distance = Main.rand.Next(3, 3);
                    NPC.ai[3] = Main.rand.Next(1);
                    NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (target.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }

                ShakeModSystem.Shake = 4;



            }







            if (timer > 50 && timer < 56)
            {


                if (StellaMultiplayer.IsHost)
                {
                    int distance = Main.rand.Next(4, 4);
                    NPC.ai[3] = Main.rand.Next(1);
                    NPC.netUpdate = true;

                    double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                    double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                    Vector2 angle = new Vector2((float)anglex, (float)angley);
                    dashDirection = (target.Center - (angle * distance)) - NPC.Center;
                    dashDistance = dashDirection.Length();
                    dashDirection.Normalize();
                    dashDirection *= speed;
                    NPC.velocity = dashDirection;
                }

                ShakeModSystem.Shake = 3;
            }

            if (timer == 150)
            {
                if (Wtimes < 4)
                {

                    Wtimes += 1;
                    timer = 0;
                }

                if (Wtimes >= 4)
                {
                    ResetTimers();
                    if (NPC.life > NPC.lifeMax / 2)
                    {
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                State = ActionState.BoostBounce1;
                                break;

                            case 1:
                                State = ActionState.BoostBounce1;
                                //BonfireRight and Left
                                break;

                            case 2:
                                State = ActionState.BoostBounce1;
                                break;


                        }

                    }


                    if (NPC.life < NPC.lifeMax / 2)
                    {
                        switch (Main.rand.Next(2))
                        {
                            case 0:
                                State = ActionState.BonfireLeft;
                                break;

                            case 1:
                                State = ActionState.BonfireRight;
                                //BonfireRight and Left
                                break;




                        }

                    }



                }

                NPC.velocity *= 0.3f;

            }
        }

        private void SunchargeGreen()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player player = Main.player[NPC.target];
            float ai1 = NPC.whoAmI;
            if (timer == 1)
            {
                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();


                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<GreenSunsSuckingProj>(), 24, 0f, Main.myPlayer, 0f, ai1);

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothSummon") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                }

                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

                }
            }

            if (timer < 80)
            {
                ShakeModSystem.Shake = 5;



                //SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash1"));




            }


            if (timer == 120)
            {
                ResetTimers();
                switch (Main.rand.Next(1))
                {
                    case 0:
                        State = ActionState.Suns1;
                        break;





                }
            }
        }

        private void SunchargeOrange()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player player = Main.player[NPC.target];
            float ai1 = NPC.whoAmI;
            if (timer == 1)
            {
                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();


                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<SunsSuckingProj>(), 24, 0f, Main.myPlayer, 0f, ai1);

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothSummon") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                }

                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

                }
            }

            if (timer < 80)
            {
                ShakeModSystem.Shake = 5;



                //SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash1"));




            }


            if (timer == 120)
            {
                ResetTimers();
                switch (Main.rand.Next(1))
                {
                    case 0:
                        State = ActionState.Suns2;
                        break;





                }
            }
        }



        private void GreenSuns()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player player = Main.player[NPC.target];

            float ai1 = NPC.whoAmI;
            if (timer == 1)
            {

                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<GreenSunsBoomProj>(), 24, 0f, Main.myPlayer, 0f, ai1);

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothSunLonger") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                }

                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Sun2>());

                    }

                }


            }



            if (timer < 560 && timer > 4)
            {

                circleDistance = 415;
                movementSpeed = 10;
                circleSpeed = 2;

                _circleDegrees += circleSpeed;
                float circleRadians = MathHelper.ToRadians(_circleDegrees);
                Vector2 offsetFromPlayer = new Vector2(circleDistance, 0).RotatedBy(circleRadians);
                Vector2 circlePosition = player.Center + offsetFromPlayer;

                //This is just how quickly the NPC will move to the circle position
                //This number should be higher than the circle speed

                NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, movementSpeed);

            }



            if (timer == 590)
            {
                ResetTimers();
                switch (Main.rand.Next(1))
                {
                    case 0:
                        State = ActionState.ReallyStartGoth;
                        break;





                }
            }
        }


        private void OrangeSuns()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player player = Main.player[NPC.target];

            float ai1 = NPC.whoAmI;
            if (timer == 1)
            {


                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<SunsBoomProj>(), 24, 0f, Main.myPlayer, 0f, ai1);

                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothSunLonger") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);
                }
                if (StellaMultiplayer.IsHost)
                {
                    var entitySource = NPC.GetSource_FromThis();
                    if (StellaMultiplayer.IsHost)
                    {
                        NPC.NewNPC(entitySource, (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Sun>());

                    }

                }


            }



            if (timer < 560 && timer > 4)
            {

                circleDistance = 415;
                movementSpeed = 10;
                circleSpeed = 2;

                _circleDegrees += circleSpeed;
                float circleRadians = MathHelper.ToRadians(_circleDegrees);
                Vector2 offsetFromPlayer = new Vector2(circleDistance, 0).RotatedBy(circleRadians);
                Vector2 circlePosition = player.Center + offsetFromPlayer;

                //This is just how quickly the NPC will move to the circle position
                //This number should be higher than the circle speed

                NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, movementSpeed);

            }



            if (timer == 590)
            {
                ResetTimers();
                switch (Main.rand.Next(1))
                {
                    case 0:
                        State = ActionState.ReallyStartGoth;
                        break;





                }
            }
        }

        private void Dichotamy()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player player = Main.player[NPC.target];
            float ai1 = NPC.whoAmI;
            if (timer == 1)
            {
                ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                shaderSystem.VignetteScreen(2f);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothSummon") { Pitch = Main.rand.NextFloat(-1f, 1f) }, NPC.Center);

                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<GothCircleShrink>(), 24, 0f, Main.myPlayer, 0f, ai1);


                }

                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;

                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<BlinkingStar>(), NPC.damage, 0f, Main.myPlayer, 0f, ai1);

                }
            }

            if (timer == 80)
            {
                ShakeModSystem.Shake = 5;

                if (StellaMultiplayer.IsHost)
                {
                    ScreenShaderSystem shaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                    shaderSystem.UnVignetteScreen();
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/DUAL2") { Pitch = Main.rand.NextFloat(-5f, 5f) }, NPC.Center);

                    Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;

                    float numberProjectiles = 3;
                    float rotation = MathHelper.ToRadians(20);
                    for (int i = 0; i < 1; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X * 3, perturbedSpeed.Y * 6, ModContent.ProjectileType<RazorBurns>(), 30, 1, Main.myPlayer, 0, 0);



                    }
                    for (int i = 0; i < 1; i++)
                    {
                        Vector2 perturbedSpeed = new Vector2(direction.X, direction.Y).RotatedBy(MathHelper.Lerp(rotation, -rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, perturbedSpeed.X * 3, perturbedSpeed.Y * 6, ModContent.ProjectileType<RazorSuns>(), 30, 1, Main.myPlayer, 0, 0);



                    }


                }

                //SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash1"));




            }


            if (timer == 150)
            {
                ResetTimers();
                switch (Main.rand.Next(3))
                {
                    case 0:
                        State = ActionState.Archery;
                        break;

                    case 1:
                        State = ActionState.Archery;
                        break;

                    case 2:
                        State = ActionState.Archery;

                        break;




                }
            }
        }









        private float _circleDegrees;
        float movementSpeed = 5;
        float circleSpeed = 2;
        float circleDistance = 350;




        private void Archery()
        {
            NPC.spriteDirection = NPC.direction;
            timer++;
            Player target = Main.player[NPC.target];


            Vector2 velocity = NPC.Center.DirectionTo(target.Center) * 10;


            float ai1 = NPC.whoAmI;
            if (timer == 3)
            {
                circleDistance = 270;
            }
            if (timer == 80)
            {
                movementSpeed = 12;
                circleSpeed = 3;
            }


            if (timer == 170)
            {
                movementSpeed = 25;

            }

            if (timer == 210)
            {
                movementSpeed = 16;
            }


            if (timer == 240)
            {
                movementSpeed = 12;
                circleSpeed = 2;

            }


            if (timer > 50)
            {

                _circleDegrees += circleSpeed;
                float circleRadians = MathHelper.ToRadians(_circleDegrees);
                Vector2 offsetFromPlayer = new Vector2(circleDistance, 0).RotatedBy(circleRadians);
                Vector2 circlePosition = target.Center + offsetFromPlayer;

                //This is just how quickly the NPC will move to the circle position
                //This number should be higher than the circle speed

                NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, movementSpeed);

            }

            if (timer < 80 && timer > 134)
            {

                _circleDegrees += circleSpeed;
                float circleRadians = MathHelper.ToRadians(_circleDegrees);
                Vector2 offsetFromPlayer = new Vector2(circleDistance, 0).RotatedBy(circleRadians);
                Vector2 circlePosition = target.Center + offsetFromPlayer;

                //This is just how quickly the NPC will move to the circle position
                //This number should be higher than the circle speed

                NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, movementSpeed);

            }

            if (timer < 164 && timer > 224)
            {

                _circleDegrees += circleSpeed;
                float circleRadians = MathHelper.ToRadians(_circleDegrees);
                Vector2 offsetFromPlayer = new Vector2(circleDistance, 0).RotatedBy(circleRadians);
                Vector2 circlePosition = target.Center + offsetFromPlayer;

                //This is just how quickly the NPC will move to the circle position
                //This number should be higher than the circle speed

                NPC.velocity = VectorHelper.VelocitySlowdownTo(NPC.Center, circlePosition, movementSpeed);

            }

            NPC.velocity *= 0.96f;


            if (timer < 30)
            {
                NPC.TargetClosest();
            }

            if (timer == 60)
            {
                ShakeModSystem.Shake = 5;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothingBow") { Pitch = Main.rand.NextFloat(-1f, 1f) }, NPC.Center);
                //
                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<GothCircleShrink>(), 24, 0f, Main.myPlayer, 0f, ai1);

                }

                switch (Main.rand.Next(2))
                {
                    case 0:
                        for (int i = 0; i < 1; i++)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 3, direction.Y * 3, ModContent.ProjectileType<GothSunBlowtorchBlastProj>(), 600, 1, Main.myPlayer, 0, 0);

                            }

                        }
                        break;

                    case 1:
                        for (int i = 0; i < 1; i++)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 3, direction.Y * 3, ModContent.ProjectileType<GothFireBlowtorchBlastProj>(), 600, 1, Main.myPlayer, 0, 0);

                            }


                        }
                        break;







                }

            }

            if (timer == 154)
            {
                ShakeModSystem.Shake = 5;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothingBow") { Pitch = Main.rand.NextFloat(-1f, 1f) }, NPC.Center);

                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<GothCircleShrink>(), 24, 0f, Main.myPlayer, 0f, ai1);

                }
                switch (Main.rand.Next(2))
                {
                    case 0:
                        for (int i = 0; i < 1; i++)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 3, direction.Y * 3, ModContent.ProjectileType<GothSunBlowtorchBlastProj>(), 600, 1, Main.myPlayer, 0, 0);

                            }

                        }
                        break;

                    case 1:
                        for (int i = 0; i < 1; i++)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 3, direction.Y * 3, ModContent.ProjectileType<GothFireBlowtorchBlastProj>(), 600, 1, Main.myPlayer, 0, 0);

                            }


                        }
                        break;







                }

                //SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash1"));




            }

            if (timer == 248)
            {
                ShakeModSystem.Shake = 5;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GothingBow") { Pitch = Main.rand.NextFloat(-1f, 1f) }, NPC.Center);

                Vector2 direction = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * 8.5f;
                if (StellaMultiplayer.IsHost)
                {
                    float speedXb = NPC.velocity.X * Main.rand.NextFloat(0f, 0f) + Main.rand.NextFloat(0f, 0f);
                    float speedYb = NPC.velocity.Y * Main.rand.Next(0, 0) * 0.0f + Main.rand.Next(0, 0) * 0f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, speedXb - 2 * 0, speedYb - 2 * 0, ModContent.ProjectileType<GothCircleShrink>(), 24, 0f, Main.myPlayer, 0f, ai1);

                }
                switch (Main.rand.Next(2))
                {
                    case 0:
                        for (int i = 0; i < 1; i++)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 3, direction.Y * 3, ModContent.ProjectileType<GothSunBlowtorchBlastProj>(), 600, 1, Main.myPlayer, 0, 0);

                            }

                        }
                        break;

                    case 1:
                        for (int i = 0; i < 1; i++)
                        {
                            if (StellaMultiplayer.IsHost)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center.X, NPC.Center.Y, direction.X * 3, direction.Y * 3, ModContent.ProjectileType<GothFireBlowtorchBlastProj>(), 600, 1, Main.myPlayer, 0, 0);

                            }


                        }
                        break;







                }

                //SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FenixSlash1"));




            }


            if (timer == 282)
            {


                if (Arrows < 3)
                {
                    Arrows += 1;
                    timer = 0;
                }
                if (Arrows >= 3)
                {




                    if (NPC.life > NPC.lifeMax / 2)
                    {
                        switch (Main.rand.Next(4))
                        {
                            case 0:
                                State = ActionState.BoostBounce1;
                                break;

                            case 1:
                                State = ActionState.Kick;
                                break;
                            //BonefireRight
                            case 2:
                                State = ActionState.Kick;
                                //BonfireLeft
                                break;

                            case 3:
                                State = ActionState.Kick;
                                //Kick
                                break;


                        }
                    }

                    if (NPC.life < NPC.lifeMax / 2)
                    {
                        switch (Main.rand.Next(5))
                        {
                            case 0:
                                State = ActionState.BonfireLeft;
                                break;

                            case 1:
                                State = ActionState.BonfireRight;
                                break;

                            case 2:
                                State = ActionState.TheZoomer;
                                break;

                            case 3:
                                State = ActionState.Kick;
                                break;

                            case 4:
                                State = ActionState.Kick;
                                break;
                        }

                    }


                    ResetTimers();
                }
            }
        }









        











       
        private void FinishResetTimers()
        {
            if (_resetTimers)
            {
                Wtimes = 0;
                Arrows = 0;
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
