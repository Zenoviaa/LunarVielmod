using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public partial class DescendingTwin
    {
        //Alright, for this attack, they'll go on opposite sides of you, wind up, and then crash into the center
        //Since it's like a set thing we can do this one with lerping
        private void AI_SuperCrashStart()
        {
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();

                //We're going to move a bit above the player, that should be good
                _simpleDashNormal = Target.Center;
                _startVelocity = NPC.velocity;
                _startRotation = NPC.rotation;

                if(AttackNumber == 0)
                {
                    SoundStyle preparationSound = AssetRegistry.Sounds.SteamPunking.MechSteaming;
                    preparationSound.PitchVariance = 0.3f;
                    SoundEngine.PlaySound(preparationSound, NPC.position);
                }
            
            }

            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
            }

            //Define a startuptime and calculate easing to the point
            float startupTime = 60;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 positionToMoveTo = _simpleDashNormal + GetFlameSwordStartOffset();
            Vector2 targetVelocity = (positionToMoveTo - NPC.Center);
            Vector2 easedVelocity = Vector2.Lerp(_startVelocity, targetVelocity, ease);
            NPC.velocity = easedVelocity;

            //We should just look at the point that we're moving to;
            float targetRotation = (_simpleDashNormal - NPC.Center).ToRotation();
            float playerRotation = TargetNormal.ToRotation();
            float rot = Utils.AngleLerp(playerRotation, targetRotation, ease);
            NPC.rotation = Utils.AngleLerp(_startRotation, rot, ease);
            TargetOutlineColor = Color.Yellow;
            if (Timer >= startupTime)
            {
                SwitchState(TwinAIState.SuperCrashWindup);
            }
        }

        private void AI_SuperCrashWindup()
        {
            Timer++;
            if (Timer == 1)
            {
                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }

            float windupTime = 30f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 start = _simpleDashNormal + GetFlameSwordStartOffset();
            Vector2 end = _simpleDashNormal + GetFlameSwordStartOffset() * 4;
            Vector2 positionToMoveTo = Vector2.Lerp(start, end, ease);
            Vector2 targetVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = targetVelocity;

            _telegraphLineRot = NPC.rotation;
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, ease);
            if (Timer >= windupTime)
            {
                SwitchState(TwinAIState.SuperCrashCrash);
            }

        }

        private void AI_SuperCrashCrash()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle dashSound = AttackNumber % 2 == 0 ?
                    AssetRegistry.Sounds.SteamPunking.DescendingDash1
                    : AssetRegistry.Sounds.SteamPunking.DescendingDash2;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }

            //Crash into each other
            float crashTime = 20f;
            float completionRatio = Timer / crashTime;
            float anticipation = EasingFunction.Anticipation(completionRatio);
            Vector2 start = _simpleDashNormal + GetFlameSwordStartOffset() * 4;
            Vector2 end = _simpleDashNormal;
            Vector2 positionToMoveTo = Vector2.Lerp(start, end, anticipation);
            Vector2 targetVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = targetVelocity;

    
            if (Timer == crashTime - 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DescendingBigBoom>(),
                        DescendingBigBoomDamage, 1, Main.myPlayer, ai1: GetVariant());
                }
                SoundStyle bangSound = AssetRegistry.Sounds.Bishinine.BigBellGroundhit;
                bangSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(bangSound, NPC.position);
            }
            _afterImageAlpha = 1f;
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, anticipation);
            _telegraphLineRot = NPC.rotation;
            TargetOutlineColor = Color.Red;
            if (Timer >= crashTime)
            {
                AttackNumber++;
                if (AttackNumber >= 6)
                {
                    SwitchState(TwinAIState.SuperCrashEnd);

                }
                else
                {
                    SwitchState(TwinAIState.SuperCrashStart);
                }

            }
        }

        private void AI_SuperCrashEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if (Timer >= 15f)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
