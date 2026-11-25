using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Palettes;

public class SpecialEffectsPlayer : ModPlayer
{
    private float _blurLerp;

    private float _blackWhiteLerp;

    private FilterManager FilterManager => Filters.Scene;



    public bool hasSpiritPendant;
    public bool hasSunGlyph;
    public float darkness;

    public float darknessCurve;
    public float whiteCurve;
    public float blackCurve;
    public float blurStrength;


    public float blackWhiteStrength;
    public float blackWhiteThreshold;

    public override void ResetEffects()
    {
        base.ResetEffects();
        hasSpiritPendant = false;
        hasSunGlyph = false;
        darkness = 0;
        darknessCurve = MathHelper.Lerp(darknessCurve, 0f, 0.005f);
        if (darknessCurve < 0.05f)
            darknessCurve = 0f;

        whiteCurve = 0f;
        blackCurve = 1f;
    }

    private void ToggleScreenShader(string name, bool isActive)
    {
        if (!ShaderRegistry.ScreenShaders.Contains(name))
            return;
        LunarVeilClientConfig clientConfig = ModContent.GetInstance<LunarVeilClientConfig>();
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

    private void SpecialBiomeEffects()
    {
        //This code should only run on each client
        if (Main.netMode == NetmodeID.Server)
            return;
        if (Main.myPlayer != Player.whoAmI)
            return;

        LunarVeilClientConfig clientConfig = LunarVeilClientConfig.Instance;
        ScreenShaderData screenShaderData;

        screenShaderData = FilterManager["LunarVeil:DarknessCurve"].GetShader();
        screenShaderData.UseProgress(darknessCurve);
        screenShaderData.Shader.Parameters["blackCurve"].SetValue(blackCurve);
        screenShaderData.Shader.Parameters["whiteCurve"].SetValue(whiteCurve);
        ToggleScreenShader("LunarVeil:DarknessCurve", darknessCurve != 0);

        blurStrength -= 0.05f;
        if (blurStrength <= 0f)
        {
            blurStrength = 0f;
        }
        bool blurActive = blurStrength != 0;
        if (blurActive)
        {
            _blurLerp = MathHelper.Lerp(_blurLerp, 1f, 0.1f);
        }
        else
        {
            _blurLerp = MathHelper.Lerp(_blurLerp, 0f, 0.1f);
        }

        screenShaderData = FilterManager["LunarVeil:Blur"].GetShader();
        screenShaderData.UseProgress(blurStrength * _blurLerp);
        ToggleScreenShader("LunarVeil:Blur", blurActive);

        bool blackWhiteActive = blackWhiteStrength != 0;
        if (blackWhiteActive)
        {
            _blackWhiteLerp += 0.1f;
            if (_blackWhiteLerp >= 1f)
            {
                _blackWhiteLerp = 1f;
            }
        }
        else
        {
            _blackWhiteLerp -= 0.1f;
            if (_blackWhiteLerp <= 0)
            {
                _blackWhiteLerp = 0f;
            }
        }
        blackWhiteStrength -= 0.05f;
        if (blackWhiteStrength <= 0f)
        {
            blackWhiteStrength = 0f;
        }

        float strength = MathHelper.Lerp(0, blackWhiteStrength, _blackWhiteLerp);

        screenShaderData = FilterManager["LunarVeil:BlackWhite"].GetShader();
        screenShaderData.Shader.Parameters["strength"].SetValue(strength);
        screenShaderData.Shader.Parameters["brightnessThreshold"].SetValue(blackWhiteThreshold);
        ToggleScreenShader("LunarVeil:BlackWhite", _blackWhiteLerp != 0);
    }


    public override void PostUpdate()
    {
        base.PostUpdate();
        SpecialBiomeEffects();
    }
}
