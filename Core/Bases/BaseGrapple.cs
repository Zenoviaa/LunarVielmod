using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.VerletIntegration;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public class GrapplePlayer : ModPlayer
    {
        public Vector2? ropePosition;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (ropePosition.HasValue)
            {
                Vector2 positionToRopeTo = ropePosition.Value;
                Vector2 targetVelocity = positionToRopeTo - Player.Center;
                Player.velocity = targetVelocity;
                ropePosition = null;
            }
        }
    }

    public abstract class GrappleLine : ModProjectile,
        IDrawPixelated
    {
        private enum AIState
        {
            Shoot,
            Hook
        }

        private Vector2 _startPosition;
        private Vector2 _hookPosition;
        private Vector2[] _grappleLinePoints;
        private Vector2[] GrappleLinePoints
        {
            get
            {
                if (_grappleLinePoints == null)
                {
                    _grappleLinePoints = new Vector2[VerletChain.points.Length];
                }

                VerletChain.FillArr(_grappleLinePoints);
                return _grappleLinePoints;
            }
        }


        private VerletChain VerletChain;
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
        
        }

        public override void AI()
        {
            base.AI();
            switch (State)
            {
                case AIState.Shoot:
                    AI_Shoot();
                    break;
                case AIState.Hook:
                    AI_Hook();
                    break;
            }

            VerletChain?.Update();
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            switch (State)
            {
                case AIState.Shoot:

                    SwitchState(AIState.Hook);
                    break;
            }


            return false;
        }

        private void AI_Shoot()
        {
            Timer++;
            if (Timer == 1)
            {
                _startPosition = Projectile.Center;
                SoundStyle hookSound = AssetRegistry.Sounds.Gun.GrappleShoot;
                hookSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hookSound, Projectile.Center);
            }

         
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.extraUpdates = 1;
        }

        public override bool ShouldUpdatePosition()
        {
            if (State == AIState.Hook)
                return false;
            return base.ShouldUpdatePosition();
        }

        private void AI_Hook()
        {
            Projectile.extraUpdates = 0;
            Timer++;
            if (Timer == 1)
            {
                float segmentLength = 16;
                VerletChain = new VerletChain(Owner.Center, Projectile.Center, segmentLength);
                SoundStyle hookSound = AssetRegistry.Sounds.Gun.GrappleCharge;
                hookSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(hookSound, Projectile.Center);
            }
            if (VerletChain == null)
                return;

          
            ref VerletPoint point = ref VerletChain.points[VerletChain.points.Length - 1];
            point.position = Projectile.Center;
            point.pinned = true;

            Vector2 ropePosition = VerletChain.points[0].position;
            GrapplePlayer grapplePlayer = Owner.GetModPlayer<GrapplePlayer>();
            grapplePlayer.ropePosition = ropePosition;
            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation();
            if (Owner.controlJump)
            {
                Projectile.Kill();
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.LightGray, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return 2;
        }
        public void DrawPixelated()
        {
            switch (State)
            {
                case AIState.Shoot:
                    DrawHookingTrail();
                    break;
                case AIState.Hook:
                    DrawGrappleLinePoints();
                    break;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHook(ref lightColor);
            return false;
        }
        private void DrawHook(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawCenter, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
        }
        private void DrawHookingTrail()
        {
            var shader = BasicLaserAlphaShader.Instance;
            shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;

            float segmentLength = 16;
            float numPoints = Vector2.Distance(_startPosition, Projectile.Center) / segmentLength;
            numPoints += 1;
      
            List<Vector2> hookTrail = new List<Vector2>();
            for (float n = 0; n < numPoints; n++)
            {
                float completionRatio = n / numPoints;
                Vector2 position = Vector2.Lerp(_startPosition, Projectile.Center, completionRatio);
                hookTrail.Add(position);
            }

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, hookTrail.ToArray(), ColorFunction, WidthFunction, shader);
        }

        private void DrawGrappleLinePoints()
        {
            if (VerletChain == null)
                return;

            var shader = BasicLaserAlphaShader.Instance;
            shader.LaserTexture = TrailRegistry.GlowTrailNoBlack;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;

            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, GrappleLinePoints, ColorFunction, WidthFunction, shader);
        }
    }
}
