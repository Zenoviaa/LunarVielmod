using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.BossesCL.EliteCommander.Projectiles
{
    public class WindShockwave : ModProjectile
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
            Projectile.width = 64;
            Projectile.height = 32;
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
            if(Timer % 8 == 0)
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

            Projectile.velocity *= 1.01f;
            Point tp = (Projectile.Center+new Vector2(0, -8)).ToTileCoordinates();
            Tile tile = Main.tile[tp];
            if (tile.HasTile && Main.tileSolid[tile.TileType] && DeathTimer == 0)
            {
                DeathTimer++;
               // Projectile.Kill();
            }

            if(DeathTimer > 0)
            {
                DeathTimer++;
                if (DeathTimer >= FadeTime)
                    Projectile.Kill();
            }
        }

        private Color GetTrailColor(float progressOnTrail)
        {
            return Color.Lerp(Color.White, Color.Transparent, progressOnTrail);
        }
        private float GetTrailWidth(float progressOnTrail)
        {
            return MathHelper.SmoothStep(64, 0f, progressOnTrail);
        }
        private void DrawPixelatedShockwave(GraphicsDevice graphicsDevice)
        {        //Draw Trail
            _shockwavePos ??= new Vector2[Projectile.oldPos.Length];

            var shader = BasicLaserShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.DarkGray;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 oldPos = Projectile.oldPos[i];
                List<Vector2> shockwavePos = new List<Vector2>();
                float totalP = (float)i / (float)Projectile.oldPos.Length;
                totalP = 1f - totalP;

                float numPoints = 4f;
                for (int s = 0; s < numPoints; s++)
                {
                    float p = (float)s / numPoints;
                    Vector2 pos = Vector2.Lerp(oldPos, oldPos - Vector2.UnitY * 80 * totalP *
                        VectorHelper.Osc(0.5f, 1f, speed: 12, offset: i * 4) * MathHelper.Clamp(Timer / 30f, 0f, 1f), p);
                    //
                    shockwavePos.Add(pos);
                }
                Vector2[] shockPos = shockwavePos.ToArray();
                Vector2 trailOffset = Projectile.Size / 2;
                TrailDrawer.Draw(Main.spriteBatch, shockPos, GetTrailColor, GetTrailWidth, shader, trailOffset);
            }


        }

        private void DrawPixelatedShockwaveV2(SpriteBatch sb, Vector2 screenPos)
        {
            float fade = MathHelper.Lerp(1f, 0f, DeathTimer / FadeTime);
            float inScale = EasingFunction.OutExpo(Timer / 30f);
            Asset<Texture2D> waveTexture = AssetManager.GlowMask.Wave;
            WaveShader waveShader = ShaderContent.GetInstance<WaveShader>();
            waveShader.Time = Main.GlobalTimeWrappedHourly * 0.5f;
            waveShader.Amplitude = 0.1f;
            waveShader.Frequency = 12;
            waveShader.XStrength = 12;
            waveShader.NoiseTexture = AssetManager.Noise.Whirly.Value;
            sb.Restart(effect: waveShader.Effect);
            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(waveTexture, Projectile.Center);
            drawer.BottomCenterOrigin();
            drawer.color = Color.White * fade;
            drawer.color.A = 0;
            drawer.scale *= 0.5f * inScale;
            if (Projectile.velocity.X < 0)
                drawer.spriteEffects = SpriteEffects.FlipHorizontally;
            sb.Draw(drawer);
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

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedShockwaveV2, DrawLayer.OverPlayers);
            //PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedShockwave);

            return false;
        }
    }
}
