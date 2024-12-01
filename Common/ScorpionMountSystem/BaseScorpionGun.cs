using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.ScorpionMountSystem
{
    internal abstract class BaseScorpionGun : ModProjectile
    {
        public enum AIState
        {
            Idle,
            Shoot
        }

        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private Player Owner => Main.player[Projectile.owner];
        private float WhiteFlash;
        private float RecoilRot;
        private float RecoilOff;
        private float UseTime;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.light = 0.38f;
            UseTime = 4f;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if (Owner.mount.Active && 
                Owner.mount._mountSpecificData is ScorpionSpecificData scorpionSpecificData && 
                scorpionSpecificData.scorpionItem.gunType == Type)
            {
                Projectile.timeLeft = 3;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            WhiteFlash *= 0.7f;
            RecoilRot *= 0.7f;
            RecoilOff *= 0.7f;
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Shoot:
                    AI_Shoot();
                    break;
            }
        }

        private void SwitchState(AIState state)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                State = state;
                Timer = 0;
                Projectile.netUpdate = true;
            }
        }

        private void AI_Idle()
        {
            ScorpionPlayer scorpionPlayer = Owner.GetModPlayer<ScorpionPlayer>();
            Projectile.Center = scorpionPlayer.gunMountPosition;
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.netUpdate = true;
            }
            bool mouseInput = Main.mouseLeft;
            if (Projectile.owner == Main.myPlayer
                && mouseInput
                && Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId))
            {
                SwitchState(AIState.Shoot);
            }
        }

        private void AI_Shoot()
        {
            Timer++;
            if (Timer == 1)
            {
                Shoot();
                WhiteFlash = 1f;

                float direction = Projectile.velocity.X < 0 ? -1f : 1f;
                RecoilRot = MathHelper.ToRadians(-9f * direction);
                RecoilOff = 12;
            }

            float useTime = UseTime / Owner.GetTotalAttackSpeed(DamageClass.Ranged);
            if (Timer >= UseTime)
            {
                SwitchState(AIState.Idle);
            }
        }

        protected virtual void Shoot()
        {

        }

        private void DrawGlow(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos -= Projectile.rotation.ToRotationVector2() * RecoilOff;

            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White;
            float drawRotation = Projectile.rotation + RecoilRot;
            float drawScale = 1f;
            SpriteEffects spriteEffects = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            spriteBatch.Restart(blendState: BlendState.Additive);
            for(float f = 0f; f < 1f; f += 0.1f)
            {
                float rot = f * MathHelper.ToRadians(360);
                rot += Main.GlobalTimeWrappedHourly * 5f;
                Vector2 offset = rot.ToRotationVector2() * VectorHelper.Osc(2f, 4f);
                Vector2 glowDrawPos = drawPos + offset;
                spriteBatch.Draw(texture, glowDrawPos, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            }
            spriteBatch.RestartDefaults();
        }
        
        private void DrawGunSprite(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos -= Projectile.rotation.ToRotationVector2() * RecoilOff;

            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation + RecoilRot;
            float drawScale = 1f;
            SpriteEffects spriteEffects = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
        }

        private void DrawGunFlash(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            drawPos -= Projectile.rotation.ToRotationVector2() * RecoilOff;

            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White;
            float drawRotation = Projectile.rotation + RecoilRot;
            float drawScale = 1f;
            SpriteEffects spriteEffects = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Restart(blendState: BlendState.Additive);
            for(int i = 0; i < 4; i++)
                spriteBatch.Draw(texture, drawPos, null, drawColor * WhiteFlash, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);
            spriteBatch.RestartDefaults();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            DrawGlow(ref lightColor);
            DrawGunSprite(ref lightColor);
            DrawGunFlash(ref lightColor);
            return false;
        }
    }
}
