﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    internal class SwordOfGlactiaProj : ModProjectile
    {
        bool Moved;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 5;
            Projectile.width = 25;
            Projectile.height = 25;
            Projectile.timeLeft = 700;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (Projectile.alpha >= 50)
            {
                Projectile.alpha -= 10;
            }

            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Moved = true;
            }
            if (Projectile.ai[1] == 1)
            {
                int Sound = Main.rand.Next(1, 4);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia1"), Projectile.position);
                }
                if (Sound == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia2"), Projectile.position);
                }
                if (Sound == 3)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia3"), Projectile.position);
                }
                Projectile.alpha = 255;
                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.position.X = Main.rand.NextFloat(Projectile.position.X - 50, Projectile.position.X + 50);
                    Projectile.position.Y = Main.rand.NextFloat(Projectile.position.Y - 50, Projectile.position.Y + 50);
                    Projectile.netUpdate = true;
                }
               
            }
            if (Projectile.ai[1] == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 0 && Projectile.ai[1] <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Projectile.ai[1] == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Projectile.ai[1] >= 21 && Projectile.ai[1] <= 60)
            {
                Projectile.velocity /= .90f;

            }
            if (Projectile.ai[1] == 60)
            {
                Projectile.tileCollide = true;
            }




        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, -2f, 0, default, .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, -2f, 0, default, .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(1, 244, 255), new Color(67, 37, 172), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.RoyalBlue.ToVector3() * 1.75f * Main.essScale);

        }
    }
}