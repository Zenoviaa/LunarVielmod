using ReLogic.Content;
using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Core.Grass;

public class GrassGlobalTile : GlobalTile
{
    public override void DrawEffects(int i, int j, int type, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        base.DrawEffects(i, j, type, spriteBatch, ref drawData);
        Tile tile = Main.tile[i, j];
        if (TileID.Sets.MarshyGrass[type] != -1 && tile.HasTile && !Framing.GetTileSafely(i, j - 1).HasTile)
        {
            GrassRenderer.GrassTilePoints.Add(new Point(i, j));
        }
    }
}
[Autoload(Side = ModSide.Client)]
public class GrassRenderer : ModSystem
{
    public struct Grass
    {
        public GrassProfile profile;
        public Asset<Texture2D> textureAsset;
        public Rectangle frame;
        public Color color;
        public Vector2 position;
        public Vector2 direction;
        public Vector2 offsetDirection;

    }
    public struct GrassBlade
    {
        public Color color;
        public Vector2 position;
        public Vector2 direction;
        public float length;
        public float width;
    }

    public const int Max_Blade_Count = 2000;

    private GrassBlade[] _grassBlades;
    private VertexPositionColor[] _grassVertices;
    private int _grassVertexIndex;

    private Grass[] _grasses;
    private float[] _windRotations;

    private FastNoiseLite Noise
    {
        get
        {
            if(field == null)
            {
                field = new FastNoiseLite();
                field.SetSeed(1337);
                field.SetFrequency(0.2f);
            }
            return field;
        }
    }
    private int _grassIndex;
    private int _grassBladeIndex;
    private float _windTimer;
    private Color _multiplyColor;
    private Vector2 _backOffset;

    private float _noiseTimer;
    private Color _darkSkyColor;
    private float _backLayerInterp;
    private Vector2 _offset;
    public static readonly List<Point> GrassTilePoints = new();
    public override void OnModLoad()
    {
        base.OnModLoad();
        _grasses = new Grass[Max_Blade_Count];
        for (int i = 0; i < _grasses.Length; i++)
        {
            _grasses[i] = new Grass();
        }
        _grassBlades = new GrassBlade[Max_Blade_Count];
        _grassVertices = new VertexPositionColor[Max_Blade_Count * 3];
        _windRotations = new float[Max_Blade_Count];
        On_Main.RenderTiles += PrepareGrasses;
        On_Main.CheckMonoliths += Monoliths_Hook;
        PixelationManager.OnBehindGrass += RenderGrassesBehindHook;
        PixelationManager.OnInFrontGrass += RenderGrassesFrontHook;

    }

    private void PrepareGrasses(On_Main.orig_RenderTiles orig, Main self)
    {
        ClearGrass();
        Noise.SetSeed(1337);
        Noise.SetFrequency(0.2f);
        GrassTilePoints.Clear();
        orig(self);

        foreach (var point in GrassTilePoints)
        {
            var tile = Main.tile[point];
            var profile = GrassTileSystem.Profiles[TileID.Sets.MarshyGrass[tile.TileType]];
            float i = point.X;
            var noise = PreGeneratedNoise.SampleRand(point.X, 0);
            if (noise > -0.95f)
            {
                GrassProfile profileToUse = profile.GetVariantProfile(point.X, point.Y);
            
                profileToUse.Grow(this, point.X, point.Y);
            }
        }

    }

    public override void OnModUnload()
    {
        base.OnModUnload();
        On_Main.CheckMonoliths -= Monoliths_Hook;
        PixelationManager.OnBehindGrass -= RenderGrassesBehindHook;
        PixelationManager.OnInFrontGrass -= RenderGrassesFrontHook;
    }
    private void RenderGrassesBehindHook()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        RenderGrassesBack(spriteBatch, Main.screenPosition);
        spriteBatch.End();
    }


    private void RenderGrassesFrontHook()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        RenderGrassesFront(spriteBatch, Main.screenPosition);
        spriteBatch.End();
    }





    private void Monoliths_Hook(On_Main.orig_CheckMonoliths orig)
    {
        SimulateWind();
        if (!Main.gameMenu && CanRenderGrass())
        {
            PixelationManager.QueuePrimitivesDrawAction(RenderGrassBack, DrawLayer.BackGrassTarget);
            PixelationManager.QueuePrimitivesDrawAction(RenderGrass, DrawLayer.FrontGrassTarget);
        }

        orig();
    }

    public bool CanRenderGrass()
    {
        return _grassIndex != 0 || _grassBladeIndex != 0;
    }

    public void ClearGrass()
    {
        _grassIndex = 0;
        _grassBladeIndex = 0;
    }
    public void AddGrassPatch(Color color, Vector2 position, Vector2 direction, float length, float width, int numGrasses)
    {

        float maxOffset = 8;
        float maxLengthOffset = length / 4f;
        float maxWidthOffset = width / 4f;
        for (int g = 0; g < numGrasses; g++)
        {
            float rand = Noise.GetNoise(position.X + g * 8, 0) * 0.5f + 0.5f;
            Vector2 newGrassPosition = position + Vector2.UnitX * MathHelper.Lerp(-maxOffset, maxOffset, rand);// * rand;
            float newLength = length + MathHelper.Lerp(-maxLengthOffset, maxLengthOffset, rand);
            float newWidth = width + MathHelper.Lerp(-maxWidthOffset, maxWidthOffset, rand);
            AddGrass(color, newGrassPosition, direction, newLength, newWidth);
        }
    }

    public void AddGrass(Color color, Vector2 position, Vector2 direction, float length, float width)
    {
        //HIt the maximum number of grasses
        if (_grassBladeIndex >= _grassBlades.Length)
            return;
        ref GrassBlade blade = ref _grassBlades[_grassBladeIndex];
        blade.color = color;
        blade.position = position;
        blade.direction = direction;
        blade.length = length;
        blade.width = width;
        _grassBladeIndex++;
    }
    private void SimulateWind()
    {
        _windTimer += Main.windSpeedCurrent * 0.1f;
        float windRange = MathHelper.ToRadians(Main.windSpeedCurrent * 6);
        VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();

        for (int i = 0; i < _grassIndex; i++)
        {
            ref Grass grass = ref _grasses[i];
            Vector2 grassPosition = grass.position;
            Vector2 baseDirection = grass.direction;
            float tileOffsetX = grassPosition.X;
            float osc = ExtraMath.Osc(0f, 1f, 0, offset: tileOffsetX + _windTimer);
            float windRadians = MathHelper.Lerp(-windRange, windRange, osc);


            Vector2 externalForces = velocityMap.GetDecayingVelocity(grassPosition - new Vector2(16, 0), 32, 80);
            externalForces = externalForces.SafeNormalize(Vector2.Zero);
            grass.offsetDirection = grass.direction;// + externalForces * 0.2f;
            _windRotations[i] = windRadians;
        }
    }
    public void AddGrassVertices(int bladeIndex, Vector2 direction)
    {
        //HIt the maximum number of grasses
        ref GrassBlade blade = ref _grassBlades[bladeIndex];
        Color color = blade.color;
        color = Color.Lerp(color, _darkSkyColor, _backLayerInterp);

        int startIndex = bladeIndex * 3;
        ref VertexPositionColor bottomLeft = ref _grassVertices[startIndex];
        ref VertexPositionColor bottomRight = ref _grassVertices[startIndex + 1];
        ref VertexPositionColor top = ref _grassVertices[startIndex + 2];

        Vector2 perpDirection = direction.RotatedBy(MathHelper.PiOver2);
        Vector2 perpOffset = perpDirection * blade.width * 0.5f;

        //Calculate vertice positions
        bottomLeft.Position = new Vector3(blade.position - perpOffset + _offset, 0);
        bottomRight.Position = new Vector3(blade.position + perpOffset + _offset, 0);

        Vector2 topBladePosition = blade.position + direction * blade.length;


        //Apply offset based on if something is interacting with this grass
        //For now we can just check the player
        VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();
        Vector2 externalForces = velocityMap.GetDecayingVelocity(topBladePosition - new Vector2(16, 0), 32, 80);
        Vector2 newPosition = topBladePosition + externalForces * 0.63f;
        topBladePosition = topBladePosition.MoveTowards(newPosition, 32);


        //Round the x position to prevent blades from fading out of existence
        topBladePosition.X = MathF.Floor(topBladePosition.X);
        topBladePosition.Y = MathF.Floor(topBladePosition.Y);
        top.Position = new Vector3(topBladePosition + _offset, 0);

        Color topColor = Color.Lerp(color, Main.ColorOfTheSkies, 0.5f);

        //Apply noise to the top color
        float noiseSample = Noise.GetNoise(blade.position.X + _noiseTimer, 0) * 0.5f + 0.5f;
        float bladeOsc = ExtraMath.Osc(0f, 1f, 0f, blade.position.X) * 0.3f;
        Color bottomColor = Color.Lerp(color, Color.Black, bladeOsc + noiseSample * 0.4f);


        Point tile = blade.position.ToTileCoordinates();
        Color lightColor = Lighting.GetColor(tile);
        bottomColor = bottomColor.MultiplyRGB(lightColor);
        topColor = topColor.MultiplyRGB(lightColor);

        bottomLeft.Color = bottomColor;
        bottomRight.Color = bottomColor;
        top.Color = topColor;
    }
    private void PrepareGrassVertices()
    {
        float windRange = MathHelper.ToRadians(Main.windSpeedCurrent * 5);
        _windTimer += Main.windSpeedCurrent * 0.05f;
        _noiseTimer = _windTimer * 2;

        _grassVertexIndex = 0;
        _darkSkyColor = Color.Lerp(Main.ColorOfTheSkies, Color.Black, 0.75f);
        Noise.SetFrequency(0.05f);

        //Simulate wind and populate the draw buffer with the grass vertex data
        FastParallel.For(0, _grassIndex, delegate (int start, int end, object context)
        {
            for (int i = start; i < end; i++)
            {
                ref GrassBlade blade = ref _grassBlades[i];
                float bladeOffset = _windTimer + blade.position.X;
                float osc = ExtraMath.Osc(0f, 1f, 0, offset: bladeOffset);
                float windRadians = MathHelper.Lerp(-windRange, windRange, osc);
                Vector2 newDirection = blade.direction.RotatedBy(windRadians);
                AddGrassVertices(i, newDirection);
            }
        });
        _grassVertexIndex = _grassBladeIndex * 3;
    }
    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
    }
    private UnifiedRandom _random;
    private void RenderGrassesBack(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        _multiplyColor = Color.Lerp(Color.White, Color.Black, 0.6f);
        _backOffset = new Vector2(24, 4);
        RenderGrassInner(spriteBatch, screenPos);

    }


    private void RenderGrassInner(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        Noise.SetSeed(1337);
        Noise.SetFrequency(0.2f);
        screenPos.X = MathF.Floor(screenPos.X);
        screenPos.Y = MathF.Floor(screenPos.Y);
        for (int i = 0; i < _grassIndex; i++)
        {
            Grass grass = _grasses[i];

            Texture2D grassTexture = grass.textureAsset.Value;
            Rectangle grassFrame = grass.frame;
            Vector2 drawOrigin = new Vector2(grassFrame.Width / 2f, grassFrame.Height);

            Point reedTile = grass.position.ToTileCoordinates();
            Color lightColor = Lighting.GetColor(reedTile.X, reedTile.Y);
            lightColor = lightColor.MultiplyRGB(_multiplyColor);
            float scale = 1f;
            scale *= ExtraMath.Osc(0.7f, 1f, 0, grass.position.X);

            Vector2 grassPosition = grass.position;
            grassPosition.X = MathF.Floor(grassPosition.X / 16) * 16;
            grassPosition.Y = MathF.Floor(grassPosition.Y / 16) * 16;
            Vector2 drawPosition = grassPosition - screenPos + _backOffset;
            drawPosition.Y += 8;

            float rotation = grass.offsetDirection.ToRotation() + MathHelper.PiOver2;

            rotation += _windRotations[i];
            spriteBatch.Draw(grassTexture, drawPosition, grassFrame, lightColor, rotation, drawOrigin, scale, SpriteEffects.None, 0);


            float n = Noise.GetNoise(grass.position.X, 0);

            int flowerOsc = (int)ExtraMath.Osc(0f, 2f, 0f, grass.position.X + _backOffset.X);
            if (n > 0.7f)
            {
                _random ??= new UnifiedRandom();
                _random.SetSeed((int)grass.position.X);
                var reedProfiles = grass.profile.GetReedProfiles();
                if (reedProfiles.Count == 0)
                    continue;

                ReedProfile profile = reedProfiles[_random.Next(0, reedProfiles.Count)];
                Texture2D reedTexture = profile.ReedTextureAsset.Value;
                Rectangle frame = profile.GetFrame(_random.Next(0, profile.frameCount));

                Vector2 reedDrawOrigin = new Vector2(frame.Width / 2f, frame.Height);
                Vector2 reedDrawPosition = drawPosition;
                reedDrawPosition.Y -= grassFrame.Height;
                reedDrawPosition.Y += 16;
                spriteBatch.Draw(reedTexture, reedDrawPosition, frame, lightColor, rotation, reedDrawOrigin, 1, SpriteEffects.None, 0);
            }
        }
    }
    private void RenderGrassesFront(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        _multiplyColor = Color.White;
        _backOffset = Vector2.Zero;
        RenderGrassInner(spriteBatch, screenPos);



    }
    public void AddGrass(GrassProfile profile, Asset<Texture2D> textureAsset, Rectangle frame, Color color, Vector2 position, Vector2 direction)
    {
        if (_grassIndex >= _grasses.Length)
            return;

        ref Grass grass = ref _grasses[_grassIndex];
        grass.profile = profile;
        grass.textureAsset = textureAsset;
        grass.frame = frame;
        grass.color = color;
        grass.position = position;
        grass.direction = direction;
        _grassIndex++;
    }

    public void AddGrass(Asset<Texture2D> textureAsset, Rectangle frame, Color color, Vector2 position, Vector2 direction)
    {
        if (_grassIndex >= _grasses.Length)
            return;

        ref Grass grass = ref _grasses[_grassIndex];
        grass.textureAsset = textureAsset;
        grass.frame = frame;
        grass.color = color;
        grass.position = position;
        grass.direction = direction;
        _grassIndex++;
    }

    /*
    public void AddGrassVertices(int bladeIndex, Vector2 direction)
    {
        //HIt the maximum number of grasses
        ref GrassBlade blade = ref _grassBlades[bladeIndex];
        Color color = blade.color;
        color = Color.Lerp(color, _darkSkyColor, _backLayerInterp);

        int startIndex = bladeIndex * 3;
        ref VertexPositionColor bottomLeft = ref _grassVertices[startIndex];
        ref VertexPositionColor bottomRight = ref _grassVertices[startIndex + 1];
        ref VertexPositionColor top = ref _grassVertices[startIndex + 2];

        Vector2 perpDirection = direction.RotatedBy(MathHelper.PiOver2);
        Vector2 perpOffset = perpDirection * blade.width * 0.5f;

        //Calculate vertice positions
        bottomLeft.Position = new Vector3(blade.position - perpOffset + _offset, 0);
        bottomRight.Position = new Vector3(blade.position + perpOffset + _offset, 0);

        Vector2 topBladePosition = blade.position + direction * blade.length;


        //Apply offset based on if something is interacting with this grass
        //For now we can just check the player
        VelocityMap velocityMap = ModContent.GetInstance<VelocityMap>();
        Vector2 externalForces = velocityMap.GetDecayingVelocity(topBladePosition - new Vector2(16, 0), 32, 80);
        Vector2 newPosition = topBladePosition + externalForces * 0.63f;
        topBladePosition = topBladePosition.MoveTowards(newPosition, 32);


        //Round the x position to prevent blades from fading out of existence
        topBladePosition.X = MathF.Floor(topBladePosition.X);
        topBladePosition.Y = MathF.Floor(topBladePosition.Y);
        top.Position = new Vector3(topBladePosition + _offset, 0);

        Color topColor = Color.Lerp(color, Main.ColorOfTheSkies, 0.5f);

        //Apply noise to the top color
        float noiseSample = _fastNoise.GetNoise(blade.position.X + _noiseTimer, 0) * 0.5f + 0.5f;
        float bladeOsc = ExtraMath.Osc(0f, 1f, 0f, blade.position.X) * 0.3f;
        Color bottomColor = Color.Lerp(color, Color.Black, bladeOsc + noiseSample * 0.4f);
        bottomLeft.Color = bottomColor;
        bottomRight.Color = bottomColor;
        top.Color = topColor;
    }*/
    public void DarkenVertex(int bladeIndex)
    {
        //HIt the maximum number of grasses
        int startIndex = bladeIndex * 3;
        ref VertexPositionColor bottomLeft = ref _grassVertices[startIndex];
        ref VertexPositionColor bottomRight = ref _grassVertices[startIndex + 1];
        ref VertexPositionColor top = ref _grassVertices[startIndex + 2];

        bottomLeft.Color = Color.Lerp(bottomLeft.Color, Color.Black, _backLayerInterp);
        bottomRight.Color = Color.Lerp(bottomRight.Color, Color.Black, _backLayerInterp);
        top.Color = Color.Lerp(top.Color, Color.Black, _backLayerInterp);

        top.Position += new Vector3(_offset, 0);
        bottomLeft.Position += new Vector3(_offset, 0);
        bottomRight.Position += new Vector3(_offset, 0);
    }
    private void PrepareDarkGrassVertices()
    {
        //Simulate wind and populate the draw buffer with the grass vertex data
        FastParallel.For(0, _grassBladeIndex, delegate (int start, int end, object context)
        {
            for (int i = start; i < end; i++)
            {
                DarkenVertex(i);
            }
        });
        _grassVertexIndex = _grassBladeIndex * 3;
    }

    private void RenderGrass(GraphicsDevice graphicsDevice)
    {
        //Yuh
        _offset = Vector2.Zero;
        _backLayerInterp = 0;
        PrepareGrassVertices();
        if (_grassVertexIndex <= 0)
            return;

        //Prepare the graphics device

        var shader = GrassShader.Instance;
        shader.ApplyPasses();

        graphicsDevice.BlendState = CustomBlendStates.Max;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;

        graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            PrimitiveType.TriangleList, _grassVertices, 0, _grassVertexIndex / 3);

    }
    private void RenderGrassBack(GraphicsDevice graphicsDevice)
    {
        //Yuh
        _backLayerInterp = 0.55f;
        _offset = new Vector2(24, 0);
        PrepareDarkGrassVertices();
        if (_grassVertexIndex <= 0)
            return;

        //Prepare the graphics device

        var shader = GrassShader.Instance;
        shader.ApplyPasses();

        graphicsDevice.BlendState = CustomBlendStates.Max;
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        graphicsDevice.VertexSamplerStates[0] = SamplerState.PointClamp;

        graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            PrimitiveType.TriangleList, _grassVertices, 0, _grassVertexIndex / 3);

    }
}
