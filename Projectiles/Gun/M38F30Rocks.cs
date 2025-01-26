using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class M38F30Rocks : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rock");
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
        }

        public override void PostDraw(Color lightColor)
        {

        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shuriken);
            AIType = ProjectileID.Shuriken;
            Projectile.penetrate = 1;
            Projectile.width = 8;
            Projectile.height = 12;
            Projectile.timeLeft = 700;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] >= 10)
            {
                Projectile.tileCollide = true;
            }

            if (Projectile.ai[1] == 2)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30"), Projectile.position);
            }

            if (Main.rand.NextBool(7))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].noGravity = false;
                Main.dust[dustnumber].velocity *= 0.3f;
            }
            Projectile.velocity.Y -= 0.01f;
            Lighting.AddLight(Projectile.Center, Color.Brown.ToVector3() * 1.75f * Main.essScale);
        }
        public override void OnKill(int timeLeft)
        {
            for (float f = 0; f < 16; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Orange, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Orange,
                    outerGlowColor: Color.Black,
                    baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                    duration: Main.rand.NextFloat(12, 24));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }


            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb2"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/M38F30Bomb1"), Projectile.position);
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 512f, 4f);
            var EntitySource = Projectile.GetSource_Death();
            for (int i = 0; i < 5; i++)
            {
                Vector2 velocity = new Vector2(Main.rand.Next(-8, 8), Main.rand.Next(-8, -5));
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                    ModContent.ProjectileType<M38F30Rocks2>(), Projectile.damage / 3, 1, Projectile.owner, 0, 0);
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.SandyBrown, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.FadedStreak);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }
    }
}