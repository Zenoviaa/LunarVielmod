
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class BookOfSkullsIIProg : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pericarditis");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 22;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (target.boss)
            {
                OnKill(1);
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.penetrate = 10;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
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
            Projectile.penetrate = 1;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 scale = new(Projectile.scale, 1f);
            Color drawColor = Projectile.GetAlpha(lightColor);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * 4f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, Color.Pink with { A = 200 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale, 0, 0);
            }

            for (int i = 0; i < 7; i++)
            {
                float scaleFactor = 1f - i / 6f;
                Vector2 drawOffset = Projectile.velocity * i * -0.34f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, drawColor with { A = 200 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale * scaleFactor, 0, 0);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor with { A = 200 }, Projectile.rotation, texture.Size() * 0.5f, scale, 0, 0);
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
            return true;
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
