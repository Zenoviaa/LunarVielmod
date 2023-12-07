using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Projectiles.Magic
{
    internal class ScytheOfSoulsProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pericarditis");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 22;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.penetrate = 3;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = 5;
            Projectile.knockBack = 12.9f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity.Y = -Projectile.velocity.Y * 3;
            Projectile.velocity.X = -Projectile.velocity.X * 3;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
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


            Projectile.alpha = Math.Max(0, Projectile.alpha - 25);

            bool flag25 = false;
            int jim = 1;
            for (int index1 = 0; index1 < 200; index1++)
            {
                if (Main.npc[index1].CanBeChasedBy(Projectile, false)
                    && Projectile.Distance(Main.npc[index1].Center) < 800
                    && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[index1].Center, 1, 1))
                {
                    flag25 = true;
                    jim = index1;
                }
            }

            if (flag25)
            {
                Projectile.velocity *= 1.02f;
                float num1 = 10f;
                Vector2 vector2 = new Vector2(Projectile.position.X + Projectile.width * 0.5f, Projectile.position.Y + Projectile.height * 0.5f);
                float num2 = Main.npc[jim].Center.X - vector2.X;
                float num3 = Main.npc[jim].Center.Y - vector2.Y;
                float num4 = (float)Math.Sqrt((double)num2 * num2 + num3 * num3);
                float num5 = num1 / num4;
                float num6 = num2 * num5;
                float num7 = num3 * num5;
                int num8 = 10;
                Projectile.velocity.X = (Projectile.velocity.X * (num8 - 1) + num6) / num8;
                Projectile.velocity.Y = (Projectile.velocity.Y * (num8 - 1) + num7) / num8;
            }
            Projectile.rotation += 0.55f;
        }

        public override void OnKill(int timeLeft)
        {
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn1"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Dreadmire_BoneSpawn2"), Projectile.position);
            }

            for (int i = 0; i < 40; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, 205, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
            }


            for (int i = 0; i < 40; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
            }


            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 2048f, 16f);
            var entitySource = Projectile.GetSource_FromThis();
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
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }
    }
}
