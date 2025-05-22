using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
	public class BincleProj : ModProjectile
	{
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 24;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            drawColor.A = 0;
            float drawRotation = Projectile.rotation;
            float drawScale = MathHelper.Lerp(0f, 1f, Easing.InOutCubic(Timer / 15f));
            spriteBatch.Draw(texture, drawPos, frame, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        private bool ShouldConsumeMana()
        {
            // Should mana be consumed this frame?
            bool consume = Timer % 6 == 0;
            return consume;
        }

        public override void AI()
        {
            base.AI();
            Timer++;


            if (Main.myPlayer == Projectile.owner)
            {
                bool manaIsAvailable = !ShouldConsumeMana() || Owner.CheckMana(Owner.HeldItem.mana, true, false);
                Projectile.velocity = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.Zero);
                // The Prism immediately stops functioning if the player is Cursed (player.noItems) or "Crowd Controlled", e.g. the Frozen debuff.
                // player.channel indicates whether the player is still holding down the mouse button to use the item.
                bool stillInUse = Owner.channel && manaIsAvailable && !Owner.noItems && !Owner.CCed;
                if (stillInUse && Timer % 6 == 0)
                {
                    Vector2 spawnPos = Projectile.Center - Projectile.velocity * 65;
                    Vector2 shootVelocity = Projectile.velocity * 12;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, shootVelocity,
                        ModContent.ProjectileType<BrincShot>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner);
                }
                else if (!stillInUse)
                {
                    Projectile.Kill();
                }

          
                Projectile.netUpdate = true;
            }

            DrawHelper.AnimateTopToBottom(Projectile, 2);
            Owner.ChangeDir(Projectile.direction);
            Projectile.spriteDirection = Owner.direction;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Owner.Center + Projectile.velocity * 120;

            if (Timer == 1)
            {
                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightPink,
                    outerGlowColor: Color.DarkViolet, duration: 15, baseSize: 0.12f);
            }

            SetHoldPosition();
        }

        private void SetHoldPosition()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Owner.direction = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
            }

            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f)); // set arm position (90 degree offset since arm starts lowered)
            Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2); // get position of hand

            armPosition.Y += Owner.gfxOffY;
            Owner.heldProj = Projectile.whoAmI;
        }
    }
}



