using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Trails;
using Terraria.Graphics.Shaders;
using static Terraria.ModLoader.ModContent;
using ParticleLibrary;
using Stellamod.Particles;
using Stellamod.Dusts;

namespace Stellamod.Projectiles.Magic
{

    internal class LampShot : ModProjectile
    {
        int Spin = 0;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Death");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 400;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        private bool Moved;
        private float alphaCounter = 0;
        public override void AI()
        {

            Projectile.ai[1]++;
          
            if (Projectile.ai[1] >= 10)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.ai[1] <= 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
            }
            if (alphaCounter <= 1)
            {
                alphaCounter += 0.08f;
            }

            Projectile.spriteDirection = Projectile.direction;




            Projectile.velocity.Y += 0.1f;
            Projectile.rotation += 0.1f;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 2)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                    float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                    Projectile.velocity.X += offsetX;
                    Projectile.velocity.Y += offsetY;
                    Projectile.netUpdate = true;
                }

                int Sound = Main.rand.Next(1, 4);
                if (Sound == 1)
                {
                    SoundEngine.PlaySound((SoundID.Item42), Projectile.position);
                }
                if (Sound == 2)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Morrowarrow"), Projectile.position);
                }
                if (Sound == 3)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/CinderBraker"), Projectile.position);

                }
                Spin = Main.rand.Next(0, 2);
            }
            if (Projectile.ai[0] >= 30)
            {


            }

        }
        public override void OnKill(int timeLeft)
        {
            for (int j = 0; j < 4; j++)
            {
                Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                ParticleManager.NewParticle(Projectile.Center, speed * 6, ParticleManager.NewInstance<FlameParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
            }
            var EntitySource = Projectile.GetSource_Death();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<StarFlowerproj3>(), Projectile.damage * 2, 1, Main.myPlayer, 0, 0);
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 524f, 14f);
            for (int i = 0; i < 2; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustType<GlowDust>(), 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 0.7f).noGravity = true;
            }
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 100, Color.Gray, 0.4f).noGravity = true;
            }
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.YellowTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 0.6f).noGravity = true;
            }
            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.YellowTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 0.2f).noGravity = false;
            }
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathImpact, Projectile.position);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightYellow, Color.Orange, completionRatio) * 0.2f;
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

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(85f * alphaCounter), (int)(45f * alphaCounter), (int)(15f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.0f * Main.essScale);
        }
    }

}

