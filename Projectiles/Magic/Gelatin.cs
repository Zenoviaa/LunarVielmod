
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class GelatinBoom : BaseIgniterExplosion
    {
        public override int FrameCount => 22;
        public override void Start()
        {
            base.Start();
            var circle = EffectsHelper.SimpleExplosionCircle(Projectile, Color.Aqua, endRadius: 80);
            for (int i = 0; i < 2; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            }

            for (int i = 0; i < 15; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0f, -2f, 0, default(Color), 1f);
                Main.dust[num].noGravity = true;
                Main.dust[num].scale = 1.9f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 10f;
                }
            }

            for (int i = 0; i < 5; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0f, -2f, 0, default(Color), 1.2f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 10f;
                }
            }
        }
    }

    public class Gelatin : ModProjectile
	{
        private ref float Timer => ref Projectile.ai[0];
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Gelatin");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

		public override void SetDefaults()
		{
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 6 == 0)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.VenomStaff);
                int dust1 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.VenomStaff);
                Main.dust[dust].noGravity = true;
                Main.dust[dust1].noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(255, 136, 247), new Color(54, 64, 195), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }


        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<GelatinBoom>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner);
        }
    }
}