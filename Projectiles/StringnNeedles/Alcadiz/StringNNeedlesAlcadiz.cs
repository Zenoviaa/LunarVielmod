using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Projectiles;
using Stellamod.UI.systems;
using Terraria.Audio;

namespace Stellamod.Projectiles.StringnNeedles.Alcadiz
{
    public class StringNNeedlesAlcadiz : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("deadass");
            Main.projFrames[Projectile.type] = 1;
        }
        private int ProjectileSpawnedCount;
        private int ProjectileSpawnedMax;
        private Projectile ParentProjectile;
        private bool RunOnce = true;
        private bool MouseLeftBool = false;
        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.aiStyle = 595;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;

            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {



            {
                Timer++;
                if (Timer > 170)
                {
                    // Our timer has finished, do something here:
                    // Main.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.
                    ShakeModSystem.Shake = 8;

                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/MorrowSalfi"));
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
                    Projectile.rotation = Projectile.velocity.ToRotation();
                else
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;





                if (Timer == 40)
                {
                    float speedX = Projectile.velocity.X * 0;
                    float speedY = Projectile.velocity.Y * 0;

                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ModContent.ProjectileType<Windeffect>(), (int)(Projectile.damage * 4), 0f, Projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ModContent.ProjectileType<CharmSpragald>(), (int)(Projectile.damage * 0), 0f, Projectile.owner, 0f, 0f);
                    SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Assets/Sounds/SpidrSummon"));
                    
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

           



            if (Timer >= 170)
            {
                float speedX = Projectile.velocity.X * 5;
                float speedY = Projectile.velocity.Y * 2;
              
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY * 0.5f, ModContent.ProjectileType<Spragald>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
        
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type];
            int startY = frameHeight * Projectile.frame;
            Rectangle sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
            Vector2 origin = sourceRectangle.Size() / 2f;
            origin.X = (float)(Projectile.spriteDirection == 1 ? sourceRectangle.Width - 30 : 30); // Customization of the sprite position

            Color drawColor = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);

            return false;
        }



    }
}