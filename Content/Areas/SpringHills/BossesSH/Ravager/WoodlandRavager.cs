using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Areas.SpringHills.BossesSH.Ravager.Projectiles;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.NPCs.Bosses.EliteCommander.Projectiles;
using Stellamod.Systems.MiscellaneousMath;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.BossesSH.Ravager
{
    public class WoodlandRavager : ScarletBoss
    {
        private enum AIState
        {
            Idle,
            IdleMad,
            Charge,
            JUmp_Shockwave,
            Roar,
            QuickRoar,
            Jump,
            Turn,
            Fall,
            Crash
        }
        private Vector2 _scale;
        private Vector2 _crashPoint;
        private bool _isDangerous;
        private bool _isWarning;
        private bool _spawned;
        private bool _showNamePlate;
        private int _frame;
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get => (AIState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }
        private ref float Cycle => ref NPC.ai[2];
        private ref float RoarCycle => ref NPC.ai[3];
        private Color OutlineColor;

        private float IdleTime => 60;

        private float IdleMadTime = 60;
        private float ChargeTime => 200;

        private float StunTime => 250;

        private float MaxChaseSpeed => 10;
        private float ChaseRange => 300;
        private int FallingRockDamage => 20;
        private int FallingRockKB => 2;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.TrailCacheLength[Type] = 8;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            Main.npcFrameCount[Type] = 23;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _scale = Vector2.One;
            NPC.width = 96;
            NPC.height = 48;
            NPC.damage = 80;
            NPC.defense = 0;
            NPC.lifeMax = 400;
            NPC.HitSound = SoundID.NPCHit16;
            NPC.value = Item.buyPrice(silver: 50);
            NPC.knockBackResist = 0f;
            NPC.noGravity = false;
            NPC.boss = true;
            NPC.npcSlots = 10f;


            OutlineColor = Color.Transparent;

            //Setup the music and boss bar
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/MysticalFoe");
            //     NPC.aiStyle = 0;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _isDangerous;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * balance);
        }

        private bool InPhase2()
        {
            float pct = 0.75f;
            return NPC.life < (NPC.lifeMax * pct);
        }

        private bool InPhase3()
        {
            float pct = 0.5f;
            return NPC.life < (NPC.lifeMax * pct);
        }
        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            NPC.frameCounter += 0.15f;
            if (NPC.frameCounter >= 1f)
            {
                _frame++;
                NPC.frameCounter = 0f;
            }

            switch (State)
            {
                default:
                case AIState.Idle:
                    if (_frame >= 4f)
                    {
                        _frame = 0;
                    }
                    break;
                case AIState.IdleMad:
                    if (_frame < 4)
                    {
                        _frame = 4;
                    }

                    if (_frame >= 12)
                    {
                        _frame = 0;
                    }
                    break;
                case AIState.Charge:
                    NPC.frameCounter += 0.15f;
                    if (_frame < 12)
                    {
                        _frame = 12;
                    }
                    if (_frame >= 18f)
                    {
                        _frame = 12;
                    }
                    break;
                case AIState.Turn:
                    _frame = 17;
                    break;
                case AIState.Roar:
                case AIState.QuickRoar:
                    if (_frame < 18)
                    {
                        _frame = 18;
                    }
                    if (_frame >= 22)
                    {
                        _frame = 18;
                    }

                    break;
                case AIState.Jump:
                case AIState.JUmp_Shockwave:
                case AIState.Fall:
                    _frame = 18;
                    break;
                case AIState.Crash:
                    _frame = 22;
                    break;

            }

            NPC.frame.Y = frameHeight * _frame;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = NPC.position - screenPos + NPC.Size / 2 + new Vector2(0f, NPC.gfxOffY);
            drawPos.Y -= 50;
            float outlineOffset = 2;
            Vector2 left = Vector2.UnitX * -outlineOffset;
            Vector2 right = Vector2.UnitX * outlineOffset;
            Vector2 up = Vector2.UnitY * -outlineOffset;
            Vector2 down = Vector2.UnitY * outlineOffset;
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;

            Color outlineColor = OutlineColor;
            Vector2 drawOrigin = NPC.frame.Size() / 2;
            spriteBatch.Restart(effect: whiteShader.Effect);

            spriteBatch.Draw(texture, drawPos + left, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + right, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + up, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);
            spriteBatch.Draw(texture, drawPos + down, NPC.frame, outlineColor, NPC.rotation, drawOrigin, _scale, spriteEffects, 0);

            spriteBatch.RestartDefaults();

            MotionBlurShader shader = MotionBlurShader.Instance;
            float maxSpeed = 0.4f;
            float speed = MathHelper.Clamp(NPC.velocity.Length() * 0.02f, 0f, maxSpeed);

            //This is gonna make it like stretch itself as it moves faster
            Vector2 scale = Vector2.Lerp(Vector2.One, new Vector2(2f, 0.18f), EasingFunction.InOutCubic(speed));

            shader.Velocity = Vector2.UnitX * speed;

            //This just affects the opacity of the blur, prob don't need to change this number
            shader.BlurStrength = 2f;
            shader.Apply();
            spriteBatch.Restart(effect: shader.Effect);

            Vector2 runScale = Vector2.Lerp(Vector2.One, new Vector2(1.2f, 1f), MathF.Abs(NPC.velocity.X) / 15f);
            spriteBatch.Draw(texture, drawPos, NPC.frame, Color.White.MultiplyRGB(drawColor) * 0.5f, NPC.rotation, drawOrigin, _scale * runScale, spriteEffects, 0);

            spriteBatch.RestartDefaults();
            spriteBatch.Draw(texture, drawPos, NPC.frame, Color.White.MultiplyRGB(drawColor), NPC.rotation, drawOrigin, _scale * runScale, spriteEffects, 0);

            return false;
        }
        public override void AI()
        {
            base.AI();
            if (_isDangerous)
            {
                OutlineColor = Color.Lerp(OutlineColor, Color.Red, 0.2f);
            }
            else if (_isWarning)
            {
                OutlineColor = Color.Lerp(OutlineColor, Color.Yellow, 0.2f);

            }
            else
            {
                OutlineColor = Color.Lerp(OutlineColor, Color.Transparent, 0.2f);
            }

            if (!_spawned)
            {
                ShowNamePlate();
                SwitchState(AIState.Roar);
                _spawned = true;
            }
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.IdleMad:
                    AI_IdleMad();
                    break;
                case AIState.Charge:
                    AI_Charge();
                    break;
                case AIState.Roar:
                    AI_Roar();
                    break;
                case AIState.Jump:
                    AI_Jump();
                    break;
                case AIState.Fall:
                    AI_Fall();
                    break;
                case AIState.Turn:
                    AI_Turn();
                    break;
                case AIState.Crash:
                    AI_Crash();
                    break;
                case AIState.JUmp_Shockwave:
                    AI_JumpShockwave();
                    break;
                case AIState.QuickRoar:
                    AI_QuickRoar();
                    break;
            }
        }

        public override bool? CanFallThroughPlatforms()
        {
            var target = Main.player[NPC.target];
            bool falling = State == AIState.Fall && target.Bottom.Y > NPC.Bottom.Y;
            bool shockwaving = State == AIState.JUmp_Shockwave && target.Bottom.Y > NPC.Bottom.Y;
            return falling || shockwaving;
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                Cycle = 0;
                if (state == AIState.Idle)
                {
                    RoarCycle = 0;
                }
                State = state;
                NPC.netUpdate = true;
            }
        }

        private void AI_JumpShockwave()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                NPC.velocity.Y -= 10;
                NPC.velocity.X = NPC.direction * 3;

            }
            NPC.spriteDirection = -NPC.direction;
            if (Timer >= 10 && NPC.collideY)
            {
                ShakeModSystem.Shake = 2;
                SoundStyle boom = SoundID.DD2_ExplosiveTrapExplode;
                boom.PitchVariance = 0.3f;
                SoundEngine.PlaySound(boom, NPC.position);
                for (int i = 0; i < 16; i++)
                {
                    float radius = 150;
                    Vector2 offset = Vector2.UnitX * Main.rand.Next(-1, 1);
                    offset *= Main.rand.NextFloat(1f, radius);
                    offset += new Vector2(radius / 2, 0);

                    Vector2 velocity = Vector2.UnitX * Main.rand.Next(-1, 1);
                    velocity *= Main.rand.NextFloat(1f, 2f);
                    Dust.NewDustPerfect(NPC.Bottom + offset, ModContent.DustType<Dusts.TSmokeDust>(), velocity, 0, Color.Black * 0.5f,
                        Main.rand.NextFloat(0.3f, 0.7f));
                }
                //Little bit of screenshake for dramatic effect
                FXUtil.ShakeCamera(NPC.position, 1024, 80);
                Particle.NewParticle<ShockParticle>(NPC.Center, Vector2.Zero, Color.White);

                if (MultiplayerHelper.IsHost)
                {
                    //This is the part where you spawn the cool ahh shockwaves
                    //But we have to make cool ahh shockwaves :(
                    int shockwaveDamage = 20;
                    int knockback = 1;
                    Vector2 velocity = Vector2.UnitX;
                    velocity *= 4;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                        ModContent.ProjectileType<WindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                    velocity = -velocity;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Bottom, velocity,
                        ModContent.ProjectileType<WindShockwave>(), shockwaveDamage, knockback, Main.myPlayer);
                }
                SwitchState(AIState.Idle);
            }
        }

        private void AI_Crash()
        {
            _isDangerous = false;
            _isWarning = false;
            Timer++;
            if (Timer == 1)
            {
                //Crashing sound ? should be quite heavy
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/RocketExplosion");
                soundStyle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(soundStyle, NPC.position);


                SoundStyle rockSmashSound = AssetRegistry.Sounds.Ravager.RavagerRockSmash1;
                if (Main.rand.NextBool(2))
                    rockSmashSound = AssetRegistry.Sounds.Ravager.RavagerRockSmash2;
                rockSmashSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(rockSmashSound, NPC.position);

                //Little bit of screenshake for dramatic effect
                FXUtil.ShakeCamera(NPC.position, 1024, 80);


                Vector2 offset = Vector2.UnitX;
                offset.X *= -NPC.direction * 150;
                offset.Y -= 700;
                _crashPoint = NPC.position;
                _crashPoint += offset;
                for (float f = 0; f < 8; f++)
                {

                    Vector2 particleSpawnPoint = NPC.direction == 1 ? NPC.Right : NPC.Left;
                    particleSpawnPoint.Y += Main.rand.NextFloat(-60, 60);
                    FXUtil.GlowStretch(particleSpawnPoint, Vector2.UnitX * NPC.direction * Main.rand.NextFloat(3, 10));
                }
                Particle.NewParticle<ShockParticle>(NPC.Center, Vector2.Zero, Color.White);
                for (int i = 0; i < 24; i++)
                {
                    Vector2 dustSpawnPoint = NPC.Bottom;
                    dustSpawnPoint.X += Main.rand.Next(-100, 100);
                    Vector2 dustVelocity = -Vector2.UnitY;
                    dustVelocity = dustVelocity.RotatedByRandom(0.05f);
                    Dust.NewDustPerfect(dustSpawnPoint, DustID.Dirt, dustVelocity, Scale: Main.rand.NextFloat(0.4f, 0.8f));
                }
            }
            _scale = Vector2.Lerp(new Vector2(0.85f, 1f), Vector2.One, EasingFunction.InExpo(Timer / 30f));
            if (Timer > 30f)
            {
                _scale = Vector2.Lerp(new Vector2(1.1f, 0.9f), new Vector2(0.9f, 1.1f), MathUtil.Osc(0f, 1f));
            }
            bool shouldCrash = (InPhase2() && RoarCycle >= 3) || (!InPhase2());
            if (shouldCrash)
            {
                float lerp = Timer / 30f;
                lerp = MathHelper.Clamp(lerp, 0, 1);
                lerp = EasingFunction.OutSine(lerp);
                NPC.velocity.X = MathHelper.Lerp(-NPC.direction, 0, lerp);

                //Rock particles and falling rock projectiles to come down
                int rockCount = InPhase2() ? 5 : 10;
                if (Timer < 51 && Timer % rockCount == 0)
                {
                    if (MultiplayerHelper.IsHost)
                    {
                        Vector2 rockSpawnPoint = _crashPoint;
                        rockSpawnPoint.X += Main.rand.Next(-400, 400);
                        Vector2 vel = Vector2.UnitX * Main.rand.NextFloat(-2f, 2f);

                        Projectile.NewProjectile(NPC.GetSource_FromThis(), rockSpawnPoint, vel,
                            ModContent.ProjectileType<RavagerRock>(), FallingRockDamage, FallingRockKB, Main.myPlayer);
                    }
                }

                if (Timer >= StunTime)
                {
                    SwitchState(AIState.Roar);
                }
            }
            else
            {
                RoarCycle++;
                PrepareChase();
            }

        }
        private void AI_Turn()
        {
            Timer++;
            if (Timer == 30)
            {
                if (NPC.direction == 0)
                    NPC.direction = 1;
                NPC.direction *= -1;
                SwitchState(AIState.Idle);
            }
            NPC.velocity.X *= 0.95f;
            NPC.rotation *= 0.99f;

        }
        private void AI_Idle()
        {
            _isDangerous = false;
            _isWarning = false;
            _scale = Vector2.One;
            Timer++;
            if (NPC.HasValidTarget)
            {
                Player target = Main.player[NPC.target];
                NPC.direction = target.position.X > NPC.position.X ? 1 : -1;
            }

            NPC.spriteDirection = -NPC.direction;
            NPC.velocity.X *= 0.9f;
            NPC.TargetClosest();


            if (Timer >= IdleTime && NPC.HasValidTarget)
            {
                SwitchState(AIState.IdleMad);
            }
        }

        private void AI_IdleMad()
        {
            _isDangerous = false;
            _isWarning = true;
            Timer++;
            if (Timer == 1)
            {

                SoundStyle soundStyle = AssetRegistry.Sounds.Ravager.RavagerAngry;
                soundStyle.PitchVariance = 0.2f;
                SoundEngine.PlaySound(soundStyle, NPC.position);
            }
            _scale = Vector2.Lerp(new Vector2(1.1f, 0.95f), Vector2.One, EasingFunction.InOutSine(Timer / 45f));
            NPC.spriteDirection = -NPC.direction;
            NPC.velocity.X *= 0.9f;
            NPC.TargetClosest();
            if (NPC.HasValidTarget)
            {
                if (Timer >= IdleMadTime)
                {
                    if (MultiplayerHelper.IsHost)
                    {
                        if (InPhase3() && Main.rand.NextBool(2))
                        {
                            SwitchState(AIState.JUmp_Shockwave);
                        }
                        else
                        {
                            PrepareChase();
                        }
                    }


                }


            }

        }

        private void PrepareChase()
        {
            Player target = Main.player[NPC.target];
            NPC.direction = target.position.X > NPC.position.X ? 1 : -1;
            //If target is above or below, jump/fall through platforms
            float yDiff = MathF.Abs(target.Bottom.Y - NPC.Bottom.Y);
            float dist = 24;
            if (yDiff > dist && target.Bottom.Y < NPC.Bottom.Y)
            {
                SwitchState(AIState.Jump);
            }
            else if (yDiff > dist && target.Bottom.Y > NPC.Bottom.Y)
            {
                SwitchState(AIState.Fall);
            }
            else
            {
                SwitchState(AIState.Charge);
            }
        }
        private void AI_Charge()
        {
            Timer++;
            NPC.spriteDirection = -NPC.direction;
            NPC.rotation = 0;

            float currentSpeed = NPC.velocity.X;
            float maxSpeed = MaxChaseSpeed;
            float acceleration = 0.5f;

            float speedInterpolant = MathHelper.Clamp(Timer / 60f, 0, 1);
            maxSpeed *= MathHelper.Lerp(0f, 1.5f, EasingFunction.InExpo(speedInterpolant));
            if (Timer < 31f)
            {
                Vector2 smokeSpawnPos = NPC.Bottom;
                smokeSpawnPos.X += Main.rand.NextFloat(-100, 100);
                Vector2 vel = Vector2.UnitX * -NPC.direction;
                vel *= Main.rand.NextFloat(3f, 6f);
                Dust.NewDustPerfect(smokeSpawnPos, ModContent.DustType<TSmokeDust>(), vel);
                if (Timer % 10 == 0)
                {
                    FXUtil.ShakeCamera(NPC.position, 1024, 5);

                    SoundStyle runSound = SoundID.Run;
                    runSound.PitchVariance = 0.2f;
                    runSound.Pitch = -0.5f;
                    SoundEngine.PlaySound(runSound, NPC.position);
                }
            }
            if (Timer >= 30f && Timer % 2 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<TSmokeDust>());
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Grass);
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.t_LivingWood);
            }

            if (currentSpeed > -maxSpeed && NPC.direction == -1)
            {
                NPC.velocity.X -= acceleration;
                if (NPC.velocity.X < -maxSpeed)
                {
                    NPC.velocity.X = -maxSpeed;
                }
            }
            else if (currentSpeed < maxSpeed && NPC.direction == 1)
            {
                NPC.velocity.X += acceleration;
                if (NPC.velocity.X > maxSpeed)
                {
                    NPC.velocity.X = maxSpeed;
                }
            }

            if (Timer >= 30)
            {
                _isDangerous = true;
            }

            if (NPC.collideX)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
                if (InPhase2())
                {
                    SwitchState(AIState.Crash);
                }
                else
                {
                    SwitchState(AIState.Crash);

                }

            }

            //Check if we hit a wall and enter crash state

            if (NPC.HasValidTarget)
            {
                if (Timer > ChargeTime)
                {
                    SwitchState(AIState.Turn);
                }
            }
            else
            {

                SwitchState(AIState.Idle);
            }
        }

        private void AI_Roar()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle roarSound = AssetRegistry.Sounds.Ravager.RavagerRoar;
                roarSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(roarSound, NPC.position);
            }
            if (Timer % 10 == 0)
            {
                FXUtil.ShakeCamera(NPC.position, 1024, 24);
                Particle.NewParticle<ShockParticle>(NPC.Center, Vector2.Zero, Color.White);
            }
            NPC.velocity.X *= 0.9f;
            if (Timer >= 80)
            {
                SwitchState(AIState.Idle);
            }
        }

        private void AI_QuickRoar()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle roarSound = AssetRegistry.Sounds.Ravager.RavagerRoar;
                roarSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(roarSound, NPC.position);
            }
            if (Timer % 10 == 0)
            {
                FXUtil.ShakeCamera(NPC.position, 1024, 24);
                Particle.NewParticle<ShockParticle>(NPC.Center, Vector2.Zero, Color.White);
            }
            NPC.velocity.X = 0.9f;
            if (Timer >= 20)
            {
                PrepareChase();
            }
        }
        private void AI_Jump()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y -= 10;
            }
            NPC.velocity.X *= 0.9f;
            NPC.rotation = 0;
            if (Timer > 10 && NPC.collideY)
            {
                SwitchState(AIState.Charge);
            }

            if (Timer >= 120)
            {
                SwitchState(AIState.Charge);
            }
        }

        private void AI_Fall()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.velocity.Y -= 2;
            }
            NPC.velocity.X *= 0.9f;
            NPC.rotation = 0;
            if (Timer > 10 && NPC.collideY)
            {
                SwitchState(AIState.Charge);
            }

            //Failsafe
            if (Timer >= 120)
            {
                SwitchState(AIState.Charge);
            }
        }
    }
}
