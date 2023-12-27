using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles
{

    internal class CarianKnightProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.timeLeft = 240;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        private ref float ai_Counter => ref Projectile.ai[0];

        public override void AI()
        {
            base.AI();
            ai_Counter++;
            Player playerToHomeTo = Main.player[Main.myPlayer];
            float closestDistance = Vector2.Distance(Projectile.position, playerToHomeTo.position);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                float distanceToPlayer = Vector2.Distance(Projectile.position, player.position);
                if (distanceToPlayer < closestDistance)
                {
                    closestDistance = distanceToPlayer;
                    playerToHomeTo = player;
                }
            }

            if(ai_Counter < 70)
            {
                float speed = 8;
                Vector2 velocity;
                Vector2 direction = Projectile.DirectionTo(playerToHomeTo.Center);
                Vector2 maxVelocity = direction * (speed * ai_Counter / 60);
                float distanceToTarget = Vector2.Distance(playerToHomeTo.Center, Projectile.Center);
                if(distanceToTarget < speed)
                {
                    velocity = direction * distanceToTarget;
                }
                else
                {
                    velocity = maxVelocity;
                }

      
                Projectile.velocity = velocity;
            }

            Projectile.rotation++;
            Projectile.spriteDirection = Projectile.direction;
            if (Main.rand.NextBool(6))
            {
                Dust d = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemAmethyst, Scale: 1.5f)];
                d.noGravity = true;
                d.fadeIn = 1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 50f);
            for (int i = 0; i < 32; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 1.5f);
                d.noGravity = true;
            }
        }

        Vector2 DrawOffset;
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.spriteDirection != 1)
            {
                DrawOffset.X = Projectile.Center.X - 18;
                DrawOffset.Y = Projectile.Center.Y;
            }
            else
            {
                DrawOffset.X = Projectile.Center.X - 25;
                DrawOffset.Y = Projectile.Center.Y;
            }


            Texture2D texture = Request<Texture2D>("Stellamod/Effects/Masks/Spiin").Value;
            float r = 234;
            float g = 118;
            float b = 135;

            for(int i = 0; i < 2; i++)
            {
                Main.spriteBatch.Draw(texture, DrawOffset - Main.screenPosition, null, new Color((int)r, (int)g, (int)b, 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            }
         
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3() * 1.75f * Main.essScale);
        }
    }
}


