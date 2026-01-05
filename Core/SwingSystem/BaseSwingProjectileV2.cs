using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Players;
using Stellamod.Common.Shaders;
using Stellamod.Core.Bases;
using Stellamod.Core.Effects;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.SwingSystem
{
    public abstract class BaseSwingProjectileV2 : ScarletProjectile,
        ISwingProjectile
    {
        private bool _hasInitialized;
        private bool _canHurtThings;
        private bool _hasHitStop;
        private float _fade;
        private List<ISwing> _swings;

        public ITrailer Trailer { get; set; }
        public ref float Timer => ref Projectile.ai[0];
        public ref float SwingDirection => ref Projectile.ai[1];
        public int ComboIndex => (int)Projectile.ai[2];

        public float HitstopTimer;
        public int ComboCount => _swings.Count;

        public float Interpolant { get; private set; }
        public Vector2[] afterImageCache;
        public Vector2[] swingTrailCache;
        public Vector2[] bigSwingTrailCache;
        public float[] swingRotationCache;
        public float[] oldTime;
        public int hitStopTime;
        public bool useAfterImage;
        public Color glowColor;
        public float growScale;
        public float swordBeamLength;
        public float swingTime;
        public float bounceTimer;
        public float extraLength;
        public Color outlineColor;
        public Color glowAfterImageColor;
        public bool drawCentered;
        public bool isChildProjectile;
        public const int EXTRA_UPDATE_COUNT = 7;

        //Default to the item sprite of the texture, we can just predraw if we need to change it
        public override string Texture => TextureRegistry.EmptyTexture;
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 8;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 7200;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.DamageType = DamageClass.Melee;


            //We're using extra updates to ensure the sword doesn't just pass through things
            Projectile.extraUpdates = EXTRA_UPDATE_COUNT - 1;
            hitStopTime = EXTRA_UPDATE_COUNT * 2;
            SetDefaults2();
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(bounceTimer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            bounceTimer = reader.ReadSingle();
        }

        public virtual Asset<Texture2D> RequestHologramTexture()
        {
            return TextureRegistry.GlowSword_Sword;
        }

        public virtual void SetDefaults2()
        {

        }

        public virtual void DefineCombo()
        {

        }

        public bool IsFinishingSwing()
        {
            //If we haven't initialized then yeah, though that won't happen lol
            if (!_hasInitialized)
                return false;
            return ComboIndex == _swings.Count - 1;
        }

        public virtual Color GetAfterImageColor(float interpolant)
        {
            return Color.Lerp(Color.White, Color.Transparent, MathHelper.SmoothStep(0, 1, interpolant));
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (ComboIndex >= _swings.Count)
            {
                SwingPlayerV2 swingPlayer = Owner.GetModPlayer<SwingPlayerV2>();
                swingPlayer.ResetCombo();
            }
        }
        private void AI_Initialize()
        {
            if (!_hasInitialized)
            {
                _swings = new List<ISwing>();
                swingTrailCache = new Vector2[200];
                bigSwingTrailCache = new Vector2[200];
                afterImageCache = new Vector2[16];
                swingRotationCache = new float[16];
                oldTime = new float[200];
                DefineCombo();
                ISwing swing = GetSwing();
                swing.SetDirection((int)SwingDirection);
                float hitCount = swing.GetHitCount();
                if (hitCount > 1)
                {
                    float duration = swing.GetDuration(1f / Owner.GetTotalAttackSpeed(Projectile.DamageType)) / hitCount;
                    duration *= EXTRA_UPDATE_COUNT - 1;

                    Projectile.localNPCHitCooldown = (int)duration;
                }
                Projectile.ResetLocalNPCHitImmunity();
                _hasInitialized = true;
            }
        }

        private ISwing GetSwing()
        {
            if (_swings.Count > ComboIndex)
            {
                return _swings[ComboIndex];
            }
            return _swings[0];
        }

        public float GetSwingTime(float baseSwingTime)
        {
            float swingTime = baseSwingTime * EXTRA_UPDATE_COUNT;
            return (int)(swingTime);
        }

        public override bool? CanDamage()
        {
            //Only damage in the mid part of the swing
            return _canHurtThings;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
  
            //Check if the sword is colliding, this does a line check instead of terraria default box.
            Texture2D texture = GetTexture();
            float length = texture.Width / 2 + texture.Height / 2;
            length *= 1.6f;
            length += swordBeamLength / 2;
            length += extraLength;

            float rotation = Projectile.rotation;

            //Oopsie
            //Ourh itboxes were 45 degrees off!!
            rotation += MathHelper.PiOver4;
            Vector2 start = Projectile.Center - rotation.ToRotationVector2() * length;
            Vector2 end = Projectile.Center + rotation.ToRotationVector2() * length;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 12, ref collisionPoint);
        }

        public override void AI()
        {
            base.AI();
            //We want to initalize like this for better MP compatibility, using a timer might not always be seen on all clients
            AI_Initialize();
            if(bounceTimer > 0)
            {
                Timer--;
                bounceTimer--;
            }
            else if (HitstopTimer <= 0)
                Timer++;
            else
                HitstopTimer--;
            ISwing swing = GetSwing();

            //Now we need to calculate the time/interpolant for this swinging
            if(swingTime == 0)
            {
                float duration = swing.GetDuration(1f / Owner.GetTotalAttackSpeed(Projectile.DamageType));
                swingTime = GetSwingTime(duration);
            }

            Interpolant = Timer / swingTime;
            Interpolant = MathHelper.Clamp(Interpolant, 0f, 1f);
            for (int i = oldTime.Length - 1; i > 0; i--)
            {
                oldTime[i] = oldTime[i - 1];
            }
            oldTime[0] = Interpolant;
            if(_fade < 1f)
            {
                _fade += 0.1f;
            }
            _canHurtThings = swing.CanHurt(this);

            //For the purposes of netcode,
            //Killing the projectile manually instead of trying to sync time left is better I think.
            if (Timer >= swingTime)
            {
                Projectile.Kill();
            }

            //We now have the offset so we can apply that to the weapon
            drawCentered = false;
            extraLength = 0;
            swing.UpdateSwing(this);

            //Set the position of the hand for the swing
            AI_OrientHand();

            //Calculate the trailing
            swing.CalculateTrailingPoints(this);
            swing.CalculateAfterImagePoints(this);
            Matrix translationMatrix = Matrix.CreateTranslation(new Vector3(Owner.Center.X, Owner.Center.Y, 0));
            //Now we transform the points
            //Calculating points locally and then translating it is a bit simpler.
            for (int t = 0; t < swingTrailCache.Length; t++)
            {
                swingTrailCache[t] = Vector2.Transform(swingTrailCache[t], translationMatrix);
            }
            for (int t = 0; t < bigSwingTrailCache.Length; t++)
            {
                bigSwingTrailCache[t] = Vector2.Transform(bigSwingTrailCache[t], translationMatrix);
            }
        }

        private void AI_OrientHand()
        {

            float rotation = Projectile.rotation;
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Owner.direction;
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }

            Owner.itemRotation = rotation * Owner.direction;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;

            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(135));// set arm position (90 degree offset since arm starts lowered)
        }

        public virtual void PrepareTrailShader()
        {

        }
        private void DrawPixelatedSwingTrails(GraphicsDevice graphicsDevice)
        {

            PrepareTrailShader();
            Color lightColor = Color.White;
            RenderSwingTrail(ref lightColor, swingTrailCache);
            DrawSwingTrail(ref lightColor, swingTrailCache);
            DrawSwingTrail2(ref lightColor, bigSwingTrailCache);
        }
        public virtual void RenderSwingTrail(ref Color lightColor, Vector2[] points)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Timer <= 3)
                return false;
            //Draw the texture, by 
            if (useAfterImage)
                DrawAfterImage(ref lightColor, OldCenterPos);

            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedSwingTrails, DrawLayer.OverNPCs);
            DrawSwordBeam(ref lightColor);
            DrawSwordSprite(ref lightColor);
            return false;
        }

        public void CloneProjectile()
        {
            if (isChildProjectile)
                return;

            if (Main.myPlayer == Projectile.owner)
            {
                ComboPlayer comboPlayer = Owner.GetModPlayer<ComboPlayer>();
                int combo = (int)(ComboIndex + 1);
                int dir = comboPlayer.ComboDirection;
                var p =Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.position, Projectile.velocity,
                    Type, Projectile.damage, Projectile.knockBack,
                               Owner.whoAmI, ai2: combo, ai1: dir);
                BaseSwingProjectileV2 swingProj = p.ModProjectile as BaseSwingProjectileV2;
                swingProj.isChildProjectile = true;
            }
        }

        public void Bounce(float bounceTicks)
        {
            Projectile.ResetLocalNPCHitImmunity();
            bounceTimer += bounceTicks * EXTRA_UPDATE_COUNT;
            Projectile.netUpdate = true;
        }

        public Vector2 CalculateTrailOffset()
        {
            return Vector2.Zero;
        }
        public float GetTrailMultiplier()
        {
            Texture2D texture = GetTexture();
            Vector2 center = texture.Size() / 2f;
            Vector2 tip = new Vector2(texture.Width, 0);
            float distance = Vector2.Distance(center, tip);
            float worldDistance = distance / 16f / 2f;
            return worldDistance;
        }
        public float GetTrailCenterMultiplier()
        {
            Texture2D texture = GetTexture();
            Vector2 center = texture.Size() / 2f;
            Vector2 tip = new Vector2(texture.Width, 0);
            float distance = Vector2.Distance(center, tip);
            float worldDistance = distance / 2f;
            return worldDistance;
        }
        public virtual Texture2D GetTexture()
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture);
            return texture;
        }

        public virtual void DrawAfterImage(ref Color lightColor, Vector2[] afterImageCache)
        {
            if (afterImageCache == null)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for (int a = 0; a < afterImageCache.Length; a++)
            {
                float interpolant = a;
                interpolant /= (float)afterImageCache.Length;
                Texture2D texture = GetTexture();
                int frameHeight = texture.Height / Main.projFrames[Projectile.type];
                int startY = frameHeight * Projectile.frame;

                Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
                Vector2 origin = sourceRectangle.Size() / 2f;
                Color drawColor = GetAfterImageColor(interpolant);
                drawColor *= EasingFunction.QuadraticBump(interpolant);
                float drawScale = 1.15f + growScale;
                Vector2 position = afterImageCache[a];
                float drawRotation = (position - Owner.Center).ToRotation() + MathHelper.PiOver4;

                spriteBatch.Draw(texture,
                  position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    sourceRectangle, drawColor, drawRotation, origin, drawScale, SpriteEffects.None, 0); // drawing the sword itself
            }
            spriteBatch.RestartDefaults();
        }

        public virtual void DrawSwingTrail(ref Color lightColor, Vector2[] swingTrailCache)
        {
            if (Trailer == null)
                return;

            //I think it makes the most sense to abstract our trails out to a trailer and shader cache,
            //so we can just replace the trailer for different trails!
            //So much simpler, and we can just make new trailers
            var oldColorFunc = Trailer.TrailColorFunction;
            Color GetTrailColor(float interpolant)
            {
                return oldColorFunc(interpolant) * EasingFunction.QuadraticBump(Interpolant) * _fade;
            }
            Trailer.TrailColorFunction = GetTrailColor;
            Trailer?.DrawTrail(ref lightColor, swingTrailCache);
            Trailer.TrailColorFunction = oldColorFunc;
        }
        public virtual void DrawSwingTrail2(ref Color lightColor, Vector2[] swingTrailCache)
        {
            if (Trailer == null)
                return;


            if (swordBeamLength <= 0)
                return;
            //I think it makes the most sense to abstract our trails out to a trailer and shader cache,
            //so we can just replace the trailer for different trails!
            //So much simpler, and we can just make new trailers
            var oldWidthFunc = Trailer.TrailWidthFunction;
            var oldColorFunc = Trailer.TrailColorFunction;
            float GetTrailWidth(float interpolant)
            {
                return oldWidthFunc(interpolant) * 2;
            }
            Color GetTrailColor(float interpolant)
            {
                return oldColorFunc(interpolant) * 0.35f * EasingFunction.QuadraticBump(Interpolant) * _fade;
            }

            Trailer.TrailWidthFunction = GetTrailWidth;
            Trailer.TrailColorFunction = GetTrailColor;
            Trailer?.DrawTrail(ref lightColor, swingTrailCache);



            Trailer.TrailWidthFunction = oldWidthFunc;
            Trailer.TrailColorFunction = oldColorFunc;
        }

        public virtual void DrawSwordSprite(ref Color lightColor)
        {
            Texture2D texture = GetTexture();
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);

            SpriteBatch spriteBatch = Main.spriteBatch;
            float drawScale = 1 + growScale;


            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            if (outlineColor.A > 0)
            {
                Color drawOutlineColor = outlineColor.MultiplyRGB(lightColor);
                SpriteWhiteShader whiteShader = SpriteWhiteShader.Instance;
                spriteBatch.Restart(effect: whiteShader.Effect);
                spriteBatch.Draw(texture,
                    drawPosition + Vector2.UnitX * 2,
                    sourceRectangle, drawOutlineColor, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture,
                    drawPosition + Vector2.UnitX * -2,
                    sourceRectangle, drawOutlineColor, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0);

                spriteBatch.Draw(texture,
                    drawPosition + Vector2.UnitY * 2,
                    sourceRectangle, drawOutlineColor, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0);
                spriteBatch.Draw(texture,
                    drawPosition + Vector2.UnitY * -2,
                    sourceRectangle, drawOutlineColor, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0);
                spriteBatch.RestartDefaults();
            }

            spriteBatch.Draw(texture, drawPosition,
                sourceRectangle, drawColor, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0);

            if (glowColor.A > 0)
            {
                spriteBatch.Restart(blendState: BlendState.Additive);
                spriteBatch.Draw(texture,
                      Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                      sourceRectangle, glowColor, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0);

                spriteBatch.Draw(texture,
                  Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                  sourceRectangle, glowColor, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0);

                spriteBatch.RestartDefaults();
            }
        
        }

        public virtual void DrawSwordBeam(ref Color lightColor)
        {
            if (swordBeamLength <= 0)
                return;

            SwordBeamShader swordBeamShader = SwordBeamShader.Instance;
            swordBeamShader.InnerColor = outlineColor;
            swordBeamShader.OuterColor = glowAfterImageColor;

            Texture2D texture = RequestHologramTexture().Value;
            Vector2 offset = (Projectile.rotation + MathHelper.ToRadians(-45)).ToRotationVector2() * swordBeamLength / 2;
            Vector2 origin = texture.Size() / 2f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            float rotationOffset = MathHelper.ToRadians(45);
            drawPos += offset;
  
            Color drawColor = Color.White.MultiplyRGB(lightColor) * 0.2f;

            SpriteBatch spriteBatch = Main.spriteBatch;
            float drawScale = 1.15f + growScale;
            spriteBatch.Restart(blendState: BlendState.AlphaBlend, effect: swordBeamShader.Effect);



            for (int a = 0; a < afterImageCache.Length; a++)
            {
                float interpolant = a;
                interpolant /= (float)afterImageCache.Length;
                interpolant = 1f - interpolant;
                Color drawColor2 = glowAfterImageColor;
                drawColor2 *= EasingFunction.InOutSine(interpolant);
                Vector2 position = afterImageCache[a];
                float drawRotation = swingRotationCache[a];

                Vector2 offset2 = (drawRotation + MathHelper.ToRadians(-45)).ToRotationVector2() * swordBeamLength / 2;
                position += offset2;
                spriteBatch.Draw(texture,
                  position - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                    null, drawColor2, drawRotation + rotationOffset, origin, drawScale, SpriteEffects.None, 0); // drawing the sword itself
            }




            spriteBatch.Draw(texture,
               drawPos,
                  null, drawColor, Projectile.rotation + rotationOffset, origin, drawScale, SpriteEffects.None, 0);

            spriteBatch.RestartDefaults();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            //In here we'd spawn the hit effects
            //Hit stop effect and minor screenshake I think
            if (!_hasHitStop)
            {

                HitstopTimer = hitStopTime;
                _hasHitStop = true;
            }
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, speedXa * 0, speedYa * 0, ModContent.ProjectileType<BaseHitEffect>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);


        }

        public void Add(ISwing swing)
        {
            _swings.Add(swing);
        }
    }
}
