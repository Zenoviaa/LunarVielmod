using CrystalMoon.Systems.MiscellaneousMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public abstract class BaseChainedBallProjectile : ModProjectile
    {
        private Vector2[] _slashPos;
        private Vector2[] _chainPos;
        private bool _playedSwingSound;
        private bool _playedBounceSound;
        private float _swingXRadius;
        private enum AIState
        {
            Dragging,
            Sling
        }

        private AIState State
        {
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }

        private ref float Timer => ref Projectile.ai[1];
        private ref float Dir => ref Projectile.ai[2];
        private Player Owner => Main.player[Projectile.owner];

        protected float DragDistance { get; set; }
        protected float SwingRange { get; set; }
        protected float OvalRotOffset { get; set; }
        protected float SwingXRadius { get; set; }
        protected float SwingYRadius { get; set; }
        protected Vector2 StartPosition { get; set; }
        protected Func<float, float> EasingFunction { get; set; }
        protected float UnEasedLerpValue { get; set; }
        protected float SmoothedLerpValue { get; set; }
        protected float BaseSwingTime { get; set; }
        protected float TrailStartOffset { get; set; }
        protected float GlowDistanceOffset { get; set; }
        protected float GlowRotationSpeed { get; set; }

        protected float TipDamageMultiplier { get; set; }

        protected float ExtraUpdateMult => 8;
        protected Texture2D ChainTexture
        {
            get
            {
                return ModContent.Request<Texture2D>(Texture + "_Chain").Value;
            }
        }

        public SoundStyle? SwingSound { get; set; }

        public SoundStyle? BounceSound { get; set; }
        public float SwingSoundProgress { get; set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _slashPos = new Vector2[ProjectileID.Sets.TrailCacheLength[Type]];
            //Setup Defaults
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = int.MaxValue;
            Projectile.timeLeft = int.MaxValue;
            Projectile.extraUpdates = (int)ExtraUpdateMult - 1;
            Projectile.friendly = true;


            //Other Variables
            EasingFunction = (float lerpValue) => Easing.InOutExpo(lerpValue, 7);
            DragDistance = 126;
            SwingRange = MathHelper.ToRadians(360);
            OvalRotOffset = MathHelper.ToRadians(-90);
            SwingXRadius = 512;
            SwingYRadius = 80;
            TrailStartOffset = 0.15f;
            UnEasedLerpValue = 0f;
            SmoothedLerpValue = 0f;
            BaseSwingTime = 48;
            GlowDistanceOffset = 0;
            GlowRotationSpeed = 0.005f;
            TipDamageMultiplier = 2;


            SoundStyle soundStyle = SoundRegistry.BallSwing;
            soundStyle.PitchVariance = 0.15f;
            SwingSound = soundStyle;

            SoundStyle bounceStyle = SoundID.DD2_WitherBeastCrystalImpact;
            bounceStyle.PitchVariance = 0.15f;
            BounceSound = bounceStyle;
        }

        public override void AI()
        {
            base.AI();
            //Dunno why I didn't just do this with uhhh Gun Holsters lol
            //This is such an easy way to instakill this thing
            if (Owner.HeldItem.shoot != Type)
            {
                Projectile.Kill();
            }

            switch (State)
            {
                case AIState.Dragging:
                    AI_Dragging();
                    break;
                case AIState.Sling:
                    AI_Sling();
                    break;
            }
        }

        private float GetSwingTime()
        {
            float distProgress = _swingXRadius / SwingXRadius;

            float swingTime = BaseSwingTime * ExtraUpdateMult * MathHelper.Lerp(0.5f, 1f, distProgress);
            return (int)(swingTime / Owner.GetAttackSpeed(Projectile.DamageType));
        }

        private void SwitchState(AIState state)
        {
            State = state;
            Timer = 0;
            Projectile.netUpdate = true;
        }

        private void AI_Dragging()
        {
            Timer++;
            //Reset Lerp Values
            //Reset Slash Pos
            _slashPos[0] = Projectile.Center;
            for (int i = 1; i < _slashPos.Length; i++)
            {
                _slashPos[i] = _slashPos[i - 1];
            }


            UnEasedLerpValue = 0f;
            SmoothedLerpValue = 0f;

            Projectile.velocity.Y += 0.01f;
            Vector2 posToCheckFrom = Projectile.Center;
            Projectile.tileCollide = Collision.CanHitLine(posToCheckFrom, 1, 1, Owner.position, 1, 1);
            float distanceToOwner = Vector2.Distance(Projectile.Center, Owner.Center);
            if (distanceToOwner > DragDistance)
            {
                //Get yanked to the player
                Vector2 targetWorld = Owner.Center;
                Vector2 diffToPosition = targetWorld - Projectile.Center;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, diffToPosition, 0.0002f);

                //This should create very smooth movement of the tome
                float targetRotation = diffToPosition.ToRotation();
                float velocityRotationOffset = Projectile.velocity.Length() * 0.004f;
                Projectile.rotation += velocityRotationOffset;
            }
            else
            {
                //Slow down when you're close enough
                Projectile.velocity.X *= 0.99f;

            }
            Projectile.rotation += Projectile.velocity.X * 0.02f;
            if (Main.myPlayer == Projectile.owner && Main.mouseLeft)
            {
                SwitchState(AIState.Sling);
            }
        }

        protected virtual void SetSlingDefaults()
        {

        }

        private void AI_Sling()
        {
            Timer++;
            if (Timer == 1)
            {
                _playedSwingSound = false;
                _playedBounceSound = false;
                _swingXRadius = MathHelper.Min(Vector2.Distance(Main.MouseWorld, Owner.Center), SwingXRadius);
                SetSlingDefaults();
                if (Projectile.owner == Main.myPlayer && Timer == 1)
                {
                    Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                }
                Projectile.ResetLocalNPCHitImmunity();
            }

            float lerpValue = Timer / GetSwingTime();
            float swingProgress = lerpValue;

            Projectile.tileCollide = false;
            //Reusing the Oval Slinging Code for this
            Vector2 targetVelocity = Vector2.Zero;

            float targetRotation = Projectile.velocity.ToRotation();

            UnEasedLerpValue = lerpValue;
            swingProgress = EasingFunction(swingProgress);
            SmoothedLerpValue = swingProgress;

            //Swinging Sound
            if (!_playedSwingSound && swingProgress >= SwingSoundProgress && SwingSound != null)
            {
                SoundEngine.PlaySound(SwingSound, Projectile.position);
                _playedSwingSound = true;
            }


            int dir2 = (int)Dir;
            float xOffset;
            float yOffset;
            if (dir2 == -1)
            {
                xOffset = _swingXRadius * MathF.Sin(swingProgress * SwingRange + SwingRange + OvalRotOffset);
                yOffset = SwingYRadius * MathF.Cos(swingProgress * SwingRange + SwingRange + OvalRotOffset);
            }
            else
            {
                xOffset = _swingXRadius * MathF.Sin((1f - swingProgress) * SwingRange + SwingRange + OvalRotOffset);
                yOffset = SwingYRadius * MathF.Cos((1f - swingProgress) * SwingRange + SwingRange + OvalRotOffset);
            }

            Projectile.Center = Owner.Center + new Vector2(xOffset, yOffset).RotatedBy(targetRotation);

            Vector2[] points = new Vector2[ProjectileID.Sets.TrailCacheLength[Projectile.type]];
            for (int i = 0; i < points.Length; i++)
            {
                float l = points.Length;
                //Lerp between the points
                float progressOnTrail = i / l;

                //Calculate starting lerp value
                float startTrailLerpValue = MathHelper.Clamp(lerpValue - TrailStartOffset, 0, 1);
                float startTrailProgress = startTrailLerpValue;
                startTrailProgress = EasingFunction(startTrailLerpValue);


                //Calculate ending lerp value
                float endTrailLerpValue = lerpValue;
                float endTrailProgress = endTrailLerpValue;
                endTrailProgress = EasingFunction(endTrailLerpValue);

                //Lerp in between points
                float smoothedTrailProgress = MathHelper.Lerp(startTrailProgress, endTrailProgress, progressOnTrail);
                float xOffset2;
                float yOffset2;
                if (dir2 == -1)
                {
                    xOffset2 = _swingXRadius * MathF.Sin(smoothedTrailProgress * SwingRange + SwingRange + OvalRotOffset);
                    yOffset2 = SwingYRadius * MathF.Cos(smoothedTrailProgress * SwingRange + SwingRange + OvalRotOffset);
                }
                else
                {
                    xOffset2 = _swingXRadius * MathF.Sin((1f - smoothedTrailProgress) * SwingRange + SwingRange + OvalRotOffset);
                    yOffset2 = SwingYRadius * MathF.Cos((1f - smoothedTrailProgress) * SwingRange + SwingRange + OvalRotOffset);
                }


                Vector2 pos = Owner.Center + new Vector2(xOffset2, yOffset2).RotatedBy(targetRotation);
                points[i] = pos;// + GetTrailOffset().RotatedBy(targetRotation);
            }
            _slashPos = points;

            if (UnEasedLerpValue >= 1f)
            {
                Dir = -Dir;
                SwitchState(AIState.Dragging);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            if (Vector2.Distance(Projectile.Center, target.Center) < Projectile.Size.Length() * 2f)
            {
                //Double damage modifier
                modifiers.FinalDamage *= TipDamageMultiplier;
            }
            else
            {
                //Weak ahh hit
                modifiers.FinalDamage *= 0.5f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            //Bounce code
            Projectile.velocity.Y = -Projectile.velocity.Y / 2f;
            if (!_playedBounceSound && BounceSound != null)
            {
                SoundEngine.PlaySound(BounceSound, Projectile.position);
                _playedBounceSound = true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Trail, Glow, Chain, Sprite, ye
            DrawSlashTrail(ref lightColor, _slashPos);
            DrawChain(ref lightColor);
            DrawBallSprite(ref lightColor);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            DrawSlashGlow();
        }

        private void DrawSlashGlow()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            Texture2D closeYourBalls = ModContent.Request<Texture2D>(Texture).Value;

            //Calculate Drawing Vars
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //We can add cool oscillation here
            drawPos.Y += MathHelper.Lerp(-5, 5, MathUtil.Osc(0f, 1f, speed: 3));


            Vector2 drawOrigin = closeYourBalls.Size() / 2f;
            Color drawColor = Color.Lerp(Color.Transparent, Color.White, Easing.SpikeOutCirc(UnEasedLerpValue));
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            SpriteEffects drawEffects = Projectile.Center.X < Owner.Center.X ? SpriteEffects.None : SpriteEffects.None;
            float layerDepth = 0;

            //Actually draw it
            spriteBatch.Draw(closeYourBalls, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
        }

        protected virtual void DrawSlashTrail(ref Color lightColor, Vector2[] slashPos)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = SimpleTrailShader.Instance;

            //Main trailing texture
            shader.TrailingTexture = TrailRegistry.GlowTrail;

            //Blends with the main texture
            shader.SecondaryTrailingTexture = TrailRegistry.GlowTrail;

            //Used for blending the trail colors
            //Set it to any noise texture
            shader.TertiaryTrailingTexture = TrailRegistry.CrystalTrail;
            shader.PrimaryColor = Color.White;
            shader.SecondaryColor = Color.LightGray;
            shader.BlendState = BlendState.Additive;
            shader.Speed = 25;
            TrailDrawer.Draw(spriteBatch, slashPos, Projectile.oldRot, DefaultColorFunction, DefaultWidthFunction, shader,
                offset: Projectile.Size / 2f);
        }

        private float DefaultWidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(0, 64, completionRatio) * Easing.SpikeOutCirc(UnEasedLerpValue);
        }

        private Color DefaultColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.LightBlue, p);
            return trailColor;
        }

        protected virtual void DrawBallSprite(ref Color lightColor)
        {
            Texture2D closeYourBalls = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Calculate Drawing Vars
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //We can add cool oscillation here
            drawPos.Y += MathHelper.Lerp(-5, 5, MathUtil.Osc(0f, 1f, speed: 3));


            Vector2 drawOrigin = closeYourBalls.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            SpriteEffects drawEffects = Projectile.Center.X < Owner.Center.X ? SpriteEffects.None : SpriteEffects.None;
            float layerDepth = 0;

            //Actually draw it
            spriteBatch.Draw(closeYourBalls, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
        }

        protected virtual void DrawChain(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2[] chainPositions = CalculateChainPositions();
            for (int i = 1; i < chainPositions.Length; i++)
            {
                Vector2 position = chainPositions[i];

                float rotation = (chainPositions[i] - chainPositions[i - 1]).ToRotation() - MathHelper.PiOver2; //Calculate rotation based on direction from last point
                float yScale = Vector2.Distance(chainPositions[i], chainPositions[i - 1]) / ChainTexture.Height; //Calculate how much to squash/stretch for smooth chain based on distance between points

                Vector2 scale = new Vector2(1, yScale); // Stretch/Squash chain segment
                Color chainLightColor = Lighting.GetColor((int)position.X / 16, (int)position.Y / 16); //Lighting of the position of the chain segment
                Vector2 origin = new Vector2(ChainTexture.Width / 2, ChainTexture.Height); //Draw from center bottom of texture
                spriteBatch.Draw(ChainTexture, position - Main.screenPosition, null, chainLightColor, rotation, origin, scale, SpriteEffects.None, 0);
            }
        }

        private Vector2[] CalculateChainPositions()
        {
            Vector2 ownerPos = Owner.Center;
            List<Vector2> controlPoints = new List<Vector2>();
            controlPoints.Add(Owner.Center);

            Vector2 controlPoint1 = Vector2.Lerp(Owner.Center, Projectile.Center, 0.25f);
            controlPoint1.Y += MathHelper.Lerp(64, 0, Easing.SpikeInOutBounce(UnEasedLerpValue));
            controlPoints.Add(controlPoint1);
            controlPoints.Add(Projectile.Center);

            int numPoints = (int)(Vector2.Distance(Projectile.Center, Owner.Center) / ChainTexture.Height);
            Vector2[] chainPositions = GetBezierApproximation(controlPoints.ToArray(), numPoints);
            return chainPositions;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, 0, 0,
                ModContent.ProjectileType<BaseHitEffect>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2[] chainPositions = CalculateChainPositions();
            float collisionPoint = 0;
            for (int i = 1; i < chainPositions.Length; i++)
            {
                Vector2 position = chainPositions[i];
                Vector2 previousPosition = chainPositions[i - 1];
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), position, previousPosition, 6, ref collisionPoint))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }


        //Found some simple bezier curve stuff :DDDD
        //YAAAAAAAAAAAAAY
        private Vector2[] GetBezierApproximation(Vector2[] controlPoints, int outputSegmentCount)
        {
            Vector2[] points = new Vector2[outputSegmentCount + 1];
            for (int i = 0; i <= outputSegmentCount; i++)
            {
                float t = (float)i / outputSegmentCount;
                points[i] = GetBezierPoint(t, controlPoints, 0, controlPoints.Length);
            }
            return points;
        }

        private Vector2 GetBezierPoint(float t, Vector2[] controlPoints, int index, int count)
        {
            if (count == 1)
                return controlPoints[index];
            var P0 = GetBezierPoint(t, controlPoints, index, count - 1);
            var P1 = GetBezierPoint(t, controlPoints, index + 1, count - 1);
            return new Vector2((1 - t) * P0.X + t * P1.X, (1 - t) * P0.Y + t * P1.Y);
        }
    }
}
