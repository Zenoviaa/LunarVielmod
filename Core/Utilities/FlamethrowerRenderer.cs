using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Rendering.RTs;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

public class FlamethrowerDistortionShader : CrystalShader<FlamethrowerDistortionShader>
{
    public Texture2D DistortionTexture
    {
        set
        {
            Main.graphics.GraphicsDevice.Textures[1] = value;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
        }
    }
    public float Distortion
    {
        set
        {
            Effect.Parameters["distortion"].SetValue(value);
        }
    }
}
public class FlamethrowerNoiseShader : CrystalShader<FlamethrowerNoiseShader>
{
    public float Time
    {
        set
        {
            Effect.Parameters["time"].SetValue(value);
        }
    }
}
public class FlamethrowerV2Shader : CrystalShader<FlamethrowerV2Shader>
{
    public Color InnerColor
    {
        set
        {
            Effect.Parameters["innerColor"].SetValue(value.ToVector3());
        }
    }

    public Color OuterColor
    {
        set
        {
            Effect.Parameters["outerColor"].SetValue(value.ToVector3());
        }
    }

    public float Threshold
    {
        set
        {
            Effect.Parameters["threshold"].SetValue(value);
        }
    }
}
public class FlamethrowerShader : CrystalShader<FlamethrowerShader>
{
    public Vector4[] Metaballs
    {
        set
        {
            Effect.Parameters["metaballs"].SetValue(value);
        }
    }

    public Color InnerColor
    {
        set
        {
            Effect.Parameters["innerColor"].SetValue(value.ToVector3());
        }
    }

    public Color OuterColor
    {
        set
        {
            Effect.Parameters["outerColor"].SetValue(value.ToVector3());
        }
    }
    public int Length
    {
        set
        {
            Effect.Parameters["length"].SetValue(value);
        }
    }

    public Vector2 ScreenResolution
    {
        set
        {
            Effect.Parameters["texelSize"].SetValue(value);
        }
    }
}

/// <summary>
/// Handles visuals for the flame thrower effect on Incinerator, Burning Glove, and related items
/// </summary>
[Autoload(Side = ModSide.Client)]
public class FlamethrowerRenderer : ModSystem
{
    private Vector4[] _metaballPositions;
    private List<Vector2> _metaballWorldPositions;
    private int _index;
    public override void Load()
    {
        base.Load();
        _metaballPositions = new Vector4[64];
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += RenderFull;
    }

    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= RenderFull;
    }


    private void RenderFull()
    {
        if (_index <= 0)
            return;

        PixelationManager.QueueSpritebatchDrawAction(DrawToScreen);
    }

    private void DrawToScreen(SpriteBatch sb, Vector2 sp)
    {
        sb.EndOut(out var oldParameters);
        RenderTargetHandle fireTarget = RenderTargets.ScreenTarget;
        RenderTargetHandle metaballTarget = RenderTargets.ScreenTarget;
        using (new RenderTargetContext(metaballTarget))
        {
            FlamethrowerShader fireShader = ShaderContent.GetInstance<FlamethrowerShader>();
            fireShader.InnerColor = Color.Yellow;
            fireShader.OuterColor = Color.Red;
            fireShader.Length = _index;
            fireShader.Metaballs = _metaballPositions;
            fireShader.ScreenResolution = new Vector2(Main.screenWidth, Main.screenHeight);
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, fireShader.Effect);
            sb.Draw(fireTarget, Vector2.Zero, Color.White);
            sb.End();
            _index = 0;
            _metaballWorldPositions.Clear();
        }

        using(new RenderTargetContext(fireTarget))
        {
            FlamethrowerNoiseShader noiseShader = ShaderContent.GetInstance<FlamethrowerNoiseShader>();
            noiseShader.Time = Main.GlobalTimeWrappedHourly * -16;
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, noiseShader.Effect);
            sb.Draw(AssetReferences.Assets.NoiseTextures.CloudNoise2.Asset.Value, Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            sb.End();
        }

        var shader = ShaderContent.GetInstance<FlamethrowerDistortionShader>();
        shader.Distortion = 0.009f;
        shader.DistortionTexture = fireTarget;
        sb.Begin(oldParameters with { effect = shader.Effect });
        Color additive = Color.White * 1f;
        sb.Draw(metaballTarget, Vector2.Zero, additive);
        sb.End();
        sb.Begin(oldParameters);
    }

    public static void AddMetaball(Vector2 pos, float time, float radius)
    {
        FlamethrowerRenderer renderer = ModContent.GetInstance<FlamethrowerRenderer>();
        renderer._metaballWorldPositions ??= new List<Vector2>();
        renderer._metaballWorldPositions.Add(pos);

        Matrix screenPosMatrix = TrailDrawer.WorldViewPoint2;

        Vector2 screenPos = new Vector2();
        screenPos.X = (pos.X - Main.screenPosition.X) / Main.screenWidth;
        screenPos.Y = (pos.Y - Main.screenPosition.Y) / Main.screenHeight;

        Vector4 metaballPos = new Vector4(screenPos, time, radius);
    
        if (renderer._index >= renderer._metaballPositions.Length)
            return;

        renderer._metaballPositions[renderer._index++] = metaballPos;
    }
}
