using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public partial class DescendingTwin
    {
        private Vector2 GetFlameSwordStartOffset()
        {
            float distanceOffset = 300f;
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return -Vector2.UnitX * distanceOffset;
                case TwinVariant.Retina:
                    return Vector2.UnitX * distanceOffset;
            }
        }
        private void SpawnSteamParticle()
        {
            Vector2 spawnPosition = NPC.Top;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.Zero;
            spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }
        private void AI_FlameSwordStart()
        {
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
                _simpleDashNormal = NPC.velocity;
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
            Vector2 flameSwordOffset = GetFlameSwordStartOffset();
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
                SwitchState(TwinAIState.FlameSwordWindup);
            }

        }

        private void AI_FlameSwordAim()
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
                SwitchState(TwinAIState.FlameSwordContinuous);
            }
        }
        private void AI_FlameSwordContinuous()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = NPC.rotation.ToRotationVector2().SafeNormalize(Vector2.Zero);
                SoundStyle flamethrower = AssetRegistry.Sounds.SteamPunking.DescendingFlamethrower;
                flamethrower.PitchVariance = 0.3f;
                SoundEngine.PlaySound(flamethrower, NPC.position);
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 fireVelocity = NPC.rotation.ToRotationVector2() * 800;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<DescendingFlameSword>(), FlameSwordDamage, 1, Main.myPlayer, ai1: NPC.whoAmI, ai2: GetVariant());
                }
                SpawnFlameDonut();
            }


            //Move downward whiel shooting
            float continuosTime = 100f;
            float completionRatio = Timer / continuosTime;
            float ease = EasingFunction.Anticipation2(completionRatio / 0.5f);
            NPC.velocity = Vector2.Lerp(Vector2.Zero, Vector2.UnitY * 10f, ease);

            float directionToRotate = _simpleDashNormal.X > 0 ? 1f : -1f;
            float radiansOffset = MathHelper.Lerp(0f, MathHelper.PiOver2 * directionToRotate, completionRatio);

            //That new direction that we are facing
            Vector2 newNormal = _simpleDashNormal.RotatedBy(radiansOffset);
            NPC.rotation = newNormal.ToRotation();

            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine(completionRatio / 0.3f));
            _telegraphLineRot = NPC.rotation;
            TargetOutlineColor = Color.Red;
            ShakeScreenPosition.Shake = 4;
            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
                SpawnSteamParticle();
            }

            if (Timer >= continuosTime)
            {
                SwitchState(TwinAIState.FlameSwordEnd);
            }
        }

        private void AI_FlameSwordEnd()
        {
            float endTime = 15f;
            Timer++;

            NPC.velocity *= 0.9f;
            TargetOutlineColor = Color.Transparent;
            if (Timer >= endTime)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
