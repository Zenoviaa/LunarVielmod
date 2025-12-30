using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.SummonerSystem
{
    public class SummoningCircle : ModProjectile
    {
        private float _colorLerp;
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
            if(Timer % 16 == 0)
            {
                Vector2 position = Owner.Center;
                position.X += Main.rand.NextFloat(-100, 100);
                Vector2 velocity = -Vector2.UnitY;
                Dust.NewDustPerfect(position, ModContent.DustType<GlyphDust>(), velocity, newColor: Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            if (Owner.HasBuff<BellSummoning>())
                Projectile.timeLeft = 30;
            Projectile.Center = Owner.Bottom;
            BellPlayer bellPlayer = Owner.GetModPlayer<BellPlayer>();
            _colorLerp = MathHelper.Lerp(_colorLerp, bellPlayer.summonRatio, 0.1f);
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
            DrawProgressBeam(ref lightColor);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
    
            return false;
        }

        private void DrawProgressBeam(ref Color lightColor)
        {
            Texture2D beamTexture = ModContent.Request<Texture2D>(Texture + "_Beam").Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = beamTexture.Size() / 2f;

            BellPlayer bellPlayer = Owner.GetModPlayer<BellPlayer>();
            Color drawColor = Color.Lerp(Color.Transparent, Color.White * 0.5f, _colorLerp);
            drawColor *= Projectile.timeLeft / 30f;
            Vector2 drawScale = new Vector2(0.85f, 1f);
     
            drawPos -= new Vector2(0, beamTexture.Height / 2);
            drawPos.Y += 4;
            spriteBatch.Draw(beamTexture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

        }
    }
}
