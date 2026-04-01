using Stellamod.Common.Shaders;
using System.Reflection;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem;


public struct PointLightSource
{
    public Color lightColor;
    public Vector2 worldPosition;
    public float radius;
    public float intensity;
}

[Autoload(Side = ModSide.Client)]
public class LightingV2 : ModSystem
{
    private bool _clearNextFrame;
    private int _lightIndex;
    private PointLightSource[] _lightSources;
    private VertexPositionColorTexture[] _pointLightVertices;
    public const int MAX_POINT_LIGHTS = 400;
    public override void Load()
    {
        base.Load();
        _lightSources = new PointLightSource[MAX_POINT_LIGHTS];
        _pointLightVertices = new VertexPositionColorTexture[MAX_POINT_LIGHTS * 4];
        On_LightingEngine.AddLight += LightingEngine_AddLight;
        On_LightingEngine.ApplyPerFrameLights += LightingEngine_Clear;
    }
    private void LightingEngine_AddLight(Terraria.Graphics.Light.On_LightingEngine.orig_AddLight orig, LightingEngine self, int x, int y, Vector3 color)
    {
        orig(self, x, y, color);
        /*
        if (_lightIndex >= _lightSources.Length)
            return;
        Vector2 position = new Vector2(x * 16 + 8, y * 16 + 8);
        ref PointLightSource lightSource = ref _lightSources[_lightIndex];
        lightSource.worldPosition = position;
        lightSource.lightColor = new Color(color);
        lightSource.radius = 666;
        lightSource.intensity = 0.5f;
        _lightIndex++;*/
    }
    private void LightingEngine_Clear(Terraria.Graphics.Light.On_LightingEngine.orig_ApplyPerFrameLights orig, LightingEngine self)
    {
        orig(self);
        //_clearNextFrame = true;
 
    }
    public override void Unload()
    {
        base.Unload();
        _lightSources = null;
        _pointLightVertices = null;
        On_LightingEngine.AddLight -= LightingEngine_AddLight;
        On_LightingEngine.ApplyPerFrameLights -= LightingEngine_Clear;
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        return;
        if (Main.GameUpdateCount % 4 == 0)
            PrepareLightSources();
    }

    private void PrepareLightSources()
    {
        _lightIndex = 0;
        Vector2 cameraCenterWorld = Main.Camera.Center;
        Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
        Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

        const float range = 256;
        cameraTopLeft -= new Vector2(range);
        cameraBottomRight += new Vector2(range);

        Point topLeftTile = cameraTopLeft.ToTileCoordinates();
        Point bottomRightTile = cameraBottomRight.ToTileCoordinates();

        LightingEngine lightingEngine = typeof(Lighting).GetField("_activeEngine", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as LightingEngine;
        TileLightScanner tileScanner = typeof(LightingEngine).GetField("_tileScanner", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lightingEngine) as TileLightScanner;

        int numTilesProducingLight = 0;
        for (int x = topLeftTile.X; x < bottomRightTile.X; x++)
        {
            for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
            {
                Tile tile = Framing.GetTileSafely(x, y);

                Point lightTilePoint = new Point(x, y);
                if (!Main.tileLighted[tile.TileType])
                    continue;

                Vector3 color;
                tileScanner.GetTileLight(x, y, out color);

                float luminosity = (color.X + color.Y + color.Z) / 3f;
     
                numTilesProducingLight++;

                //   Main.NewText(luminosity);
                Vector2 position = lightTilePoint.ToWorldCoordinates();
                Color lightColor = new Color(color);
                lightColor.A = 1;

                if (_lightIndex >= _lightSources.Length)
                    break;

                //Prepare light source for rendering
                ref PointLightSource lightSource = ref _lightSources[_lightIndex];
                lightSource.worldPosition = position;
                lightSource.lightColor = lightColor;
                lightSource.radius = 666;
                lightSource.intensity = luminosity * 1f;
                _lightIndex++;
            }
        }
      //  Main.NewText(numTilesProducingLight);
    }


    public void RenderLightSources()
    {
        //Prepare vertices
        for (int i = 0; i < _lightIndex; i++)
        {
            ref var data = ref _lightSources[i];
            Vector2 position = data.worldPosition;
            float radius = data.radius;
            Color color = data.lightColor;

            Vector2 topLeft = position + new Vector2(-radius, -radius);
            Vector2 bottomLeft = position + new Vector2(-radius, radius);
            Vector2 bottomRight = position + new Vector2(radius, radius);
            Vector2 topRight = position + new Vector2(radius, -radius);

            int startIndex = i * 4;
            _pointLightVertices[startIndex] = new VertexPositionColorTexture(new Vector3(topLeft, 0), color, new Vector2(0, 0));
            _pointLightVertices[startIndex + 2] = new VertexPositionColorTexture(new Vector3(topRight, 0), color, new Vector2(1, 0));
            _pointLightVertices[startIndex + 3] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), color, new Vector2(0, 1));
            _pointLightVertices[startIndex + 1] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), color, new Vector2(1, 1));
        }

        GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
        graphicsDevice.DepthStencilState = DepthStencilState.None;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.BlendState = BlendState.Additive;
        graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        //I think the black square issue was just a race condition with the graphics state?
        var shader = PointLightShader.Instance;
        shader.TransformMatrix = TrailDrawer.WorldViewPoint2;
        int numVertices = _lightIndex * 4;
        int numPrimitives = numVertices / 2;
        foreach (EffectPass pass in shader.Effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            // Set this _after_ Apply, otherwise EffectParameters override it!
            graphicsDevice.Textures[0] = null;
            graphicsDevice.DrawUserIndexedPrimitives(
              PrimitiveType.TriangleList, _pointLightVertices, 0, numVertices, SunLightManager.ShadowIndexBuffer, 0, numPrimitives);
        }
    }
}
