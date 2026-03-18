using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders;


[Autoload(Side = ModSide.Client)]
public class AuroraShootingStarSystem : ModSystem
{
    public struct AuroraStar
    {
        public Vector2 position;
        public Vector2 velocity;
        public float time;
        public bool active;
    }

    private int _lastIndex;
    private float _activeTimer;
    private float _particleSpawnTimer;
    private float[] _alphaValues;
    private AuroraStar[] _starParticles;

    public override void Load()
    {
        base.Load();
        _alphaValues = new float[120];
        CalculateAlphaValues();
        _starParticles = new AuroraStar[100];
        On_OverlayManager.Draw += DrawAuroraStars;
    }

    private void CalculateAlphaValues()
    {
        for (int i = 0; i < _alphaValues.Length; i++)
        {
            float tick = i + 1;
            float inAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.InOutSine(tick / 30f));
            float outAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.InOutSine((tick-90) / 30f));
            _alphaValues[i] = inAlpha * outAlpha;
        }
    }

    public override void Unload()
    {
        base.Unload();
        On_OverlayManager.Draw -= DrawAuroraStars;
    }

    public void SpawnStarParticle(AuroraStar star)
    {
        for (int i = 0; i < _starParticles.Length; i++)
        {
            _lastIndex++;
            _lastIndex %= _starParticles.Length;
            ref AuroraStar starParticle = ref _starParticles[_lastIndex];
            if (!starParticle.active)
            {
                starParticle = star;
                starParticle.active = true;
                break;
            }
        }
    }

    private void UpdateParticles()
    {
     
        float starTime = 120;
        Vector2 cameraMovement = Main.screenPosition - Main.screenLastPosition;
        for (int i = 0; i < _starParticles.Length; i++)
        {
            ref AuroraStar starParticle = ref _starParticles[i];
            if (!starParticle.active)
            {
                continue;
            }

            starParticle.position += starParticle.velocity;
            Vector2 parallax = cameraMovement * 0.9f;
            parallax *= starParticle.velocity.Length() * 0.2f;
            starParticle.position -= parallax;
            starParticle.time++;
            if (starParticle.time >= starTime)
            {
                starParticle.active = false;
            }
        }
    }
    private void DrawAuroraStars(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
    {
        if (layer == RenderLayers.Background)
        {
            if (!Main.gameMenu && _activeTimer > 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred,
                      BlendState.AlphaBlend,
                      SamplerState.PointWrap,
                      DepthStencilState.None,
                      RasterizerState.CullCounterClockwise,
                      null);

                //Render all particles
                RenderStarParticles(spriteBatch);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            }
        }

        orig(self, spriteBatch, layer, beginSpriteBatch);
    }

    private void RenderStarParticles(SpriteBatch spriteBatch)
    {
        Asset<Texture2D> glint = AssetManager.GlowMask.ShootingStarGlint;
        //Draw glint
        Vector2 glintDrawOrigin = glint.Size() * 0.5f;

        Asset<Texture2D> trail = AssetManager.GlowMask.ShootingStarTrail;
        Vector2 trailDrawOrigin = new Vector2(trail.Width(), trail.Height() * 0.5f);
        for (int i = 0; i < _starParticles.Length; i++)
        {
            ref AuroraStar starParticle = ref _starParticles[i];
            if (!starParticle.active)
            {
                continue;
            }

            int alphaIndex = (int)starParticle.time;
            if (alphaIndex >= 120)
                alphaIndex = 120 - 1;
            float alpha = _alphaValues[alphaIndex];
            Color trailDrawColor = Color.White;
            trailDrawColor *= alpha;
            trailDrawColor.A = 0;
            spriteBatch.Draw(trail.Value, starParticle.position, null, trailDrawColor, starParticle.velocity.ToRotation(), trailDrawOrigin, Vector2.One, SpriteEffects.None, 0);

            Color glintDrawColor = Color.White;
            glintDrawColor *= alpha;
            glintDrawColor *= ExtraMath.Osc(0.5f, 1f, speed: 16, offset: i);
            glintDrawColor.A = 0;
            Vector2 glintDrawScale = Vector2.One * 1f;
            spriteBatch.Draw(glint.Value, starParticle.position, null, glintDrawColor, 0, glintDrawOrigin, glintDrawScale, SpriteEffects.None, 0);
        }
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        UpdateParticles();
        if (Main.LocalPlayer.ZoneSnow && !Main.dayTime && Main.LocalPlayer.ZoneOverworldHeight)
        {
            _activeTimer++;
        }
        else
        {
            _activeTimer--;
        }

        _activeTimer = Math.Clamp(_activeTimer, 0f, 60f);
        if (_activeTimer <= 0)
            return;

        _particleSpawnTimer++;
        if (_particleSpawnTimer % 5 == 0)
        {
            Rectangle spawnRectangle = new Rectangle(0, 0, Main.screenWidth, (int)(Main.screenHeight * 0.5f));
            float randX = Main.rand.NextFloat();
            float randY = Main.rand.NextFloat();
            float x = MathHelper.Lerp(spawnRectangle.Left, spawnRectangle.Right, randX);
            float y = MathHelper.Lerp(spawnRectangle.Top, spawnRectangle.Bottom, randY);
            Vector2 spawnPosition = new Vector2(x, y);
            Vector2 spawnVelocity = new Vector2(-1, 1);
            spawnVelocity = spawnVelocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 5f);
            AuroraStar auroraStar = new AuroraStar
            {
                position = spawnPosition,
                velocity = spawnVelocity,
            };
            SpawnStarParticle(auroraStar);
        }
    }
}

[Autoload(Side = ModSide.Client)]
public class AuroraEffectRenderer : ModSystem
{
    private float _activeTimer;
    private ManagedRenderTarget _auroraRT;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _auroraRT = ManagedRenderTarget.New(downSamples: 4);
    }

    public override void Load()
    {
        base.Load();
        On_OverlayManager.Draw += DrawAurora;
        On_Main.CheckMonoliths += RenderToAuroraRT;
    }

    private void DrawAurora(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
    {
        if (layer == RenderLayers.Background)
        {
            if (!Main.gameMenu && _activeTimer > 0)
            {
                float opacity = _activeTimer / 120f;
            
                var starsTexture = TextureRegistry.StarNoise2;
                var noiseTexture = TextureRegistry.BlurryPerlinNoise2;
                MiscShaderData eff = GameShaders.Misc["LunarVeil:RoyalCapitalStars"];

                eff.Shader.Parameters["primaryTexture"].SetValue(starsTexture.Value);
                eff.Shader.Parameters["primaryTextureSize"].SetValue(starsTexture.Value.Size());
                eff.Shader.Parameters["resolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
                eff.UseImage2(noiseTexture);
                eff.Shader.Parameters["uDirection"].SetValue(Main.screenPosition * 0.0001f);
                Vector2 parallax = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
                eff.Shader.Parameters["uImageOffset"].SetValue(parallax);
                eff.UseOpacity(opacity);
                eff.Apply();

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, eff.Shader, Main.BackgroundViewMatrix.TransformationMatrix);
                spriteBatch.Draw(starsTexture.Value,
                   new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
                    null, Color.White * 0.3f);


                /*
                spriteBatch.Draw(starsTexture.Value, 
                    new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), 
                    new Rectangle((int)-_parallax.X, (int)-_parallax.Y, Main.screenWidth, Main.screenHeight), Color.White * 0.3f);
                */
                spriteBatch.End();

          //      spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred,
                      BlendState.AlphaBlend,
                      SamplerState.PointWrap,
                      DepthStencilState.None,
                      RasterizerState.CullCounterClockwise,
                      null);
                Color rayColor = Color.White;
                spriteBatch.Draw(_auroraRT, new Vector2(0, -256), null, rayColor, 0, Vector2.Zero, 4, SpriteEffects.None, 0);
                rayColor *= 0.5f;
                rayColor.A = 0;
                spriteBatch.Draw(_auroraRT, new Vector2(0, -256), null, rayColor, 0, Vector2.Zero, 4, SpriteEffects.None, 0);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            }



        }

        orig(self, spriteBatch, layer, beginSpriteBatch);
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (Main.LocalPlayer.ZoneSnow && !Main.dayTime && Main.LocalPlayer.ZoneOverworldHeight)
        {
            _activeTimer++;
            if (Main.rand.NextBool(5))
            {
                float xRand = Main.rand.NextFloat(-1000, 1000);
                float yRand = Main.rand.NextFloat(-1000, 1000);
                LegacyParticle.NewParticle<StarParticle>(Main.LocalPlayer.Center + new Vector2(xRand, yRand), Vector2.Zero);
            }
            if (Main.rand.NextBool(9))
            {
                Vector2 startPosition = Main.LocalPlayer.Top;
                startPosition.Y -= 1000;
                startPosition.X += Main.rand.NextFloat(-1000, 1000);
                Point tilePosition = startPosition.ToTileCoordinates();
                for(int i = 0; i < 200; i++)
                {
                    if (!WorldGen.InWorld(tilePosition.X, tilePosition.Y) || !WorldGen.SolidTile(tilePosition))
                    {
                        tilePosition.Y += 1;
                    }

                    else
                    {
                        break;
                    }
                }

                Vector2 spawnPoint = tilePosition.ToWorldCoordinates();
                float scale = Main.rand.NextFloat(0.2f, 0.3f);
                Vector2 spawnVelocity = -Vector2.UnitY * 1;
                SparkleParticle sp = SparkleParticle.Spawn(spawnPoint, spawnVelocity, Color.White, Scale: scale);
                sp.innerColor = Color.White;
                sp.outerColor = Color.Blue;
                sp.gravity = 0;
            }
        }
        else
        {
            _activeTimer--;
        }

        _activeTimer = Math.Clamp(_activeTimer, 0f, 60f);


    }


    public override void Unload()
    {
        base.Unload();
        On_OverlayManager.Draw -= DrawAurora;
        On_Main.CheckMonoliths -= RenderToAuroraRT;
        _auroraRT = null;
    }

    private void RenderToAuroraRT(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        if (_activeTimer <= 0)
            return;

        float ease = EasingFunction.InOutSine(_activeTimer / 60f);
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.SetRenderTarget(_auroraRT);
        graphicsDevice.Clear(Color.Transparent);


        SkyGradientShader skyGradientShader = SkyGradientShader.Instance;
        skyGradientShader.H = 0;
        skyGradientShader.Bend = -0.24f;
        skyGradientShader.StartColor = Color.Transparent;
        skyGradientShader.MidColor = Color.Lerp(Color.Transparent, Color.Blue * 0.5f, ease);
        skyGradientShader.EndColor = Color.Transparent;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone,
            skyGradientShader.Effect);


        Rectangle targetRectangle = new Rectangle(0, 0, _auroraRT.Width, _auroraRT.Height);
        spriteBatch.Draw(AssetManager.GlowMask.EmptyGradient.Value, targetRectangle, Color.White * ease * 0.02f);
        spriteBatch.End();

        AuroraShader auroraShader = AuroraShader.Instance;
        auroraShader.Parallax = new Vector2(Main.screenPosition.X, 0) * (Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight)) * 0.25f;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone,
            auroraShader.Effect);




        Rectangle dstRect = new Rectangle(-8, 32, (int)(_auroraRT.Width * 1.5f), (int)(_auroraRT.Height * 0.5f));

        Texture2D texture = AssetManager.Noise.AuroraRays.Value;
        Rectangle srcRect = new Rectangle(256, 0, texture.Width, texture.Height);
        Rectangle srcRect2 = new Rectangle(127, 0, texture.Width, texture.Height);
        Color rayColor = Color.White;
        rayColor.A = 0;

        ease *= ExtraMath.Osc(0.75f, 1f, speed: 0.4f);
        Vector2 origin = new Vector2(texture.Width, texture.Height) * 0.5f;
        float rotation = MathHelper.ToRadians(5);
        float backRotation = MathHelper.ToRadians(-2);
        spriteBatch.Draw(texture, dstRect, srcRect, rayColor * 0.8f * ease, rotation, Vector2.Zero, SpriteEffects.None, 0);
        //    spriteBatch.Draw(texture, dstRect, srcRect2, rayColor * 0.3f, rotation, Vector2.Zero, SpriteEffects.None, 0);

        Rectangle dstRect2 = new Rectangle(-8, 0, (int)(_auroraRT.Width * 1.5f), (int)(_auroraRT.Height * 0.25f));
        spriteBatch.Draw(texture, dstRect2, srcRect, rayColor * 0.125f * ease * 0.5f, backRotation, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
        spriteBatch.Draw(texture, dstRect2, srcRect, rayColor * 0.125f * ease * 0.5f, -backRotation, Vector2.Zero, SpriteEffects.FlipVertically, 0);

        Color rayColorGlow = Color.White;
        rayColorGlow.A = 0;
        Rectangle dstRect3 = new Rectangle(-8, 0, (int)(_auroraRT.Width * 2f), (int)(_auroraRT.Height * 0.4f));
        spriteBatch.Draw(texture, dstRect3, srcRect, rayColorGlow * 0.125f * ease * 0.5f, rotation, Vector2.Zero, SpriteEffects.None, 0);

        //    spriteBatch.Draw(texture, dstRect2, srcRect2, rayColor * 0.3f, backRotation, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);

        Color rayColorGlow2 = Color.White;
        rayColorGlow2.A = 0;
        Rectangle dstRect4 = new Rectangle(-8, 0, (int)(_auroraRT.Width * 2f), (int)(_auroraRT.Height * 0.4f));
        spriteBatch.Draw(texture, dstRect4, srcRect, rayColorGlow2 * 0.35f * ease * 0.75f, rotation, Vector2.Zero, SpriteEffects.None, 0);

        Color rayColorGlow3 = Color.White;
        rayColorGlow3.A = 0;
        spriteBatch.Draw(texture, dstRect4, srcRect, rayColorGlow3 * 0.4f * ease * 0.75f, rotation, Vector2.Zero, SpriteEffects.None, 0);
        spriteBatch.End();


    }
}
public class AuroraShader : BaseShader
{
    private static AuroraShader _instance;
    public static AuroraShader Instance
    {
        get
        {
            _instance ??= new AuroraShader();
            _instance.SetDefaults();
            return _instance;
        }
    }

    private EffectParameter _gradientTextureParam;
    private EffectParameter _gradientBackTextureParam;
    private EffectParameter _distortionTextureParam;
    private EffectParameter _timeParam;
    private EffectParameter _wavinessParam;
    private EffectParameter _rtSizeParam;
    private EffectParameter _parallaxParam;
    public Texture2D GradientTexture
    {
        set
        {
            _gradientTextureParam ??= Effect.Parameters["GradientTexture"];
            _gradientTextureParam.SetValue(value);
        }
    }
    public Texture2D GradientBackTexture
    {
        set
        {
            _gradientBackTextureParam ??= Effect.Parameters["GradientBackTexture"];
            _gradientBackTextureParam.SetValue(value);
        }
    }


    public Texture2D DistortionTexture
    {
        set
        {
            _distortionTextureParam ??= Effect.Parameters["DistortionTexture"];
            _distortionTextureParam.SetValue(value);
        }
    }

    public float Time
    {
        set
        {
            _timeParam ??= Effect.Parameters["Time"];
            _timeParam.SetValue(value);
        }
    }
    public float Waviness
    {
        set
        {
            _wavinessParam ??= Effect.Parameters["Waviness"];
            _wavinessParam.SetValue(value);
        }
    }

    public Vector2 RTSize
    {
        set
        {
            _rtSizeParam ??= Effect.Parameters["RTSize"];
            _rtSizeParam.SetValue(value);
        }
    }
    public Vector2 Parallax
    {
        set
        {
            _parallaxParam ??= Effect.Parameters["Parallax"];
            _parallaxParam.SetValue(value);
        }
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Time = Main.GlobalTimeWrappedHourly * 0.1f;
        Waviness = 2.2f;
        GradientTexture = AssetManager.GlowMask.AuroraGradient.Value;
        GradientBackTexture = AssetManager.GlowMask.AuroraBackGradient.Value;
        //GradientTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/AuroraGradient").Value;
        //RTSize = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
    }
}
