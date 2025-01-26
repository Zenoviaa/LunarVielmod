using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Bow;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class WinterStormProg : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Winter Storm");
            ProjectileID.Sets.TrailingMode[Type] = 1;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }


        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 180;
            Projectile.alpha = 0;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Timer++;
            if (Timer >= 10)
            {
                Projectile.tileCollide = true;
            }

            Projectile.rotation += Projectile.velocity.Length() * 0.0f;
            Projectile.velocity.Y += 0.2f;
            if (Main.rand.NextBool(7))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].noGravity = false;
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            var source = Projectile.GetSource_Death();
            if (Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 vel = new Vector2();
                    vel.X = Main.rand.Next(-4, 5);
                    vel.Y = Main.rand.Next(-4, 5);
                    Projectile.NewProjectile(source, Projectile.Center, vel,
                        ModContent.ProjectileType<WinterStormFragProg>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = new Vector2();
                    vel.X = Main.rand.Next(-2, 2);
                    vel.Y = Main.rand.Next(-2, 2);
                    Projectile.NewProjectile(source, Projectile.Center, vel,
                        ModContent.ProjectileType<WinterboundArrowFlake>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
            }

            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterStorm"), Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Snow, (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(25.0), 0, default(Color), 1f).noGravity = false;
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
            return Color.Lerp(Color.LightBlue, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.CrystalTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }
    }
}