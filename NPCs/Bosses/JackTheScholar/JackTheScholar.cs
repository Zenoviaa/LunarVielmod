using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Igniter;
using Stellamod.Items.Consumables;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Weapons.Mage;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Ranged;
using Stellamod.NPCs.Bosses.DaedusRework;
using Stellamod.NPCs.Bosses.Jack;
using Stellamod.NPCs.Bosses.JackTheScholar.Projectiles;
using Stellamod.Projectiles.Gun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.JackTheScholar
{
    internal class JackTheScholar : ModNPC
    {
        private enum AIState
        {
            Idle,
            Hop_Around,
            Super_Jump,
            Jack_Flames,
            Jack_Bombs,
            Fire_Stomp,
            Flamethrower,
            Flame_Pillar,
            Phase_2_Transition
        }

        private enum AnimationState
        {
            Idle,
            Cast_Hand_Up,
            Cast_Hold_Out,
            Cast_Put_Down,
            Summon_Hand_Up,
            Summon_Hold_Out,
            Summon_Hand_Down
        }

        private AnimationState Animation;
        private ref float Timer => ref NPC.ai[0];
        private ref float AttackCycle => ref NPC.ai[1];
        private AIState State
        {
            get
            {
                return (AIState)NPC.ai[2];
            }
            set
            {
                NPC.ai[2] = (float)value;
            }
        }

        private ref float AttackCount => ref NPC.ai[3];
        private int _frame;

        private bool InPhase2 => NPC.life <= NPC.lifeMax / 2;
        private bool HasDonePhase2Transition;
        private Player Target => Main.player[NPC.target];
        private Vector2 DirectionToTarget => (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
        private Vector2 FlamethrowerVelocity;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            Main.npcFrameCount[Type] = 28;

            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers();
            drawModifiers.CustomTexturePath = "Stellamod/NPCs/Bosses/Jack/JackBestiary";
            drawModifiers.PortraitScale = 1f; // Portrait refers to the full picture when clicking on the icon in the bestiary
            drawModifiers.PortraitPositionYOverride = 0f;
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 30;
            NPC.height = 75;
            NPC.damage = 32;
            NPC.defense = 6;
            NPC.lifeMax = 1100;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.boss = true;
            NPC.npcSlots = 10f;

            //Setup the music and boss bar
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Jack");
            NPC.aiStyle = 0;
            NPC.BossBar = ModContent.GetInstance<JackBossBar>();
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
            
            switch (Animation)
            {
                default:
                case AnimationState.Idle:
                    if(_frame >= 4f)
                    {
                        _frame = 0;
                    }
                    break;
                case AnimationState.Cast_Hand_Up:
                    if(_frame >= 8f)
                    {
                        Animation = AnimationState.Cast_Hold_Out;
                    }
                    break;
                case AnimationState.Cast_Hold_Out:
                    if(_frame >= 12)
                    {
                        _frame = 8;
                    }
                    break;
                case AnimationState.Cast_Put_Down:
                    if(_frame >= 16)
                    {
                        _frame = 0;
                        Animation = AnimationState.Idle;
                    }
                    break;
                case AnimationState.Summon_Hand_Up:
                    if (_frame >= 20)
                    {
                        Animation = AnimationState.Summon_Hold_Out;
                    }
                    break;
                case AnimationState.Summon_Hold_Out:
                    if (_frame >= 24)
                    {
                        _frame = 20;
                    }
                    break;
                case AnimationState.Summon_Hand_Down:
                    if (_frame >= 28)
                    {
                        _frame = 0;
                        Animation = AnimationState.Idle;
                    }
                    break;
            }

            NPC.frame.Y = frameHeight * _frame;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            //NO CONTACT DAMAGE
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.Center - screenPos;
            Rectangle frame = NPC.frame;
            Vector2 drawOrigin = frame.Size() / 2f;
            float drawRotation = NPC.rotation;
            float glowDrawOffset = VectorHelper.Osc(3f, 4f, 5);

            SpriteEffects effects = NPC.spriteDirection != -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            //Ok so we need some glowing huhh
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            
            //Glow Code :) 
            for (float f = 0f; f < 1.0f; f += 0.2f)
            {
                Vector2 offsetDrawPos = NPC.Center - screenPos + (f * MathHelper.TwoPi).ToRotationVector2() * glowDrawOffset;
                offsetDrawPos.Y -= 8;
                spriteBatch.Draw(texture, offsetDrawPos, frame, drawColor, drawRotation, drawOrigin, NPC.scale, effects, layerDepth: 0);
            }

            if (InPhase2)
            {
                glowDrawOffset = VectorHelper.Osc(3f, 4f, 25);
                for (float f = 0f; f < 1.0f; f += 0.1f)
                {
                    Vector2 offsetDrawPos = NPC.Center - screenPos + (f * MathHelper.TwoPi).ToRotationVector2() * glowDrawOffset;
                    offsetDrawPos.Y -= 8;
                    spriteBatch.Draw(texture, offsetDrawPos, frame, drawColor, drawRotation, drawOrigin, NPC.scale, effects, layerDepth: 0);
                }

                //Trail Code
                for (int k = 0; k < NPC.oldPos.Length; k++)
                {
                    Color startColor = new Color(255, 255, 113);
                    Color endColor = new Color(232, 111, 24);
                    Vector2 trailDrawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                    trailDrawPos.Y -= 8;
                    trailDrawPos += Main.rand.NextVector2Circular(2, 2);
                                        Color color = NPC.GetAlpha(Color.Lerp(startColor, endColor, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                    spriteBatch.Draw(texture, trailDrawPos, NPC.frame, color, NPC.oldRot[k], NPC.frame.Size() / 2, NPC.scale, effects, 0f);
                }
            }
            
            //Trail Code
            for (int k = 0; k < NPC.oldPos.Length; k++)
            {
                Color startColor = new Color(255, 255, 113);
                Color endColor = new Color(232, 111, 24);
                Vector2 trailDrawPos = NPC.oldPos[k] - Main.screenPosition + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
                trailDrawPos.Y -= 8;
                  Color color = NPC.GetAlpha(Color.Lerp(startColor, endColor, 1f / NPC.oldPos.Length * k) * (1f - 1f / NPC.oldPos.Length * k));
                spriteBatch.Draw(texture, trailDrawPos, NPC.frame, color, NPC.oldRot[k], NPC.frame.Size() / 2, NPC.scale, effects, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Hop_Around:
                    AI_HopAround();
                    break;
                case AIState.Super_Jump:
                    AI_SuperJump();
                    break;
                case AIState.Jack_Flames:
                    AI_JackFlames();
                    break;
                case AIState.Jack_Bombs:
                    AI_JackBombs();
                    break;
                case AIState.Fire_Stomp:
                    AI_FireStomp();
                    break;
                case AIState.Flamethrower:
                    AI_Flamethrower();
                    break;
                case AIState.Flame_Pillar:
                    AI_FlamePillar();
                    break;
                case AIState.Phase_2_Transition:
                    AI_Phase2Transition();
                    break;
            }

            //Some uh visual stuff
            //Change rotation
            float targetRotation = NPC.velocity.X * 0.03f;
            NPC.rotation = targetRotation;
        }

        private void SwitchState(AIState state)
        {
            if (StellaMultiplayer.IsHost)
            {
                Timer = 0;
                AttackCycle = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void AI_Idle()
        {
            //Despawning Code
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(0, -8), 0.025f);
                    NPC.EncourageDespawn(60);
                    return;
                }
            }

            //Jack just sits around for a bit while he decides what to do, nothing special here
            Animation = AnimationState.Idle;

            Timer++;
            NPC.velocity.X *= 0.96f;
            if (!NPC.collideY && NPC.velocity.Y < 0)
            {
                NPC.velocity.Y += 0.05f;
            }


            //He could cycle a bit faster in phase 2?
            //Hmmm, not sure htough
            //Gonna make sure this code is easy to work with though

            //He cycles a bit faster in phase 2
            float timeToWait = InPhase2 ? 30 : 60;
            if(Timer >= timeToWait)
            {
                //How we choosing attack uhh, oh i know
                if (StellaMultiplayer.IsHost)
                {
                    AIState nextAttack = AIState.Hop_Around;
                    switch (AttackCount)
                    {
                        case 0:
                            if (Main.rand.NextBool(2))
                            {
                                nextAttack = AIState.Jack_Flames;
                            }
                            else
                            {
                                nextAttack = AIState.Jack_Bombs;
                            }
                            break;
                        case 1:
                            if (Main.rand.NextBool(2))
                            {
                                nextAttack = AIState.Super_Jump;
                            }
                            else
                            {
                                nextAttack = AIState.Jack_Flames;
                            }
                            break;
                        case 2:
                            if (Main.rand.NextBool(2))
                            {
                                nextAttack = AIState.Flame_Pillar;
                            }
                            else
                            {
                                nextAttack = AIState.Jack_Bombs;
                            }
                            break;
                        case 3:
                            nextAttack = AIState.Super_Jump;
                            break;
                    }
                    AttackCount++;
                    if (AttackCount >= 4)
                    {
                        AttackCount = 0;
                    }

                    if(!HasDonePhase2Transition && InPhase2)
                    {
                        nextAttack = AIState.Phase_2_Transition;
                        HasDonePhase2Transition = true;
                    }
                    SwitchState(nextAttack);
                }
            }
        }

        private void AI_Phase2Transition()
        {
            Timer++;
            if (!CheckCurrentAnimation(
                 AnimationState.Summon_Hold_Out,
                 AnimationState.Summon_Hand_Up,
                 AnimationState.Summon_Hand_Down))
            {
                Animation = AnimationState.Summon_Hand_Up;
            }

            //Jump Up
            if (Timer == 1)
            {
                NPC.velocity.Y = -16;

                float jumpHorizontalSpeed = 6;
                NPC.velocity.X = DirectionToTarget.X * jumpHorizontalSpeed;

                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
                SoundEngine.PlaySound(SoundID.Item73, NPC.position);
            }

            if(Timer > 20 && Timer < 120)
            {
                float progress = (Timer / 120f);
                float dist = MathHelper.Lerp(48f, 0f, progress);
                float scale = MathHelper.Lerp(3f, 0f, progress);
                Vector2 pos = NPC.Center + (progress * MathHelper.TwoPi).ToRotationVector2() * dist;
                Vector2 pos2 = NPC.Center + (progress * MathHelper.TwoPi + MathHelper.Pi).ToRotationVector2() * dist;
                Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: scale);
                Dust.NewDustPerfect(pos2, DustID.Torch, Vector2.Zero, Scale: scale);
            }

            if(Timer > 60)
            {
                Vector2 directionToTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                float speed = 3f;
                Vector2 velocityToTarget = directionToTarget * speed;
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, velocityToTarget.X, 0.1f);
                NPC.velocity.Y *= 0.2f;
            }

            if (Timer >= 60 && Timer < 180)
            {
                if (Timer % 12 == 0)
                {
                    float progress = (Timer - 60f) / 120f;
                    Vector2 spawnPoint = NPC.Center + Vector2.UnitY.RotatedBy(MathHelper.TwoPi * progress) * 128;
                    Vector2 startVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
                    int projType = ModContent.ProjectileType<WillOWisp>();
                    int damage = 12;
                    int knockback = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, startVelocity, projType, damage, knockback, Main.myPlayer);
                    }
                }
            }

            if(Timer == 210)
            {
                for(int i = 0; i < 32; i++)
                {
                    float progress = ((float)i / 32f);
                    float scale = MathHelper.Lerp(3f, 0f, progress);
                    Vector2 pos = NPC.Center;
                    Vector2 vel = (progress * MathHelper.TwoPi).ToRotationVector2() * 4;
                    Dust.NewDustPerfect(pos, DustID.Torch, vel, Scale: scale);
                }
            }

            if(Timer > 240)
            {
                SwitchState(AIState.Hop_Around);
            }
        }

        private void AI_HopAround()
        {
            //Jack hops/dances around towards you for a bit
            Animation = AnimationState.Idle;

            //Ok so how do we make cjumping code uhh
            switch (AttackCycle)
            {
                case 0:
                    Timer++;
                    //Y velocity + x velocity duhh
                    if(Timer == 1)
                    {
                        NPC.TargetClosest();
                        float jumpSpeed = 5;
                        float jumpHorizontalSpeed = 12;
                        if (InPhase2)
                        {
                            jumpSpeed *= 0.85f;
                            jumpHorizontalSpeed *= 1.4f;
                        }
                        NPC.velocity.Y = -jumpSpeed;

                    
                    
                        NPC.velocity.X = DirectionToTarget.X * jumpHorizontalSpeed;
                        SoundStyle jumpStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_Jump");
                        jumpStyle.PitchVariance = 0.1f;
                        SoundEngine.PlaySound(jumpStyle, NPC.position);
                    }

                    if(Timer > 10 && NPC.collideY)
                    {
                        NPC.velocity.X = 0;
                    }

                    if(Timer >= 45 && NPC.collideY || (Timer >= 90))
                    {
                        SoundStyle wee = new SoundStyle("Stellamod/Assets/Sounds/Jack_Land");
                        wee.PitchVariance = 0.1f;
                        SoundEngine.PlaySound(wee, NPC.position);
                        if (StellaMultiplayer.IsHost)
                        {
                            //Nuber of times he hops gonan be random
                            if (Main.rand.NextBool(2) && !InPhase2)
                            {
                                Timer = 0;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                SwitchState(AIState.Idle);
                            }
                        }
                    }
                    break;
            }
        }

        private bool CheckCurrentAnimation(params AnimationState[] animations)
        {
            for(int i = 0; i < animations.Length; i++)
            {
                AnimationState animation = animations[i];
                if (Animation == animation)
                    return true;
            }
            return false;
        }

        private void AI_JackFlames()
        {
            //Jack summons flames like in his original fight, but they look a bit cooler and have better movement
            //I'm thinking they'll like 'ping' and quickly home to the player but then stop homing, so you just have to react to them
            //This is one of the attacks that is much easier to dodge with your dash ^
            if (!CheckCurrentAnimation(
                AnimationState.Cast_Hand_Up, 
                AnimationState.Cast_Hold_Out, 
                AnimationState.Cast_Put_Down))
            {
                Animation = AnimationState.Cast_Hand_Up;
            }

            NPC.velocity.X *= 0.98f;
            Timer++;
            if(Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }
            if (Timer < 20f && Timer % 2 == 0)
            {
                float progress = (Timer / 20f);
                float dist = MathHelper.Lerp(48f, 0f, progress);
                float scale = MathHelper.Lerp(2f, 0f, progress);
                Vector2 pos = NPC.Center + (progress * MathHelper.TwoPi).ToRotationVector2() * dist;
                Vector2 pos2 = NPC.Center + (progress * MathHelper.TwoPi + MathHelper.Pi).ToRotationVector2() * dist;
                Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: scale);
                Dust.NewDustPerfect(pos2, DustID.Torch, Vector2.Zero, Scale: scale);
            }
            if (Timer >= 20 && Timer < 150) 
            { 
                if(Timer % 15 == 0)
                {
                    Vector2 spawnPoint = NPC.Center + Main.rand.NextVector2Circular(64, 64);
                    Vector2 startVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
                    int projType = ModContent.ProjectileType<WillOWisp>();
                    int damage = 12;
                    int knockback = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, startVelocity, projType, damage, knockback, Main.myPlayer);
                    }
                }
            }

            if(Timer == 150)
            {
                Animation = AnimationState.Cast_Put_Down;
            }

            if(Timer >= 180)
            {
                SwitchState(AIState.Hop_Around);
            }
        }

        private void AI_JackBombs()
        {
            //Jack creates a swirl of fire around you that forms into a bomb
            //The bomb slowly expands and flares up
            //Eventually the bomb explodes like in the original fight
            //Jack makes 3 of these and they explode in X / + patterns
            if (!CheckCurrentAnimation(
                 AnimationState.Summon_Hold_Out,
                 AnimationState.Summon_Hand_Up,
                 AnimationState.Summon_Hand_Down))
            {
                Animation = AnimationState.Summon_Hand_Up;
            }

            NPC.velocity.X *= 0.98f;
            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = SoundID.DD2_BetsysWrathShot;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            if(Timer < 60 && Timer % 2 == 0)
            {
                float progress = (Timer / 60f);
                float dist = MathHelper.Lerp(48f, 0f, progress);
                float scale = MathHelper.Lerp(2f, 0f, progress);
                Vector2 pos = NPC.Center + (progress * MathHelper.TwoPi).ToRotationVector2() * dist;
                Vector2 pos2 = NPC.Center + (progress * MathHelper.TwoPi + MathHelper.Pi).ToRotationVector2() * dist;
                Dust.NewDustPerfect(pos, DustID.Torch, Vector2.Zero, Scale: scale);
                Dust.NewDustPerfect(pos2, DustID.Torch, Vector2.Zero, Scale: scale);
            }
            if(Timer % 60 == 0)
            {
                Vector2 spawnPoint = NPC.Center + Main.rand.NextVector2Circular(24, 24);
                Vector2 startVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
                startVelocity.Y = -8;

                int projType = ModContent.ProjectileType<LaughingBomb>();
                int damage = 24;
                int knockback = 1;
                if (StellaMultiplayer.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, startVelocity, projType, damage, knockback, Main.myPlayer);
                    
                
                }
            }

            if (Timer == 150)
            {
                Animation = AnimationState.Summon_Hand_Down;
            }

            if (Timer >= 180)
            {
                SwitchState(AIState.Hop_Around);
            }
        }

        private void AI_FlamePillar()
        {
            //This is sort of a like, get away from me attack
            //Jack puts his hands up and erupts flame pillars on the sides of him, like Gintzia shockwave but fire
            if (!CheckCurrentAnimation(
                AnimationState.Summon_Hold_Out,
                AnimationState.Summon_Hand_Up,
                AnimationState.Summon_Hand_Down))
            {
                Animation = AnimationState.Summon_Hand_Up;
            }

            NPC.velocity.X *= 0.98f;
            Timer++;
            if(Timer == 1)
            {
                SoundStyle soundStyle = SoundID.DD2_BetsysWrathShot;
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            if(Timer >= 30 && Timer < 240)
            {
                float progress = (Timer - 30f) / 210f;
                float offset = MathHelper.Lerp(16, 1024, progress);
                if(Timer % 5 == 0)
                {
                    Vector2 spawnPoint1 = NPC.Bottom + new Vector2(offset, 0);
                    Vector2 spawnPoint2 = NPC.Bottom - new Vector2(offset, 0);
                    Vector2 startVelocity = -Vector2.UnitY * 24;
                    int projType = ModContent.ProjectileType<FlamePillar>();
                    int damage = 16;
                    int knockback = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint1, startVelocity, projType, damage, knockback, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint2, startVelocity, projType, damage, knockback, Main.myPlayer);
                    }

                    if(Timer % 10 == 0)
                    {
                        SoundEngine.PlaySound(SoundID.Item73, NPC.position);
                    }
                  
                }
            }

            //Put your hand down!!!!
            if (Timer == 270)
            {
                Animation = AnimationState.Summon_Hand_Down;
            }

            if (Timer >= 300)
            {
                SwitchState(AIState.Hop_Around);
            }
        }

        private void AI_SuperJump()
        {
            //Jack holds his hands up in the air and swirls fire around before blasting it down and jumping up really high
            //This leaves behind lingering flames on the ground and flames go out outward
            Timer++;
            if (!CheckCurrentAnimation(
                AnimationState.Summon_Hold_Out,
                AnimationState.Summon_Hand_Up,
                AnimationState.Summon_Hand_Down))
            {
                Animation = AnimationState.Summon_Hand_Up;
            }

            if (Timer == 60)
            {
                NPC.velocity.Y = -16;

                float jumpHorizontalSpeed = 6;
                NPC.velocity.X = DirectionToTarget.X * jumpHorizontalSpeed;

                int explosion = ModContent.ProjectileType<DaedusBombExplosion>();
                int damage = 24;
                int knockback = 2;
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, Vector2.Zero, explosion, damage, knockback);

                //Dust Particles
                for (int k = 0; k < 7; k++)
                {
                    Vector2 newVelocity = NPC.velocity.RotatedByRandom(MathHelper.ToRadians(7));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Dust.NewDust(NPC.Bottom, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                    Dust.NewDust(NPC.Bottom, 0, 0, DustID.InfernoFork, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
                }
                SoundEngine.PlaySound(SoundID.Item73, NPC.position);
            }

            if(Timer > 60 && NPC.collideY)
            {
                NPC.velocity.X *= 0.8f;
            }

            if(Timer == 120)
            {
                if (StellaMultiplayer.IsHost)
                {
                    if (Main.rand.NextBool(2))
                    {
                        SwitchState(AIState.Fire_Stomp);
                    }
                    else
                    {
                        SwitchState(AIState.Flamethrower);
                    }
                }

            }
        }

        private void AI_FireStomp()
        {
            //Jack has a chance to do this attack after being under half health and doing the super jump
            //He'll use flames to slow his fall, wait until he is above you and then come crashing down in a dance of fire
            Timer++;

            Vector2 directionToTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            float speed = 3f;
            Vector2 velocityToTarget = directionToTarget * speed;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, velocityToTarget.X, 0.1f);
            NPC.velocity.Y *= 0.2f;
            if (Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }

            if (Timer >= 20 && Timer < 150)
            {
                if (Timer % 12 == 0)
                {
                    Vector2 spawnPoint = NPC.Center + Main.rand.NextVector2Circular(128, 128);
                    Vector2 startVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
                    int projType = ModContent.ProjectileType<WillOWisp>();
                    int damage = 12;
                    int knockback = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, startVelocity, projType, damage, knockback, Main.myPlayer);
                    }
                }
            }

            if (Timer == 150)
            {
                Animation = AnimationState.Cast_Put_Down;
            }

            if (Timer >= 180)
            {
                SwitchState(AIState.Hop_Around);
            }
        }

        private void AI_Flamethrower()
        {
            //Jack has a chance to do this attack after being under half health and doing the super jump
            //He'll use flames to slow his fall and then blast flames directly at you in bursts, pretty easy to dodge though
            Vector2 directionToTarget = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            float speed = 3;
            Vector2 velocityToTarget = directionToTarget * speed;
            NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, velocityToTarget.X, 0.1f);
            NPC.velocity.Y *= 0.2f;
            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/Jack_Laugh");
                soundStyle.PitchVariance = 0.1f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
                FlamethrowerVelocity = directionToTarget * 8;
            }

            float degreesRotate = 0.5f;
            float length = FlamethrowerVelocity.Length();
            float targetAngle = NPC.Center.AngleTo(Target.Center);
            Vector2 newVelocity = FlamethrowerVelocity.ToRotation().AngleTowards(targetAngle, MathHelper.ToRadians(degreesRotate)).ToRotationVector2() * length;
            FlamethrowerVelocity = newVelocity;
            if (Timer >= 45 && Timer < 150)
            {
                if (Timer % 3 == 0)
                {
                    Vector2 spawnPoint = NPC.Center;
                    Vector2 startVelocity = (Target.Center - NPC.Center).SafeNormalize(Vector2.Zero) * 8;
                    int projType = ModContent.ProjectileType<Flamethrow>();
                    int damage = 12;
                    int knockback = 1;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPoint, FlamethrowerVelocity, projType, damage, knockback, Main.myPlayer);
                    }
                }
            }

            if (Timer == 150)
            {
                Animation = AnimationState.Cast_Put_Down;
            }

            if (Timer >= 180)
            {
                SwitchState(AIState.Hop_Around);
            }
        }

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref DownedBossSystem.downedJackBoss, -1);
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<JackoBag>()));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Gambit>(), 1, 1, 1));
            LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<WanderingFlame>(), minimumDropped: 20, maximumDropped: 50));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<JackoShot>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<StaffOFlame>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<ScarecrowSaber>(), chanceDenominator: 2));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<Cinderscrap>(), minimumDropped: 7, maximumDropped: 50));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AlcadizScrap>(), minimumDropped: 7, maximumDropped: 50));
            notExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TomedDustingFlames>(), chanceDenominator: 1));

            //Dunno if she should drop verlia brooch in classic mode or not
            npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.Placeable.JackBossRel>()));
            npcLoot.Add(notExpertRule);
        }
    }
}
