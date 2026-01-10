using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.DungeonGeneration;
using Stellamod.Helpers;
using Stellamod.Tiles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
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
    public byte value;
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

public struct ZTileSaveData
{
    public int x;
    public int y;
    public int z;
    public ushort type;
    public int rotation;
    public bool flipX;
    public float scale;
    public int frameNumber;
    public byte value;
}

public class ZTileSerializer : TagSerializer<ZTileSaveData, TagCompound>
{
    public override ZTileSaveData Deserialize(TagCompound tag)
    {
        ZTileLoader tileLoader = ModContent.GetInstance<ZTileLoader>();
        ZTileSaveData deserializedData = new ZTileSaveData();
        deserializedData.x = tag.Get<int>("x");
        deserializedData.y = tag.Get<int>("y");
        deserializedData.z = tag.Get<int>("z");
        deserializedData.type = tileLoader.GetTile(tag.Get<string>("type")).type;
        deserializedData.rotation = tag.Get<int>("rotation");
        deserializedData.flipX = tag.Get<bool>("flipx");
        deserializedData.scale = tag.Get<float>("scale");
        deserializedData.frameNumber = tag.Get<int>("frameNumber");
        deserializedData.value = tag.Get<byte>("value");
        return deserializedData;
    }

    public override TagCompound Serialize(ZTileSaveData value)
    {
        /*
         *     public ushort type;
                public Rotation rotation;
                public bool flipX;
                public float scale;
                public ushort frameNumber;
         */
        return new TagCompound
        {
            ["x"] = value.x,
            ["y"] = value.y,
            ["z"] = value.z,
            ["type"] = ModContent.GetInstance<ZTileLoader>().GetTile(value.type).GetType().Name,
            ["rotation"] = (int)value.rotation,
            ["flipx"] = value.flipX,
            ["scale"] = value.scale,
            ["frameNumber"] = (int)value.frameNumber,
            ["value"] = (byte)value.value,
        };
    }
}
/// <summary>
/// Represents a collection of tiles to render
/// </summary>
public class TileScene : IEnumerable
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
            ZTileDrawParams drawParams = new ZTileDrawParams
            {
                tilePosition = tilePosition,
                tileData = tileData,
                lightColor = Lighting.GetColor(tilePosition.x, tilePosition.y)
            };
            tile.Draw(spriteBatch, screenPos, drawParams);
        }
    }
    public void RenderRedBoxes(SpriteBatch spriteBatch, Vector2 screenPos)
    {
        //At this point we can assume that everything in this scene is either on screen or very close to being on screen
        //So we should render everything within the scene
        var sortedDict = _tiles.OrderBy(x => x.Key.z);
        Rectangle frame = new Rectangle(0, 0, 16, 16);
        foreach (var kvp in sortedDict)
        {
            ZTilePosition tilePosition = kvp.Key;
            Vector2 position = new Vector2(tilePosition.x, tilePosition.y).ToWorldCoordinates();
            Vector2 drawPosition = position - screenPos;
            spriteBatch.Draw(TextureAssets.Tile[0].Value, drawPosition, frame, Color.Red, 0, frame.Size() / 2f, 1f, SpriteEffects.None, 0);
        }
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable)_tiles).GetEnumerator();
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
    public void Render(SpriteBatch spriteBatch, Vector2 screenPos, Point chunk, bool redBoxes = false)
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
            if (redBoxes)
            {
                scene.RenderRedBoxes(spriteBatch, screenPos);
            } else
            {
                scene.Render(spriteBatch, screenPos);
            }
    

        }
    }


    public TileScene[] GetScenes()
    {
        return _tileScenes.Values.ToArray();
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


    public override void SaveWorldData(TagCompound tag)
    {
        base.SaveWorldData(tag);
        List<List<ZTileSaveData>> tileDataList = new List<List<ZTileSaveData>>();
        for(int i = 0; i < _renderLayers.Length; i++)
        {
            var layer = _renderLayers[i];
            TileScene[] scenes = layer.GetScenes();
            List<ZTileSaveData> saveData = new List<ZTileSaveData>();
            for (int j = 0; j < scenes.Length; j++)
            {
                TileScene scene = scenes[j];

                foreach(KeyValuePair<ZTilePosition, ZTileInstanceData> tilePair in scene)
                {
                    ZTileSaveData tileSaveData = new ZTileSaveData();
                    tileSaveData.x = tilePair.Key.x;
                    tileSaveData.y = tilePair.Key.y;
                    tileSaveData.z = tilePair.Key.z;
                    tileSaveData.scale = tilePair.Value.scale;
                    tileSaveData.flipX = tilePair.Value.flipX;
                    tileSaveData.frameNumber = tilePair.Value.frameNumber;
                    tileSaveData.rotation = (int)tilePair.Value.rotation;
                    tileSaveData.type = tilePair.Value.type;
                    tileSaveData.value = tilePair.Value.value;
                    saveData.Add(tileSaveData);
                }

            }
            tileDataList.Add(saveData);
        }

        tag["zTileData"] = tileDataList;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        base.LoadWorldData(tag);
        List<List<ZTileSaveData>> tileDataList = tag.Get<List<List<ZTileSaveData>>>("zTileData");
        for(int i = 0; i < tileDataList.Count; i++)
        {
            ZTileRenderLayer layer = _renderLayers[i];
            List<ZTileSaveData> tileSaveDataList = tileDataList[i];
            for(int j = 0; j < tileSaveDataList.Count; j++)
            {
                ZTileSaveData saveData = tileSaveDataList[j];
                ZTilePosition zTilePosition = new ZTilePosition();
                zTilePosition.x = saveData.x;
                zTilePosition.y = saveData.y;
                zTilePosition.z = saveData.z;

                ZTileInstanceData instanceData = new ZTileInstanceData();
                instanceData.type = saveData.type;
                instanceData.rotation = (Rotation)saveData.rotation;
                instanceData.frameNumber = (ushort)saveData.frameNumber;
                instanceData.scale = saveData.scale;
                instanceData.flipX = saveData.flipX;
                instanceData.value = saveData.value;
                layer.Add(zTilePosition, instanceData);
            }
        }
    }

    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        for (int i = 0; i < _renderLayers.Length; i++)
        {
            var layer = _renderLayers[i];
            TileScene[] scenes = layer.GetScenes();
            for (int j = 0; j < scenes.Length; j++)
            {
                TileScene scene = scenes[j];
                foreach (KeyValuePair<ZTilePosition, ZTileInstanceData> tilePair in scene)
                {
                    writer.Write((byte)i);
                    writer.Write((ushort)tilePair.Key.x);
                    writer.Write((ushort)tilePair.Key.y);
                    writer.Write((ushort)tilePair.Key.z);
                    writer.Write((float)tilePair.Value.scale);
                    writer.Write((bool)tilePair.Value.flipX);
                    writer.Write((ushort)tilePair.Value.frameNumber);
                    writer.Write((byte)tilePair.Value.rotation);
                    writer.Write((ushort)tilePair.Value.type);
                    writer.Write((byte)tilePair.Value.value);
                }
            }
        }
    }

    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);
        //As long as there is a character we have more thingies
        while(reader.PeekChar() != -1)
        {
            ZRenderLayer renderLayer = (ZRenderLayer)reader.ReadByte();
            ZTilePosition tilePosition = new ZTilePosition();
            tilePosition.x = reader.ReadUInt16();
            tilePosition.y = reader.ReadUInt16();
            tilePosition.z = reader.ReadUInt16();

            ZTileInstanceData instanceData = new ZTileInstanceData();
            instanceData.scale = reader.ReadSingle();
            instanceData.flipX = reader.ReadBoolean();
            instanceData.frameNumber = reader.ReadUInt16();
            instanceData.rotation = (Rotation)reader.ReadByte();
            instanceData.type = reader.ReadUInt16();
            instanceData.value = reader.ReadByte();
            Add(renderLayer, tilePosition, instanceData);
        }
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

    public bool IsHoldingDecorationBuilder => Main.LocalPlayer.HeldItem.type == ModContent.ItemType<DecorationBuilder>();
    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        //Draw the preview for what you're placing
        if (!IsHoldingDecorationBuilder)
            return;
        ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
        ZTile tile = zTileLoader.GetTile(DecorationBuilder.templateData.type);

        Point tileCoordinates = Main.MouseWorld.ToTileCoordinates();
        ZTilePosition zTilePosition = new ZTilePosition();
        zTilePosition.x = tileCoordinates.X;
        zTilePosition.y = tileCoordinates.Y;
        zTilePosition.z = DecorationBuilder.z;

        ZTileInstanceData instanceData = zTileLoader.InstanceTileData(tile);
        instanceData.rotation = DecorationBuilder.rotation;
        instanceData.frameNumber = DecorationBuilder.frame;
        instanceData.scale = DecorationBuilder.scale;
        instanceData.flipX = DecorationBuilder.flip;
        instanceData.value = DecorationBuilder.value;
        ZTileDrawParams drawParams = new ZTileDrawParams
        {
            tilePosition = zTilePosition,
            tileData = instanceData,
            lightColor = Color.White * 0.75f
        };

        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        tile.Draw(spriteBatch, Main.screenPosition, drawParams);
        Rectangle frame = new Rectangle(0, 0, 16, 16);
        spriteBatch.Draw(TextureAssets.Tile[0].Value, Main.MouseWorld.ToTileCoordinates().ToWorldCoordinates() - Main.screenPosition, frame, Color.Green, 0, frame.Size() / 2f, 1f, SpriteEffects.None, 0);
        spriteBatch.End();


    }

    private void DrawBehindWalls()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Point chunk = GetCameraChunk();
        ZTileRenderLayer renderLayer = GetRenderLayer(ZRenderLayer.BehindWalls);
        renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        if (IsHoldingDecorationBuilder)
            renderLayer.Render(spriteBatch, Main.screenPosition, chunk, true);
    }

    private void DrawInFrontOfWalls()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Point chunk = GetCameraChunk();
        ZTileRenderLayer renderLayer = GetRenderLayer(ZRenderLayer.InFrontOfWalls);
        renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        if(IsHoldingDecorationBuilder)
            renderLayer.Render(spriteBatch, Main.screenPosition, chunk, true);
    }

    private void DrawInFrontOfPlayer()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Point chunk = GetCameraChunk();
        ZTileRenderLayer renderLayer = GetRenderLayer(ZRenderLayer.Midground);
        renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        if (IsHoldingDecorationBuilder)
            renderLayer.Render(spriteBatch, Main.screenPosition, chunk, true);
        spriteBatch.End();

    }
    private void DrawForeground()
    {
        SpriteBatch spriteBatch = Main.spriteBatch;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        Point chunk = GetCameraChunk();
        ZTileRenderLayer renderLayer = GetRenderLayer(ZRenderLayer.Foreground);
        renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        if (IsHoldingDecorationBuilder)
            renderLayer.Render(spriteBatch, Main.screenPosition, chunk, true);
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

    /// <summary>
    /// Kills a tile a chosen position
    /// </summary>
    /// <param name="renderLayer"></param>
    /// <param name="mouseWorld"></param>
    /// <param name="z"></param>
    public void KillTile(ZRenderLayer renderLayer, Vector2 mouseWorld, int z)
    {
        Point tileCoordinates = mouseWorld.ToTileCoordinates();
        ZTilePosition zTilePosition = new ZTilePosition();
        zTilePosition.x = tileCoordinates.X;
        zTilePosition.y = tileCoordinates.Y;
        zTilePosition.z = z;

        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            int clientToIgnore = Main.LocalPlayer.whoAmI;
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.BreakDecoration,
                (byte)renderLayer,
                (ushort)zTilePosition.x,
                (ushort)zTilePosition.y,
                (ushort)zTilePosition.z).Send(ignoreClient: clientToIgnore);
        }

        Remove(renderLayer, zTilePosition);
    }

    /// <summary>
    /// Creates a tile at the chosen position
    /// </summary>
    /// <param name="renderLayer"></param>
    /// <param name="worldPosition"></param>
    /// <param name="z"></param>
    /// <param name="tileData"></param>
    public void CreateTile(ZRenderLayer renderLayer, Vector2 worldPosition, int z, ZTileInstanceData tileData)
    {
        Point tileCoordinates = worldPosition.ToTileCoordinates();
        ZTilePosition zTilePosition = new ZTilePosition();
        zTilePosition.x = tileCoordinates.X;
        zTilePosition.y = tileCoordinates.Y;
        zTilePosition.z = z;

        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            int clientToIgnore = Main.LocalPlayer.whoAmI;
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.PlaceDecoration,
                (byte)renderLayer,
                (ushort)zTilePosition.x,
                (ushort)zTilePosition.y,
                (ushort)zTilePosition.z,
                (float)tileData.scale,
                (bool)tileData.flipX,
                (ushort)tileData.frameNumber,
                (byte)tileData.rotation,
                (ushort)tileData.type,
                (byte)tileData.value).Send(ignoreClient: clientToIgnore);
        }
        Add(renderLayer, zTilePosition, tileData);
    }

    public void SyncPlaceTile(int toWho, int fromWho, ZRenderLayer renderLayer, ZTilePosition tilePosition, ZTileInstanceData tileData)
    {
        int clientToIgnore = Main.LocalPlayer.whoAmI;
        Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.PlaceDecoration,
            (byte)renderLayer,
            (ushort)tilePosition.x,
            (ushort)tilePosition.y,
            (ushort)tilePosition.z,
            (float)tileData.scale,
            (bool)tileData.flipX,
            (ushort)tileData.frameNumber,
            (byte)tileData.rotation,
            (ushort)tileData.type,
            (byte)tileData.value).Send(toWho, fromWho);
    }
    public void SyncBreakTile(int toWho, int fromWho, ZRenderLayer renderLayer, ZTilePosition tilePosition)
    {
        int clientToIgnore = Main.LocalPlayer.whoAmI;
        Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.BreakDecoration,
            (byte)renderLayer,
            (ushort)tilePosition.x,
            (ushort)tilePosition.y,
            (ushort)tilePosition.z).Send(toWho, fromWho);
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
        for(int i = 0; i < _renderLayers.Length; i++)
        {
            _renderLayers[i].Clear();
        }
    }


}

