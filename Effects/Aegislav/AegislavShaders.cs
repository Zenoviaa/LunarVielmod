using Newtonsoft.Json.Linq;
using ReLogic.Peripherals.RGB;
using Stellamod.Assets;
using Stellamod.Common.ConsoleMenu;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.Effects;
using Stellamod.Core.Rendering;
using Stellamod.Effects.RoyalMagic;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Effects.Aegislav;


[Autoload(Side = ModSide.Client)]
public class AegisCloudsRenderer : ModSystem
{
    private Vector2 _parallax;
    private Vector2 _lastCameraPos;
    private Vector2 _movementDiff;
    private RenderTargetProvider _rt = new RenderTargetProvider(() => RenderTargetParameters.DefaultScreenTarget with { Usage = RenderTargetUsage.PreserveContents });
    private RenderTargetProvider _rtSwap = new RenderTargetProvider(() => RenderTargetParameters.DefaultScreenTarget with { Usage = RenderTargetUsage.PreserveContents });
    private RenderTargetProvider _cloudsRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
    private int _lastRender;

    private RenderTarget2D OnScreen
    {
        get
        {
            return _lastRender == 0 ? _rtSwap : _rt;
        }
    }

    public Texture2D BackgroundTexture => _cloudsRT;
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderAegisClouds;
    }

    private Vector2 GetScreenOffset(float scale)
    {
        //Apply an offset so the texture doesn't move when you're moving
        //This will wrap inside the shader
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        Vector2 screenoffset = Main.screenPosition * texelSize;
        screenoffset *= (1f / scale);
        return screenoffset;
    }
    private void Parallax()
    {
        Vector2 parallaxAmt = new Vector2(0.5f, 0.5f);
        Vector2 refPosition = Main.Camera.UnscaledPosition;
        Vector2 diff = _lastCameraPos - refPosition;
        _parallax += diff * parallaxAmt;
        _movementDiff = diff * parallaxAmt;
        _lastCameraPos = refPosition;
    }
    private void RenderAegisClouds(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        if (!LunarDebugging.clouds && !Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneCrimsonBridewell)
            return;

        int steps = 1;
        if (Main.mouseLeft && LunarDebugging.clouds)
        {
            steps += 16;
        }

        var shader = AegisCurlingCloudsShader.Instance;
        shader.FirstFrame = 1;
        if (Main.mouseRight && LunarDebugging.clouds)
        {
            shader.FirstFrame = 0;
        }
        Parallax();

        SpriteBatch spriteBatch = Main.spriteBatch;
        shader.ConvectionTexture = AssetManager.LoadBackground("AegislavCloudConvection").Value;
        shader.Time = Main.GlobalTimeWrappedHourly * 4;
        shader.Res = new Vector2(Main.screenWidth, Main.screenHeight);
        shader.Parallax = -_movementDiff * 24;
        for (int i = 0; i < steps; i++)
        {
            var target = _lastRender == 0 ? _rtSwap : _rt;
            var draw = _lastRender == 0 ? _rt : _rtSwap;

            spriteBatch.GraphicsDevice.SetRenderTarget(target);

            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.AnisotropicClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                effect: shader.Effect);

            spriteBatch.Draw(draw, Vector2.Zero, Color.Lerp(Color.White, Color.Red, 0.8f));

            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
            _lastRender++;
            _lastRender %= 2;
        }
        spriteBatch.GraphicsDevice.SetRenderTarget(_cloudsRT);
        spriteBatch.GraphicsDevice.Clear(Color.Transparent);

        BackgroundParallaxShader parallaxShader = ShaderContent.GetInstance<BackgroundParallaxShader>();
        parallaxShader.Parallax = Main.Camera.Center * 0.00025f * new Vector2(0.33f, 0.18f) * 0.83f;

        Texture2D tex = AssetManager.LoadBackground("AegislavJail").Value;
        Texture2D texGlow = AssetManager.LoadBackground("AegislavJailGlow").Value;
        Rectangle dstRect = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            effect: parallaxShader.Effect);
        spriteBatch.Draw(tex, dstRect, Color.White);
        spriteBatch.End();


        Color outlineColor = Color.Lerp(Color.DarkBlue, Color.Black, 0.5f);
        Vector2 texelSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
        RoyalOutlineShader mixerShader2 = ShaderContent.GetInstance<RoyalOutlineShader>();
        mixerShader2.TexelSize = texelSize;
        mixerShader2.OutlineColor = outlineColor;

        Vector2 centerPos = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
        centerPos += _parallax * new Vector2(0.005f);
        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            effect: null);

        spriteBatch.Draw(OnScreen, Vector2.Zero, null, Color.White * 0.7f, 0, new Vector2(Main.screenWidth, Main.screenHeight) * 0f, 1, SpriteEffects.None, 0);
        Color glowColor = Color.White;
     
        spriteBatch.End();

        spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            effect: null);

      //  spriteBatch.Draw(OnScreen, Vector2.Zero, null, Color.White * 0.7f, 0, new Vector2(Main.screenWidth, Main.screenHeight) * 0f, 1, SpriteEffects.None, 0);


        spriteBatch.Draw(texGlow, dstRect, glowColor);
        spriteBatch.End();
        spriteBatch.GraphicsDevice.SetRenderTarget(null);
    }

    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        if (Main.gameMenu)
            return;
        if (!LunarDebugging.clouds)
            return;

        Main.spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            effect: null);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_cloudsRT, Main.screenPosition);
        drawer.drawOrigin = Vector2.Zero;
        drawer.color = Color.White;
        Main.spriteBatch.Draw(drawer);
        Main.spriteBatch.End();



        var starsTexture = TextureRegistry.StarNoise2;
        var noiseTexture = TextureRegistry.BlurryPerlinNoise2;
        MiscShaderData eff = GameShaders.Misc["LunarVeil:RoyalCapitalStars"];

        eff.Shader.Parameters["primaryTexture"].SetValue(starsTexture.Value);
        eff.Shader.Parameters["primaryTextureSize"].SetValue(starsTexture.Value.Size());
        eff.Shader.Parameters["resolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
        eff.Shader.Parameters["screenOffset"].SetValue(GetScreenOffset(scale: 1));
        eff.UseImage2(noiseTexture);
        eff.Shader.Parameters["parallax"].SetValue(-_parallax * 0.00005f);
        eff.Shader.Parameters["gradientFade"].SetValue(0f);
        eff.UseOpacity(1f);
        eff.Apply();

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, Main.Rasterizer, eff.Shader);
        Main.spriteBatch.Draw(starsTexture.Value,
           new Rectangle(0, 0, Main.screenWidth, Main.screenHeight),
            null, Color.White * 0.3f);
        Main.spriteBatch.End();

    }
}

public class AegisUndercloudsShader : CrystalShader<AegisUndercloudsShader>
{
    public Texture2D CloudDetailTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Effect.Parameters["detailSize"].SetValue(value.Size());
        }
    }
    public Vector2 Resolution
    {
        set
        {
            Effect.Parameters["resolution"].SetValue(value);
        }
    }
    public Vector2 SpriteSize
    {
        set
        {
            Effect.Parameters["spriteSize"].SetValue(value);
        }
    }
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float DistortionStrength
    {
        set
        {
            Effect.Parameters["distortionStrength"].SetValue(value);
        }
    }
}
public class AegisCurlingCloudsShader : CrystalShader<AegisCurlingCloudsShader>
{
    public Texture2D ConvectionTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
        }
    }
    public Texture2D MaskTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[2] = value;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.AnisotropicClamp;
        }
    }
    public Texture2D SwirlNormalTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[3] = value;
            Main.graphics.GraphicsDevice.SamplerStates[3] = SamplerState.AnisotropicWrap;
        }
    }

    public Vector2 Res
    {
        set
        {
            Effect.Parameters["res"].SetValue(value);
        }
    }
    public Vector2 Parallax
    {
        set
        {
            Effect.Parameters["cameraMovement"].SetValue(value);
        }
    }

    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }

    public float FirstFrame
    {
        set
        {
            Effect.Parameters["firstFrame"].SetValue(value);
        }
    }

}
