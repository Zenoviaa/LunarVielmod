using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.SunStalker
{
    internal class SunFeather : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Sun Feather");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            base.Projectile.width = 19;
            base.Projectile.height = 19;
            base.Projectile.timeLeft = 700;
            base.Projectile.friendly = false;
            base.Projectile.hostile = true;
            base.Projectile.ignoreWater = true;
            base.Projectile.tileCollide = true;
        }
        public override void AI()
        {
            base.Projectile.frame = (((int)base.Projectile.ai[0] % 4 > 2) ? 1 : 0);
            base.Projectile.velocity = base.Projectile.velocity.RotatedBy(Math.Sin(base.Projectile.ai[0] * 0.45f) * 0.02500000037252903);
            base.Projectile.rotation = base.Projectile.velocity.ToRotation();
            base.Projectile.ai[0] += 0.25f;
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
        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightGoldenrodYellow.ToVector3() * 1.75f * Main.essScale);

        }
    }

}
