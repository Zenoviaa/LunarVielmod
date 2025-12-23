using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public partial class E
    {
        private Vector2 _forwardVector;
        private int ForwardSlashDamage => 20;
        private void ForwardSlashStartupMovement(float moveTime)
        {
            //Find a position to move to
            float startTime = moveTime;
            float completionRatio = Timer / startTime;
            float easeIn = EasingFunction.InOutExpo7(completionRatio);

            float distanceToBeAway = MathHelper.Lerp(300, 400, EasingFunction.InOutSine(completionRatio));
            Vector2 directionFromTarget = (NPC.Center - MyTarget.Center);
            directionFromTarget = directionFromTarget.SafeNormalize(Vector2.Zero);

            Vector2 positionToMoveTo = MyTarget.Center + directionFromTarget * distanceToBeAway;
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 smoothVelocity = Vector2.Lerp(TargetVector, targetVelocity, easeIn);
            NPC.velocity = smoothVelocity;
        }

        private void AI_ForwardSlashStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
                SoundStyle hurrilock = AssetRegistry.Sounds.E.Hurridown;
                hurrilock.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hurrilock, NPC.position);
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero,
                        ModContent.ProjectileType<BlackStarTrail>(), 1, 0f, Main.myPlayer, ai1: NPC.whoAmI);
                }

            }

            float startTime = 180f;
            float completionRatio = Timer / startTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 offset = new Vector2(0, -400);
            offset = offset.RotatedBy(MathHelper.TwoPi * ease);
            Vector2 positionToMoveTo = MyTarget.Center + offset;
            Vector2 targetVelcoity = positionToMoveTo - NPC.Center;
            NPC.velocity = Vector2.Lerp(TargetVector, targetVelcoity, completionRatio);

            _extraAfterImageAlpha = MathHelper.Lerp(0f, 0.7f, completionRatio);
            TargetOutlineColor = Color.Lerp(Color.Transparent, Color.White, ExtraMath.Osc(0f, 1f, speed: 40));
            Animator.PlayAnimation(Anim_Holding);
            if (Timer >= startTime)
            {
                SwitchState(AIState.ForwardSlash);
            }
        }

        private void AI_ForwardSlashQuickStart()
        {
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            _extraAfterImageAlpha = 0.7f;
            float lerp = _attackNumber / 10f;
            float ease = EasingFunction.InOutSine(lerp);
            float startTime = MathHelper.Lerp(50, 5, ease);
            ForwardSlashStartupMovement(startTime);
            if (Timer >= startTime)
            {
                SwitchState(AIState.ForwardSlash);
            }
        }

        private void Slash()
        {
            ShakeModSystem.Shake = 16;
            FXUtil.ShakeCamera(NPC.position, 1024, 4);


            Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 startPosition = NPC.Center - direction * 1200;
            ScreenSmearEffectManager.NewParticle(startPosition, direction, 2400, 45);

            for (float i = 0; i < 3; i++)
            {
                var donutParticle = Particle.NewParticle<GlowDonutParticle>(NPC.Center, -direction * MathHelper.Lerp(15, 1f, i / 3f));
                donutParticle.Scale *= MathHelper.Lerp(1f, 3f, i / 3f);

            }
            var strike = Particle.NewParticle<GlowDonutParticle>(NPC.Center, direction);
            strike.xMult = 6;
            strike.rotOffset += MathHelper.PiOver2;
            if (MultiplayerHelper.IsHost)
            {
                Vector2 shootVelocity = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                shootVelocity *= 0.45f;
                int projType = ModContent.ProjectileType<EBuster>();
                Projectile.NewProjectile(SourceFromThis, NPC.Center, shootVelocity, projType, ForwardSlashDamage, 1, Main.myPlayer);
            }
            var strike2 = Particle.NewParticle<GlowDonutParticle>(NPC.Center, direction);
            strike2.xMult = 32;
            strike2.rotOffset += MathHelper.PiOver2;
            SoundStyle hurriSlash = AssetRegistry.Sounds.E.Hurrislash;
            hurriSlash.PitchVariance = 0.3f;
            SoundEngine.PlaySound(hurriSlash, NPC.position);
        }

        private void AI_ForwardSlash()
        {
            Timer++;
            if (Timer == 1)
            {
                TargetVector = NPC.Center;
                _forwardVector = (NPC.Center - MyTarget.Center).SafeNormalize(Vector2.Zero);

                SoundStyle newSlashSound = new SoundStyle("Stellamod/Assets/Sounds/SwordSlice");
                newSlashSound.PitchVariance = 0.2f;
                newSlashSound.Volume = 0.5f;
                SoundEngine.PlaySound(newSlashSound, NPC.position);
                Slash();
                NPC.direction = _forwardVector.X > 0 ? -1 : 1;
            }

            _extraAfterImageAlpha = 0.7f;
            if (_attackNumber % 2 == 0)
            {
                Animator.PlayAnimation(Anim_ForwardSlash);
            }
            else
            {
                Animator.PlayAnimation(Anim_BackSlash);
            }

            float forwardSlashTime = 5;



            float completionRatio = Timer / forwardSlashTime;
            float ease = EasingFunction.OutSine(completionRatio);

            if(_attackNumber > 8)
            {
                float osc = ExtraMath.Osc(0f, 1f, speed: 17);
                BlackSea blackSea = ScreenShader.GetInstance<BlackSea>();
                blackSea.amplitude = MathHelper.SmoothStep(0.05f, 0f, ease);
            }

            float maxRadians = MathHelper.PiOver4;
            float radiansOffset = completionRatio * maxRadians;
            Vector2 forwardVector = _forwardVector.RotatedBy(radiansOffset);
            Vector2 recoilStartVector = TargetVector;
            Vector2 recoilEndVector = recoilStartVector + forwardVector * 100;

            Vector2 recoilPosition = Vector2.Lerp(recoilStartVector, recoilEndVector, ease);
            Vector2 targetVelocity = recoilPosition - NPC.Center;
            NPC.velocity = targetVelocity;
            if (Timer >= forwardSlashTime)
            {


                SwitchState(AIState.ForwardSlash_RePosition);
            }
        }

        private void AI_ForwardSlashReposition()
        {
            Timer++;
            if (Timer == 1)
            {
                _forwardVector = (NPC.Center - MyTarget.Center);
                TargetVector = NPC.velocity;
            }

            float rotateTime = 15;
            float completionRatio = Timer / rotateTime;
            float maxRadians = MathHelper.PiOver4;
            float radiansOffset = completionRatio * maxRadians;
            Vector2 forwardVector = _forwardVector.RotatedBy(radiansOffset);
            Vector2 targetPosition = MyTarget.Center + forwardVector;
            Vector2 targetVelocity = targetPosition - NPC.Center;

            _extraAfterImageAlpha = 0.7f;
            float ease = EasingFunction.InOutSine(completionRatio);
            NPC.velocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            if (Timer >= rotateTime)
            {
                SwitchState(AIState.ForwardSlash_End);
            }
        }
        private void AI_ForwardSlashEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if (Timer >= 1)
            {
                _attackNumber++;
                if (_attackNumber >= 32)
                {
                    SwitchState(AIState.Idle);
                }
                else
                {
                    SwitchState(AIState.ForwardSlash_QuickStart);
                }
            }
        }
    }
}
