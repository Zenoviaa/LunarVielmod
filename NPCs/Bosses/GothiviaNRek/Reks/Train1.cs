﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.GothiviaNRek.Reks
{

    public class Train1 : ModNPC
    {
        public enum ActionState
        {
            Idle,
            StartGothivia,
            StartRollLeft,
            RollLeft,
            StartRollRight,
            RollRight,
            PunchingFirstPhaseLaserBomb,
            Jump,
            Fall,
            JumpToMiddle,
            PunchingSecondPhaseFlameBalls,
            PunchingSecondPhaseStopSign,
            Land,
            FallToMiddle,
            LandToMiddle,
            Fallslowly,
            Fallslowly2,
            Dashright,
            Dashright2,
            Dashleft,
            Dashleft2,
            Riseslowly,
            Across,
            StopRight,
            StopLeft,
            Acrossfinish,
        }

        private bool _resetTimers;
        private ActionState _state = ActionState.Fallslowly;
        // Current state
        public ActionState State
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
                if(StellaMultiplayer.IsHost)
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

        public int rippleCount = 20;
        public int rippleSize = 5;
        public int rippleSpeed = 15;
        public float distortStrength = 300f;


        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Verlia of The Moon");

            Main.npcFrameCount[Type] = 1;

            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
            NPCID.Sets.TrailingMode[NPC.type] = 0;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            // Add this in for bosses that have a summon item, requires corresponding code in the item (See MinionBossSummonItem.cs)
            // Automatically group with other bosses
            NPCID.Sets.BossBestiaryPriority.Add(Type);


            // Influences how the NPC looks in the Bestiary
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.PortraitScale = 0.8f;
            drawModifiers.PortraitPositionYOverride = 0f;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.Size = new Vector2(45, 45);
            NPC.damage = 0;
            NPC.defense = 9999;
            NPC.lifeMax = 99999;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.noTileCollide = true;
            NPC.scale = 2.5f;
            NPC.friendly = true;







            // Take up open spawn slots, preventing random NPCs from spawning during the fight

            // Don't set immunities like this as of 1.4:
            // NPC.buffImmune[BuffID.Confused] = true;
            // immunities are handled via dictionaries through NPCID.Sets.DebuffImmunitySets

            // Custom AI, 0 is "bound town NPC" AI which slows the NPC down and changes sprite orientation towards the target
            NPC.aiStyle = -1;

            // Custom boss bar

            // The following code assigns a music track to the boss in a simple way.

        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            // Sets the description of this NPC that is listed in the bestiary
            bestiaryEntry.Info.AddRange(new List<IBestiaryInfoElement> {
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Plain black background
				new FlavorTextBestiaryInfoElement("A part of gothivia's largest creations")
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

        private Vector2 dashDirection = Vector2.Zero;
        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 20; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.SolarFlare, 2.5f * hit.HitDirection, -2.5f, 180, default, .6f);
            }
        }

        //Custom function so that I don't have to copy and paste the same thing in FindFrame

        //private float HomeY = 330f;
        private int bee = 220;
        //private int Timer2 = 0;
        public override void AI()
        {

            bee--;
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                NPC ba = Main.npc[k];
                // Check if NPC able to be targeted. It means that NPC is
                if (!ba.active && ba.type == ModContent.NPCType<Rek>())
                {
                    NPC.Kill();
                }
            }


            if (bee == 0)
            {
                bee = 220;
            }

            Vector3 RGB = new(2.30f, 0.21f, 0.72f);
            Lighting.AddLight(NPC.position, RGB.X, RGB.Y, RGB.Z);

            Player player = Main.player[NPC.target];
            NPC.TargetClosest();

            if (player.dead)
            {
                // If the targeted player is dead, flee
                NPC.velocity.Y += 0.5f;
                NPC.noTileCollide = true;
                NPC.noGravity = false;
                // This method makes it so when the boss is in "despawn range" (outside of the screen), it despawns in 10 ticks
                NPC.EncourageDespawn(2);
            }

            FinishResetTimers();
            switch (State)
            {
                case ActionState.Fallslowly:
                    NPC.damage = 0;
                    counter++;
                    NPC.velocity.Y *= 0.7f;
                    Fallslowly();
                    NPC.noTileCollide = true;
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Fallslowly2:
                    NPC.damage = 0;
                    counter++;
                    NPC.velocity.Y *= 0.93f;
                    NPC.noTileCollide = true;
                    Fallslowly2();
                    NPC.aiStyle = -1;
                    break;


                case ActionState.Riseslowly:
                    NPC.damage = 0;
                    counter++;
                    NPC.velocity.Y *= 0.93f;
                    NPC.noTileCollide = true;
                    Riseslowly();
                    NPC.aiStyle = -1;
                    break;



                case ActionState.Dashright:
                    NPC.damage = 600;
                    counter++;
                    NPC.velocity.X *= 0.98f;
                    Dashright();
                    NPC.noTileCollide = false;
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Dashright2:
                    NPC.damage = 600;
                    counter++;
                    NPC.velocity.X *= 0.97f;
                    NPC.noTileCollide = false;
                    Dashright2();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.StopRight:
                    NPC.damage = 600;
                    counter++;
                    NPC.velocity.X *= 0.97f;
                    NPC.velocity.Y *= 0f;
                    StopRight();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.StopLeft:
                    NPC.damage = 600;
                    counter++;
                    NPC.velocity.X *= 0.96f;
                    NPC.velocity.Y *= 0f;
                    StopLeft();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Dashleft:
                    NPC.damage = 600;
                    NPC.noTileCollide = false;
                    counter++;
                    NPC.velocity.X *= 0.98f;
                    Dashleft();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Dashleft2:
                    NPC.damage = 600;
                    counter++;
                    NPC.noTileCollide = false;
                    NPC.velocity.X *= 0.98f;
                    Dashleft2();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Across:
                    NPC.damage = 0;
                    counter++;
                    NPC.noTileCollide = false;
                    NPC.velocity.X *= 0.985f;
                    Across();
                    NPC.aiStyle = -1;
                    break;

                case ActionState.Acrossfinish:
                    NPC.damage = 0;
                    counter++;
                    NPC.velocity.X *= 0.94f;
                    NPC.noTileCollide = false;
                    NPC.velocity.Y *= 0f;
                    Acrossfinish();
                    NPC.aiStyle = -1;
                    break;
            }
        }



        private void Fallslowly()
        {
            timer++;
            if (timer == 1)
            {
                NPC.spriteDirection *= -1;
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }

            if (timer == 3)
            {
                NPC.velocity = new Vector2(NPC.direction * 2, +15f);

            }

            if (timer == 40)
            {
                ResetTimers();
                State = ActionState.Across;
            }
        }

        private void StopRight()
        {
            timer++;

            Player player = Main.player[NPC.target];
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }

            if (timer == 50)
            {
                ResetTimers();
                State = ActionState.Dashright2;
            }
        }


        private void StopLeft()
        {
            timer++;
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }

            if (timer == 50)
            {
                ResetTimers();
                State = ActionState.Dashleft2;
            }
        }


        private void Acrossfinish()
        {
            timer++;
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }

            if (timer == 50)
            {
                ResetTimers();
                State = ActionState.Dashright2;
            }
        }



        private void Fallslowly2()
        {
            timer++;
            Player player = Main.player[NPC.target];
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }
            if (timer == 20)
            {
                NPC.velocity = new Vector2(NPC.direction * 2, +22f);
            }


            if (timer == 40)
            {
                ResetTimers();      
                NPC.spriteDirection *= -1;
                State = ActionState.Dashleft;
            }
        }


        private void Riseslowly()
        {
            timer++;

            Player player = Main.player[NPC.target];
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }
            if (timer == 20)
            {
                NPC.velocity = new Vector2(NPC.direction * 2, -22f);

            }

            if (timer == 40)
            {
                ResetTimers();
                NPC.spriteDirection *= -1;
                State = ActionState.Across;
            }
        }



        private void Dashright()
        {
            timer++;
            float speed = 25f;
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }
            if (timer < 3)
            {

                int distance = Main.rand.Next(-15, -15);
                NPC.ai[3] = Main.rand.Next(1);
                double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                Vector2 angle = new Vector2((float)anglex, (float)angley);
                dashDirection = ((angle * distance)) - NPC.Center;
                float dashDistance = dashDirection.Length();
                dashDirection.Normalize();
                dashDirection *= speed;
                NPC.velocity = -dashDirection;
                NPC.velocity.Y = 0;
                ShakeModSystem.Shake = 3;
            }

            if (timer == 40)
            {
                ResetTimers();
                NPC.damage = 0;
                State = ActionState.StopRight;
            }
        }


        private void Dashright2()
        {
            timer++;
            float speed = 25f;
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }
            if (timer < 3)
            {

                int distance = Main.rand.Next(-15, -15);
                NPC.ai[3] = Main.rand.Next(1);
                double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                Vector2 angle = new Vector2((float)anglex, (float)angley);
                dashDirection = ((angle * distance)) - NPC.Center;
                float dashDistance = dashDirection.Length();
                dashDirection.Normalize();
                dashDirection *= speed;
                NPC.velocity = -dashDirection;
                NPC.velocity.Y = 0;
                ShakeModSystem.Shake = 3;
            }

            if (timer == 40)
            {
                ResetTimers();
                NPC.damage = 0;
                State = ActionState.Fallslowly2;
            }
        }


        private void Dashleft()
        {
            timer++;
            float speed = 25f;
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }
            if (timer < 3)
            {

                int distance = Main.rand.Next(-15, -15);
                NPC.ai[3] = Main.rand.Next(1);
                double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                Vector2 angle = new Vector2((float)anglex, (float)angley);
                dashDirection = ((angle * distance)) - NPC.Center;
                float dashDistance = dashDirection.Length();
                dashDirection.Normalize();
                dashDirection *= speed;
                NPC.velocity = dashDirection;
                NPC.velocity.Y = 0;
                ShakeModSystem.Shake = 3;
            }

            if (timer == 40)
            {
                ResetTimers();
                NPC.damage = 0;
                State = ActionState.StopLeft;
            }
        }


        private void Dashleft2()
        {
            timer++;

            Player player = Main.player[NPC.target];

            float speed = 25f;
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }
            if (timer < 3)
            {

                int distance = Main.rand.Next(-15, -15);
                NPC.ai[3] = Main.rand.Next(1);
                double anglex = Math.Sin(NPC.ai[3] * (Math.PI / 180));
                double angley = Math.Abs(Math.Cos(NPC.ai[3] * (Math.PI / 180)));
                Vector2 angle = new Vector2((float)anglex, (float)angley);
                dashDirection = ((angle * distance)) - NPC.Center;
                float dashDistance = dashDirection.Length();
                dashDirection.Normalize();
                dashDirection *= speed;
                NPC.velocity = dashDirection;
                NPC.velocity.Y = 0;
                ShakeModSystem.Shake = 3;
            }

            if (timer == 40)
            {
                ResetTimers();
                NPC.damage = 0;
                State = ActionState.Riseslowly;
            }
        }







        private void Across()
        {
            timer++;

            Player player = Main.player[NPC.target];
            if (timer == 1)
            {
                // SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/Veridash1"));
            }
            if (timer < 30)
            {
                NPC.velocity.Y *= 0f;
                NPC.velocity.X += 0.3f;
            }

            if (timer == 120)
            {
                ResetTimers();
                NPC.damage = 0;
                State = ActionState.Acrossfinish;
            }
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
        }
    }
}
