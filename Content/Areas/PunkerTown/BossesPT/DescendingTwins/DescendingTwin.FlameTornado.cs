using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
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
        private Vector2 GetFlameTornadoStartOffset()
        {
            float distanceOffset = 450f;
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return -Vector2.UnitX * distanceOffset;
                case TwinVariant.Retina:
                    return Vector2.UnitX * distanceOffset;
            }
        }

        private void AI_FlameTornadoStart()
        {
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
                _simpleDashNormal = NPC.velocity;
            }

            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
                SpawnFlameDust();
            }

            /*
             * 
             * Both of them aim above you, shooting a type of fire (Descender Retina), shoots a red fire,
             * while Descender Spazz, shoots a green flame, and they make a crossing sword, and continuously going downwards, making you dodge
             */

            //So first we need to get them ina  good position for doing this attack
            //I think it'd be best if they position themselves on opposite sides of you
            //Alright so
            //First let's get that position and move to it
            Vector2 flameSwordOffset = GetFlameTornadoStartOffset();
            Vector2 positionToMoveTo = Target.Center + flameSwordOffset;


            float windupTime = 80f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 movementVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, movementVelocity, completionRatio);

            //Look at the player
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //There's no afterimage on this preparation state
            _afterImageAlpha = 0f;

            //Alert the player that something is about to happen fr
            TargetOutlineColor = Color.Yellow;

            //Here we wait a bit longer before they do the sword so that you get a bit of time to react
            if (Timer >= windupTime * 1.3f)
            {
                SwitchState(TwinAIState.FlameTornadoWindup);
            }
        }

        private void AI_FlameTornadoWindup()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle beep = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                beep.PitchVariance = 0.3f;
                SoundEngine.PlaySound(beep, NPC.position);

                _simpleDashNormal = TargetNormal;
            }

            NPC.velocity.Y -= 1;
            NPC.velocity *= 0.9f;

            //We need to look up at a 30 degree angle, shoot, and then move downward
            //Alright
            float windupTime = 30f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.Anticipation(completionRatio);
            float directionToRotate = _simpleDashNormal.X > 0 ? 1f : -1f;
            float radiansOffset = MathHelper.Lerp(0f, -MathHelper.PiOver4 / 2f * directionToRotate, ease);

            //That new direction that we are facing
            Vector2 newNormal = _simpleDashNormal.RotatedBy(radiansOffset);
            NPC.rotation = newNormal.ToRotation();
            TargetOutlineColor = Color.Yellow;

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = NPC.rotation;
            if (Timer >= windupTime)
            {
                SwitchState(TwinAIState.FlameTornadoShoot);
            }
        }

        private void AI_FlameTornadoShoot()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle flamethrower = AssetRegistry.Sounds.SteamPunking.DescendingFlamethrower;
                flamethrower.PitchVariance = 0.3f;
                SoundEngine.PlaySound(flamethrower, NPC.position);
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 fireVelocity = NPC.rotation.ToRotationVector2() * 15;
                    DescendingRisingTornado tornado = Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<DescendingRisingTornado>(), FlameSwordDamage, 1, Main.myPlayer, ai2: GetVariant()).ModProjectile as DescendingRisingTornado;
                    tornado.ReTargetPosition = Target.Center;
                }
                SpawnFlameDonut();
            }


            //Move downward whiel shooting
            float continuosTime = 100f;
            float completionRatio = Timer / continuosTime;
            float ease = EasingFunction.Anticipation2(completionRatio / 0.5f);
            NPC.velocity = Vector2.Lerp(Vector2.Zero, -NPC.rotation.ToRotationVector2() * 10f, ease);
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, ease);
            TargetOutlineColor = Color.Yellow;
            ShakeModSystem.Shake = 4;
            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
                SpawnSteamParticle();
            }

            if (Timer >= continuosTime)
            {
                SwitchState(TwinAIState.FlameTornadoEnd);
            }
        }

        private void AI_FlameTornadoEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if (Timer >= 15)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
