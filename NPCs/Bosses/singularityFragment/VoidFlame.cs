using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.singularityFragment
{
    public class VoidFlame : ModProjectile
    {
        public int Time = 1;
        public float Set = 0.99f;
        public override string Texture => TextureRegistry.EmptyTexture;

        private PrimitiveTrail TrailDrawer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 500;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.scale = 1.2f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffType<AbyssalFlame>(), 200);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.Blue.R,
                Color.Blue.G,
                Color.Blue.B, 0);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black, Color.Black, completionRatio) * (1f - completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return (Projectile.width * Projectile.scale * 1f - completionRatio) * (1f - Easing.OutCubic(completionRatio));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var textureAsset = TextureRegistry.CloudTexture;


            float drawScale = 0.1f;
            float drawRotation = Projectile.rotation;

            Vector2 drawSize = textureAsset.Value.Size();
            Vector2 drawOrigin = drawSize / 2;
            Color drawColor = (Color)GetAlpha(lightColor);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + new Vector2(16, 16);

            TrailDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true, TrailRegistry.LaserShader);
            TrailDrawer.SpecialShader = TrailRegistry.LaserShader;
            TrailDrawer.SpecialShader.UseColor(Color.Black);
            TrailDrawer.SpecialShader.SetShaderTexture(TrailRegistry.WaterTrail);
            TrailDrawer.Draw(Projectile.oldPos, -Main.screenPosition + new Vector2(16, 16), 32);

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            spriteBatch.Draw(textureAsset.Value, drawPosition, null, drawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);

            // Retrieve reference to shader
            var shader = ShaderRegistry.MiscFireWhitePixelShader;

            //You have to set the opacity/alpha here, alpha in the spritebatch won't do anything
            //Should be between 0-1
            float opacity = 0.75f;
            shader.UseOpacity(opacity);

            //How intense the colors are
            //Should be between 0-1
            shader.UseIntensity(0.5f);

            //How fast the extra texture animates
            float speed = 1.0f;
            shader.UseSaturation(speed);

            //Color
            shader.UseColor(new Color(Color.Black.R, Color.Black.G, Color.Black.B, 1));

            //Texture itself
            shader.UseImage1(TrailRegistry.WaterTrail);

            // Call Apply to apply the shader to the SpriteBatch. Only 1 shader can be active at a time.
            shader.Apply(null);

            spriteBatch.Draw(textureAsset.Value, drawPosition, null, (Color)GetAlpha(lightColor), drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0f);

    

            spriteBatch.End();
            spriteBatch.Begin();
            return false;
        }


        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.RoyalBlue, 1f).noGravity = true;
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.RoyalBlue, 1f).noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 2048f, 64f);
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 1.0f * Main.essScale);
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;
            Projectile.ai[0]++;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 256f, 16f);
            if (Main.netMode != NetmodeID.Server)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.BlueTorch);
                dust.velocity *= -1f;
                dust.scale *= .8f;
                dust.noGravity = true;
                Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                vector2_1.Normalize();
                Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                dust.velocity = vector2_2;
                vector2_2.Normalize();
                Vector2 vector2_3 = vector2_2 * 34f;
                dust.position = Projectile.Center - vector2_3;
                Projectile.netUpdate = true;
            }

            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BlueTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.BlueTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }
        }
    }
}