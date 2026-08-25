using Stellamod.Assets;
using Stellamod.Helpers;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public class PetalStorm : ScreenShader
{
    private EffectParameter _timeParam;
    private EffectParameter _petalTextureParam;
    private EffectParameter _distortingNoiseTextureParam;
    private EffectParameter _offsetParam;
    private EffectParameter _tilingParam;
    public override void ApplyEffect(ScreenShaderData screenShaderData)
    {
        base.ApplyEffect(screenShaderData);
        Effect effect = screenShaderData.Shader;
        _timeParam = effect.Parameters["time"];
        _timeParam.SetValue(Main.GlobalTimeWrappedHourly * 12);

        _petalTextureParam = effect.Parameters["petalTexture"];
        _petalTextureParam.SetValue(AssetManager.LaserTextures.PetalNoise.Value);


        _distortingNoiseTextureParam = effect.Parameters["distortingNoiseTexture"];
        _distortingNoiseTextureParam.SetValue(AssetManager.Noise.Whirly.Value);


        _offsetParam = effect.Parameters["offset"];
        _offsetParam.SetValue(Main.Camera.Center * 0.002f);


        _tilingParam = effect.Parameters["tiling"];
        _tilingParam.SetValue(new Vector2(1f, 16));
    }
}

public class WorldDepthGradient : ScreenShader
{
    private EffectParameter _gradientStrengthParam;
    private EffectParameter _gradientColorParam;
    public Vector3 gradientStrength;
    public Vector3 gradientColor;
    public override void ApplyEffect(ScreenShaderData screenShaderData)
    {
        base.ApplyEffect(screenShaderData);
        Effect effect = screenShaderData.Shader;
        _gradientStrengthParam = effect.Parameters["gradientStrength"];
        _gradientStrengthParam.SetValue(gradientStrength);

        _gradientColorParam = effect.Parameters["gradientColor"];
        _gradientColorParam.SetValue(gradientColor);
    }
}

public class Rippler : ScreenShader
{

    private Vector4[] _ripples;
    public Texture2D rippleTexture;
    public Vector4[] Ripples
    {
        get
        {
            if (_ripples == null)
                _ripples = new Vector4[8];
            return _ripples;
        }
    }
    public int rippleLength;
    public override void ApplyEffect(ScreenShaderData screenShaderData)
   
    {
        base.ApplyEffect(screenShaderData);
        Effect effect = screenShaderData.Shader;
        effect.Parameters["rippleTexture"].SetValue(rippleTexture);
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        effect.Parameters["texelSize"].SetValue(texelSize);
        effect.Parameters["ripples"].SetValue(Ripples);
        effect.Parameters["rippleLength"].SetValue(rippleLength);
    }

    public void PrepareShader(IList<Vector4> ripples)
    {
        for(int i = 0; i < Ripples.Length && i < ripples.Count; i++)
        {
            Ripples[i] = ripples[i];
        }
        rippleLength = ripples.Count;
    }
}

public class SuperShockwave : ScreenShader
{
    private EffectParameter _interpParam;
    private EffectParameter _epicenterParam;
    private EffectParameter _strengthParam;
    private EffectParameter _radiusParam;
    public float strength = 32f;
    public float radius;
    public Vector2 epicenter;
    public float interp;
    public override void ApplyEffect(ScreenShaderData screenShaderData)
    {
        base.ApplyEffect(screenShaderData);
        Effect effect = screenShaderData.Shader;
        _radiusParam ??= effect.Parameters["radius"];
        _strengthParam ??= effect.Parameters["strength"];
        _epicenterParam ??= effect.Parameters["epicenter"];
        _interpParam ??= effect.Parameters["interp"];

        _radiusParam.SetValue(radius);
        _strengthParam.SetValue(strength);
        _epicenterParam.SetValue(epicenter);
        _interpParam.SetValue(interp);

        effect.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
        screenShaderData.UseOpacity(strength);
    }
}

public class DarkSmear : ScreenShader
{
    private EffectParameter _maskTextureParam;
    private EffectParameter _texelSizeParam;
    public Texture2D maskTexture;
    public float strength = 32f;
    public override void ApplyEffect(ScreenShaderData screenShaderData)
    {
        base.ApplyEffect(screenShaderData);
        Effect effect = screenShaderData.Shader;
        _maskTextureParam ??= effect.Parameters["maskTexture"];
        _maskTextureParam.SetValue(maskTexture);

        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        _texelSizeParam ??= effect.Parameters["texelSize"];
        _texelSizeParam.SetValue(texelSize);
        //  effect.Parameters["uScreenResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
        screenShaderData.UseOpacity(strength);
    }
}

public class Invert : ScreenShader
{
    public override void ApplyEffect(ScreenShaderData screenShaderData)
    {
        base.ApplyEffect(screenShaderData);
        screenShaderData.UseOpacity(alpha);
    }
}

public class BlackSea : ScreenShader
{
    private EffectParameter _frequencyParam;
    private EffectParameter _amplitudeParam;
    private EffectParameter _levelsParam;
    private EffectParameter _timeParam;
    private EffectParameter _seatilingParam;
    private EffectParameter _seaNoiseTextureParam;
    private EffectParameter _seaThresholdParam;
    private EffectParameter _seaDarknessParam;
    private EffectParameter _ringPowerParam;
    private EffectParameter _ringColorParam;

    public float amplitude;
    public override void ApplyEffect(ScreenShaderData screenShaderData)
    {
        base.ApplyEffect(screenShaderData);
        float whiteBend = 0f;
        float blackBend = 0.8f;
        float lightThreshold = 0.6f;
        Vector3 paramsColor = new Vector3(whiteBend, blackBend, lightThreshold);
        screenShaderData.UseColor(paramsColor);
        Effect effect = screenShaderData.Shader;

        _frequencyParam ??= effect.Parameters["frequency"];
        _levelsParam ??= effect.Parameters["levels"];
        _amplitudeParam ??= effect.Parameters["amplitude"];
        _timeParam ??= effect.Parameters["time"];
        _seatilingParam ??= effect.Parameters["seaTiling"];
        _seaNoiseTextureParam ??= effect.Parameters["seaNoiseTexture"];
        _seaThresholdParam ??= effect.Parameters["seaThreshold"];
        _seaDarknessParam ??= effect.Parameters["seaDarkness"];
        _ringPowerParam ??= effect.Parameters["ringPower"];
        _ringColorParam ??= effect.Parameters["ringColor"];


        _frequencyParam.SetValue(0.25f);
        _levelsParam.SetValue(64);
        _amplitudeParam.SetValue(amplitude);
        _timeParam.SetValue(Main.GlobalTimeWrappedHourly * 8);
        _seatilingParam.SetValue(new Vector2(1, 8f));
        _seaNoiseTextureParam.SetValue(AssetRegistry.NoiseTextures.IceWaterCaustics.Value);
        _seaThresholdParam.SetValue(0.05f);
        _seaDarknessParam.SetValue(0.96f);
        _ringPowerParam.SetValue(12);

        float b = 0.004f;
        _ringColorParam.SetValue(new Vector3(b, b, b));
    }
}

public class DomainExpansion : ScreenShader
{
    private EffectParameter _radiusParam;
    private EffectParameter _epicenterParam;

    public float radius;
    public Vector2 epicenter;

    public override void ApplyEffect(ScreenShaderData screenShaderData)
    {
        base.ApplyEffect(screenShaderData);
        var effect = screenShaderData.Shader;
        //Cache these paramers just so it's a little more performance
        _radiusParam ??= effect.Parameters["radius"];
        _epicenterParam ??= effect.Parameters["epicenter"];

        _radiusParam.SetValue(radius);
        _epicenterParam.SetValue(epicenter);
    }
}

public abstract class ScreenShader : ModType
{
    //Alpha will be subtracted every frame to dictate if the shader should be active
    //So if something uses it and sets the alpha to 1 yeah
    public float alpha;
    public string EffectFileName => this.GetType().Name;
    public string ShaderName => $"LunarVeil:{EffectFileName}";
    public sealed override void SetupContent()
    {
        base.SetupContent();
        SetStaticDefaults();
    }

    protected override void Register()
    {
        ModTypeLookup<ScreenShader>.Register(this);
    }

    public ScreenShaderData GetScreenShaderData()
    {
        return ShaderHelpers.FilterManager[ShaderName].GetShader();
    }

    public void UpdateEffect()
    {
        ScreenShaderData screenShaderData = GetScreenShaderData();
        ApplyEffect(screenShaderData);
    }
    public void ManageScreenShader(Player player)
    {
        string name = ShaderName;
        bool isActive = IsActive(player);

        if (isActive)
        {
            FilterManager filterManager = ShaderHelpers.FilterManager;
            if (!filterManager[name].IsActive())
            {
                filterManager.Activate(name);
            }
        }
        else if (!isActive)
        {
            FilterManager filterManager = ShaderHelpers.FilterManager;
            if (filterManager[name].IsActive())
            {
                filterManager.Deactivate(name);
            }
        }
    }

    public virtual void ApplyEffect(ScreenShaderData screenShaderData)
    {

    }

    public virtual bool IsActive(Player player)
    {
        return alpha > 0;
    }


    public static T GetInstance<T>() where T : ScreenShader
    {
        return ModContent.GetInstance<T>();
    }
}

[Autoload(Side = ModSide.Client)]
public class ScreenShaderManager : ModSystem
{
    private ScreenShader[] _screenShaders;
    public override void OnModLoad()
    {
        base.OnModLoad();
        _screenShaders = ModContent.GetContent<ScreenShader>().ToArray();
    }
    public override void PreUpdateNPCs()
    {
        base.PreUpdateNPCs();
        for (int i = 0; i < _screenShaders.Length; i++)
        {
            ScreenShader screenShader = _screenShaders[i];
            if (screenShader.alpha > 0)
            {
                screenShader.UpdateEffect();

                screenShader.alpha -= 0.02f;
            }
            screenShader.ManageScreenShader(Main.LocalPlayer);
        }
    }
}

public static class ShaderHelpers
{
    public static FilterManager FilterManager => Filters.Scene;
    public static void ManageScreenShader(string name, bool isActive)
    {
        if (!ShaderRegistry.ScreenShaders.Contains(name))
            return;

        if (isActive)
        {
            if (!FilterManager[name].IsActive())
            {
                FilterManager.Activate(name);
            }
        }
        else if (!isActive)
        {
            if (FilterManager[name].IsActive())
            {
                FilterManager.Deactivate(name);
            }
        }
    }
}
