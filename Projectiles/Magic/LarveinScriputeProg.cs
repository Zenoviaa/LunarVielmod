using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Magic
{
    public class LarveinScriputeProg : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private ref float AlphaCounter => ref Projectile.ai[1];
        private ref float Red => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            AlphaCounter = 1;
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.timeLeft = 100;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }


        public override void AI()
        {
            Projectile.velocity *= 0.98f;
            Timer++;
            if (Timer == 1)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.alpha = 255;
            }

            if (Timer <= 1)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    float offsetX = Main.rand.Next(-200, 200) * 0.01f;
                    float offsetY = Main.rand.Next(-200, 200) * 0.01f;
                    float A = Main.rand.Next(0, 2);

                    if (A == 0)
                    {
                        Red = 15;
                    }
                    else
                    {
                        Red = 65;
                    }
                    Projectile.velocity.X += offsetX;
                    Projectile.velocity.Y += offsetY;
                    Projectile.netUpdate = true;
                }

                Projectile.scale = 1.5f;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
            }

            if (Main.rand.NextBool(3))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].velocity.Y += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].velocity.X += Main.rand.Next(-2, 2);
                Main.dust[dustnumber].noGravity = true;
                Main.dust[dustnumber].noLight = false;
            }

            if (Timer >= 90)
            {
                if (Projectile.scale >= 0)
                {
                    Projectile.scale -= 0.22f;
                }
                if (AlphaCounter >= 0)
                {
                    AlphaCounter -= 0.08f;
                }
            }


            Projectile.spriteDirection = Projectile.direction;
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 1.0f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            var EntitySource = Projectile.GetSource_Death();
            for (int i = 0; i < 5; i++)
            {
                Projectile.timeLeft = 2;
                Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-4, 5), Main.rand.Next(-4, 5),
                    ModContent.ProjectileType<LarveinScriputeProg2>(), Projectile.damage, 1, Projectile.owner, 0, 0);
            }


            ShakeModSystem.Shake = 7;
            float speedXa = -Projectile.velocity.X * Main.rand.NextFloat(.4f, .7f) + Main.rand.NextFloat(-8f, 8f);
            float speedYa = -Projectile.velocity.Y * Main.rand.Next(0, 0) * 0.01f + Main.rand.Next(-20, 21) * 0.0f;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, speedXa * 0, speedYa * 0,
                ModContent.ProjectileType<MooningKaboom>(), (int)(0), 0f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterStorm"), Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default(Color), 1f).noGravity = false;
            }
            for (int i = 0; i < 15; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, 205, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default(Color), 1f).noGravity = false;
            }
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightBlue, Color.BlueViolet, completionRatio) * 0.7f;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.LightningTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(Red * AlphaCounter), (int)(15f * AlphaCounter), (int)(85f * AlphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(Red * AlphaCounter), (int)(15f * AlphaCounter), (int)(85f * AlphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(Red * AlphaCounter), (int)(15f * AlphaCounter), (int)(85f * AlphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(Red * AlphaCounter), (int)(15f * AlphaCounter), (int)(85f * AlphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
        }
    }
}


