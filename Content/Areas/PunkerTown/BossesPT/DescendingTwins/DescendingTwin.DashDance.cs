using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public partial class DescendingTwin
    {
        //Both dash at you multiple times, crossing each other in the middle, making like a swirl dance
        //Alright, this attack is kinda like that one silksong attack from the cogwork dancers
        //We're going to need to make some really cool movement and visuals for this
        //We'll split this into two attacks
        private Vector2 _simpleDashNormal;
        private void AI_SimpleDashStart()
        {
            //The first attack is a basic dash where the eye looks at you
            //A telegraph line appears, and after a bit of anticipation, they go backward and then forward and do a quick dash
            //Alright so
            //Step 1. target a player, look at them
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
                _simpleDashNormal = TargetNormal;
                AttackNumber++;
            }


            float targetAngle = _simpleDashNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //2. Calculate anticipation
            float windUpTime = 30f;
            if (AttackNumber == 0)
            {
                windUpTime *= 2f;
            }

            if (_phaseShift)
            {
                windUpTime *= 0.5f;
            }

            float completionRatio = Timer / windUpTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            Vector2 movementNormal = Vector2.Lerp(-_simpleDashNormal * 0.5f, _simpleDashNormal, ease);
            Vector2 anticipationVelocity = movementNormal * 10f;
            NPC.velocity = anticipationVelocity;

            //3. Draw the telegraph line
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio / 0.5f);
            _telegraphLineRot = _simpleDashNormal.ToRotation();

            TargetOutlineColor = Color.Yellow;

            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.SimpleDash);
            }
        }

        private int GetDustType()
        {
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return DustID.CursedTorch;
                case TwinVariant.Retina:
                    return DustID.RedTorch;
            }
        }
        private Color GetTwinColor()
        {
            return DescendingTwins.GetTwinColor(GetVariant());
        }
        private void SpawnFlameDust()
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, GetDustType(), Scale: Main.rand.NextFloat(1f, 2f));
            var p = Particle.NewParticle<GlowFragmentParticle>(NPC.Center, Vector2.Zero, Color.White);
            Color twinColor = GetTwinColor();
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }

        private void SpawnFlameDonut()
        {
            //movement donut particles
            var donut = Particle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.Zero) * 2, newColor: Color.White);
            Color twinColor = GetTwinColor();
            donut.innerColor = twinColor;
            donut.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            donut.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }
        private void AI_SimpleDash()
        {
            Timer++;
            if (Timer == 1)
            {
                AttackNumber++;

                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AttackNumber % 2 == 0 ?
                    AssetRegistry.Sounds.SteamPunking.DescendingDash1
                    : AssetRegistry.Sounds.SteamPunking.DescendingDash2;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }

            if (Timer % 5 == 0)
            {
                SpawnFlameDonut();
            }

            if (Timer % 3 == 0)
            {
                SpawnFlameDust();
            }

            //Fade out the dash line and just move in the direction that we were moving
            //We can just multiply the velocity
            float dashTime = 25f;
            if (_phaseShift)
            {
                dashTime *= 0.5f;
            }
            float completionRatio = Timer / dashTime;

            float dashSpeed = 35f;
            if (NPC.velocity.Length() < dashSpeed)
            {
                NPC.velocity *= 1.5f;
            }



            NPC.rotation = NPC.velocity.ToRotation();

            //Fade out the dash line
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, completionRatio);
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(completionRatio));

            //Stretch the sprite a little bit to give a bit of a motion blurring effect
            _scale = Vector2.Lerp(new Vector2(1.5f, 1f), Vector2.One, completionRatio);

            //Set contact damage to be true
            //Make sure we telegraph this properly with red outlines.
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= dashTime)
            {
                SwitchState(TwinAIState.SimpleDashEnd);
            }
        }

        private void AI_SimpleDashEnd()
        {
            Timer++;

            //Simply just slow down
            TargetOutlineColor = Color.Transparent;
            float endDashTime = 15f;
            NPC.velocity = NPC.velocity.RotatedBy(-0.05f);
            NPC.velocity *= 0.95f;
            NPC.rotation = NPC.velocity.ToRotation();
            if (Timer >= endDashTime)
            {
                SwitchState(TwinAIState.Idle);
            }
        }



        private void AI_DashDanceStart()
        {
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
                SoundStyle circlePrepareSound = AssetRegistry.Sounds.SteamPunking.DescendingCircle;
                circlePrepareSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(circlePrepareSound, NPC.position);
                _simpleDashNormal = NPC.velocity;
            }

            //So how do we want this attack to look?
            //I think the twins should orbit around a circle for a bit, on opposite points
            //Then after a while, they look towards you and dash to the point, when they touch each other
            //They'll burst into the dash
            //Alright so

            //First we need to create a circle around our target
            float windUpTime = 80f;
            float circleRadius = 300f;
            Vector2 initialDirection = -Vector2.UnitY;
            Vector2 dashVector = initialDirection * circleRadius;

            //Get an offset based on the variant that this goober is
            float radiansOffset = Variant == TwinVariant.Spazz ? MathHelper.Pi : 0;
            radiansOffset -= MathHelper.PiOver2;

            //get a ratio of how far we are into this prepation state
            float completionRatio = Timer / windUpTime;
            float rads = (MathHelper.TwoPi * 2);
            float radiansToRotateBy = MathHelper.Lerp(0f, rads, completionRatio);
            Vector2 rotatedVector = dashVector.RotatedBy(radiansToRotateBy + radiansOffset);
            Vector2 positionToMoveTo = Target.Center + rotatedVector;
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;

            float inLerp = EasingFunction.InOutSine(completionRatio / 0.5f);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, targetVelocity, completionRatio);

            //We also need to rotate towards the target, we are facing them after all!
            Vector2 targetNormal = TargetNormal;
            float targetAngle = TargetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, completionRatio);

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = targetAngle;
            TargetOutlineColor = Color.Yellow;
            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.DashDancePrepare);
            }
        }

        private void AI_DashDancePrepare()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = NPC.rotation.ToRotationVector2();
                _startVelocity = NPC.velocity;
                SoundStyle windupPrepareSound = AssetRegistry.Sounds.SteamPunking.DescendingWindup;
                windupPrepareSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(windupPrepareSound, NPC.position);
            }
            _telegraphLineAlpha *= 0.5f;
            //Make sure there's a bit of preparation time
            float prepareTime = 30f;
            float completionRatio = Timer / prepareTime;
            float anticipationEase = EasingFunction.Anticipation2(completionRatio);
            Vector2 anticipationVelocity = Vector2.Lerp(-_simpleDashNormal * 5f, _simpleDashNormal * 5f, anticipationEase);
            NPC.velocity = anticipationVelocity;

            //So we build up some anticipation before the dash happens
            //And also fade out the dash line
            TargetOutlineColor = Color.Yellow;
            if (Timer >= prepareTime)
            {
                SwitchState(TwinAIState.DashDance);
            }
        }

        private void AI_DashDance()
        {
            Timer++;
            float dashTime = 15f;

            //Speed up the dash speed
            float dashSpeed = 30f;
            if (NPC.velocity.Length() < dashSpeed)
            {
                NPC.velocity *= 1.5f;
            }


            if (Timer % 5 == 0)
            {
                SpawnFlameDonut();
            }

            if (Timer % 3 == 0)
            {
                SpawnFlameDust();
            }

            //Create a cool little effect for have motion blurring
            float completionRatio = Timer / dashTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _scale = Vector2.Lerp(new Vector2(1.5f, 1f), Vector2.One, ease);

            //Add an after image
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, completionRatio / 0.5f);

            //Enable the contact damage
            _contactDamage = true;
            TargetOutlineColor = Color.Red;

            if (Timer >= dashTime)
            {
                SwitchState(TwinAIState.DashDanceTwirl);
            }
        }

        private void AI_DashDanceTwirl()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle twirlSound = AssetRegistry.Sounds.SteamPunking.DescendingTwirl;
                twirlSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(twirlSound, NPC.position);
            }
            if (Timer % 3 == 0)
            {
                SpawnFlameDust();
            }

            //In this state, the twins rotate their velocity and sin a bit upwards
            //Alright so
            float twirlTime = 30f;
            float radiansToRotateVelocityBy = (MathHelper.TwoPi + MathHelper.Pi) / twirlTime;

            //We need to calculate the direction to rotate by, whether clockwise or counter clockwise
            //This is based on the way the twin
            float direction = Variant == TwinVariant.Spazz ? -1f : 1f;
            radiansToRotateVelocityBy *= direction;

            NPC.velocity = NPC.velocity.RotatedBy(-radiansToRotateVelocityBy);
            NPC.rotation = NPC.velocity.ToRotation();

            //By this point we already smoothed into this, so we can just set the draw variables
            _scale = Vector2.One;
            _afterImageAlpha = 1f;
            if (Timer >= twirlTime)
            {
                SwitchState(TwinAIState.DashDanceEnd);
            }

            //Enable contact damage
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
        }

        private void AI_DashDanceEnd()
        {
            Timer++;
            float endTime = 45f;
            NPC.velocity *= 0.9f;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, TargetNormal.ToRotation(), 0.1f);

            //Fade out the after image
            float completionRatio = Timer / endTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            _afterImageAlpha = MathHelper.Lerp(1f, 0f, ease);
            if (Timer >= endTime)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
