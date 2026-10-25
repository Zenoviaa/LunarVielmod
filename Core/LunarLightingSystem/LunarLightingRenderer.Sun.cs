using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.Rendering.RTs;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using System;

namespace Stellamod.Core.LunarLightingSystem;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TileShadowData : IVertexType
{
    public Vector4 Position;
    public static readonly VertexDeclaration VertexDeclaration;

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    static TileShadowData()
    {
        VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector4, VertexElementUsage.Color, 0));
    }

    public TileShadowData(Vector4 position)
    {
        Position = position;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        return "{{Position:" + Position.ToString();
    }

    public static bool operator ==(TileShadowData left, TileShadowData right)
    {
        if (left.Position == right.Position)
        {
            return true;
        }

        return false;
    }

    public static bool operator !=(TileShadowData left, TileShadowData right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return this == (TileShadowData)obj;
    }
}

public class InstancedSunShadows
{
    GPUInstanceBuffer<TileShadowData> _instanceBuffer;
    int _maxCount;
    int _length;
    public InstancedSunShadows(int maxCount = 2_000)
    {
        _maxCount = maxCount;
    }

    public void Load()
    {
        _instanceBuffer = new(_maxCount, 50, 200);

    }

    public void Unload()
    {
        _instanceBuffer?.Dispose();
        _instanceBuffer = null;
    }

    public void Clear()
    {
        _length = 0;
    }

    public void CollectShadowData()
    {
        using(new TickTimer())
        {
            //grab every point that should be casting a shadow
            Rectangle areaREctangle = TileUtilities.CameraTileRectangle(384);
            for (var x = areaREctangle.Left; x < areaREctangle.Right; x++)
            {
                for (var y = areaREctangle.Top; y < areaREctangle.Bottom; y++)
                {
                    var tile = Main.tile[x, y];
                    if (tile.HasTile && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType] && WorldGen.TileIsExposedToAir(x, y))
                    {

                        Push(new()
                        {
                            Position = new Vector4(x * 16 + 8, y * 16 + 8, 0, 0)
                        });
                    }
                }
            }
        }

        Main.NewText(_length);
    }

    public void Draw()
    {
        if (_length <= 0)
            return;



        Vector2 sunDirection = LightingGlobals.ShadowDirection;
        _instanceBuffer.SetIndices(new ushort[] { 0, 1,3, 0, 2, 3 });
        _instanceBuffer.SetVertices(16, 53, sunDirection.ToRotation() - MathHelper.PiOver2);
        _instanceBuffer.PrepareForDrawing(Main.graphics.GraphicsDevice);
        var instancedSunShadows = AssetReferences.Effects.CrystalShaders.InstancedSunShadows.CreatePixelPass();
        instancedSunShadows.Parameters.projection = TrailDrawer.WorldViewPoint2;
        instancedSunShadows.Parameters.shadowColor = (Color.Black * 0.4f * LightingHelper.DayLightEase).ToVector4();
        instancedSunShadows.Apply();
        instancedSunShadows.Shader.CurrentTechnique.Passes[0].Apply();
        Main.graphics.GraphicsDevice.BlendState = BlendState.AlphaBlend;
        Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
        Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
        Main.graphics.GraphicsDevice.DrawInstancedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2, _length);
    }
    //For the sun shadows, we'll pass in all the parameters through the color variable I think?
    //Actually probably better to just have proper instancedata hold on

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Push(TileShadowData data)
    {
        if (_length >= _instanceBuffer.instances.Length  )
            return;
        _instanceBuffer.instances[_length++] = data;
    }
}

public partial class LunarLightingRenderer
{
    private static InstancedSunShadows _instancedSunShadows;
    public static Color[] DefaultSunColors
    {
        get
        {
            field ??= new Color[]
            {
                    new Color(8, 79, 126).Towards(Color.White, 0.5f),
                  Color.SkyBlue,

                    new Color(255, 173, 63),
                       new Color(255, 173, 63),
                                new Color(255, 173, 63),
                                         new Color(255, 173, 63),
                                            new Color(255, 173, 63),



                    Color.White,
                   Color.White,
                        Color.White,
                             Color.White,
                                  Color.White,
                    Color.White,
                   Color.White,
                        Color.White,
                    Color.White,
                    Color.White,

                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    new Color(255, 173, 63),
                    Color.SkyBlue,
                    new Color(8, 79, 126).Towards(Color.White, 0.5f),
            };
            return field;
        }
    }
    public static Color GetSunColor(float interpolant)
    {
        if (interpolant >= 1f)
            return DefaultSunColors[0];
        Color interpolatedColor = DrawUtilities.InterpolateColorArray(interpolant, DefaultSunColors);
        return interpolatedColor;
    }

    public static float DayProgress => Main.dayTime ? (float)Main.time / (float)Main.dayLength : (float)Main.time / (float)Main.nightLength;
    public Color GetSunColor()
    {
        var sunColors = DefaultSunColors;
        var interpolatedColor = DrawUtilities.InterpolateColorArray(DayProgress, sunColors);
        if (!Main.dayTime)
            interpolatedColor = sunColors[0];
        if (Main.LocalPlayer.ZoneRockLayerHeight && !Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMistyDungeonAnywhere)
        {
            interpolatedColor = SmoothedBackLightColor;
        }
        else
        {

        }
        if (Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneCrimsonBridewell || Main.LocalPlayer.GetModPlayer<MyPlayer>().ZoneAbyss)
        {
            interpolatedColor = SmoothedBackLightColor;
        }
           
        if (ModContent.GetInstance<DomainExpansionManager>().hoveringPlatform)
            interpolatedColor = Color.White;
        return interpolatedColor;
    }

    private void RenderSunLight()
    {
        if (!Lighting.UsingNewLighting)
            return;

        if (ModContent.GetInstance<DomainExpansionManager>().noRender)
            return;

        if(Main.GameUpdateCount % 30 == 0)
        {
           // _instancedSunShadows.Clear();
         //   _instancedSunShadows.CollectShadowData();
        }


        Vector2 stepSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        stepSize *= 4 * -LightingGlobals.ShadowDirection;

        var shader = ShaderContent.GetInstance<SunLightShader>();
        shader.StepSize = stepSize;
        shader.ShadowAlpha = LightingHelper.DayLightEase * sunShadowAlpha;

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
            DepthStencilState.None, RasterizerState.CullNone, shader.Effect, Main.GameViewMatrix.TransformationMatrix);


        Vector2 drawPosition = Vector2.Zero;


        spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, null,
           SunColor, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

        spriteBatch.End();

       // _instancedSunShadows.Draw();
        /*
        Vector2 stepSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
        stepSize *= 4 * -LightingGlobals.ShadowDirection;

        var shader = ShaderContent.GetInstance<SunLightShader>();
        shader.StepSize = stepSize;
        shader.ShadowAlpha = LightingHelper.DayLightEase * sunShadowAlpha;

        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
            DepthStencilState.None, RasterizerState.CullNone, shader.Effect, Main.GameViewMatrix.TransformationMatrix);


        Vector2 drawPosition = Vector2.Zero;
      

        spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, null,
           SunColor, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

        spriteBatch.End();*/
    }

    private void RenderShadows(RenderTargetHandle tileBlurRT, RenderTargetHandle tileSunShadowRT)
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

        //This is where the sun shadows are drawn
        //A bit slow and dated
        //Gonna try drawing with anistropic clamp and with the instanced geometry
        //Should be way faster.
        using(RT.Clear(tileBlurRT, Color.Transparent))
        {
            Effect effect = GameShaders.Misc["LunarVeil:SunShadow"].Shader;
            effect.Parameters["mipBias"].SetValue(0.1f);

            Vector2 sunDirection = LightingGlobals.ShadowDirection;
            effect.Parameters["sunDirection"].SetValue(-sunDirection * 1400);
            effect.Parameters["falloff"].SetValue(0.1f);
            effect.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2());
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, effect);
            spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, null, Color.Black * 0.9f * LightingHelper.DayLightEase, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();

        }

        using(RT.Clear(tileSunShadowRT, Color.Transparent))
        {
            Effect blurEffect = GameShaders.Misc["LunarVeil:SunBlur"].Shader;
            blurEffect.Parameters["mipBias"].SetValue(12);
            blurEffect.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2());
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, blurEffect);
            spriteBatch.Draw(tileBlurRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }
    }
}
