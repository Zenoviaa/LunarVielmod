using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class AlcadBombHeldProj : ModProjectile
    {
        public override string Texture => "Stellamod/Items/Weapons/Mage/AlcadBomb";
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 7;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        private bool ShouldConsumeMana()
        {
            return Timer % 4 == 0;
        }

        private void UpdateDamageForManaSickness(Player player)
        {
            Projectile.damage = (int)player.GetDamage(DamageClass.Magic).ApplyTo(player.HeldItem.damage);
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 4 == 0)
            {
                if (!Owner.CheckMana(Owner.HeldItem.mana, true))
                {
                    Projectile.Kill();
                }
            }

            UpdateDamageForManaSickness(Owner);
            Projectile.velocity = -Vector2.UnitY;
            Projectile.Center = Owner.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 40;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            HandleOwner();
            SetHandPosition();
        }

        private void HandleOwner()
        {
            if (Main.myPlayer != Projectile.owner)
                return;

            if (Timer == 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Owner.Center, Projectile.velocity,
                    ModContent.ProjectileType<AlcadBombProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }


            // player.CheckMana returns true if the mana cost can be paid. Since the second argument is true, the mana is actually consumed.
            // If mana shouldn't consumed this frame, the || operator short-circuits its evaluation player.CheckMana never executes.

            bool manaIsAvailable = !ShouldConsumeMana() || Owner.CheckMana(Owner.HeldItem.mana, true, false);

            // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
            // player.channel indicates whether the player is still holding down the mouse button to use the item.
            bool stillInUse = Owner.channel && !Owner.noItems && !Owner.CCed;
            if (!stillInUse)
            {
                Projectile.Kill();
            }
        }
        private void SetHandPosition()
        {
            // Set composite arm allows you to set the rotation of the arm and stretch of the front and back arms independently
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand
            Owner.heldProj = Projectile.whoAmI; // set held projectile to this projectile
        }

        protected virtual void DrawTomeSprite(ref Color lightColor)
        {
            Texture2D closeYourTomeTyrant = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Calculate Drawing Vars
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //We can add cool oscillation here
            drawPos.Y += MathHelper.Lerp(-5, 5, VectorHelper.Osc(0f, 1f, speed: 3));


            Vector2 drawOrigin = Projectile.Frame().Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawScale = Projectile.scale;
            float drawRotation = Projectile.rotation;
            SpriteEffects drawEffects = SpriteEffects.None;
            float layerDepth = 0;
            float glowDistanceOffset = 4;
            float glowRotationSpeed = 0.05f;

            //Draw Glow Effects
            //Let's do some additive glow
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            for (float f = 0; f < 1; f += 0.2f)
            {
                float rotation = (f * MathHelper.TwoPi) + Timer * glowRotationSpeed;
                Vector2 velocityRot = rotation.ToRotationVector2();
                velocityRot *= glowDistanceOffset;

                Vector2 glowDrawPos = drawPos + velocityRot;
                spriteBatch.Draw(closeYourTomeTyrant, glowDrawPos, Projectile.Frame(), drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
            }
            spriteBatch.End();
            spriteBatch.Begin();


            //Actually draw it
            spriteBatch.Draw(closeYourTomeTyrant, drawPos, Projectile.Frame(), drawColor, drawRotation, drawOrigin, drawScale, drawEffects, layerDepth);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawTomeSprite(ref lightColor);
            return false;
        }
    }
}
