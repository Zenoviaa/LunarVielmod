using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders;

[Autoload(Side = ModSide.Client)]
public class AuroraEffectRenderer : ModSystem
{
    private float _activeTimer;
    private ManagedRenderTarget _auroraRT;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _auroraRT = ManagedRenderTarget.New(downSamples: 2);
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
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred,
                      BlendState.AlphaBlend,
                      SamplerState.PointWrap,
                      DepthStencilState.None,
                      RasterizerState.CullCounterClockwise,
                      null);
                Color rayColor = Color.White;
                spriteBatch.Draw(_auroraRT, new Vector2(0, -64), null, rayColor, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);

            }


        
        }

        orig(self, spriteBatch, layer, beginSpriteBatch);
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (Main.LocalPlayer.ZoneSnow && !Main.dayTime)
        {
            _activeTimer++;
            if (Main.rand.NextBool(5))
            {
                float xRand = Main.rand.NextFloat(-1000, 1000);
                float yRand = Main.rand.NextFloat(-1000, 1000);
                LegacyParticle.NewParticle<StarParticle>(Main.LocalPlayer.Center + new Vector2(xRand, yRand), Vector2.Zero);
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
        skyGradientShader.MidColor = Color.Lerp(Color.Transparent, Color.Blue * 0.83f, ease);
        skyGradientShader.EndColor = Color.Transparent;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone,
            skyGradientShader.Effect);


        Rectangle targetRectangle = new Rectangle(0, 0, _auroraRT.Width, _auroraRT.Height);
        spriteBatch.Draw(AssetManager.GlowMask.EmptyGradient.Value, targetRectangle, Color.White * ease * 0.02f);
        spriteBatch.End();

        AuroraShader auroraShader = AuroraShader.Instance;
        auroraShader.Parallax = new Vector2(Main.screenPosition.X, 0) * (Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight));
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone,
            auroraShader.Effect);



 
        Rectangle dstRect = new Rectangle(-8, 0, (int)(_auroraRT.Width * 1.5f), (int)(_auroraRT.Height * 0.35f));

        Texture2D texture = AssetManager.Noise.AuroraRays.Value;
        Rectangle srcRect = new Rectangle(256, 0, texture.Width, texture.Height);
        Rectangle srcRect2 = new Rectangle(127, 0, texture.Width, texture.Height);
        Color rayColor = Color.White;
        rayColor.A = 0;

        Vector2 origin = new Vector2(texture.Width, texture.Height) * 0.5f;
        float rotation = MathHelper.ToRadians(5);
        float backRotation = MathHelper.ToRadians(-2);
        spriteBatch.Draw(texture, dstRect, srcRect, rayColor * 1 * ease, rotation, Vector2.Zero, SpriteEffects.None, 0);
        //    spriteBatch.Draw(texture, dstRect, srcRect2, rayColor * 0.3f, rotation, Vector2.Zero, SpriteEffects.None, 0);

        Rectangle dstRect2 = new Rectangle(-8, 0, (int)(_auroraRT.Width * 1.5f), (int)(_auroraRT.Height * 0.25f));
        spriteBatch.Draw(texture, dstRect2, srcRect, rayColor * 0.5f * ease, backRotation, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
        spriteBatch.Draw(texture, dstRect2, srcRect, rayColor * 0.5f * ease, -backRotation, Vector2.Zero, SpriteEffects.FlipVertically, 0);

        Color rayColorGlow = Color.Blue;
        rayColorGlow.A = 0;
        Rectangle dstRect3 = new Rectangle(-8, 0, (int)(_auroraRT.Width * 2f), (int)(_auroraRT.Height * 0.4f));
        spriteBatch.Draw(texture, dstRect3, srcRect, rayColorGlow * 1 * ease, rotation, Vector2.Zero, SpriteEffects.None, 0);

        //    spriteBatch.Draw(texture, dstRect2, srcRect2, rayColor * 0.3f, backRotation, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);

        Color rayColorGlow2 = Color.Green;
        rayColorGlow2.A = 0;
        Rectangle dstRect4 = new Rectangle(-8, -64, (int)(_auroraRT.Width * 2f), (int)(_auroraRT.Height * 0.4f));
        spriteBatch.Draw(texture, dstRect4, srcRect, rayColorGlow2 * 0.5f * ease, rotation, Vector2.Zero, SpriteEffects.None, 0);
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
        Waviness = 1.2f;
        //GradientTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/AuroraGradient").Value;
        //RTSize = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
    }
}
