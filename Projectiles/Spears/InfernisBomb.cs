using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Spears
{

    internal class InfernisBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.hide = true;
            Projectile.penetrate = 5;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.timeLeft = 660;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 3.2f;
            Projectile.velocity.X = 0;
            Projectile.ai[1]++;
            Projectile.spriteDirection = Projectile.direction;
        }

        public override void OnKill(int timeLeft)
        {

            int side = Math.Sign(Projectile.velocity.Y);
            int bitSide = (side != -1) ? 1 : 0;
            int offsetY = (int)(Projectile.ai[1] / 60f * Projectile.height) * 3;
            if (offsetY > Projectile.height)
            {
                offsetY = Projectile.height;
            }

            Vector2 position9 = Projectile.position + ((side == -1) ? new Vector2(0f, Projectile.height - offsetY) : Vector2.Zero);
            Vector2 vector154 = Projectile.position + ((side == -1) ? new Vector2(0f, Projectile.height) : Vector2.Zero);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 50f);
            var EntitySource = Projectile.GetSource_FromThis();
            Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ProjectileID.DD2ExplosiveTrapT2Explosion, Projectile.damage * 2, 1, Projectile.owner, 0, 0);
            for (int i = 0; i < 60; i++)
            {
                if (Main.rand.Next(3) < 2)
                {
                    Dust dust81 = Main.dust[Dust.NewDust(position9, Projectile.width, offsetY, DustID.Torch, 0f, 0f, 90, default, 2.5f)];
                    dust81.noGravity = true;
                    dust81.fadeIn = 1f;
                    if (dust81.velocity.Y > 0f)
                        dust81.velocity.Y *= -1f;

                    if (Main.rand.NextBool(2))
                    {
                        dust81.position.Y = MathHelper.Lerp(dust81.position.Y, vector154.Y, 0.5f);
                        dust81.velocity *= 5f;
                        dust81.velocity.Y -= 3f;
                        dust81.position.X = Projectile.Center.X;
                        dust81.noGravity = false;
                        dust81.noLight = true;
                        dust81.fadeIn = 0.4f;
                        dust81.scale *= 0.3f;
                    }
                    else
                        dust81.velocity = Projectile.DirectionFrom(dust81.position) * dust81.velocity.Length() * 0.25f;

                    dust81.velocity.Y *= (0f - side);
                    dust81.customData = bitSide;
                }
            }
        }

        float alphaCounter = 0;
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

            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Spiin").Value;
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);

            SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(254, 231, 97), new Color(247, 118, 34), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
        }
    }
}


