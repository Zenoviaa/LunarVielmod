using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Dusts;
using Stellamod.Core;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.CrumblingTowerOfIlluria.Projectiles
{
    public class CrumblingSoul : ScarletProjectile
    {
        private float _completionRatio;
        private Vector2 _startCenter;
        private ref float Timer => ref Projectile.ai[0];
        private NPC Target
        {
            get => Main.npc[(int)Projectile.ai[1]];
        }
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 32;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                _startCenter = Projectile.Center;
            }

            Vector2 endCenter = Target.Center;
            float flyTime = 180f;
            _completionRatio = Timer / flyTime;
            float ease = EasingFunction.Anticipation2(_completionRatio);
            Vector2 between = Vector2.Lerp(_startCenter, endCenter, ease);
            Vector2 velocityTo = (between - Projectile.Center);
            Projectile.velocity = velocityTo;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Timer % 10 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Vector2.Zero, newColor: Color.White, Scale: 0.5f);
            }
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(69, 196, 182), Color.SpringGreen, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(16, 0, completionRatio) * EasingFunction.InExpo(_completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }

        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
        }
    }
}
