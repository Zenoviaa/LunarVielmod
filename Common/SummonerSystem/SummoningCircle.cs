using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem
{
    public class SummoningCircle : ModProjectile
    {
        private float _colorLerp;
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        private TexturedQuad _texturedQuad;
        private TexturedQuad TexturedQuad
        {
            get
            {
                _texturedQuad ??= new TexturedQuad();
                return _texturedQuad;
            }
        }
        public override string Texture => TextureRegistry.EmptyTexture;
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
            if (Timer % 16 == 0)
            {
                Vector2 position = Owner.Center;
                position.X += Main.rand.NextFloat(-100, 100);
                Vector2 velocity = -Vector2.UnitY * Main.rand.NextFloat(1f, 3f);
                DustParticleSpawnParams spawnParams = new DustParticleSpawnParams
                {
                    outerColor = Color.White,
                    gravity = 0,
                    scaleRange = new Vector2(0.2f, 0.5f)

                };

                var dp = DustParticle.Spawn(position, velocity, spawnParams);
                dp.parent = Owner;
                dp.fast = true;
            }
            if (Owner.HasBuff<BellSummoning>())
                Projectile.timeLeft = 30;
            Projectile.Center = Owner.Bottom;
            BellPlayer bellPlayer = Owner.GetModPlayer<BellPlayer>();
            _colorLerp = MathHelper.Lerp(_colorLerp, bellPlayer.summonRatio, 0.1f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedRings);
            SpriteBatch spriteBatch = Main.spriteBatch;
            var glowMask = AssetManager.GlowMask.SimpleGlowCircle;
            Color glowColor = Color.White;
            glowColor = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(Timer / 30f) * 0.25f);
            glowColor.A = 0;
            Vector2 scale = new Vector2(0.7f, 0.15f);
            spriteBatch.Draw(glowMask.Value, Projectile.Center + Vector2.UnitY * 16 - Main.screenPosition, null, glowColor, 0, glowMask.Size() / 2f, scale, SpriteEffects.None, 0);
            return false;
        }

        private void DrawPixelatedRings(GraphicsDevice graphicsDevice)
        {
            float ease = EasingFunction.InOutSine(Timer / 30f);
            Vector2 ring1Scale = Vector2.Lerp(Vector2.Zero, Vector2.One * new Vector2(1, 0.35f), ease);
            float perspectiveRotation = Main.GlobalTimeWrappedHourly * 8;
            DrawRingInner(ring1Scale, Color.White, -Vector2.UnitY, perspectiveRotation);
        }

        private void DrawRingInner(Vector2 size, Color color, Vector2 velocity, float perspectiveRotation)
        {
            MagicCircleShader magicCircleShader = MagicCircleShader.Instance;

            //Here we need to prepare the shader
            float numFrames = 1f;
            float f = 0;
            Vector2 tiling = new Vector2(1f, 1f / numFrames);
            Vector2 offset = new Vector2(0, f * 1f / numFrames);
            Vector4 tilingOffset = new Vector4(offset.X, offset.Y, tiling.X, tiling.Y);
            magicCircleShader.TilingOffset = tilingOffset;
            magicCircleShader.RingTexture = AssetManager.GlowMask.MagicCircle;

            Color auraColor = Color.Lerp(Color.Black, Color.White, EasingFunction.InOutSine(Timer / 30f));
            auraColor = auraColor.MultiplyRGB(color);

            TexturedQuad.CalculatePerspectiveCenterVertices2(Projectile.Center + Vector2.UnitY * 16, 180, 180, velocity.ToRotation(), perspectiveRotation);
            TexturedQuad.SetColor(auraColor);
            TexturedQuad.DrawWithShader(magicCircleShader);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

        }
    }
}
