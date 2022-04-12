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
    public class MorrowShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("morrowshot");
            Main.projFrames[Projectile.type] = 1;
            //The recording mode
        }

        private int ProjectileSpawnedCount;
        private int ProjectileSpawnedMax;
        private Projectile ParentProjectile;
        private bool RunOnce = true;
        private bool MouseLeftBool = false;

        public override void SetDefaults()
        {
            Projectile.damage = 12;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.maxPenetrate = 3;
            


            Projectile.ownerHitCheck = true;

        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }



        public override bool PreAI()
        {
           
            int moredust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f);


          
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

        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float speedX = Projectile.velocity.X * Main.rand.NextFloat(.2f, .3f) + Main.rand.NextFloat(-4f, 4f);
            float speedY = Projectile.velocity.Y * Main.rand.Next(20, 35) * 0.01f + Main.rand.Next(-10, 11) * 0.2f;
            Projectile.NewProjectile(Projectile.GetProjectileSource_FromThis(), Projectile.position.X + speedX, Projectile.position.Y + speedY, speedX, speedY, ProjectileID.Spark, (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
            Projectile.Kill();


        }


        




        public override bool PreDraw(ref Color lightColor)
        
           
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Projectile.spriteDirection == -1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Asset<Texture2D> asset = TextureAssets.Projectile[Projectile.type];
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

            return true ;
        }

    }

    


}
