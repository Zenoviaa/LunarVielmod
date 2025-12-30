using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using static Stellamod.WorldG.StructureManager.Snapshot;

namespace Stellamod.Core.ZTileSystem;

public enum ZRenderLayer : byte
{
    BehindWalls,
    InFrontOfWalls,
    Midground,
    Foreground,
}

public enum Rotation : byte
{
    Degrees_0,
    Degrees_90,
    Degrees_180,
    Degrees_270,
}

/// <summary>
/// Data structure for the decorative tile
/// </summary>
public struct ZTileInstanceData
{
    public ushort type;
    public Rotation rotation;
    public bool flipX;
    public float scale;
    public ushort frameNumber;
}

/// <summary>
/// Structure representing the positioning data for the Z-Tile
/// </summary>
public struct ZTilePosition
{
    public int x;
    public int y;
    public int z;
}

/// <summary>
/// How the tile should interact with solid tiles
/// </summary>
public enum TilePlacementRules : byte
{
    /// <summary>
    /// Can be placed anywhere
    /// </summary>
    None,

    /// <summary>
    /// Must be hanging from the ceiling, a tile must be above it.
    /// </summary>
    FromCeiling,

    /// <summary>
    /// Must be touching the ground, a tile must be below it
    /// </summary>
    Grounded
}

/// <summary>
/// How the tile should draw
/// </summary>
public enum TileDrawOrigin
{
    BottomUp,
    TopDown,
    Center,
}

/// <summary>
/// Represents a collection of tiles to render
/// </summary>
public class TileScene
{
    private IDictionary<ZTilePosition, ZTileInstanceData> _tiles;
    public TileScene()
    {
        _tiles = new Dictionary<ZTilePosition, ZTileInstanceData>();
    }

    public void AddorSet(ZTilePosition tilePosition, ZTileInstanceData tileData)
    {
        if (_tiles.ContainsKey(tilePosition))
            _tiles[tilePosition] = tileData;
        else
            _tiles.Add(tilePosition, tileData);
    }

    public void Remove(ZTilePosition tilePosition)
    {
        _tiles.Remove(tilePosition);
    }

    public void Clear()
    {
        _tiles.Clear();
    }

    public bool TryGet(ZTilePosition key, out ZTileInstanceData tileData)
    {
        if (_tiles.TryGetValue(key, out tileData))
            return true;
        return false;
    }

    public void Render(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        //At this point we can assume that everything in this scene is either on screen or very close to being on screen
        //So we should render everything within the scene
        var sortedDict = _tiles.OrderBy(x => x.Key.z);
        foreach (var kvp in sortedDict)
        {
            ZTilePosition tilePosition = kvp.Key;
            ZTileInstanceData tileData = kvp.Value;



            //Get the z tile
            ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
            ZTile tile = zTileLoader.GetTile(tileData.type);
            tile.Draw(spriteBatch, screenPos, tilePosition, tileData);
        }
    }
}

public class ZTileRenderLayer
{
    private readonly TileScene[] _sceneRenderBuffer;
    private readonly IDictionary<Point, TileScene> _tileScenes;
    public ZTileRenderLayer()
    {
        _sceneRenderBuffer = new TileScene[9];
        _tileScenes = new Dictionary<Point, TileScene>();
    }

    /// <summary>
    /// Adds a z tile to the render layer
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="tileData"></param>
    public void Add(ZTilePosition tilePosition, ZTileInstanceData tileData)
    {
        //Calculate the chunk
        int chunkX = tilePosition.x / ZTileMap.Chunk_Size;
        int chunkY = tilePosition.y / ZTileMap.Chunk_Size;
        Point chunk = new Point(chunkX, chunkY);

        //Get the tile scene
        //If it doesn't exist we have to create a new one
        TileScene tileScene;
        if (!_tileScenes.TryGetValue(chunk, out tileScene))
        {
            tileScene = new TileScene();
            _tileScenes.Add(chunk, tileScene);
        }

        //Add it to the tile scene
        tileScene.AddorSet(tilePosition, tileData);
    }
    public void Remove(ZTilePosition tilePosition)
    {
        //Calculate the chunk
        int chunkX = tilePosition.x / ZTileMap.Chunk_Size;
        int chunkY = tilePosition.y / ZTileMap.Chunk_Size;
        Point chunk = new Point(chunkX, chunkY);

        //Get the tile scene
        //If it doesn't exist we have to create a new one
        TileScene tileScene;
        if (!_tileScenes.TryGetValue(chunk, out tileScene))
        {
            tileScene = new TileScene();
            _tileScenes.Add(chunk, tileScene);
        }

        //Add it to the tile scene
        tileScene.Remove(tilePosition);
    }
    public void Clear()
    {
        _tileScenes.Clear();
    }

    /// <summary>
    /// Draws the entire scene, make sure to begin the spritebatch before calling this function
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="chunk"></param>
    public void Render(SpriteBatch spriteBatch, Vector2 screenPos, Point chunk)
    {
        //We have to get all of our chunks
        int index = 0;

        Point left = new Point(-1, 0);
        Point right = new Point(1, 0);
        Point up = new Point(0, -1);
        Point down = new Point(0, 1);

        _tileScenes.TryGetValue(chunk, out _sceneRenderBuffer[index++]);

        _tileScenes.TryGetValue(chunk + left, out _sceneRenderBuffer[index++]);
        _tileScenes.TryGetValue(chunk + right, out _sceneRenderBuffer[index++]);
        _tileScenes.TryGetValue(chunk + up, out _sceneRenderBuffer[index++]);
        _tileScenes.TryGetValue(chunk + down, out _sceneRenderBuffer[index++]);

        _tileScenes.TryGetValue(chunk + up + left, out _sceneRenderBuffer[index++]);
        _tileScenes.TryGetValue(chunk + up + right, out _sceneRenderBuffer[index++]);
        _tileScenes.TryGetValue(chunk + down + left, out _sceneRenderBuffer[index++]);
        _tileScenes.TryGetValue(chunk + down + right, out _sceneRenderBuffer[index++]);

        for (int i = 0; i < index; i++)
        {
            TileScene scene = _sceneRenderBuffer[i];
            if (scene == null)
                continue;
            scene.Render(spriteBatch, screenPos);
        }
    }


}

public class ZTileMap : ModSystem
{
    private ZTileRenderLayer[] _renderLayers;
    public const int Chunk_Size = 64;
    public override void OnModLoad()
    {
        base.OnModLoad();
        int numLayers = Enum.GetValues<ZRenderLayer>().Length;

        //Initialize our render layers
        _renderLayers = new ZTileRenderLayer[numLayers];
        for (int i = 0; i < _renderLayers.Length; i++)
        {
            _renderLayers[i] = new ZTileRenderLayer();
        }
        On_Main.DoDraw_WallsAndBlacks += RenderOverWalls;
        On_Main.DrawPlayers_AfterProjectiles += RenderOverPlayers;
        On_Main.DrawDust += RenderForeground;
    }


    public override void Unload()
    {
        base.Unload();
        On_Main.DoDraw_WallsAndBlacks -= RenderOverWalls;
        On_Main.DrawPlayers_AfterProjectiles -= RenderOverPlayers;
        On_Main.DrawDust -= RenderForeground;
    }


    private void RenderOverWalls(On_Main.orig_DoDraw_WallsAndBlacks orig, Main self)
    {
        DrawBehindWalls();
        orig(self);
        DrawInFrontOfWalls();
    }

    private void RenderOverPlayers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        orig(self);
        DrawInFrontOfPlayer();
    }

    private void RenderForeground(On_Main.orig_DrawDust orig, Main self)
    {
        orig(self);
        DrawForeground();
    }

    private void DrawBehindWalls()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Point chunk = GetCameraChunk();
        ZTileRenderLayer renderLayer = GetRenderLayer(ZRenderLayer.BehindWalls);
        renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
    }

    private void DrawInFrontOfWalls()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Point chunk = GetCameraChunk();
        ZTileRenderLayer renderLayer = GetRenderLayer(ZRenderLayer.InFrontOfWalls);
        renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
    }

    private void DrawInFrontOfPlayer()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Point chunk = GetCameraChunk();
        ZTileRenderLayer renderLayer = GetRenderLayer(ZRenderLayer.Midground);
        renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        spriteBatch.End();
    }
    private void DrawForeground()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Point chunk = GetCameraChunk();
        ZTileRenderLayer renderLayer = GetRenderLayer(ZRenderLayer.Foreground);
        renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        spriteBatch.End();
    }

    private ZTileRenderLayer GetRenderLayer(ZRenderLayer renderLayer)
    {
        int index = (int)renderLayer;
        ZTileRenderLayer tileRenderLayer = _renderLayers[index];
        return tileRenderLayer;
    }

    private Point GetCameraChunk()
    {
        Vector2 worldPosition = Main.Camera.Center;
        Point tilePosition = worldPosition.ToTileCoordinates();
        int chunkX = tilePosition.X / ZTileMap.Chunk_Size;
        int chunkY = tilePosition.Y / ZTileMap.Chunk_Size;
        Point chunk = new Point(chunkX, chunkY);
        return chunk;
    }
    public void KillTile(ZRenderLayer renderLayer, Vector2 mouseWorld, int z)
    {
        Point tileCoordinates = mouseWorld.ToTileCoordinates();
        ZTilePosition zTilePosition = new ZTilePosition();
        zTilePosition.x = tileCoordinates.X;
        zTilePosition.y = tileCoordinates.Y;
        zTilePosition.z = z;
        Remove(renderLayer, zTilePosition);
    }
    public void CreateTile(ZRenderLayer renderLayer, Vector2 worldPosition, int z, ZTileInstanceData tileData)
    {
        Add(renderLayer, worldPosition, z, tileData);
    }

    public void Add(ZRenderLayer renderLayer, Vector2 worldPosition, int z, ZTileInstanceData tileData)
    {
        Point tileCoordinates = worldPosition.ToTileCoordinates();
        ZTilePosition zTilePosition = new ZTilePosition();
        zTilePosition.x = tileCoordinates.X;
        zTilePosition.y = tileCoordinates.Y;
        zTilePosition.z = z;
        Add(renderLayer, zTilePosition, tileData);
    }

    public void Add(ZRenderLayer renderLayer, ZTilePosition tilePosition, ZTileInstanceData tileData)
    {
        ZTileRenderLayer tileRenderLayer = GetRenderLayer(renderLayer);
        tileRenderLayer.Add(tilePosition, tileData);
    }
    public void Remove(ZRenderLayer renderLayer, ZTilePosition tilePosition)
    {
        ZTileRenderLayer tileRenderLayer = GetRenderLayer(renderLayer);
        tileRenderLayer.Remove(tilePosition);
    }
    public override void ClearWorld()
    {
        base.ClearWorld();
    }


}
