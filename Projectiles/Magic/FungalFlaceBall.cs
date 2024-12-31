
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class FungalFlaceBall : ModProjectile
    {


        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fungal Flace Ball");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightPink.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Plantera_Pink, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }

        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shuriken);
            AIType = ProjectileID.Shuriken;
            Projectile.penetrate = 1;
            base.Projectile.width = 26;
            base.Projectile.height = 26;
            base.Projectile.timeLeft = 700;
            base.Projectile.alpha = 255;
            base.Projectile.friendly = true;
            base.Projectile.hostile = false;
            base.Projectile.ignoreWater = true;
            base.Projectile.tileCollide = false;
        }
        public override void AI()
        {

            if (Projectile.ai[1] <= 3)
            {
                int Sound = Main.rand.Next(1, 3);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FungalFlaceBall1"), Projectile.position);
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FungalFlaceBall2"), Projectile.position);
                }
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 10)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.alpha >= 0)
            {
                Projectile.alpha -= 2;
            }
            Projectile.velocity.Y -= 0.01f;
        }
        public override void OnKill(int timeLeft)
        {
            var EntitySource = Projectile.GetSource_Death();
            for (int i = 0; i < 3; i++)
            {
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), 
                    ModContent.ProjectileType<FungalFlaceCloud>(), 5, 1, Projectile.owner);
            }
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
            }

            for (int i = 0; i < 14; i++)
            {

                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGoldenrod, 1f).noGravity = true;
            }
            for (int i = 0; i < 2; i++)
            {
                Projectile.timeLeft = 2;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), 
                        ModContent.ProjectileType<FungalFlaceCloudGreen>(), 5, 1, Projectile.owner);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(-2, 2)).RotatedByRandom(19.0), 0, Color.DarkOliveGreen, 1f).noGravity = true;
            }
            for (int i = 0; i < 20; i++)
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
            }
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FungalFlaceBall3"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/FungalFlaceBall4"), Projectile.position);
            }

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.VenomStaff, 0f, 0f, 150, Color.MediumPurple, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Pink) * (float)(((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) / 2);
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

    }

}


