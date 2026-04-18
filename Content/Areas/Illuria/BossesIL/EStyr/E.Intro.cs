using Microsoft.Xna.Framework;
using Stellamod.Core;
using Stellamod.Core.Camera;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public partial class E
    {

        private void IntroHoverMovement()
        {
            //Floats
            Vector2 hoverVelocity = CalculateHoverVelocity();
            NPC.velocity = hoverVelocity;

            //First we make the camera move to him
            CameraTargetSystem.AddTarget(NPC.Center);

            //Face away the player
            NPC.direction = TargetDirection;
        }
        private void AI_IntroPreFight()
        {
            Timer++;
            //Make sure to target the player
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            //Play the head turn animation
            Animator.PlayAnimation(Anim_Idle);

            Vector2 hoverVelocity = CalculateHoverVelocity();
            NPC.velocity = hoverVelocity;
            //Face away the player
            NPC.direction = TargetDirection;
            Main.windSpeedCurrent = 0;
        }

        /*
         * Starts out by Zui calling out to you and pointing at the mysterious aura farming Styr black figure,
         * As they turn around a little with their head, and holds their hand out and starts the uh domain expansion thing,
         * which turns the screen white and black mostly with some greys in between
         * */
        private void AI_IntroIdle()
        {
            Timer++;
            //Make sure to target the player
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            //Calculate easing for the hover effect
            float headTurnTime = 120f;
            float completionRatio = Timer / headTurnTime;
            float easing = EasingFunction.OutExpo(completionRatio);

         
            //Play the head turn animation
            Animator.PlayAnimation(Anim_Idle);

            IntroHoverMovement();
            Main.windSpeedCurrent = 0;

            //After a decent amount of time, switch to the hand out state 
            if (Timer >= headTurnTime)
            {
                SwitchState(AIState.Intro_SwordHold);
            }
        }

        private void AI_IntroSwordHold()
        {
            Timer++;
            //Make sure to target the player
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            //Calculate easing for the hover effect
            float headTurnTime = 120f;
            float completionRatio = Timer / headTurnTime;
            float easing = EasingFunction.OutExpo(completionRatio);

            //Floats
            IntroHoverMovement();

            //Play the head turn animation
            Animator.PlayAnimation(Anim_SwordHold);
        }


 
        private void AI_IntroHandOut()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
                SoundStyle starCharge = new SoundStyle("Stellamod/Assets/Sounds/StarCharge");
                starCharge.Pitch = -0.6f;
                SoundEngine.PlaySound(starCharge);
            }

            float handOutTime = 200f;
            float completionRatio = Timer / handOutTime;
            float easing = EasingFunction.OutExpo(completionRatio);
            Main.windSpeedCurrent = MathHelper.Lerp(0f, 6, completionRatio);
            //Bring in the mini orb
            float orbEase = EasingFunction.InOutExpo(completionRatio);
            if(Main.netMode != NetmodeID.Server)
            {
                BlackSeaRenderer blackseaRenderer = ModContent.GetInstance<BlackSeaRenderer>();
                blackseaRenderer.miniOrbDrawPosition = NPC.Center;
                blackseaRenderer.miniOrbDrawScale = MathHelper.Lerp(0f, 0.4f, orbEase);
            }

            if(Timer >= 100)
            {
                ShakeModSystem.Shake = 4;
            }

            //Spawn particles to go into him
            if(Timer % 6 == 0 && Timer < 100)
            {
                Vector2 particleSpawnPosition = NPC.Center + Main.rand.NextVector2CircularEdge(512, 512);
                Vector2 particleVelocity = NPC.Center - particleSpawnPosition;
                particleVelocity *= Main.rand.NextFloat(0.01f, 0.12f);
                FXUtil.GlowStretch(particleSpawnPosition, particleVelocity);
                if (Main.rand.NextBool(2))
                {
                    Dust.NewDustPerfect(particleSpawnPosition, ModContent.DustType<GlowDust>(), particleVelocity, 
                        newColor: Color.White, 
                        Scale: 0.25f);
                }
            }


            //Floats and charges himself up, slowly turning white

            IntroHoverMovement();
            Animator.PlayAnimation(Anim_HandOut);

            //Face away the player
            NPC.direction = TargetDirection;
        }

        private void AI_IntroHeadTurn()
        {
            Timer++;
            //Make sure to target the player
            if (Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            //Calculate easing for the hover effect
            float headTurnTime = 270f;
            float completionRatio = Timer / headTurnTime;
            float easing = EasingFunction.OutExpo(completionRatio);


            //Play the head turn animation
            Animator.PlayAnimation(Anim_LookOver);

            IntroHoverMovement();
            ShakeModSystem.Shake = 4;
            Main.windSpeedCurrent = 6;
            if (Main.netMode != NetmodeID.Server)
            {
                BlackSeaRenderer blackseaRenderer = ModContent.GetInstance<BlackSeaRenderer>();
                blackseaRenderer.miniOrbDrawPosition = NPC.Center;
                blackseaRenderer.miniOrbDrawScale = 0.4f;
            }

            DomainExpansionManager fallSystem = ModContent.GetInstance<DomainExpansionManager>();
            Vector2 targetPlatform = new Vector2(NPC.Center.X, fallSystem.hoverPlatformY - 128);
            Vector2 targetVelocity = targetPlatform - NPC.Center;
            NPC.velocity = Vector2.Lerp(TargetVector, targetVelocity, EasingFunction.InOutSine(completionRatio));

            //After a decent amount of time, switch to the hand out state 
            if (Timer >= headTurnTime)
            {
                SwitchState(AIState.Intro_DomainExpansion);
            }
        }


        private void AI_IntroDomainExpansion()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.Center;
                var donut = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, Vector2.Zero, Color.White);
                donut.innerColor = Color.White;
                donut.Scale *= 4;
                donut.noStretch = true;
                _drawDarkened = true;
                ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Black, 0.5f, 30);
                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/VoidBlasterExplosionBomb2");
                explosionSound.Pitch = -0.3f;
                SoundEngine.PlaySound(explosionSound);
            }

            //Domain expansion time
            float domainExpansionTime = 120f;
            float completionRatio = Timer / domainExpansionTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Animator.PlayAnimation(Anim_HandOut);

            if (Main.netMode != NetmodeID.Server)
            {
                BlackSeaRenderer blackseaRenderer = ModContent.GetInstance<BlackSeaRenderer>();
                blackseaRenderer.miniOrbDrawPosition = Vector2.Lerp(TargetVector, Main.Camera.Center, ease);
                blackseaRenderer.miniOrbDrawScale = MathHelper.Lerp(0.4f, 1f, ease);
            }
            ShakeModSystem.Shake = 8;

            //Keep the camera on the boss
            CameraTargetSystem.AddTarget(NPC.Center);
            NPC.velocity = Vector2.Zero;

            //Set the black sea shader active
            BlackSea blackSea = ScreenShader.GetInstance<BlackSea>();
            blackSea.alpha = 1f;

            //Increase the size of the spiral
            DomainExpansion domainExpansion = ScreenShader.GetInstance<DomainExpansion>();
            domainExpansion.radius = MathHelper.Lerp(0f, 2f, ease);

            //Invert the screen color during this effect
            Invert invert = ScreenShader.GetInstance<Invert>();
            invert.alpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(completionRatio));
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(completionRatio));
            //Calculate the epicenter of the effect
            Vector2 diff = NPC.Center - Main.screenPosition;
            float x = diff.X / (float)Main.screenWidth;
            float y = diff.Y / (float)Main.screenHeight;
            Vector2 epicenter = new Vector2(x, y);
            domainExpansion.epicenter = epicenter;
            domainExpansion.alpha = 1f;

            //Face away the player
            NPC.direction = TargetDirection;
            if (Timer >= domainExpansionTime)
            {
                SwitchState(AIState.Intro_Finish);
            }
        }

        private void AI_IntroFinish()
        {
            Timer++;

            if (!_showNamePlate)
            {
                ShowNamePlate();
                _showNamePlate = true;
                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/VTeleportOut");
                SoundEngine.PlaySound(explosionSound);
            }
            _intro = true;
            float domainShrinkTime = 60f;
            float completionRatio = Timer / domainShrinkTime;
            float ease = EasingFunction.InOutExpo(completionRatio);
            DomainExpansion domainExpansion = ScreenShader.GetInstance<DomainExpansion>();
            domainExpansion.radius = MathHelper.Lerp(2f, 0f, ease);
            CameraTargetSystem.AddTarget(NPC.Center);

            //Face away the player
            NPC.direction = TargetDirection;
            if (Timer >= domainShrinkTime * 2f)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
