using Stellamod.Common.Shaders;
using System.Threading;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem;

public class SunLightManager : ModSystem
{
    private static int _primitiveIndex;
    private static float _overSunTimer;
    private static float _daylightFadeTimer;
    public const int Max_Primitive_Count = 15000;

    public static int[] ShadowIndexBuffer;
    public static VertexPositionColor[] ShadowVertices = new VertexPositionColor[Max_Primitive_Count * 3];
    public static Vector2 ShadowDirection;
    public static Color ShadowColor;
    public static Color SunColor;

    private static Matrix _sunMatrix;
    public override void OnModLoad()
    {
        ShadowIndexBuffer = new int[Max_Primitive_Count * 6];
        int connectIndex = 0;
        for (int i = 0; i < ShadowIndexBuffer.Length; i += 6)
        {
            ShadowIndexBuffer[i] = connectIndex + 0;
            ShadowIndexBuffer[i + 1] = connectIndex + 1;
            ShadowIndexBuffer[i + 2] = connectIndex + 2;
            ShadowIndexBuffer[i + 3] = connectIndex + 0;
            ShadowIndexBuffer[i + 4] = connectIndex + 1;
            ShadowIndexBuffer[i + 5] = connectIndex + 3;
            connectIndex += 4;
        }
    }
    public override void PostUpdateDusts()
    {
        base.PostUpdateDusts();
        Update();
    }

    public static void Update()
    {
        Point point = Main.LocalPlayer.position.ToTileCoordinates();
        bool overworld = (double)point.Y <= Main.worldSurface;
        if (!overworld)
        {
            _overSunTimer--;
            if (_overSunTimer <= 0)
                return;
        }
        else
        {
            _overSunTimer++;
        }


        _overSunTimer = MathHelper.Clamp(_overSunTimer, 0, 120);
        float interpolant = _overSunTimer / 120f;
        Vector2 sunLeft = Main.Camera.Center + new Vector2(-Main.screenWidth / 2, -Main.screenHeight / 2);
        Vector2 sunRight = Main.Camera.Center + new Vector2(Main.screenWidth / 2, -Main.screenHeight / 2);

        float dayProgress = Main.dayTime ? (float)Main.time / (float)Main.dayLength : (float)Main.time / (float)Main.nightLength;
        float radians = MathHelper.Lerp(MathHelper.ToRadians(-45), MathHelper.ToRadians(45), dayProgress);
        Vector2 sunDirection = Vector2.UnitY.RotatedBy(radians) * 400;
        if (dayProgress <= 0.1f || dayProgress >= 0.9f)
        {

            _daylightFadeTimer--;
        }
        else
        {
            _daylightFadeTimer++;
        }

        _sunMatrix = Matrix.CreateRotationZ(ShadowDirection.ToRotation());
        _daylightFadeTimer = MathHelper.Clamp(_daylightFadeTimer, 0, 120);
        float shadowDaylightFadeInterpolant = _daylightFadeTimer / 120f;

        Vector2 sunPosition = Main.Camera.Center + new Vector2(0, 0);
        SunColor = Main.ColorOfTheSkies * interpolant;
        ShadowDirection = sunDirection;
        ShadowColor = Color.Black * 0.05f * shadowDaylightFadeInterpolant;

        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        if (!config.SunShadows)
            return;

        if (Main.GameUpdateCount % 4 == 0)
            ScanShadows();
    }

    public static void RenderSunLight()
    {

        
        Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Circle").Value;
        Vector2 drawOrigin = texture.Size() / 2f;
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        for (int i = 0; i < 3; i++)
            spriteBatch.Draw(texture, Vector2.Zero + new Vector2(Main.screenWidth, Main.screenHeight) / 2f, null, SunColor, 0, drawOrigin, 40, SpriteEffects.None, 0);
        spriteBatch.End();
        

        GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;

        //shadows
        var config = ModContent.GetInstance<LunarVeilClientConfig>();
        if (!config.SunShadows)
            return;
        int primitiveCount = _primitiveIndex / 3;
        if (primitiveCount <= 0)
            return;

        var shader = TileShadowShader.Instance;
        shader.ApplyPasses();


        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.BlendState = BlendState.AlphaBlend;
        graphicsDevice.DrawUserIndexedPrimitives(
          PrimitiveType.TriangleList, ShadowVertices, 0, _primitiveIndex, ShadowIndexBuffer, 0, _primitiveIndex / 2);
        graphicsDevice.RasterizerState = Main.Rasterizer;
    }

    private static bool IsFull()
    {
        return _primitiveIndex >= ShadowVertices.Length;
    }


    private static void AddQuad(Vector2 xy1, Vector2 xy2)
    {


        Vector2 origin = xy1 + xy2;
        origin *= 0.5f;

        xy1 = Vector2.Transform(xy1 - origin, _sunMatrix) + origin;
        xy2 = Vector2.Transform(xy2 - origin, _sunMatrix) + origin;


        int vertexIndex = _primitiveIndex;
        _primitiveIndex += 4;

        //For the shadow color I want to take the inverse of the pointlight color and then lerp it towards black a bit
        ref VertexPositionColor tl1 = ref ShadowVertices[vertexIndex];
        tl1.Position = new Vector3(xy1, 0);
        tl1.Color = ShadowColor;

        ref VertexPositionColor tr1 = ref ShadowVertices[vertexIndex + 1];
        tr1.Position = new Vector3(xy2.X, xy1.Y, 0);
        tr1.Color = ShadowColor;

        ref VertexPositionColor br1 = ref ShadowVertices[vertexIndex + 2];
        br1.Position = new Vector3(xy2, 0);
        br1.Color = ShadowColor;

        ref VertexPositionColor br2 = ref ShadowVertices[vertexIndex + 3];
        br2.Position = new Vector3(xy1.X, xy2.Y, 0);
        br2.Color = ShadowColor;

        //0, 1, 2
        ShadowVertices[vertexIndex] = tl1;
        ShadowVertices[vertexIndex + 1] = tr1;
        ShadowVertices[vertexIndex + 2] = br1;
        ShadowVertices[vertexIndex + 3] = br2;
    }

    private static void ScanShadows()
    {
        _primitiveIndex = 0;
        Vector2 topLeftOfPointLight = Main.screenPosition;

        Point topLeftTile = topLeftOfPointLight.ToTileCoordinates();
        topLeftTile.Y -= 16;
        Point bottomRightTIle = topLeftTile + new Point(150, 70);


        int startTileX = topLeftTile.X;
        int startTileY = topLeftTile.Y;
        int endTileX = bottomRightTIle.X;
        int endTileY = bottomRightTIle.Y;

        for (int x = startTileX; x < endTileX; x++)
        {
            for (int y = startTileY; y < endTileY; y++)
            {
                if (IsFull())
                {
                    break;
                }
                //If a tile is outside of the world just ignore it, otherwise we'll get an error
                if (!WorldGen.InWorld(x, y))
                    continue;

                Tile tile = Main.tile[x, y];
                if (!tile.HasTile)
                    continue;

                //Only cast a shadow if a tile is touching air, so we aren't drawing unnecessary shadows
                if (!WorldGen.TileIsExposedToAir(x, y))
                    continue;

                if (!Main.tileSolid[tile.TileType])
                    continue;

                if (LightingSets.NoShadows[tile.TileType])
                    continue;


                Point tilePoint = new Point(x, y);
                Vector2 worldPoint = tilePoint.ToWorldCoordinates(0, 0);

                //Now we calculate vertices
                //There's no texture here so it doesn't matter what order we do the triangles in
                //Pretty sure we start from top left?


                //Vertex 0
                Vector2 topLeft = worldPoint;

                //Vertex 1
                Vector2 topRight = worldPoint + new Vector2(16, 0);

                //Vertex 2
                Vector2 bottomLeft = worldPoint + new Vector2(0, 16);

                //Vertex 3
                Vector2 bottomRight = worldPoint + new Vector2(16, 16);

                AddQuad(topLeft, bottomRight);
                //    AddQuad(topRight, bottomLeft);
            }
        }
    }
}
