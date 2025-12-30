using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public class DescendingTwinFlameTrail : ScarletProjectile
    {
        private float _deathTimer;

        private int Variant => (int)Projectile.ai[0];
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[1]];
        }
        private ref float DeathState => ref Projectile.ai[2];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2000;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 32;
            Projectile.width = 1;
            Projectile.height = 1;
           
        }

        private Color GetFlamingTrailColor(float completionRatio)
        {
            float fade = 1f - (_deathTimer / 60f);
            return Color.Lerp(Color.White, Color.Transparent, completionRatio) * fade;
        }

        private float GetFlamingTrailWidth(float completionRatio)
        {
            float fade = 1f - (_deathTimer / 60f);
            return MathHelper.SmoothStep(222, 222, completionRatio) * fade;
        }


        private void DrawFlamingTrail(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var shader = BlackFireShader.Instance;
            shader.Time = Main.GlobalTimeWrappedHourly * 16;
            Color innerColor = DescendingTwins.GetTwinColor(Variant);
            shader.InnerColor = innerColor;
            shader.OuterColor = Color.Lerp(innerColor, Color.Black, 0.5f);
            TrailDrawer.Draw(spriteBatch, OldCenterPos, GetFlamingTrailColor, GetFlamingTrailWidth, shader);
        }
        public override void AI()
        {
            base.AI();
            if (DeathState == 1f)
            {
                _deathTimer++;
                return;
            }
      
       

            Vector2 nextPos = Parent.Center;
            float distanceToPos = Vector2.Distance(Projectile.Center, nextPos);
            if(distanceToPos > 300 || !Parent.active || Parent.ai[1] == (int)DescendingTwin.TwinAIState.Idle)
            {
                DeathState = 1f;
            }
            else
            {
                Projectile.Center = Parent.Center;
            }
            Projectile.timeLeft = 60;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawFlamingTrail(Main.spriteBatch, Main.screenPosition, Color.White);
            return false;
        }
    }
    public partial class DescendingTwin
    {
        private float _traveledDistance;
        private float _startRotation;
        private Vector2 _startVelocity;
        private void AI_SpeedyDashStart()
        {
            Timer++;
            Vector2 flameSwordOffset = GetFlameSwordStartOffset();
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
                _simpleDashNormal = -flameSwordOffset.SafeNormalize(Vector2.Zero);
                _startVelocity = NPC.velocity;
                _startRotation = NPC.rotation;
            }


            //So for the speedy dash, the twin is going to look at the player
            //Then look 30 degrees up diagonally, wind up into a dash
            //Then keep wrapping around the screen and coming back from the bottom left to the top right
            //While the other one comes from the top right to the bottom left

            if(Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            float startTime = 120f;
            float completionRatio = Timer / startTime;
            float ease = EasingFunction.InOutExpo(completionRatio);
            _telegraphLineRot = NPC.rotation;
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, ease);

            
            Vector2 positionToMoveTo = Target.Center + flameSwordOffset;


            Vector2 movementVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = Vector2.Lerp(_startVelocity, movementVelocity, EasingFunction.InExpo(completionRatio));

            //Calculate whether to turn clockwise or counter clockwise
            //We can do this based on the target normal
            float rotationDirection = _simpleDashNormal.X > 0 ? -1f : -1f;
            float maxRadiansToRotate = MathHelper.PiOver4 * rotationDirection;
            float radiansOffset = ease * maxRadiansToRotate;
            Vector2 newNormal = _simpleDashNormal.RotatedBy(radiansOffset);
            NPC.rotation = Utils.AngleLerp(_startRotation, newNormal.ToRotation(), ease);


            //Set the outline color, good telegraphing
            TargetOutlineColor = Color.Yellow;
            if(Timer >= startTime)
            {
                SwitchState(TwinAIState.SpeedyDashWindup);
            }
        }

        private void AI_SpeedyDashWindup()
        {
            Timer++;
            if(Timer == 1)
            {
                _simpleDashNormal = NPC.rotation.ToRotationVector2();
            }

            //Add a little bit of velocity and go to the red
            float windUpTime = 30f;
            float completionRatio = Timer / windUpTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            Vector2 speedyDashVelocity = Vector2.Lerp(-_simpleDashNormal * 5f, _simpleDashNormal * 25f, ease);
            NPC.velocity = speedyDashVelocity;

            _telegraphLineRot = NPC.rotation;
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, ease);
            ShakeModSystem.Shake = 4;
            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.SpeedyDashLoop);
            }
        }

        private void AI_SpeedyDashLoop()
        {
            Timer++;
            if(Timer == 1)
            {
                _traveledDistance = 0f;
                SoundStyle dashSound = AttackNumber % 2 == 0 ?
    AssetRegistry.Sounds.SteamPunking.DescendingDash1
    : AssetRegistry.Sounds.SteamPunking.DescendingDash2;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);

                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DescendingTwinFlameTrail>(), 0, 0, Main.myPlayer, ai0: GetVariant(), ai1: NPC.whoAmI);
                }
            }

            float dashInterp = EasingFunction.InOutSine(AttackNumber / 9f);
            float maxSpeed = MathHelper.Lerp(100, 150f, dashInterp);

            float speedUp = EasingFunction.InOutSine(AttackNumber / 18f);
            float lowerBound = MathHelper.Lerp(20f, 12, speedUp);
            if(Timer < lowerBound)
            {
                if(NPC.velocity.Length() > 2)
                    NPC.velocity *= MathHelper.Lerp(0.5f, 0.75f, dashInterp);
            }
            else if(NPC.velocity.Length() < maxSpeed)
            {
                NPC.velocity *= 1.1f;
            }

            if(Timer % 2 == 0)
            {
                SpawnFlameDust();
            }

            if (Timer % 4 == 0)
            {
                SpawnFlameDonut();
                SpawnSteamParticle();
            }

            if(Timer == 60)
            {
                //Play the zooming sound
                SoundStyle zoom = AssetRegistry.Sounds.SteamPunking.DescendingZoom;
                zoom.PitchVariance = 0.3f;
                SoundEngine.PlaySound(zoom, NPC.position);
            }

            float distanceFactor = _traveledDistance / 2000f;
            float distanceLerp = EasingFunction.QuadraticBump(distanceFactor);
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, distanceLerp);
            _afterImageAlpha = 0f;
            //Set the contact damage to be enabled, turn on the cool trails
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if(Timer >= 5)
            {
                float distanceMoved = Vector2.Distance(NPC.oldPosition, NPC.position);
                _traveledDistance += distanceMoved;
            }

            float targetDistance = 3000;
            if(_traveledDistance >= targetDistance)
            {
                if (MultiplayerHelper.IsHost)
                {
                    _traveledDistance = 0;
                    AttackNumber++;
                    Vector2 targetCenter = Target.Center;
                    float direction = NPC.velocity.X > 0 ? -1 : 1;
                    float range = 1200;
                    _teleportPosition = targetCenter + new Vector2(range * direction, -range * direction);
                    if(Variant == TwinVariant.Spazz)
                    {
                        _teleportPosition.X += Target.velocity.X * 80;
                    }
      
                    SwitchState(TwinAIState.SpeedyDashLoop);
                }
            }
            else if(AttackNumber >= 21)
            {
                SwitchState(TwinAIState.SpeedyDashEnd);
            }
        }

        private void AI_SpeedyDashEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if(Timer >= 15)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
