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

namespace Stellamod.Projectiles
{
    public class MorrowShotArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("morrowshotarrow");
            Main.projFrames[Projectile.type] = 1;
            //The recording mode
        }

       

        public override void SetDefaults()
        {
            Projectile.damage = 12;
            Projectile.width = 12;
            Projectile.height = 24;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.maxPenetrate = 2;
            Projectile.CloneDefaults(ProjectileID.WoodenArrowFriendly);

            Projectile.ownerHitCheck = true;

        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }



        public override bool PreAI()
        {
            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FireworkFountain_Green, 0f, 0f);
            int moredust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.AncientLight, 0f, 0f);


            Main.dust[dust].scale = 0.5f;
            Main.dust[moredust].scale = 0.5f;
            return true;
        }

        public override void AI()
        {
            Timer++;
            if (Timer > 3)
            {
                // Our timer has finished, do something here:
                // Main.PlaySound, Dust.NewDust, Projectile.NewProjectile, etc. Up to you.

            }



            float distance = 0f;
            Player player = Main.player[Projectile.owner];
            Vector2 playerDistanceCursor = player.Center - Main.MouseWorld;
            if (Timer <= 1)
            {
                Projectile.position = player.position + Vector2.UnitY.RotatedBy(MathHelper.ToRadians(Projectile.ai[1])) * distance;
            }

            if (Timer < 2)
            {
                Projectile.velocity = 20 * Vector2.Normalize(Projectile.DirectionTo(Main.MouseWorld));
            }


            Projectile.direction = Projectile.spriteDirection = Projectile.velocity.X > 0f ? 1 : -1;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            // Since our sprite has an orientation, we need to adjust rotation to compensate for the draw flipping.
            if (Projectile.spriteDirection == -1)
            {
                Projectile.rotation += MathHelper.Pi;

            }



            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;






        }



        public override bool PreDraw(ref Color lightColor)

        {
            Asset<Texture2D> asset = TextureAssets.Projectile[Projectile.type];
            Texture2D tex = asset.Value;
            //Redraw the Projectile with the color not influenced by light
            int height = Main.player[Projectile.owner].height / 1; // 5 is frame count
            int y = height * Projectile.frame;
            var rect = new Rectangle(0, y, Main.player[Projectile.owner].width, height);
            var drawOrigin = new Vector2(Main.player[Projectile.owner].width / 2, Projectile.height / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw((Texture2D)TextureAssets.Projectile[Projectile.type], drawPos, rect, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.Vortex, 0, 0, 100, Color.Blue, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 2f;
                dust = Dust.NewDustDirect(Projectile.position - Projectile.velocity, Projectile.width, Projectile.height, DustID.Vortex, 0f, 0f, 1000, Color.Blue, 3.5f);
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Projectile.Kill();
        }






    }




}
