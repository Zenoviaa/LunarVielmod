using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Stellamod.Core.LunarLightingSystem;

/*
public class LightsShader : CrystalShader<LightsShader>
{
    private EffectParameter _transformMatrixParam;
    private EffectParameter _stepSizeParam;
    private EffectParameter _geometryTextureParam;
    public Matrix WorldViewProjection
    {
        set
        {
            _transformMatrixParam ??= Effect.Parameters["worldViewProjection"];
            _transformMatrixParam.SetValue(value);
        }
    }
    public Vector2 StepSize
    {
        set
        {
            _stepSizeParam ??= Effect.Parameters["stepSize"];
            _stepSizeParam.SetValue(value);
        }
    }
    public Texture2D GeometryTexture
    {
        set
        {
            _geometryTextureParam ??= Effect.Parameters["geometryTexture"];
            _geometryTextureParam.SetValue(value);
        }
    }
}

public struct LightingVertex : IVertexType
{
    private Vector3 _position;
    private Vector4 _color;
    private Vector2 _textureCoordinate;
    private Vector2 _screenCenterCoordinate;
    public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
    (
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(12, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
        new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
        new VertexElement(36, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 1)
    );

    VertexDeclaration IVertexType.VertexDeclaration
    {
        get { return VertexDeclaration; }
    }

    public LightingVertex(Vector3 position, Color color, Vector2 textureCoordinate, Vector2 screenCenterCoordinate)
    {
        _position = position;
        _color = color.ToVector4();
        _textureCoordinate = textureCoordinate;
        _screenCenterCoordinate = screenCenterCoordinate;
    }
}
public record struct LightInstanceData(Vector2 position, Color color);

[Autoload(Side = ModSide.Client)]
public class LunarLighter : ModSystem,
    IPostProcessingPass
{
    //We need a base quad for our light now

    private Asset<Texture2D> _pointLightTextureAsset;
    private ManagedRenderTarget _lightMap;
    private ManagedRenderTarget _tileGeometry;
    public int PostProcessPriority => 25;
    public const int Max_Lights = 1000;
    public override void Load()
    {
        base.Load();
        Main.QueueMainThreadAction(InitializeBuffers);
        On_Main.CheckMonoliths += RenderLighting;
    }

    public override void Unload()
    {
        base.Unload();
        Main.QueueMainThreadAction(DisposeBuffers);
    }

    private void InitializeBuffers()
    {
    //    _lightVB = new VertexBuffer(Main.graphics.GraphicsDevice, typeof(LightingVertex), 4000, BufferUsage.WriteOnly);
    }

    private void DisposeBuffers()
    {
    //    _lightVB?.Dispose();
    //    _lightVB = null;
    }

    public override void OnModLoad()
    {
        base.OnModLoad();
        _pointLightTextureAsset = ModContent.Request<Texture2D>("Stellamod/Assets/GlowMasks/PointLight");
        _lightMap = ManagedRenderTarget.New();
        _tileGeometry = ManagedRenderTarget.New();
        PostProcessingRenderer.AddPass(this);
    }


    private LightingVertex[] PrepareLightInstances()
    {
        (Point topLeftTile, Point bottomRightTile) = TileUtilities.CameraTileBounds(fluff: 256);
        List<LightingVertex> instances = new List<LightingVertex>(capacity: 400);
        LightingEngine lightingEngine = typeof(Lighting).GetField("_activeEngine", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as LightingEngine;
        TileLightScanner tileScanner = typeof(LightingEngine).GetField("_tileScanner", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lightingEngine) as TileLightScanner;

        void Add(Vector2 lightPosition, Color lightingColor)
        {
            float radius = 800;
            Vector2 topLeft = lightPosition + new Vector2(-radius, -radius);
            Vector2 bottomLeft = lightPosition + new Vector2(-radius, radius);
            Vector2 bottomRight = lightPosition + new Vector2(radius, radius);
            Vector2 topRight = lightPosition + new Vector2(radius, -radius);

            Vector2 screenCenterCoordinate = lightPosition - Main.screenPosition;
            screenCenterCoordinate /= new Vector2(Main.screenWidth, Main.screenHeight);
            instances.Add(new LightingVertex(new Vector3(topLeft, 0), lightingColor, new Vector2(0, 0), screenCenterCoordinate));
            instances.Add(new LightingVertex(new Vector3(bottomRight, 0), lightingColor, new Vector2(1, 1), screenCenterCoordinate));
            instances.Add(new LightingVertex(new Vector3(topRight, 0), lightingColor, new Vector2(1, 0), screenCenterCoordinate));
            instances.Add(new LightingVertex(new Vector3(bottomLeft, 0), lightingColor, new Vector2(0, 1), screenCenterCoordinate));
        }
        for (int x = topLeftTile.X; x < bottomRightTile.X; x++)
        {
            for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
            {
                Point lightTilePoint = new Point(x, y);
                Tile tile = Main.tile[x, y];
                if (!Main.tileLighted[tile.TileType])
                    continue;
                if (!TileID.Sets.Torch[tile.TileType])
                    continue;


                //Get the light that this tile should be emitting
                Vector3 lightVec;
                tileScanner.GetTileLight(x, y, out lightVec);
                Vector2 position = lightTilePoint.ToWorldCoordinates();
                Color lightColor = new Color(lightVec);

              //  Main.NewText(lightColor);
                lightColor.A = 1;

     
                LightInstanceData lightInstanceData = new LightInstanceData(position, lightColor);
                instances.Add(lightInstanceData);


                Add(position, lightColor);


            }
        }

     //   Add(Main.LocalPlayer.Center, Color.White);


        return instances.ToArray();
    }

    private void RenderLighting(On_Main.orig_CheckMonoliths orig)
    {
        if (!Main.gameMenu)
        {
            GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            var lightData = PrepareLightInstances();
        //    Main.NewText(lightData.Length);
       
           // Main.NewText(lightData.Length);
            LunarLightingRenderer lr = ModContent.GetInstance<LunarLightingRenderer>();

            //Render out tile geometry
            gDevice.SetRenderTarget(_tileGeometry);
            gDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            Main.screenPosition += new Vector2(Main.offScreenRange);
            TileDrawing tilesRenderer = Main.instance.TilesRenderer;
 
            tilesRenderer.Draw(true, true, true);
            spriteBatch.End();
            Main.screenPosition -= new Vector2(Main.offScreenRange);

            
            gDevice.SetRenderTarget(_lightMap);
            gDevice.Clear(lr.SmoothedBackLightColor);
            SunLightManager.RenderSunLight();

            
            if (lightData.Length != 0)
            {
                gDevice.RasterizerState = RasterizerState.CullNone;
                gDevice.BlendState = BlendState.Additive;
                gDevice.DepthStencilState = DepthStencilState.None;

                LightsShader lightsShader = LightsShader.Instance;
                lightsShader.WorldViewProjection = TrailDrawer.WorldViewPoint2;
                lightsShader.StepSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight) * 2;
                lightsShader.GeometryTexture = _tileGeometry;
                lightsShader.Effect.CurrentTechnique.Passes[0].Apply();
                gDevice.DrawUserIndexedPrimitives<LightingVertex>(PrimitiveType.TriangleList, lightData, 0,
                    lightData.Length, SunLightManager.ShadowIndexBuffer, 0, lightData.Length / 2);

            }

            
           
            
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                lightsShader.Effect, Main.GameViewMatrix.TransformationMatrix); 

            SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(_pointLightTextureAsset, Vector2.Zero);
            for(int l = 0;  l < lights.Length; l++)
            {
                ref LightInstanceData light = ref lights[l];
                drawer.worldPosition = light.position;
                drawer.color =light.color;
                drawer.scale = Vector2.One * ExtraMath.Osc(0.75f, 1f, speed: 1);
              //  drawer.color.A = 0;
                spriteBatch.Draw(drawer);
            }
            spriteBatch.End();
            //   gDevice.DrawInstancedPrimitives
            gDevice.SetRenderTarget(null);
     
        }

        orig();
    }

    public void RenderToScreen()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, 
            null);
        spriteBatch.Draw(_lightMap, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        spriteBatch.End();
    }
}
*/