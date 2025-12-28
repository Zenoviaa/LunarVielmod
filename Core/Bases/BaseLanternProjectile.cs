using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Core.LunarLightingSystem;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    /// <summary>
    /// Base class for a lantern type projectile, which emits light and acts as a great pet!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseLanternProjectile<T> : ModProjectile, ILightEmitter,
        IDrawOutlines where T : ModBuff
    {
        private enum AIState
        {
            Pet,
            Flashlight
        }
        private Vector2 _lightVelocity;
        private ILight _light;
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

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_lightVelocity);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _lightVelocity = reader.ReadVector2();
        }
        
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projPet[Type] = true;
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
            if (!Owner.active)
            {
                Projectile.active = false;
                return;
            }

            // Keep the projectile disappearing as long as the player isn't dead and has the pet buff.
            if (!Owner.dead && Owner.HasBuff(ModContent.BuffType<T>()))
            {
                Projectile.timeLeft = 2;
            }

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
                _lightVelocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.netUpdate = true;
            }
        }

        protected abstract ILight GetLight();
        private void AI_Pet()
        {

            if (Owner.HeldItem.shoot == Type)
            {
                State = AIState.Flashlight;
            }

            Vector2 targetPos = Owner.Center + new Vector2(Owner.direction * 12, -32);
            Vector2 velocity = targetPos - Projectile.Center;
            Projectile.velocity = velocity * 0.2f;
            Projectile.rotation = Projectile.velocity.X / 60f;

            _light = GetLight();
            _light.RayCast(Projectile.Center, _lightVelocity, 400, 400);
        }

        private void AI_Flashlight()
        {
            if (Owner.HeldItem.shoot != Type)
            {
                State = AIState.Pet;
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

            _light = GetLight();
            _light.RayCast(Projectile.Center, _lightVelocity, 760, 800);
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
            _light?.Draw();
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