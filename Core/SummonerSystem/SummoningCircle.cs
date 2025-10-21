using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.SummonerSystem
{
    public class SummoningCircle : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Owner.HasBuff<BellSummoning>())
                Projectile.timeLeft = 30;
            Projectile.Center = Owner.Bottom;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D ringTexture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = RadiantShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.LightBlue;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, shader.Effect, Main.GameViewMatrix.TransformationMatrix);

            Color auraColor = Color.White;
            auraColor *= Timer / 30f;
            auraColor *= Projectile.timeLeft / 30f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Rectangle? frameRect = null;
            Vector2 scale = new Vector2(1f, 0.05f);
            Vector2 drawScale = scale * Vector2.One;
            drawScale *= MathHelper.Lerp(0.8f, 1f, ExtraMath.Osc(0f, 1f));

            float drawRotation = Projectile.rotation;
            Vector2 drawOrigin = ringTexture.Size() / 2f;
            spriteBatch.Draw(ringTexture, drawPos, frameRect, auraColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Draw(ringTexture, drawPos, frameRect, auraColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

        }
    }
}
