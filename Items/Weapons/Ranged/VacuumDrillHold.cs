using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using Stellamod.Projectiles.Crossbows.Lasers;
using Stellamod.Projectiles.Gun;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class VacuumDrillHold : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;//number of frames the animation has
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = 595;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.timeLeft = int.MaxValue;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position + Projectile.velocity.SafeNormalize(Vector2.Zero) * 25, Projectile.velocity,
                ModContent.ProjectileType<VacuumLightningBolt>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
        }

        public override void AI()
        {
            Timer++;
            if (Timer > 15)
            {
                SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap, Projectile.position);
                Timer = 0;
            }


            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            float swordRotation = 0f;
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                swordRotation = (Main.MouseWorld - player.Center).ToRotation();
                if (!player.channel)
                    Projectile.Kill();
            }

            Projectile.velocity = swordRotation.ToRotationVector2();
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
            {
                float targetRotation = Projectile.velocity.ToRotation();
                Projectile.rotation = targetRotation;
            }
            else
            {
                float targetRotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
                Projectile.rotation = targetRotation;
            }

            if (Timer > 1)
            {

                ShakeModSystem.Shake = 1;
            }


            Projectile.Center = playerCenter + new Vector2(40, 0).RotatedBy(swordRotation); ;// customization of the hitbox position

            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);

            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 1)
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Player player = Main.player[Projectile.owner];
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - 30 : 30; // Customization of the sprite position

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
            return false;
        }
    }
}
