using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public partial class DescendingTwin
    {
        private void AI_PhaseShiftStart()
        {
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
                SoundStyle phaseShift = AssetRegistry.Sounds.SteamPunking.DescendingPhaseShift;
                phaseShift.PitchVariance = 0.2f;
                SoundEngine.PlaySound(phaseShift, NPC.position);

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

            if (Timer % 4 == 0)
            {
                Color twinColor = GetTwinColor();
                Color outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
                Color fadeToColor = Color.Lerp(outerColor, Color.Black, 0.5f);
                var p = FXUtil.GlowCircleBoom(NPC.Center, twinColor, outerColor, fadeToColor);
                p.Scale = MathHelper.Lerp(1f, 4f, completionRatio);

                if (Main.rand.NextBool(2))
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                    Vector2 normalVelocity = NPC.Center - spawnPos;
                    normalVelocity = normalVelocity.SafeNormalize(Vector2.Zero);
                    Vector2 velocity = normalVelocity * 4;
                    FXUtil.GlowStretch(spawnPos, velocity);
                }
                if (Main.rand.NextBool(2))
                {
                    Vector2 spawnPos = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                    Vector2 normalVelocity = NPC.Center - spawnPos;
                    normalVelocity = normalVelocity.SafeNormalize(Vector2.Zero);
                    Vector2 velocity = normalVelocity * 4;
                    Dust.NewDustPerfect(spawnPos, ModContent.DustType<GlowDust>(), velocity, newColor: twinColor);
                }
            }

            if (Timer % 8 == 0)
            {
                Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(64, 64);
                var zapParticle = Particle.NewParticle<ZapParticle>(pos, Main.rand.NextVector2Circular(4, 4), newColor: Color.White);
                zapParticle.innerColor = GetTwinColor();
                zapParticle.outerColor = Color.Lerp(zapParticle.innerColor, Color.Black, 0.5f);
                zapParticle.fadeToColor = Color.Lerp(zapParticle.outerColor, Color.Black, 0.5f);

                //Add zap particle
                SoundStyle zapSound = SoundID.DD2_LightningAuraZap;
                zapSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(zapSound, NPC.position);
            }

            //There's no afterimage on this preparation state
            _afterImageAlpha = 0f;
            _telegraphLineAlpha = 0f;
            _shiftAlpha = MathHelper.Lerp(0f, 1f, ease);

            //Alert the player that something is about to happen fr
            TargetOutlineColor = Color.Yellow;

            //Here we wait a bit longer before they do the sword so that you get a bit of time to react
            if (Timer >= windupTime * 1.3f)
            {
                SwitchState(TwinAIState.PhaseShiftEnd);
            }
        }

        private void AI_PhaseShiftEnd()
        {
            Timer++;
            if (Timer == 1)
            {

            }
            _phaseShift = true;
            _afterImageAlpha = 0f;
            _telegraphLineAlpha = 0f;
            NPC.velocity *= 0.9f;
            if (Timer >= 30)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
