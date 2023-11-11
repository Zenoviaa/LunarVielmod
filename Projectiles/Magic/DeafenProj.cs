using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class DeafenProj : ModProjectile
    {
        bool Moved;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            base.Projectile.penetrate = 5;
            base.Projectile.width = 25;
            base.Projectile.height = 25;
            base.Projectile.timeLeft = 700;
            base.Projectile.alpha = 255;
            base.Projectile.friendly = true;
            base.Projectile.hostile = false;
            base.Projectile.ignoreWater = true;
            base.Projectile.tileCollide = false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3) && !target.boss)
                target.AddBuff(BuffID.Confused, 180);
        }
        public override void AI()
        {
            Projectile.velocity *= 1.02f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/TheDeafen"), Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/TheDeafen2"), Projectile.position);
                }

                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.alpha = 255;
                Moved = true;
            }
            if ( Projectile.ai[1] >= 20)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.alpha >= 0)
            {
                Projectile.alpha -= 2;
            }

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
        }
        public override void OnKill(int timeLeft)
        {
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
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
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, Color.MediumPurple with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale, 0, 0);
            }
            for (int i = 0; i < 7; i++)
            {
                float scaleFactor = 1f - i / 6f;
                Vector2 drawOffset = Projectile.velocity * i * -0.34f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, null, drawColor with { A = 160 } * Projectile.Opacity, Projectile.rotation, texture.Size() * 0.5f, scale * scaleFactor, 0, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.MediumPurple) * (float)(((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);

        }
    }

}


