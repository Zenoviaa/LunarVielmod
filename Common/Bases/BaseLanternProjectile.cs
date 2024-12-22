using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Common.Lights;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal abstract class BaseLanternProjectile : ModProjectile,
           IDrawLightCast
    {
        private enum AIState
        {
            Pet,
            Flashlight
        }

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


            //Draw Glow Effects
            //Let's do some additive glow
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            for (float f = 0; f < 1; f += 0.2f)
            {
                float rotation = (f * MathHelper.TwoPi) + Timer * GlowRotationSpeed;
                Vector2 velocityRot = rotation.ToRotationVector2();
                velocityRot *= GlowDistanceOffset;

                Vector2 glowDrawPos = drawPos + velocityRot;
                spriteBatch.Draw(closeYourTomeTyrant, glowDrawPos, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
            }
            spriteBatch.End();
            spriteBatch.Begin();


            //Actually draw it
            spriteBatch.Draw(closeYourTomeTyrant, drawPos, null, drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLanternSprite(ref lightColor);
            return false;
        }

        public void DrawLightCast(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Beam").Value;
            float degree = FlashlightDegrees * PetLightModifier;
            float range = MathHelper.ToRadians(degree);
            float steps = degree;

            for (float i = 0; i < steps; i++)
            {
                float progress = i / steps;
                float rot = MathHelper.Lerp(-range / 2f, range / 2f, progress);
                Vector2 vel = LightVelocity.RotatedBy(rot);
                float distance = ProjectileHelper.PerformBeamHitscan(Projectile.Center, vel, maxBeamLength: FlashlightLength * PetLightModifier);
                float fallOff = 0.0015f;
                for (float j = 0; j < distance; j += texture.Size().X * 3)
                {
                    Vector2 drawPos = Projectile.Center + vel.SafeNormalize(Vector2.Zero) * j;

                    Lighting.AddLight(drawPos, Color.White.ToVector3() * 0.6f);
                    drawPos -= Main.screenPosition;
                    Color drawColor = Color.White;

                    drawColor *= MathHelper.Lerp(0.5f, 0f, fallOff * j);
                    spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, texture.Size() / 2, 16, SpriteEffects.None, 0f);
                }
            }
        }
    }
}