
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class Warrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Archarilite Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 17;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.extraUpdates += 1;
        }

        public override void AI()
        {
            Timer++;
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Aquamarine, Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }

            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MushroomTorch, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            if (Projectile.velocity.Length() < 35)
                Projectile.velocity *= 1.05f;

            Lighting.AddLight(Projectile.Center, Color.AliceBlue.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            FXUtil.ShakeCamera(Projectile.Center, 1024, 4);
            for (float f = 0; f < 12; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Aquamarine, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.MushroomTorch, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default, 1f).noGravity = false;
            }

            for (int i = 0; i < 24; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<WataBoom2>(), (int)(Projectile.damage * 2), 0f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/WinterboundFlare"), Projectile.position);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Aquamarine, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.LoveTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            SpriteBatch spriteBatch = Main.spriteBatch;

            //Setup the shader
            MotionBlurShader shader = MotionBlurShader.Instance;
            float maxSpeed = 0.4f;
            float speed = MathHelper.Clamp(Projectile.velocity.Length() * 0.02f, 0f, maxSpeed);

            //This is gonna make it like stretch itself as it moves faster
            Vector2 scale = Vector2.Lerp(Vector2.One, new Vector2(2f, 0.18f), Easing.InOutCubic(speed));

            shader.Velocity = Vector2.UnitY * speed;

            //This just affects the opacity of the blur, prob don't need to change this number
            shader.BlurStrength = 2f;
            shader.Apply();

            //Draw the texture
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle frame = Projectile.Frame();
            Vector2 drawOrigin = frame.Size() / 2f;

            float rotation = Projectile.rotation;
            Color finalColor = Color.White.MultiplyRGB(lightColor);
            spriteBatch.Draw(texture, drawPos, frame, finalColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);

            //Draw the blurring on top
            spriteBatch.Restart(effect: shader.Effect);
            spriteBatch.Draw(texture, drawPos, frame, finalColor * 0.5f, rotation, drawOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }
    }
}
