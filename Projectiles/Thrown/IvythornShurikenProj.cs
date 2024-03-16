using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    internal class IvythornShurikenProj : ModProjectile
    {
        bool Moved;
        float Timer;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.Shuriken);
            AIType = ProjectileID.Shuriken;
            Projectile.scale = 1.2f;
        }


        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.JunglePlants, (Vector2.One).RotatedByRandom(25.0), 0, default(Color), 1f).noGravity = false;
            }
            for (int i = 0; i < 5; i++)
            {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.JunglePlants, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num1].position != Projectile.Center)
                    Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, 0f, -2f, 0, default(Color), .8f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                if (Main.dust[num].position != Projectile.Center)
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
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
            return Color.Lerp(Color.DarkGreen, Color.Transparent, completionRatio) * 0.1f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.StringTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            return false;
        }
    }
}


