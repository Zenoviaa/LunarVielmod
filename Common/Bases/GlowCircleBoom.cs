using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal class GlowCircleBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private float Progress => Timer / 15;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(TextureRegistry.EmptyBigTexture).Value;
            Vector2 centerPos = Projectile.Center - Main.screenPosition;
            GlowCircleShader shader = GlowCircleShader.Instance;
            shader.Speed = 5;

            float bp = 0.5f;
            shader.BasePower = MathHelper.Lerp(bp, bp * 2, Easing.SpikeOutCirc(Progress));

            float s = 0.00525f;
            shader.Size = MathHelper.Lerp(s, s * 2, Easing.SpikeOutCirc(Progress));

            Color startInner = Color.White;
            Color startGlow = Color.Yellow;
            Color startOuterGlow = Color.Red;

            Color endColor = startOuterGlow;


            shader.InnerColor = Color.Lerp(startInner, startGlow, Progress);
            shader.GlowColor = Color.Lerp(startGlow, startOuterGlow, Progress);
            shader.OuterGlowColor = Color.Lerp(startOuterGlow, Color.Black, Progress);
            shader.Apply();

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
            for (int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture, centerPos, null, Color.White, Projectile.rotation, texture.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
            return false;
        }
    }
}
