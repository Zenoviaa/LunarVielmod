
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class BriskflyProg : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Star Sheith");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        bool Sounded = false;
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Magic;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }
        public override void AI()
        {
            if (!Sounded)
            {
                int Type = Main.rand.Next(1, 5);
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Briskfly"));
                Sounded = true;
            }

            if(Type == 1)
            {
                base.Projectile.velocity = base.Projectile.velocity.RotatedBy(1.1);
            }
            if (Type == 2)
            {
                base.Projectile.velocity = base.Projectile.velocity.RotatedBy(-1.1);
            }
            if (Type == 3)
            {
                base.Projectile.velocity = base.Projectile.velocity.RotatedBy(2.2);
            }
            if (Type == 4)
            {
                base.Projectile.velocity = base.Projectile.velocity.RotatedBy(-2.2);
            }

            base.Projectile.rotation = base.Projectile.velocity.ToRotation() + 45;
            base.Projectile.ai[0] += 0f;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.BlueViolet.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(8))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.FireworkFountain_Pink, 0f, 0f, 150, Color.BlueViolet, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            if (Main.rand.NextBool(8))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Blue, 0f, 0f, 150, Color.BlueViolet, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }

        }


    }
}


