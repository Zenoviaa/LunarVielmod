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
        //they also have an attack where they both charge a giant electric ball that shoots at you, 
        //and explodes before they do another attack
        //Alright, so for this attack, what we're going to do is have the twin basically choose an anchor position based on the current target
        //Go to the point, one eye looks at said point, while the other eye orbits around it, feeding it size
        //the projectile grows and grows and then it shoots out at you and explodes
        //Similar to daedus lightning ball, but we're going to use a different shader here
        //So let's get started

        //We can break this up into 4 states
        //First we need to get that starting point to move to
        private int ElectricBallDamage => 20;
        private DescendingElectricBall _electricBallProjectile;
        private void AI_ElectricBallStart()
        {
            Timer++;
            if(Timer == 1)
            {
                SetTargetToCommanderTarget();

                //We're going to move a bit above the player, that should be good
                _simpleDashNormal = Target.Center - new Vector2(0, 200);
                _startVelocity = NPC.velocity;
                _startRotation = NPC.rotation;

                SoundStyle preparationSound = AssetRegistry.Sounds.SteamPunking.MechSteaming;
                preparationSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(preparationSound, NPC.position);
            }

            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
            }

            //Define a startuptime and calculate easing to the point
            float startupTime = 60f;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutExpo(completionRatio);
            Vector2 positionToMoveTo = _simpleDashNormal + GetFlameSwordStartOffset();
            Vector2 targetVelocity = (positionToMoveTo - NPC.Center);
            Vector2 easedVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
            NPC.velocity = easedVelocity;

            //We should just look at the point that we're moving to;
            float targetRotation = (_simpleDashNormal - NPC.Center).ToRotation();
            NPC.rotation = Utils.AngleLerp(_startRotation, targetRotation, ease);
            TargetOutlineColor = Color.Yellow;
            if(Timer >= startupTime)
            {
                SwitchState(TwinAIState.ElectricBallWindup);
            }
        }

        private void AI_ElectricBallWindup()
        {
            //So at this point we're going to orbit around this point and create the electric ball
            Timer++;
            if(Timer == 1)
            {
                //Start charging up the ball
                SoundStyle chargingSound = AssetRegistry.Sounds.SteamPunking.DescendingElectricCharge;
                chargingSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(chargingSound, NPC.position);

                _startVelocity = GetFlameSwordStartOffset();
                _startRotation = NPC.rotation;
                if(MultiplayerHelper.IsHost && Variant == TwinVariant.Spazz)
                {
                    _electricBallProjectile = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), _simpleDashNormal, Vector2.Zero, 
                        ModContent.ProjectileType<DescendingElectricBall>(), ElectricBallDamage, 1, Main.myPlayer).ModProjectile as DescendingElectricBall;
                }
            }

            if(Timer % 5 == 0)
            {
                SpawnFlameDust();
            }

            float chargingTime = 120f;
            float completionRatio = Timer / chargingTime;
            float ease = EasingFunction.InOutExpo(completionRatio);
            float radiansToRotateBy = MathHelper.Lerp(0f, MathHelper.TwoPi, ease);
            Vector2 newOffset = _startVelocity.RotatedBy(radiansToRotateBy);
            Vector2 positionToMoveTo = _simpleDashNormal + newOffset;
            Vector2 targetVelocity = (positionToMoveTo - NPC.Center);

            float ease2 = completionRatio / 0.5f;
            ease2 = EasingFunction.InOutSine(ease2);
            Vector2 easedVelocity = Vector2.Lerp(Vector2.Zero, targetVelocity, ease2);
            NPC.velocity = easedVelocity;

            float targetRotation = (_simpleDashNormal - NPC.Center).ToRotation();
            NPC.rotation = Utils.AngleLerp(_startRotation, targetRotation, ease2);

            _afterImageAlpha = MathHelper.Lerp(0f, 1f, ease);
            TargetOutlineColor = Color.Yellow;
            if(Timer >= chargingTime)
            {
                SwitchState(TwinAIState.ElectricBallShoot);
            }
        }

        private void AI_ElectricBallShoot()
        {
            Timer++;
            if(Timer == 1)
            {
                SoundStyle chargingSound = AssetRegistry.Sounds.SteamPunking.DescendingCircle;
                chargingSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(chargingSound, NPC.position);

                if (MultiplayerHelper.IsHost && Variant == TwinVariant.Spazz)
                {
                    _electricBallProjectile.Fire();
                }
            }

            float shootTime = 100;
            NPC.velocity *= 0.9f;

            float targetRotation = TargetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetRotation, 0.1f);

            float completionRatio = Timer / shootTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _afterImageAlpha = MathHelper.Lerp(1f, 0f, ease);
            TargetOutlineColor = Color.Transparent;
            if(Timer >= shootTime)
            {
                SwitchState(TwinAIState.ElectricBallEnd);
            }
        }

        private void AI_ElectricBallEnd()
        {
            Timer++;
            float endTime = 15;
            if(Timer >= endTime)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
