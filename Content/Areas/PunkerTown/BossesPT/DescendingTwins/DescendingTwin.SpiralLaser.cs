using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public partial class DescendingTwin
    {
        private int SpiralLaserDamage => 25;
        private Vector2 GetSpiralStartOffset()
        {
            float distanceOffset = 60f;
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return -Vector2.UnitX * distanceOffset;
                case TwinVariant.Retina:
                    return Vector2.UnitX * distanceOffset;
            }
        }
        private void AI_SpiralLaserStart()
        {
            Timer++;
            if(Timer == 1)
            {
                SetTargetToCommanderTarget();
                _simpleDashNormal = Target.Center + new Vector2(0, -200);
                _startVelocity = NPC.velocity;
                _startRotation = NPC.rotation;

                SoundStyle beep = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                beep.PitchVariance = 0.3f;
                SoundEngine.PlaySound(beep, NPC.position);

            }

            //First thing wen eed to do is get a point
            float startTime = 60f;
            float completionRatio = Timer / startTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);

            Vector2 spiralOffset = GetSpiralStartOffset();
            Vector2 targetPosition = _simpleDashNormal + spiralOffset;
            Vector2 targetVelocity = (targetPosition - NPC.Center);
            Vector2 easeVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
            NPC.velocity = easeVelocity;

            float targetRotation = -spiralOffset.ToRotation();
            float lerpRotation = Utils.AngleLerp(_startRotation, targetRotation, ease);
            NPC.rotation = lerpRotation;

            TargetOutlineColor = Color.Yellow;
            if (Timer >= startTime)
            {
                SwitchState(TwinAIState.SpiralLaserWindup);
            }
        }

  
        private void AI_SpiralLaserWindup()
        {
            Timer++;
            if(Timer == 1)
            {
                SoundStyle beep = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                beep.PitchVariance = 0.3f;
                SoundEngine.PlaySound(beep, NPC.position);

            }
            float windupTime = 30f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.QuadraticBump(completionRatio);
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, ease);
            _telegraphLineRot = NPC.rotation;
            NPC.velocity = Vector2.Zero;
            TargetOutlineColor = Color.Yellow;
            //Now what we need to do is take that and make a telegraph line for a second
            if (Timer >= windupTime)
            {
                SwitchState(TwinAIState.SpiralLaserLoop);
            }
        }

        private void AI_SpiralLaserLoop()
        {
            Timer++;
            if(Timer == 1)
            {
                _startRotation = NPC.rotation;
                SoundStyle flamethrower = AssetRegistry.Sounds.SteamPunking.DescendingFlamethrower;
                flamethrower.PitchVariance = 0.3f;
                SoundEngine.PlaySound(flamethrower, NPC.position);

                if (MultiplayerHelper.IsHost)
                {
                    Vector2 initialVelocity = NPC.rotation.ToRotationVector2();
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, initialVelocity * 1000, 
                        ModContent.ProjectileType<DescendingLaser>(), SpiralLaserDamage, 1, Main.myPlayer, ai1: NPC.whoAmI, ai2: GetVariant());

                }
            }

            if(Timer % 5 == 0)
            {
                SpawnFlameDust();
            }

            if(Timer % 10 == 0)
            {
                SpawnSteamParticle();
            }
            float loopTime = 600f;
            float radiansToAdd = MathHelper.Lerp(0f, 0.04f, EasingFunction.InOutSine(Timer / 120f));
            NPC.rotation += radiansToAdd;
            Vector2 newNormal = NPC.rotation.ToRotationVector2();
            Vector2 newOffset = newNormal * 60f;
            Vector2 positionToMoveTo = _simpleDashNormal + newOffset;
            Vector2 targetVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = targetVelocity;


            Vector2 velocityToPlayer = (Target.Center - _simpleDashNormal);
            velocityToPlayer = velocityToPlayer.SafeNormalize(Vector2.Zero);
            _simpleDashNormal += velocityToPlayer * 4;

            _afterImageAlpha = 1f;
            _telegraphLineAlpha = MathHelper.Lerp(_telegraphLineAlpha, 0f, 0.1f);
            TargetOutlineColor = Color.Red;
            if(Timer >= loopTime)
            {
                SwitchState(TwinAIState.SpiralLaserEnd);
            }
        }

        private void AI_SpiralLaserEnd()
        {
            Timer++;
            _afterImageAlpha = MathHelper.Lerp(1f, 0f, Timer / 15f);
            TargetOutlineColor = Color.Transparent;
            NPC.velocity *= 0.95f;
            if(Timer >= 15)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
