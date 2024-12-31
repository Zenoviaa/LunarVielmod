using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.NPCs.Bosses.DreadMire;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Stellamod.NPCs.Bosses.Fenix.Projectiles;
using Stellamod.Projectiles.Crossbows;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Spears
{
    public class VeiizalsUmbrellaProjOpen : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private ref float SwordRotation => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;//number of frames the animation has
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
            Projectile.timeLeft = 95;
        }

        public override bool? CanDamage()
        {
            return false;
        }

        public override void AI()
        {
            Timer++;
            if (Timer > 155)
            {
                // Our timer has finished, do something here:
                // Main.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.		
                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/MorrowSalfi"));
                Timer = 0;
            }

            Player player = Main.player[Projectile.owner];
            if (player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Vector2 playerCenter = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                player.ChangeDir(Projectile.direction);
                SwordRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.netUpdate = true;
                if (!Main.mouseRight)
                    Projectile.Kill();
            }

            Projectile.velocity = SwordRotation.ToRotationVector2();
            Projectile.spriteDirection = player.direction;
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation();
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;


            if (Timer == 1)
            {
                float speedX = Projectile.velocity.X * 10;
                float speedY = Projectile.velocity.Y * 7;

                SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/CrossbowPull"));
            }

            if (Timer == 80)
            {


                //Funny Screenshake
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);
                float speedX = Projectile.velocity.X * 10;
                float speedY = Projectile.velocity.Y * 7;
                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, new Vector2(0,0), ModContent.ProjectileType<DreadSpawnEffect>(), Projectile.damage * 1, Projectile.knockBack, player.whoAmI);
                Projectile.NewProjectile(player.GetSource_ItemUse_WithPotentialAmmo(player.HeldItem, AmmoID.Arrow), Projectile.Center, Projectile.velocity * 32f, ModContent.ProjectileType<VeiizalsUmbrellaWaveProj>(), Projectile.damage * 2, Projectile.knockBack, player.whoAmI);
                SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, player.position);
                float recoilStrength = 7;
                Vector2 targetVelocity = -Projectile.velocity.SafeNormalize(Vector2.Zero) * recoilStrength;
                player.velocity = VectorHelper.VelocityUpTo(player.velocity, targetVelocity);
            }



            Projectile.Center = playerCenter + Projectile.velocity * 1f;// customization of the hitbox position

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

        private void UpdatePlayerVisuals(Player player, Vector2 playerhandpos)
        {
            Projectile.Center = playerhandpos;
            Projectile.spriteDirection = Projectile.direction;

            // Constantly resetting player.itemTime and player.itemAnimation prevents the player from switching items or doing anything else.
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 3;
            player.itemAnimation = 3;
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
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