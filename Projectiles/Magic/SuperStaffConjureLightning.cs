using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class SuperStaffConjureLightning : ModProjectile
    {
        private float _scale;
        private float _width;
        private Vector2[] _lightningZaps;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Charge => ref Projectile.ai[1];

        public LightningTrail[] LightningTrailPath;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _width = 1;
            _lightningZaps = new Vector2[4];
            LightningTrailPath = new LightningTrail[4];
            for(int i = 0; i < 4; i++)
            {
                LightningTrailPath[i] = new LightningTrail();
            }

            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.timeLeft = 360;
            Projectile.light = 0.78f;
        }

        private float WidthFunction(float completionRatio)
        {
            float progress = completionRatio / 0.3f;
            float rounded = Easing.SpikeOutCirc(progress);
            float spikeProgress = Easing.SpikeOutExpo(completionRatio);
            float fireball = MathHelper.Lerp(rounded, spikeProgress, Easing.OutExpo(1.0f - completionRatio));
            float midWidth = 6 * _width;
            return MathHelper.Lerp(0, midWidth, fireball);
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.White, Color.Yellow, p);
            return trailColor;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White.MultiplyRGB(lightColor);
            float drawRotation = Projectile.rotation;
            float drawScale = _scale;

            SpriteBatch spriteBatch = Main.spriteBatch;

            var prevBelndState = Main.graphics.GraphicsDevice.BlendState;
            Main.graphics.GraphicsDevice.BlendState = BlendState.Additive;
            _width = 1;
            for (int i = 0; i < 4; i++)
            {
                LightningTrailPath[i].Draw(spriteBatch, _lightningZaps, Projectile.oldRot, ColorFunction, WidthFunction, Projectile.Size / 2);
                _width -= 0.1f;
            }

            Main.graphics.GraphicsDevice.BlendState = prevBelndState;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Draw(texture, drawPos, Projectile.Frame(), drawColor, drawRotation, Projectile.Frame().Size() / 2f, drawScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

      
            for (int i = 0; i < 2; i++)
            {
                Vector2 flameDrawPos = drawPos + Main.rand.NextVector2Circular(2, 2);
                float rot = Main.rand.NextFloat(0f, 3.14f);
        
                spriteBatch.Draw(texture, flameDrawPos, Projectile.Frame(), drawColor, drawRotation + rot, Projectile.Frame().Size() / 2f, 
                    drawScale , SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 3 == 0)
            {
                for (int i = 0; i < _lightningZaps.Length; i++)
                {
                    _lightningZaps[i] = Projectile.Center + Main.rand.NextVector2Circular(32, 32);
                }

                for (int i = 0; i < LightningTrailPath.Length; i++)
                {
                    LightningTrailPath[i].RandomPositions(_lightningZaps);
                }
            }

        
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }
            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }
            if (Timer <= 15)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.8f, 1f), Easing.InCubic(Timer / 15f));
            }

            DrawHelper.AnimateTopToBottom(Projectile, 4);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 16; i++)
            {
                float progress = (float)i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }
        }
    }
}
