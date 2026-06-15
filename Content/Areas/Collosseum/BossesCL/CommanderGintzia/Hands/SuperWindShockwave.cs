using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.CommanderGintzia.Hands
{
    public class SuperWindShockwave : ModProjectile
    {
        private Vector2[] _shockwavePos;
        private float FadeTime => 15f;
        private ref float Timer => ref Projectile.ai[0];
        private ref float DeathTimer => ref Projectile.ai[1];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 160;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 16 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GemDiamond, Scale: 0.5f);
            }
            if (Timer % 8 == 0)
            {
                Vector2 pos = Projectile.Center;
                pos.X += Main.rand.NextFloat(-64, 64);
                pos.Y -= 16;
                Vector2 vel = -Vector2.UnitY;
                vel *= 7f;
                var dp = DustParticle.Spawn(pos, vel);
                dp.outerColor = Color.White;
                dp.Scale *= 0.5f;
                dp.noTileCollide = true;
                dp.gravity = 0;
                dp.dampening = 0.05f;
            }

            if (Timer < 20)
                ShakeScreenPosition.Shake = 4;
            Projectile.velocity *= 1.01f;
            Point tp = (Projectile.Center + new Vector2(0, -8)).ToTileCoordinates();
            Tile tile = Main.tile[tp];
            if (tile.HasTile && Main.tileSolid[tile.TileType] && DeathTimer == 0)
            {
                if(Timer == 1)
                {
                    Projectile.Center += new Vector2(0, -16);
                }
                else
                {
                    DeathTimer++;
                }
       
                // Projectile.Kill();
            }

            if (DeathTimer > 0)
            {
                DeathTimer++;
                if (DeathTimer >= FadeTime)
                    Projectile.Kill();
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.62f;
            return MathHelper.SmoothStep(baseWidth * 2, baseWidth, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            Color startColor = Color.White;
            float easedCompletion = Easing.InCubic(completionRatio);
            return Color.Lerp(startColor, Color.Transparent, easedCompletion);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw Trail
            _shockwavePos ??= new Vector2[Projectile.oldPos.Length];

            DrawPixelatedShockwaveV2(Main.spriteBatch, Main.screenPosition);
            return false;
        }

        private void DrawPixelatedShockwaveV2(SpriteBatch sb, Vector2 sp)
        {
            float fade = MathHelper.Lerp(1f, 0f, DeathTimer / FadeTime);
            float inScale = EasingFunction.OutExpo(Timer / 30f);
            Asset<Texture2D> waveTexture = AssetManager.GlowMask.Wave;
            WaveShader waveShader = ShaderContent.GetInstance<WaveShader>();
            waveShader.Time = Main.GlobalTimeWrappedHourly * 0.5f;
            waveShader.Amplitude = 0.2f;
            waveShader.Frequency = 24;
            waveShader.XStrength = 32;
            waveShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
            sb.Restart(effect: waveShader.Effect);
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(waveTexture, Projectile.Center);
            drawer.BottomCenterOrigin();
            drawer.color = Color.White * fade;
            drawer.color.A = 0;
            drawer.scale.Y *= MathHelper.Lerp(6f, 4.5f, EasingFunction.InOutSine(Timer / 90f)) * MathHelper.Lerp(1f, 0.6f, Timer / 200f);
            drawer.scale *= 0.5f * inScale;
            if (Projectile.velocity.X < 0)
                drawer.spriteEffects = SpriteEffects.FlipHorizontally;
            sb.Draw(drawer);

            var d = drawer;
            d.color *= 0.6f;
            d.worldPosition -= Projectile.velocity.SafeNormalize(Vector2.Zero) * 16;
            d.scale.Y *= 0.7f;
            sb.Draw(d);
            drawer.TopCenterOrigin();
            drawer.scale.Y *= 0.4f;
            drawer.spriteEffects |= SpriteEffects.FlipVertically;
            sb.Draw(drawer);

            sb.RestartDefaults();

            Asset<Texture2D> bloomLine = AssetManager.GlowMask.SimpleGlowCircle;
            SpritebatchDrawer drawer2 = SpritebatchDrawer.FromTextureAsset(bloomLine, Projectile.Center);
            //      drawer2.BottomCenterOrigin();
            drawer2.scale *= new Vector2(0.55f, 0.05f) * ExtraMath.Osc(0.8f, 1f, speed: 3) * inScale;
            drawer2.color = Color.White * fade; ;
            drawer2.color.A = 0;
            sb.Draw(drawer2);
        }
    }
}
