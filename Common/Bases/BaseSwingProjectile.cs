using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    public abstract class BaseSwingProjectile : ModProjectile
    {
        private bool _init;
        private bool _hashit;
        private bool _hasMissed;

        //This is for smoothin the trail
        public static int ExtraUpdateMult => 6;
        public Vector2[] _trailPoints = new Vector2[0];
        private List<BaseSwingStyle> _swingStyles = new();


        public float _smoothedLerpValue;
        public float uneasedLerpValue;
        public float hitstopTimer = 0;
        public float bounceTimer = 0;
        public Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.ai[0];
        protected int ComboDirection => (int)Projectile.ai[1];
        protected int ComboIndex => (int)Projectile.ai[2];

        public float holdOffset = 60f;
        public float trailStartOffset = 0.15f;
        public float missTimeIncrease = 12;
        public float extraSwingTime = 0;
        public float hitboxLengthMult = 1;

        public bool thrust;
        public float OvalRotOffset;
        public bool spinCenter;
        public float spinCenterOffset;

        public Vector2 TrueProjectileCenter
        {
            get
            {
                Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
                int frameHeight = texture.Height / Main.projFrames[Projectile.type];
                int startY = frameHeight * Projectile.frame;

                Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
                Vector2 pos = Projectile.Center;
                Vector2 origin = sourceRectangle.Size() / 2f;
                return pos - origin + Projectile.rotation.ToRotationVector2() * holdOffset / 2f;
            }
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 90;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            _trailPoints = new Vector2[90];
            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.height = 64;
            Projectile.width = 64;
            Projectile.friendly = true;
            Projectile.extraUpdates = ExtraUpdateMult - 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10000;
        }

        public virtual void SetSwingStyle(ref BaseSwingStyle swingStyle, int comboIndex)
        {

        }

        public float GetSwingTime(float baseSwingTime)
        {
            float swingTime = baseSwingTime * ExtraUpdateMult;
            return (int)(swingTime / Owner.GetAttackSpeed(Projectile.DamageType));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawSlashTrail(_trailPoints, -Main.screenPosition + GetFramingSize() / 2f);
            DrawSwordSprite(ref lightColor);
            return false;
        }


        protected virtual void DrawSwordSprite(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;

            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawScale = MathHelper.Lerp(1f, 1.2f, Easing.SpikeOutCirc(uneasedLerpValue));

            Main.spriteBatch.Draw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0); // drawing the sword itself
        }
        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage()
        {
            return _smoothedLerpValue > 0.1f && _smoothedLerpValue < 0.9f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            float length = texture.Width / 2 + texture.Height / 2;

            Vector2 start = Projectile.Center - Projectile.rotation.ToRotationVector2() * length;
            Vector2 end = Projectile.Center + Projectile.rotation.ToRotationVector2() * length;
            float collisionPoint = 0f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.scale, ref collisionPoint);
        }
        public virtual void SetComboDefaults(List<BaseSwingStyle> swings)
        {

        }

        public override void AI()
        {
            base.AI();

            if (!_init)
            {
                SetComboDefaults(_swingStyles);
                var sw = _swingStyles[(int)Projectile.ai[0]];
                Projectile.timeLeft = (int)GetSwingTime(sw.swingTime);

                InitSwingAI();
                _init = true;
            }

            BaseSwingStyle swingStyle = _swingStyles[(int)ComboIndex];
            swingStyle.SwingProjectile = this;
            if (hitstopTimer > 0)
            {
                Timer--;
                Projectile.timeLeft++;
                hitstopTimer--;
            }
            else if (bounceTimer > 0)
            {
                Timer -= 2;

                bounceTimer--;
                if (bounceTimer <= 0)
                {
                    Projectile.ResetLocalNPCHitImmunity();
                }
                extraSwingTime++;
                Projectile.timeLeft++;
            }

            if (!_hashit && !_hasMissed && _smoothedLerpValue > 0.9f)
            {
                Projectile.timeLeft += (int)(missTimeIncrease * ExtraUpdateMult);
                _hasMissed = true;
            }

            Timer++;

            swingStyle.AI();
            OrientHand();
        }


        protected virtual void InitSwingAI()
        {

        }

        public virtual Vector2 GetFramingSize()
        {
            return ModContent.Request<Texture2D>(Texture).Size();
        }

        public virtual Vector2 GetTrailOffset()
        {
            return Vector2.One * 30;
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

        protected virtual void DrawSlashTrail(Vector2[] trailPoints, Vector2 drawOffset)
        {

        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);

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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y, speedXa * 0, speedYa * 0, ModContent.ProjectileType<BaseHitEffect>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);
            
            _hashit = true;
        }
    }
}
