using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
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

[Autoload(Side = ModSide.Client)]
public class FlamethrowerRenderer : ModSystem
{
    private ManagedRenderTarget _metaballTarget;
    private ManagedRenderTarget _fireTarget;
    private Vector4[] _metaballPositions;
    private int _index;
    public override void Load()
    {
        base.Load();
        _metaballPositions = new Vector4[32];
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady += RenderFull;
    }

    public override void Unload()
    {
        base.Unload();
        PrepareRenderTargetDrawsSystem.OnRenderTargetDrawsReady -= RenderFull;
    }

    public override void OnModLoad()
    {
        base.OnModLoad();
        _fireTarget = ManagedRenderTarget.New();
        _metaballTarget = ManagedRenderTarget.New();
    }

    private void RenderFlameNoise()
    {
        SpriteBatch sb = Main.spriteBatch;
        GraphicsDevice gDevice = sb.GraphicsDevice;
        gDevice.SetRenderTarget(_fireTarget);
        gDevice.Clear(Color.Transparent);

        FlamethrowerNoiseShader shader = ShaderContent.GetInstance<FlamethrowerNoiseShader>();
        shader.Time = Main.GlobalTimeWrappedHourly * -3;
        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, shader.Effect);
        sb.Draw(ModContent.Request<Texture2D>($"Stellamod/Assets/NoiseTextures/LavaDepths").Value,
    Vector2.Zero, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
        sb.End();
    }
    private void RenderMetaballs()
    {
        FlamethrowerShader shader = ShaderContent.GetInstance<FlamethrowerShader>();
        shader.InnerColor = Color.Yellow;
        shader.OuterColor = Color.Red;
        shader.Length = _index;
        shader.Metaballs = _metaballPositions;
        shader.ScreenResolution = new Vector2(Main.screenWidth, Main.screenHeight);
        SpriteBatch sb = Main.spriteBatch;
        GraphicsDevice gDevice = sb.GraphicsDevice;
        gDevice.SetRenderTarget(_metaballTarget);
        gDevice.Clear(Color.Transparent);
        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, shader.Effect);
        sb.Draw(_fireTarget, Vector2.Zero, Color.White);
        sb.End();
        _index = 0;

    }
    private void RenderFull()
    {
        if (_index <= 0)
            return;

        RenderFlameNoise();
        RenderMetaballs();
        PixelationManager.QueueSpritebatchDrawAction(DrawToScreen);
    }

    private void DrawToScreen(SpriteBatch sb, Vector2 sp)
    {
        var shader = ShaderContent.GetInstance<FlamethrowerDistortionShader>();
        shader.Distortion = 0.009f;
        shader.DistortionTexture = _fireTarget;
        sb.Restart(effect: shader.Effect);
        sb.Draw(_metaballTarget, Vector2.Zero, Color.White);
        sb.RestartDefaults();
        //    sb.Draw(_fireTarget, Vector2.Zero, Color.White);
    }

    public static void AddMetaball(Vector2 pos, float time, float radius)
    {
        Matrix screenPosMatrix = TrailDrawer.WorldViewPoint2;

        Vector2 screenPos = new Vector2();
        screenPos.X = (pos.X - Main.screenPosition.X) / Main.screenWidth;
        screenPos.Y = (pos.Y - Main.screenPosition.Y) / Main.screenHeight;

        Vector4 metaballPos = new Vector4(screenPos, time, radius);
        FlamethrowerRenderer renderer = ModContent.GetInstance<FlamethrowerRenderer>();
        if (renderer._index >= renderer._metaballPositions.Length)
            return;

        renderer._metaballPositions[renderer._index++] = metaballPos;
    }
}
