using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class RazzleDazzleProj : ModProjectile
    {
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 30;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity.Y += 0.05f;

            float rotation = (Timer / 30) * MathHelper.TwoPi;
            Projectile.rotation = Projectile.velocity.ToRotation() + rotation;
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.3f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White * 0.2f, Color.Transparent, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<Sparkle>(), 0, 0, 0, Color.White, 1f);
                Main.dust[dustnumber].noGravity = true;
            }

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<Sparkle>(), 
                    (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(MathHelper.TwoPi), 0, Color.White, 1f).noGravity = false;
            }

            for (int i = 0; i < Main.rand.Next(2, 5); i++)
            {
                Vector2 velocity = -Projectile.velocity;
                velocity = velocity.RotatedByRandom(MathHelper.PiOver4 + MathHelper.PiOver4/ 2);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, velocity,
                             ProjectileID.BabySpider, Projectile.damage, Projectile.knockBack, Projectile.owner);
            }         
        }
    }
}
