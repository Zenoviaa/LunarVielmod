using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Core.Lights;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public abstract class BaseLanternProjectile : ModProjectile, ILightEmitter,
        IDrawOutlines
    {
        private enum AIState
        {
            Pet,
            Flashlight
        }

        private ConeLight _coneLight;
        private ref float Timer => ref Projectile.ai[0];
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }

        private Player Owner => Main.player[Projectile.owner];
        public float HoldDistance { get; set; }
        public float GlowRotationSpeed { get; set; }
        public float GlowDistanceOffset { get; set; }

        public float FlashlightLength { get; set; }
        public float FlashlightDegrees { get; set; }
        public float PetLightModifier { get; set; }
        public Vector2 LightVelocity { get; set; }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.LightPet[Projectile.type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
            Projectile.netImportant = true;
            HoldDistance = 36;
            GlowDistanceOffset = 4;
            GlowRotationSpeed = 0.05f;
            FlashlightDegrees = 25;
            FlashlightLength = 512;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            switch (State)
            {
                case AIState.Pet:
                    PetLightModifier = 0.5f;
                    AI_Pet();
                    break;
                case AIState.Flashlight:
                    PetLightModifier = 1f;
                    AI_Flashlight();
                    break;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                LightVelocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero);
            }

        }

        private void AI_Pet()
        {
            if (!Owner.active)
            {
                Projectile.active = false;
                return;
            }

            // Keep the projectile disappearing as long as the player isn't dead and has the pet buff.
            if (!Owner.dead && Owner.HasBuff(ModContent.BuffType<RadiatingLantern>()))
            {
                Projectile.timeLeft = 2;
            }

            Vector2 targetPos = Owner.Center + new Vector2(Owner.direction * 12, -32);
            Vector2 velocity = targetPos - Projectile.Center;
            //    velocity = velocity.SafeNormalize(Vector2.Zero);
            Projectile.velocity = velocity * 0.2f;
            Projectile.rotation = Projectile.velocity.X / 60f;

            _coneLight ??= new ConeLight();
            _coneLight.lightColor = Color.White;
            _coneLight.RayCast(Projectile.Center, LightVelocity, 400, 400);
        }

        private void AI_Flashlight()
        {
            if (Owner.ownedProjectileCounts[Type] > 1 || Owner.HeldItem.shoot != Type)
            {
                Projectile.Kill();
            }
            if (Main.myPlayer == Projectile.owner)
            {
                //Calculate where the heck we need to go
                Vector2 mouseWorld = Main.MouseWorld;
                Vector2 directionToMouse = (mouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                Vector2 velocityToMouse = directionToMouse * HoldDistance;
                Vector2 targetPosition = Owner.Center + velocityToMouse;

                Vector2 diffToPosition = targetPosition - Projectile.Center;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, diffToPosition, 0.5f);

                //This should create very smooth movement of the tome
                float targetRotation = directionToMouse.ToRotation();
                float velocityRotationOffset = Projectile.velocity.Length() * 0.04f;
                Projectile.rotation = targetRotation + velocityRotationOffset;
                Projectile.netUpdate = true;
            }

            _coneLight ??= new ConeLight();
            _coneLight.lightColor = Color.White;
            _coneLight.RayCast(Projectile.Center, LightVelocity, 760, 800);
        }

        protected virtual void DrawLanternSprite(ref Color lightColor)
        {
            Texture2D closeYourTomeTyrant = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Calculate Drawing Vars
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //We can add cool oscillation here
            drawPos.Y += MathHelper.Lerp(-5, 5, VectorHelper.Osc(0f, 1f, speed: 3));


            Vector2 drawOrigin = closeYourTomeTyrant.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            SpriteEffects drawEffects = (Projectile.Center.X < Owner.Center.X && State == AIState.Flashlight) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float layerDepth = 0;

            //Actually draw it
            spriteBatch.Draw(closeYourTomeTyrant, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);

            Texture2D glowTexture = ModContent.Request<Texture2D>(Texture).Value;
            Color glowColor = Color.White;
            glowColor *= ExtraMath.Osc(0f, 1f);
            glowColor *= 0.5f;
            glowColor.A = 0;
            spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, null, glowColor, 0, glowTexture.Size() / 2f, 1, SpriteEffects.None, 0);

            glowTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            glowColor *= 0.5f;
            spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, null, glowColor, 0, glowTexture.Size() / 2f, 0.75f, SpriteEffects.None, 0);

        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLanternSprite(ref lightColor);
            return false;
        }

        public void RenderLight(SpriteBatch spriteBatch)
        {
            _coneLight?.Draw();
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            if (State != AIState.Flashlight)
                return;

            Texture2D closeYourTomeTyrant = ModContent.Request<Texture2D>(Texture).Value;

            //Calculate Drawing Vars
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //We can add cool oscillation here
            drawPos.Y += MathHelper.Lerp(-5, 5, VectorHelper.Osc(0f, 1f, speed: 3));


            Vector2 drawOrigin = closeYourTomeTyrant.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            SpriteEffects drawEffects = (Projectile.Center.X < Owner.Center.X && State == AIState.Flashlight) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            float layerDepth = 0;

            //Actually draw it
            spriteBatch.Draw(closeYourTomeTyrant, drawPos + Vector2.UnitX * 2, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
            spriteBatch.Draw(closeYourTomeTyrant, drawPos - Vector2.UnitX * 2, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
            spriteBatch.Draw(closeYourTomeTyrant, drawPos + Vector2.UnitY * 2, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
            spriteBatch.Draw(closeYourTomeTyrant, drawPos - Vector2.UnitY * 2, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
        }
    }
}