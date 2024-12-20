
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

namespace Stellamod.Projectiles.Magic
{
    internal class BookOfSkullsIIProg : ModProjectile
    {
        private PrimDrawer TrailDrawer = null;
        private ref float Timer => ref Projectile.ai[0];
        private bool ChosenFrame;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.penetrate = 2;
            Projectile.friendly = true;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (!ChosenFrame)
            {
                Projectile.frame = Main.rand.Next(3);
                ChosenFrame = true;
            }

            if (Timer % 6 == 0)
            {
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Projectile.velocity * 0.1f, 0, Color.Red, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                if (Main.rand.NextBool(2))
                    Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Projectile.velocity * 0.1f, 0, Color.Red, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            NPC nearest = ProjectileHelper.FindNearestEnemy(Projectile.position, 512);
            if (nearest != null)
            {
                Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, nearest.Center, 4);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 5f;
            return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.OrangeRed, Color.Red, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.OrangeRed;
            glowColor.A = 0;


            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 5; i++)
            {
                spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
            }


            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, Projectile.Frame(), Color.White, Projectile.rotation, Projectile.Frame().Size() / 2f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.DottedTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 32; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Yellow, 1f).noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.Orange, 1f).noGravity = true;
            }

            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp"), Projectile.position);
            FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Red,
                duration: Main.rand.Next(10, 25),
                baseSize: Main.rand.NextFloat(0.05f, 0.16f));
        }
    }
}
