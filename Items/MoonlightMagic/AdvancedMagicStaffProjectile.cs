using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic
{
    internal class AdvancedMagicStaffProjectile : ModProjectile
    {
        private bool _hasShot;
        private float _smoothedLerpValue;
        private Vector2[] _trailPoints = new Vector2[0];
        protected int SwingTime;

        public override string Texture => TextureRegistry.EmptyTexture;

        private Asset<Texture2D> _texture;

        private ref float Countertimer => ref Projectile.ai[0];
        private ref float _swingDirection => ref Projectile.ai[1];
        private ref float _swingTime => ref Projectile.ai[2];
        private Player Owner => Main.player[Projectile.owner];
        public float trailStartOffset = 0.15f;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 32;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.timeLeft = SwingTime;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 64;
            Projectile.width = 64;
            Projectile.friendly = true;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private void FetchTexture()
        {
            if (_texture == null)
            {
                _texture = ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture);
            }
        }

        private Vector2 GetFramingSize()
        {
            return _texture.Size();
        }

        public override void AI()
        {
            base.AI();
            if (Countertimer == 0)
            {
                SwingTime = (int)(_swingTime / Owner.GetAttackSpeed(Projectile.DamageType));
                Projectile.timeLeft = SwingTime;
            }

            FetchTexture();
            AI_MoveInOval();
        }

        protected virtual void ModifyOvalSwingAI(float targetRotation, float lerpValue,
            ref float swingXRadius,
            ref float swingYRadius,
            ref float swingRange,
            ref float swingProgress)
        {
            swingXRadius = 86 / 1.5f;
            swingYRadius = 28 / 1.5f;
            swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4;
            swingProgress = Easing.InOutExpo(lerpValue, 10);
        }

        public void Shoot()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Item heldItem = Owner.HeldItem;
                BaseStaff staff = heldItem.ModItem as BaseStaff;
                AdvancedMagicUtil.NewMagicProjectile(staff, Projectile);
            }
        }

        private void AI_MoveInOval()
        {
            float swingXRadius = 32;
            float swingYRadius = 128;
            float swingRange = MathHelper.Pi + MathHelper.PiOver2 + MathHelper.PiOver4;

            Countertimer++;
            float lerpValue = Countertimer / SwingTime;
            float swingProgress = lerpValue;
            float targetRotation = Projectile.velocity.ToRotation();

            _smoothedLerpValue = swingProgress;
            ModifyOvalSwingAI(targetRotation, lerpValue, ref swingXRadius, ref swingYRadius, ref swingRange, ref swingProgress);
            if (swingProgress >= 0.5f && !_hasShot)
            {
                Shoot();
                _hasShot = true;
            }
            int dir2 = (int)_swingDirection;
            float xOffset;
            float yOffset;
            if (dir2 == -1)
            {
                xOffset = swingXRadius * MathF.Sin(swingProgress * swingRange + swingRange);
                yOffset = swingYRadius * MathF.Cos(swingProgress * swingRange + swingRange);
            }
            else
            {
                xOffset = swingXRadius * MathF.Sin((1f - swingProgress) * swingRange + swingRange);
                yOffset = swingYRadius * MathF.Cos((1f - swingProgress) * swingRange + swingRange);
            }

            Projectile.Center = Owner.Center + new Vector2(xOffset, yOffset).RotatedBy(targetRotation);
            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;
            OrientHand();

            Vector2[] points = new Vector2[ProjectileID.Sets.TrailCacheLength[Type]];
            for (int i = 0; i < points.Length; i++)
            {
                float l = points.Length;
                //Lerp between the points
                float progressOnTrail = i / l;

                //Calculate starting lerp value
                float startTrailLerpValue = MathHelper.Clamp(lerpValue - trailStartOffset, 0, 1);
                float startTrailProgress = startTrailLerpValue;
                ModifyOvalSwingAI(targetRotation, startTrailLerpValue,
                    ref swingXRadius, ref swingYRadius, ref swingRange, ref startTrailProgress);

                //Calculate ending lerp value
                float endTrailLerpValue = lerpValue;
                float endTrailProgress = endTrailLerpValue;
                ModifyOvalSwingAI(targetRotation, endTrailLerpValue,
                    ref swingXRadius, ref swingYRadius, ref swingRange, ref endTrailProgress);


                //Lerp in between points
                float smoothedTrailProgress = MathHelper.Lerp(startTrailProgress, endTrailProgress, progressOnTrail);
                float xOffset2;
                float yOffset2;
                if (dir2 == -1)
                {
                    xOffset2 = swingXRadius * MathF.Sin(smoothedTrailProgress * swingRange + swingRange);
                    yOffset2 = swingYRadius * MathF.Cos(smoothedTrailProgress * swingRange + swingRange);
                }
                else
                {
                    xOffset2 = swingXRadius * MathF.Sin((1f - smoothedTrailProgress) * swingRange + swingRange);
                    yOffset2 = swingYRadius * MathF.Cos((1f - smoothedTrailProgress) * swingRange + swingRange);
                }


                Vector2 pos = Owner.Center + new Vector2(xOffset2, yOffset2).RotatedBy(targetRotation);
                points[i] = pos - (GetFramingSize() / 2);// + GetTrailOffset().RotatedBy(targetRotation);
            }
            _trailPoints = points;
        }

        private void OrientHand()
        {
            float rotation = Projectile.rotation;
            Owner.heldProj = Projectile.whoAmI;
            Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
            Owner.itemRotation = rotation * Owner.direction;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            DrawSlashTrail();
            if (_texture == null)
                return false;

            Texture2D texture = _texture.Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);


            Main.spriteBatch.Draw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); // drawing the sword itself

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            if (_texture == null)
                return;

            Texture2D texture = _texture.Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Color.White * Easing.SpikeOutCirc(1f - _smoothedLerpValue);

            spriteBatch.Draw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0); // drawing the sword itself
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }

        private float WidthFunction(float p)
        {
            float trailWidth = MathHelper.Lerp(0, 54, p);
            float fadeWidth = MathHelper.Lerp(trailWidth, 0, _smoothedLerpValue) * Easing.OutExpo(_smoothedLerpValue, 4);
            return fadeWidth;
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.Black, p);
            Color fadeColor = Color.Lerp(trailColor, Color.Black, _smoothedLerpValue);
            //This will make it fade out near the end
            return fadeColor;
        }

        private void DrawSlashTrail()
        {
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

            SpriteBatch spriteBatch = Main.spriteBatch;
            TrailDrawer.Draw(spriteBatch,
            _trailPoints,
              Projectile.oldRot,
              ColorFunction,
              WidthFunction, shader, offset: GetFramingSize() / 2f);
        }
    }
}
