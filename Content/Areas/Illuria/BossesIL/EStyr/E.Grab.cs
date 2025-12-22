using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Content.Gores;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class GrabSlice : ModProjectile,
        IDrawBlackStar
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Target
        {
            get => Main.player[(int)Projectile.ai[1]];
        }
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
      

                ShakeModSystem.Shake = 32;
                FXUtil.ShakeCamera(Projectile.position, 1024, 4);

                Vector2 direction = Projectile.velocity;
                Vector2 startPosition = Projectile.Center - direction * 1200;
                ScreenSmearEffectManager.NewParticle(startPosition, direction, 2400, 45);

                for (float i = 0; i < 3; i++)
                {
                    var donutParticle = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, -direction * MathHelper.Lerp(15, 1f, i / 3f));
                    donutParticle.Scale *= MathHelper.Lerp(1f, 3f, i / 3f);

                }
                var strike = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, direction);
                strike.xMult = 6;
                strike.rotOffset += MathHelper.PiOver2;
                var strike2 = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, direction);
                strike2.xMult = 32;
                strike2.rotOffset += MathHelper.PiOver2;

                var strike3 = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, direction);
                strike3.xMult = 48;
                strike3.rotOffset += MathHelper.Pi;

                SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
                hurriSlash.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hurriSlash, Projectile.position);
            }

            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(64, 64), ModContent.DustType<Sparkle>());
            Projectile.Center = Target.Center;
        }

        public void DrawBlackStar(SpriteBatch spriteBatch)
        {

        }
    }
    public class EGrabPlayer : ModPlayer
    {
        public int HookNPC=-1;
        public Vector2? ThrowVelocity;
        public float? ThrowRotation;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (ThrowVelocity.HasValue)
            {
                Player.velocity = ThrowVelocity.Value;
                ThrowVelocity = null;
            }
            if (ThrowRotation.HasValue)
            {
                Player.fullRotation = ThrowRotation.Value;
                ThrowRotation = null;
            }
            if (HookNPC == -1)
                return;
            NPC grabbedByNPC = Main.npc[HookNPC];
            Vector2 targetVelocity = grabbedByNPC.Center - Player.Center;
            Player.velocity = targetVelocity;
        }
    }

    public partial class E
    {
        /*
         * 
         * Very slowly walks up to you, 
         * then a glint appears in their eyes and 
         * they quickly dash at you trying to grab you
         * , does this multiple times 3 times in phase 1, 
         * and up to 7 times in phase 2 (faster cycles too), 
         * if they actually grab you they basically do the same thing as any command grab in elden ring 
         * and the attack ends
        
        */

        private bool _isGrabbing;
        private int GrabDamage => 120;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            base.OnHitPlayer(target, hurtInfo);
            BlackStars.AddBuff(target, 50);
            if (!_isGrabbing)
                return;
            EGrabPlayer grabPlayer = target.GetModPlayer<EGrabPlayer>();
            grabPlayer.HookNPC = NPC.whoAmI;
        }
        private void AI_GrabStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }

            //First we want to position ourselves near the player, so we'll just move up to them
            float distanceToTarget = Vector2.Distance(NPC.Center, MyTarget.Center);
            if (distanceToTarget > 384)
            {
                Vector2 targetVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetVelocity * distanceToTarget / 16f, 0.1f);
            }
            else
            {
                NPC.velocity *= 0.85f;
                SwitchState(AIState.Grab_Walk);
            }
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
            TargetOutlineColor = Color.Yellow;

        }

        private void AI_GrabWalk()
        {
            Timer++;

            //Here we're going to slowly walk towards the player for bit
            //AI move left lol
            Vector2 targetNormal = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 walkVelocity = targetNormal * 4;
            NPC.velocity = Vector2.Lerp(NPC.velocity, walkVelocity, 0.1f);
            TargetOutlineColor = Color.Yellow;

            float walkTime = 60f;
            int halfWalkTime = (int)walkTime / 2;
            if (Timer == halfWalkTime)
            {
                var donut = Particle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity,
                    newColor: Color.White,
                    Scale: 1f);
                donut.noStretch = false;
                donut.Scale *= 2;

            }

            if (Timer >= halfWalkTime)
            {
                NPC.velocity *= 0.2f;
 
            }

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(Timer / (float)walkTime));
            _telegraphLineRot = NPC.velocity.ToRotation();

            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
            if (Timer >= walkTime)
            {
                SwitchState(AIState.Grab_Dash);
            }
        }

        private void AI_GrabDash()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            }

    
   
            NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
            float dashTime = 30;
            float completionRatio = Timer / dashTime;

    
            Vector2 dashVelocity = TargetVector * 45f;
            NPC.velocity = Vector2.Lerp(NPC.velocity, dashVelocity, 0.1f);
            _drawScale.X = MathHelper.Lerp(1f, 2f, EasingFunction.QuadraticBump(completionRatio));
            if (Timer >= dashTime)
            {
                EGrabPlayer grabPlayer = MyTarget.GetModPlayer<EGrabPlayer>();
                if (grabPlayer.HookNPC != -1)
                {
                    SwitchState(AIState.Grab_Punish);
                }
                else
                {
                    SwitchState(AIState.Grab_End);
                }
            }

     
            _isGrabbing = true;
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
        }

        private void DunkImpact()
        {
            ShakeModSystem.Shake = 16;
            int[] gores = AutoGoreLoader.FindGores("GrayRock");
            foreach (int g in gores)
            {
                Gore.NewGore(MyTarget.GetSource_FromThis(),
                    MyTarget.Center,
                    -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), g, Main.rand.NextFloat(0f, 1f));
            }

            var p = Particle.NewBlackParticle<BlackSmokeParticle>(MyTarget.Bottom, Vector2.Zero, Color.DarkGray);

            p.color *= 0.5f;
            p.fadeToColor = Color.Black;
            p.innerColor = Color.DarkGray;
            p.outerColor = Color.Black;

            var sear = Particle.NewParticle<SearParticle>(MyTarget.Center, Vector2.Zero);
            sear.innerColor = Color.Gray;
            sear.outerColor = Color.Blue;
            sear.fadeToColor = Color.Black;
            FXUtil.ShakeCamera(MyTarget.Center, 1024, 8);


            for (float f = 0; f < 4f; f++)
            {
                Vector2 pos = MyTarget.Center;
                pos += Main.rand.NextVector2Circular(80, 80);
                var zap = Particle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(10) * Main.rand.NextFloat(2, 15));
                zap.innerColor = Color.Gray;
                zap.outerColor = Color.Blue;
                zap.fadeToColor = Color.Black;
                zap.Scale *= Main.rand.NextFloat(0f, 0.5f);
                zap.Rotation = Main.rand.NextFloat(0f, 3f);
            }

            FXUtil.ShakeCamera(MyTarget.Bottom, 1024, 32);
            var p3 = FXUtil.GlowCircleBoom(MyTarget.Bottom,
               innerColor: Color.Gray,
               glowColor: Color.LightBlue,
               outerGlowColor: Color.DarkBlue, duration: 15, baseSize: .09f);
            p3.Scale *= 4;


            SoundStyle smashSound = Main.rand.NextBool(2) ? SoundRegistry.HammerHit1 : SoundRegistry.HammerHit2;
            smashSound.PitchVariance = 0.2f;
            SoundEngine.PlaySound(smashSound, NPC.position);

            smashSound = AssetRegistry.Sounds.Bishinine.Comet2;
            SoundEngine.PlaySound(smashSound, NPC.position);

            var part = Particle.NewParticle<GlowDonutParticle>(MyTarget.Bottom, Vector2.Zero, Color.White);
            part.fadeToColor = Color.Black;
            part.outerColor = Color.White;
            part.noStretch = true;
            part.shrink = true;

            var part2 = Particle.NewParticle<GlowDonutParticle>(MyTarget.Bottom, Vector2.Zero, Color.White);
            part2.fadeToColor = Color.Black;
            part2.outerColor = Color.White;
            part2.noStretch = true;
            part2.color *= 0.5f;
        }

        private void AI_GrabDunk()
        {
            Timer++;
            if (Timer == 1)
            {
                //Go down and then throw the palyer up, and throw a sword into them lmao
                TargetVector = NPC.Center;

                //Calculate which direction we're going to dunk
                _forwardVector = NPC.velocity.X > 0 ? Vector2.UnitX : -Vector2.UnitX;
            }


            //Ok so in this state we just do a simple dunk
            DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
            float dunkTime = 22;
            float completionRatio = Timer / dunkTime;
            float ease = completionRatio;
            Vector2 startCenter = TargetVector;
            Vector2 endCenter = startCenter + _forwardVector * 512;
            endCenter.Y = fallSystem.hoverPlatformY + 16;

            Vector2 interpolatedPosition = Vector2.Lerp(startCenter, endCenter, ease);
            interpolatedPosition.Y -= MathHelper.Lerp(0, 256, EasingFunction.QuadraticBump(completionRatio)); ;
            if(Timer == dunkTime - 1)
            {
                DunkImpact();
            }

            //We're going to move in an arc
            Vector2 targetVelocity = interpolatedPosition - NPC.Center;
            NPC.velocity = targetVelocity;
            if (Timer >= dunkTime)
            {
                SwitchState(AIState.Grab_EatDirt);
            }
        }

        private void AI_GrabEatDirt()
        {
            Timer++;
            if(Timer == 1)
            {

            }

            ShakeModSystem.Shake = 6;
            int[] gores = AutoGoreLoader.FindGores("GrayRock");
            int goreToSpawn = gores[Main.rand.Next(0, gores.Length)];
            Gore.NewGore(MyTarget.GetSource_FromThis(),
                            MyTarget.Bottom,
                            -Vector2.UnitY.RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(5f, 15f), goreToSpawn, Main.rand.NextFloat(0f, 1f));
          
            float eatDirtTime = 100;
            NPC.velocity.Y = 0;
          
            if(Timer % 5 == 0)
            {
                PlayerDeathReason reason = new PlayerDeathReason();
                MyTarget.Hurt(reason, 15, 1, cooldownCounter: 1);
                for (int i = 0; i < MyTarget.hurtCooldowns.Length; i++)
                {
                    MyTarget.hurtCooldowns[i] = 0;
                }
            }

            if(Timer % 10 == 0)
            {
                var p = Particle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero), newColor: Color.White, Scale: 1);
                p.Scale *= 0.5f;
                Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<Sparkle>());
           
            }
            Dust.NewDustPerfect(MyTarget.Bottom, ModContent.DustType<TSmokeDust>(), -Vector2.UnitY, newColor: Color.White, Scale: 0.76f);
            if (MathF.Abs(NPC.velocity.X) < 20)
            {

                NPC.velocity.X += _forwardVector.X;
                NPC.velocity.X *= 1.01f;
            }
           OffsetCameraModifier.FocusTargetOffset = new Vector2(NPC.velocity.X * 40, 0);
            _extraAfterImageAlpha = MathHelper.Lerp(0f, 0.6f, Timer / eatDirtTime);
            if(Timer >= eatDirtTime)
            {
                SwitchState(AIState.Grab_ThrowSword);
            }
        }

        private void AI_GrabThrowSword()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.velocity;
                _forwardVector = NPC.velocity.X > 0 ? -Vector2.UnitX : Vector2.UnitX;
                ScreenSmearEffectManager.NewParticle(MyTarget.Center, -Vector2.UnitY, 1000, 25);
            }

            _extraAfterImageAlpha = 0.6f;

            float throwTime = 90;
            float completionRatio = Timer / throwTime;
            float ease = EasingFunction.OutExpo(completionRatio);
            Vector2 throwVelocity = Vector2.Lerp(-Vector2.UnitY * 75, Vector2.UnitY * 10, ease);
            EGrabPlayer grabPlayer = MyTarget.GetModPlayer<EGrabPlayer>();
            grabPlayer.HookNPC = -1;
            grabPlayer.ThrowVelocity = throwVelocity;
            grabPlayer.ThrowRotation = MathHelper.Lerp(0f, MathHelper.TwoPi * 2 + MathHelper.PiOver4, completionRatio);

            float offset = 353;
            Vector2 startOffset = new Vector2(offset * _forwardVector.X, 128);
            Vector2 startCenter = MyTarget.Center - startOffset;
            Vector2 endCenter = startCenter + new Vector2(offset * 2 * _forwardVector.X, 384);

            if(Timer < 15)
            {
                Dust.NewDustPerfect(MyTarget.Center, ModContent.DustType<TSmokeDust>(), Vector2.UnitY, newColor: Color.White, Scale: 0.76f);
            }
         
            if (Timer < 30f)
            {
              
                float prepEase = EasingFunction.InOutCubic(Timer / 30f);
                Vector2 targetVelocity = startCenter - NPC.Center;
                NPC.velocity = Vector2.Lerp(TargetVector, targetVelocity, prepEase);
                NPC.direction = (int)(-1 * _forwardVector.X);
            } else if (Timer < 90)
            {
                float dashRatio = (Timer - 30f) / 60f;
                float dashEase = EasingFunction.InOutSine(dashRatio);
       
                Vector2 dashVelocity = endCenter - NPC.Center;
                NPC.velocity = Vector2.Lerp(Vector2.Zero, dashVelocity, dashEase);
                NPC.direction = (int)(1 * _forwardVector.X);


                //Squash and stretch always looks cool
                _drawScale.X = MathHelper.Lerp(1f, 1.5f, EasingFunction.QuadraticBump(dashRatio));
            }
            else
            {
                
            }

          //  RetargetCameraModifier.ReTargetPosition = MyTarget.Center - new Vector2(0, 300);
            if (Timer == 50)
            {

                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(SourceFromThis, MyTarget.Center, Vector2.UnitX.RotatedByRandom(1.2f), ModContent.ProjectileType<GrabSlice>(), GrabDamage, 1,
                        Main.myPlayer, ai1: MyTarget.whoAmI);
                }
            }

            _attackNumber = 999;
            NPC.velocity *= 0.9f;
            if (Timer >= throwTime)
            {
                SwitchState(AIState.Grab_End);
            }
        }

        private void AI_GrabEnd()
        {
            Timer++;
            _extraAfterImageAlpha *= 0.2f;
            NPC.velocity *= 0.9f;
            if (Timer >= 15)
            {
                if (_attackNumber >= 3)
                {
                    SwitchState(AIState.Idle);
                }
                else
                {
                    SwitchState(AIState.Grab_Start);
                }
            }
        }
    }
}
