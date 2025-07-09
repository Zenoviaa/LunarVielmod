using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Effects;
using Stellamod.Core.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.SwingSystem
{
    public abstract class BaseSwingProjectile : ScarletProjectile
    {
        private bool _hasInitialized;
        private bool _canHurtThings;
        private bool _hasHitStop;
        private List<ISwing> _swings;

        public ITrailer Trailer { get; set; }
        public ref float Timer => ref Projectile.ai[0];
        public ref float SwingDirection => ref Projectile.ai[1];
        public int ComboIndex => (int)Projectile.ai[2];

        public float HitstopTimer;

        public float Interpolant { get; private set; }
        public Vector2[] swingTrailCache;
        public int hitStopTime;

        public const int EXTRA_UPDATE_COUNT = 7;

        //Default to the item sprite of the texture, we can just predraw if we need to change it
        public override string Texture => AssetHelper.EmptyTexture;
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.timeLeft = 7200;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
       

            //We're using extra updates to ensure the sword doesn't just pass through things
            Projectile.extraUpdates = EXTRA_UPDATE_COUNT - 1;
            hitStopTime = EXTRA_UPDATE_COUNT * 2;
            SetDefaults2();
        }

        public virtual void SetDefaults2()
        {

        }

        public virtual void DefineCombo(List<ISwing> swings)
        {

        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            if (ComboIndex >= _swings.Count)
            {
                SwingPlayer swingPlayer = Owner.GetModPlayer<SwingPlayer>();
                swingPlayer.ResetCombo();
            }
        }
        private void AI_Initialize()
        {
            if (!_hasInitialized)
            {
                _swings = new List<ISwing>();
                swingTrailCache = new Vector2[32];
                DefineCombo(_swings);
                ISwing swing = GetSwing();
                swing.SetDirection((int)SwingDirection);
                float hitCount = swing.GetHitCount();
                if(hitCount > 1)
                {
                    float duration = swing.GetDuration() / hitCount;
                    duration *= EXTRA_UPDATE_COUNT - 1;
                    Projectile.localNPCHitCooldown = (int)duration;
                }
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
            return (int)(swingTime / Owner.GetAttackSpeed(Projectile.DamageType));
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

            Vector2 start = Projectile.Center - Projectile.rotation.ToRotationVector2() * length;
            Vector2 end = Projectile.Center + Projectile.rotation.ToRotationVector2() * length;
            float collisionPoint = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.scale, ref collisionPoint);
        }

        public override void AI()
        {
            base.AI();
            //We want to initalize like this for better MP compatibility, using a timer might not always be seen on all clients
            AI_Initialize();
            if (HitstopTimer <= 0)
                Timer++;
            else
                HitstopTimer--;
            ISwing swing = GetSwing();

            //Now we need to calculate the time/interpolant for this swinging
            float duration = swing.GetDuration();
            float swingTime = GetSwingTime(duration);
            Interpolant = Timer / swingTime;
            Interpolant = MathHelper.Clamp(Interpolant, 0f, 1f);

            _canHurtThings = Interpolant > 0.1f && Interpolant <= 0.9f;

            //For the purposes of netcode,
            //Killing the projectile manually instead of trying to sync time left is better I think.
            if (Timer >= swingTime)
            {
                Projectile.Kill();
            }

            //We now have the offset so we can apply that to the weapon
            swing.UpdateSwing(Interpolant, Projectile.Center, Projectile.velocity, out Vector2 offset);
            Projectile.Center = Owner.Center + offset;
            Projectile.rotation = (Projectile.Center - Owner.Center).ToRotation() + MathHelper.PiOver4;

            //Set the position of the hand for the swing
            AI_OrientHand();

            //Calculate the trailing
            swing.CalculateTrailingPoints(Interpolant, Projectile.velocity, ref swingTrailCache);

            //Now we transform the points
            //Calculating points locally and then translating it is a bit simpler.
            for(int t = 0; t < swingTrailCache.Length; t++)
            {
                Matrix translationMatrix = Matrix.CreateTranslation(new 
                    Vector3(Owner.Center.X, Owner.Center.Y, 0));
                swingTrailCache[t] = Vector2.Transform(swingTrailCache[t], translationMatrix);
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
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));// set arm position (90 degree offset since arm starts lowered)
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the texture, by 
            DrawSwingTrail(ref lightColor, swingTrailCache);
            DrawSwordSprite(ref lightColor);
            return false;
        }

        public virtual Texture2D GetTexture()
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Owner.HeldItem.ModItem.Texture);
            return texture;
        }

        public virtual void DrawSwingTrail(ref  Color lightColor, Vector2[] swingTrailCache)
        {
            //I think it makes the most sense to abstract our trails out to a trailer and shader cache,
            //so we can just replace the trailer for different trails!
            //So much simpler, and we can just make new trailers
           
            Trailer?.DrawTrail(ref lightColor, swingTrailCache);
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
            float drawScale = 1f;
            spriteBatch.Draw(texture,
                Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY),
                sourceRectangle, drawColor, Projectile.rotation, origin, drawScale, SpriteEffects.None, 0); // drawing the sword itself
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

        }
    }
}
