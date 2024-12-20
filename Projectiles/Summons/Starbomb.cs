using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons
{
    public class Starbomb : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private float ScaleProgress => Easing.InExpo(Timer / 60f);
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();

            if (Main.rand.NextBool(8))
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Yellow, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            if (Main.rand.NextBool(8))
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Purple, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }
            Projectile.velocity *= 0.98f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f + 0.05f;
            if(Projectile.velocity.Length() <= 0.25f)
            {
                Timer++;
                if(Timer >= 60)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 4;
            return MathHelper.SmoothStep(baseWidth, 0.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Goldenrod, Color.Blue, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.Aqua;
            glowColor.A = 0;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor, Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f), SpriteEffects.None, 0f);
      
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), Projectile.scale + ScaleProgress, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.LoveTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            base.PostDraw(lightColor);
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/DimLight").Value;
            Color glowColor = Color.Yellow;
            glowColor.A = 0;
            SpriteBatch spriteBatch = Main.spriteBatch;
            for(int i = 0; i < 8; i++)
            {
                spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, glowColor * ScaleProgress, Projectile.rotation, new Vector2(32, 32), 0.17f * (5 + 0.6f) + ScaleProgress, SpriteEffects.None, 0f);
            }
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<StarBoomer>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }

    public class StarBoomer : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            float progress = Timer / 120f;
            int divisor = (int)MathHelper.Lerp(20, 10, progress);
            if(Timer % divisor == 0 || Timer == 1)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + Main.rand.NextVector2Circular(24, 24), Vector2.Zero, 
                        ModContent.ProjectileType<StarBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            if(Timer >= 120)
            {
                Projectile.Kill();
            }
        }
    }

    public class StarBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 15;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {

                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
                for (float f = 0; f < 16; f++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(),
                        (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Yellow, Main.rand.NextFloat(1f, 3f)).noGravity = true;
                }

                SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, Projectile.position);

                switch (Main.rand.Next(3))
                {
                    case 0:
                        morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
                        break;
                    case 1:
                        morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
                        break;
                    case 2:
                        morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower3");
                        break;
                }

                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, Projectile.position);

                FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: 0.24f);

                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }

                if(Main.myPlayer == Projectile.owner)
                {
                    float oofed = Main.rand.NextFloat(-1f, 1f);
                    for (float f = 0; f < 4; f++)
                    {
                        float rot = (f / 4f) * MathHelper.TwoPi;
                        rot += oofed;
                        Vector2 velocity = rot.ToRotationVector2() * 8;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ProjectileID.SuperStar, Projectile.damage, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
        }
    }
}