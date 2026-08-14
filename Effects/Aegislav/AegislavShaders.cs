using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Effects;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Effects.Aegislav;


[Autoload(Side = ModSide.Client)]
public class AegisCloudsRenderer : ModSystem
{
    private ManagedRenderTarget _rt;
    private ManagedRenderTarget _rtSwap;
    private int _lastRender;
    private bool _frameOne;

    private RenderTarget2D OnScreen
    {
        get
        {
            return _lastRender == 0 ? _rtSwap : _rt;
        }
    }
    public override void Load()
    {
        base.Load();
        On_Main.CheckMonoliths += RenderAegisClouds;
        _rt = ManagedRenderTarget.New(preserve: RenderTargetUsage.PreserveContents);
        _rtSwap = ManagedRenderTarget.New(preserve: RenderTargetUsage.PreserveContents);
    }
    private void RenderAegisClouds(On_Main.orig_CheckMonoliths orig)
    {
        orig();
        if (Main.gameMenu)
            return;
        return;

        int steps = 1;
        if (Main.mouseLeft)
        {
            steps += 16;
        }
        for(int i = 0; i < steps; i++)
        {
            var target = _lastRender == 0 ? _rtSwap : _rt;
            var draw = _lastRender == 0 ? _rt : _rtSwap;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.GraphicsDevice.SetRenderTarget(target);

            var shader = AegisCurlingCloudsShader.Instance;
            shader.FirstFrame = 1;
            if (Main.mouseRight)
            {
                shader.FirstFrame = 0;
            }

            shader.ConvectionTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/NormalNoise1").Value;
            shader.Time = Main.GlobalTimeWrappedHourly * 4;
            shader.Res = new Vector2(Main.screenWidth, Main.screenHeight);
            spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.AnisotropicClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                effect: shader.Effect);

            spriteBatch.Draw(draw, Vector2.Zero, Color.White);

            spriteBatch.End();

            spriteBatch.GraphicsDevice.SetRenderTarget(null);
            _lastRender++;
            _lastRender %= 2;
        }

    }

    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        if (Main.gameMenu)
            return;
        return;
        Main.spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp,
            DepthStencilState.None,
            RasterizerState.CullNone,
            effect: null);
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(OnScreen, Main.screenPosition);
        drawer.drawOrigin = Vector2.Zero;
        Main.spriteBatch.Draw(drawer);
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

    public Vector2 Res
    {
        set
        {
            Effect.Parameters["res"].SetValue(value);
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
