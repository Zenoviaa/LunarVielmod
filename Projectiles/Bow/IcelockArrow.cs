using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class IcelockArrow : ModProjectile
    {
        public override string Texture => TextureRegistry.ZuiEffect;
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Archarilite Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Timer++;
            if (Timer % 7 == 0)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(2, 2);
                Color[] colors = new Color[] { Color.LightCyan, Color.Cyan, Color.Blue, Color.White };
                Color color = colors[Main.rand.Next(0, colors.Length)];
                float scale = Main.rand.NextFloat(0.5f, 0.8f);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, color, scale);
            }

        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White * 0.3f, Color.Transparent, completionRatio);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
                Color.LightCyan.G,
                Color.LightCyan.B, 0) * (1f - Projectile.alpha / 50f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 120);
            target.AddBuff(BuffID.Frostburn, 120);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw the texture
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 drawSize = texture.Size();
            Vector2 drawOrigin = drawSize / 2;

            //Draw the trail
            if (TrailDrawer == null)
            {
                TrailDrawer = new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            }

            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.WhispyTrail);

            Vector2 frameSize = new Vector2(32, 32);
            //Could also set this manually like
            //frameSize = new Vector2(58, 34);
            TrailDrawer.DrawPrims(Projectile.oldPos, frameSize * 0.5f - Main.screenPosition, 155);

            float scale = 2f;
            Color drawColor = (Color)GetAlpha(lightColor);
            SpriteBatch spriteBatch = Main.spriteBatch;
            for (int i = 0; i < 2; i++)
            {
                float rotOffset = MathHelper.TwoPi * (i / 4f);
                rotOffset += Timer * 0.003f;
                float drawScale = scale * (i / 4f);
                spriteBatch.Draw(texture, drawPosition, null, drawColor, Projectile.rotation + rotOffset,
                    drawOrigin, drawScale, SpriteEffects.None, 0f);
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 32; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(16, 16);
                float scale = Main.rand.NextFloat(0.3f, 0.5f);
                ParticleManager.NewParticle<StarParticle>(Projectile.Center, velocity, Color.White, scale);
            }
        }
    }
}
