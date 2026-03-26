using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Special.DeadRomancesExcalibur;
using Stellamod.Trails;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common.Shaders;
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
    }
}

[Autoload(Side = ModSide.Client)]
public class GoldenAuroraEffectRenderer : ModSystem
{
    private float _auroraTimer;
    public override void Load()
    {
        base.Load();
        On_Main.Draw += DrawGoldenAuroraHook;
    }

    public override void Unload()
    {
        base.Unload();
        On_Main.Draw -= DrawGoldenAuroraHook;
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        if (Main.LocalPlayer.HasBuff<HeavenlyLove>())
            _auroraTimer++;
        else
            _auroraTimer--;
        _auroraTimer = MathHelper.Clamp(_auroraTimer, 0f, 60f);
    }
    private void DrawGoldenAuroraHook(On_Main.orig_Draw orig, Main self, GameTime gameTime)
    {
        orig(self, gameTime);
        if (Main.gameMenu)
            return;
        if (_auroraTimer <= 0)
            return;

        DrawGoldenAurora();
    }

    private void DrawGoldenAurora()
    {
        float alpha = _auroraTimer / 60f;
        SpriteBatch sb = Main.spriteBatch;
        GoldenAuroraShader shader = GoldenAuroraShader.Instance;
        shader.DistortionTexture = AssetManager.Noise.AuroraRays.Value;
        shader.DistortionAmt = 0.05f;
        shader.Tiling = Vector2.One * 1;
        Asset<Texture2D> textureAsset = AssetManager.Noise.Whirly; ;
        Vector2 drawOrigin = textureAsset.Size() * 0.5f;
        Rectangle drawRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
        Color drawColor = Color.Goldenrod;
        drawColor *= alpha * 0.7f;
        //shader.Time = 0;
        sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, shader.Effect);
        sb.Draw(textureAsset.Value, drawRect, drawColor);
        sb.End();
    }
}

public class GoldenAuroraShader : 
    CrystalShader<GoldenAuroraShader>
{
    private EffectParameter _distortionAmtParam;
    private EffectParameter _distortionTextureParam;
    private EffectParameter _timeParam;
    private EffectParameter _tilingParam;

    public Vector2 Tiling
    {
        set
        {
            _tilingParam ??= Effect.Parameters["Tiling"];
            _tilingParam.SetValue(value);
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

    public float DistortionAmt
    {
        set
        {
            _distortionAmtParam ??= Effect.Parameters["DistortionAmt"];
            _distortionAmtParam.SetValue(value);
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

    public override void SetDefaults()
    {
        base.SetDefaults();
        Time = Main.GlobalTimeWrappedHourly * 0.1f;
    }
}
