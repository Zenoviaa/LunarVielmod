using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Thrown
{
    public class SkullSaw : ModProjectile
	{
		public override void SetStaticDefaults()
        {
            Projectile.scale = 0.7f;
            // DisplayName.SetDefault("Cactius2");
            Projectile.penetrate = 5;
            Projectile.scale = 0.9f;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.timeLeft = 500;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.scale += 0.1f;
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 7; i++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.OasisCactus);
            }
            return false;
        }

        public override void AI()
        {

            Projectile.ai[1]++;
            Projectile.spriteDirection = Projectile.direction;
            if (Projectile.ai[1] == 1)
            {
                float A = Main.rand.Next(0, 2);
                if (A == 0)
                {
                    SoundEngine.PlaySound(SoundID.Zombie83, Projectile.position);
                    SoundEngine.PlaySound(SoundID.Item104, Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.Zombie82, Projectile.position);
                    SoundEngine.PlaySound(SoundID.Item103, Projectile.position);
                }
            }
            Projectile.rotation += 0.55f;
        }

        Vector2 DrawOffset;
        float alphaCounter = 7;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;





            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/Spiin").Value;
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(05f * alphaCounter), (int)(65f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(05f * alphaCounter), (int)(65f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(05f * alphaCounter), (int)(65f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, DrawOffset - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(05f * alphaCounter), (int)(65f * alphaCounter), 0), Projectile.rotation, new Vector2(200, 200), 0.07f * (5 + 0.6f), SpriteEffects.None, 0f);


            Main.instance.LoadProjectile(Projectile.type);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(106, 255, 255), new Color(151, 46, 175), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightBlue.ToVector3() * 1.75f * Main.essScale);
        }
    }
}