using Microsoft.CodeAnalysis.Text;
using Stellamod.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

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

//So to fix this
//We drop the dictionary
//and instead straight up store a List of every ZTile in the world?

public class ZTileData
{
    public ZTileData()
    {

    }
    public ZTileData(ZTilePosition position, ZTileInstanceData instanceData, ZRenderLayer renderLayer)
    {
        this.position = position;
        this.instanceData = instanceData;
        this.renderLayer = renderLayer;
    }
    public ZTilePosition position;
    public ZTileInstanceData instanceData;
    public ZRenderLayer renderLayer;
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
    BottomLeft
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
            ["rotation"] = value.rotation,
            ["flipx"] = value.flipX,
            ["scale"] = value.scale,
            ["frameNumber"] = value.frameNumber,
            ["value"] = value.value,
        };
    }
}

public class ZTileMap : ModSystem
{
    private bool _needsResorting;
    private Point _lastChunk = new Point(-9999, -9999);
    private List<ZTileData> _zTileInstances = new List<ZTileData>();
    private List<ZTileData>[] _zTileActiveDrawingInstances;

    public const int Chunk_Size = 64;

    public static event Action OnRenderForeground;
    public ZTilePosition Find(ushort type)
    {
        ZTileData tileData = _zTileInstances.Find(x => x.instanceData.type == type)!;
        if (tileData != null)
            return tileData.position;
        return default;
    }

    public override void OnModLoad()
    {
        base.OnModLoad();
        _zTileInstances = new List<ZTileData>();
        _zTileActiveDrawingInstances = new List<ZTileData>[Enum.GetValues<ZRenderLayer>().Length];
        for(int i = 0; i < _zTileActiveDrawingInstances.Length; i++)
        {
            _zTileActiveDrawingInstances[i] = new List<ZTileData>();
        }
        On_Main.DoDraw_WallsAndBlacks += RenderOverWalls;
        On_Main.DrawPlayers_AfterProjectiles += RenderOverPlayers;
        On_Main.DrawDust += RenderForeground;
    }

    public override void PostUpdateEverything()
    {
        base.PostUpdateEverything();
        Point chunk = GetCameraChunk();
        if (_lastChunk == chunk)
            return;

        _lastChunk = chunk;
        CollectInstanceData(chunk);
    }

    public void Refresh()
    {
        _lastChunk = new Point(-9999, -9999);
        _needsResorting = true;
    }
    public void CollectInstanceData(in Point currentChunk)
    {
    //    Stopwatch instanceDataWatch = Stopwatch.StartNew();
        if (_needsResorting)
        {
            _zTileInstances = _zTileInstances.OrderBy(X => X.position.z).ToList();
            _needsResorting = false;
        }
       
        for(int i = 0; i < _zTileActiveDrawingInstances.Length; i++)
        {
            _zTileActiveDrawingInstances[i].Clear();
        }
        foreach(ZTileData tileData in _zTileInstances)
        {
            //Calculate the chunk
            int chunkX = tileData.position.x / ZTileMap.Chunk_Size;
            int chunkY = tileData.position.y / ZTileMap.Chunk_Size;
            Point chunk = new Point(chunkX, chunkY);
            int dx = Math.Abs(chunk.X - currentChunk.X);
            int dy = Math.Abs(chunk.Y - currentChunk.Y);

            //If not adjacent or inside don't render
            if (dx + dy > 2)
                continue;
            int index = (int)tileData.renderLayer;
            _zTileActiveDrawingInstances[index].Add(tileData);
        }
    //    instanceDataWatch.Stop();
    //    Mod.Logger.Info($"{instanceDataWatch.ElapsedTicks} collect z tile data ticks");
    }

    public void RenderRedBoxesLayer(SpriteBatch spriteBatch, in List<ZTileData> drawingData)
    {
        Rectangle frame = new Rectangle(0, 0, 16, 16);
        foreach (var tileData in drawingData)
        {
            ZTilePosition tilePosition = tileData.position;
            Vector2 position = new Vector2(tilePosition.x, tilePosition.y).ToWorldCoordinates();
            Vector2 drawPosition = position - Main.screenPosition;
            spriteBatch.Draw(TextureAssets.Tile[0].Value, drawPosition, frame, Color.Red, 0, frame.Size() / 2f, 1f, SpriteEffects.None, 0);
        }
    }
    public void RenderLayer(SpriteBatch spriteBatch, in List<ZTileData> drawingData)
    {
        ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
        foreach (var zTile in drawingData)
        {
            ZTilePosition tilePosition = zTile.position;
            ZTileInstanceData tileData = zTile.instanceData;

            //Get the z tile
            ZTile tile = zTileLoader.GetTile(tileData.type);
            ZTileDrawParams drawParams = new ZTileDrawParams
            {
                tilePosition = tilePosition,
                tileData = tileData,
                lightColor = Lighting.GetColor(tilePosition.x, tilePosition.y)
            };

            if (tile.interactable)
            {
                (int width, int height) = tile.GetBounds();
                (float topLeftX, float topLeftY) = (0, 0);


                Vector2 worldCoordinates = new Point(tilePosition.x, tilePosition.y).ToWorldCoordinates();
                topLeftX = worldCoordinates.X;
                topLeftY = worldCoordinates.Y;

                //TODO: take into account draw origin
                topLeftX -= width / 2;
                topLeftY -= height;

                topLeftX -= Main.screenPosition.X;
                topLeftY -= Main.screenPosition.Y;
                Rectangle selectionBoundary = new Rectangle((int)topLeftX, (int)topLeftY, width, height);

                Vector2 mouseWorld = Main.MouseScreen;
                if (selectionBoundary.Contains((int)mouseWorld.X, (int)mouseWorld.Y))
                {
                    if (Main.mouseRight && Main.mouseRightRelease)
                    {
                        tile.RightClick(new Point(tilePosition.x, tilePosition.y));
                        Main.mouseRightRelease = false;
                    }
                    tile.DrawOutline(spriteBatch, Main.screenPosition, drawParams);
                }
            }
            tile.Draw(spriteBatch, Main.screenPosition, drawParams);
        }
    }
   

    public override void SaveWorldData(TagCompound tag)
    {
        base.SaveWorldData(tag);
        List<List<ZTileSaveData>> tileDataList = new List<List<ZTileSaveData>>();
        for(int i = 0; i < 4; i++)
            tileDataList.Add(new());

        for(int i = 0; i < _zTileInstances.Count; i++)
        {
            var tileData = _zTileInstances[i];
            ZTileSaveData tileSaveData = new ZTileSaveData();
            tileSaveData.x = tileData.position.x;
            tileSaveData.y = tileData.position.y;
            tileSaveData.z = tileData.position.z;
            tileSaveData.scale = tileData.instanceData.scale;
            tileSaveData.flipX = tileData.instanceData.flipX;
            tileSaveData.frameNumber = tileData.instanceData.frameNumber;
            tileSaveData.rotation = (int)tileData.instanceData.rotation;
            tileSaveData.type = tileData.instanceData.type;
            tileSaveData.value = tileData.instanceData.value;
            tileDataList[(int)tileData.renderLayer].Add(tileSaveData);
        }

        tag["zTileData"] = tileDataList;
    }

    public override void LoadWorldData(TagCompound tag)
    {
        base.LoadWorldData(tag);
        _zTileInstances.Clear();
        List<List<ZTileSaveData>> tileDataList = tag.Get<List<List<ZTileSaveData>>>("zTileData");
        for (int i = 0; i < tileDataList.Count; i++)
        {
            List<ZTileSaveData> tileSaveDataList = tileDataList[i];
            for (int j = 0; j < tileSaveDataList.Count; j++)
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
                ZTileData zTileData = new ZTileData(zTilePosition, instanceData, (ZRenderLayer)i);
                _zTileInstances.Add(zTileData);
            }
        }
        Refresh();
    }

    public void SaveTileData(TagCompound tag, Rectangle worldBounds, Point bottomLeft)
    {
        List<List<ZTileSaveData>> tileDataList = new List<List<ZTileSaveData>>();
        for (int i = 0; i < 4; i++)
            tileDataList.Add(new());
        for (int i = 0; i < _zTileInstances.Count; i++)
        {
            var tileData = _zTileInstances[i];
            if (!worldBounds.Contains(new Point(tileData.position.x, tileData.position.y)))
                continue;


            int xOffset = tileData.position.x - bottomLeft.X;
            int yOffset = bottomLeft.Y - tileData.position.y;

            ZTileSaveData tileSaveData = new ZTileSaveData();
            tileSaveData.x = xOffset;
            tileSaveData.y = yOffset;
            tileSaveData.z = tileData.position.z;
            tileSaveData.scale = tileData.instanceData.scale;
            tileSaveData.flipX = tileData.instanceData.flipX;
            tileSaveData.frameNumber = tileData.instanceData.frameNumber;
            tileSaveData.rotation = (int)tileData.instanceData.rotation;
            tileSaveData.type = tileData.instanceData.type;
            tileSaveData.value = tileData.instanceData.value;
            tileDataList[(int)tileData.renderLayer].Add(tileSaveData);
        }


        if (tileDataList.Count <= 0)
            return;

        tag["zTileData"] = tileDataList;
    }

    public void LoadTileData(TagCompound tag, Point bottomLeft)
    {
        List<List<ZTileSaveData>> tileDataList = tag.Get<List<List<ZTileSaveData>>>("zTileData");
        for (int i = 0; i < tileDataList.Count; i++)
        {
            List<ZTileSaveData> tileSaveDataList = tileDataList[i];
            for (int j = 0; j < tileSaveDataList.Count; j++)
            {
                ZTileSaveData saveData = tileSaveDataList[j];
                ZTilePosition zTilePosition = new ZTilePosition();

                int x = bottomLeft.X + saveData.x;
                int y = bottomLeft.Y - saveData.y;


                zTilePosition.x = x;
                zTilePosition.y = y;
                zTilePosition.z = saveData.z;

                ZTileInstanceData instanceData = new ZTileInstanceData();
                instanceData.type = saveData.type;
                instanceData.rotation = (Rotation)saveData.rotation;
                instanceData.frameNumber = (ushort)saveData.frameNumber;
                instanceData.scale = saveData.scale;
                instanceData.flipX = saveData.flipX;
                instanceData.value = saveData.value;
                ZTileData zTileData = new ZTileData(zTilePosition, instanceData, (ZRenderLayer)i);
                _zTileInstances.Add(zTileData);
            }
        }
        Refresh();
    }

    public override void NetSend(BinaryWriter writer)
    {
        base.NetSend(writer);
        SendZTileSyncPacket();
    }


    public override void NetReceive(BinaryReader reader)
    {
        base.NetReceive(reader);

    }

    public void SendZTileSyncPacket()
    {
        //We need a completely separate packet to sync this, so we just send this when world data gets sent
        //Should work just fine lol
        try
        {
            ModPacket packet = Stellamod.Instance.GetPacket(capacity: 65536);
            packet.Write((byte)MessageType.ZTileSync);
            packet.Write(_zTileInstances.Count);
            for (int i = 0; i < _zTileInstances.Count; i++)
            {
                var tileData = _zTileInstances[i];
                packet.Write((byte)i);
                packet.Write((ushort)tileData.position.x);
                packet.Write((ushort)tileData.position.y);
                packet.Write((ushort)tileData.position.z);
                packet.Write(tileData.instanceData.scale);
                packet.Write(tileData.instanceData.flipX);
                packet.Write(tileData.instanceData.frameNumber);
                packet.Write((byte)tileData.instanceData.rotation);
                packet.Write(tileData.instanceData.type);
                packet.Write(tileData.instanceData.value);
            }
            packet.Send();
        }
        catch (System.Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public void HandleZTileSyncPacket(BinaryReader reader)
    {
        _zTileInstances.Clear();
        int length = reader.ReadInt32();
        for (int i = 0; i < length; i++)
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
        OnRenderForeground?.Invoke();
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

    private List<ZTileData> GetZTileDatas(ZRenderLayer renderLayer)
    {
        return _zTileActiveDrawingInstances[(int)renderLayer];
    }

    private void DrawBehindWalls()
    {
        var data = GetZTileDatas(ZRenderLayer.BehindWalls);
        if (data.Count <= 0)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        RenderLayer(spriteBatch, data);
        //renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        if (IsHoldingDecorationBuilder)
            RenderRedBoxesLayer(spriteBatch, data);
    }

    private void DrawInFrontOfWalls()
    {
        var data = GetZTileDatas(ZRenderLayer.InFrontOfWalls);
        if (data.Count <= 0)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.End();
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        RenderLayer(spriteBatch, data);
        //renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        if (IsHoldingDecorationBuilder)
            RenderRedBoxesLayer(spriteBatch, data);
    }

    private void DrawInFrontOfPlayer()
    {
        var data = GetZTileDatas(ZRenderLayer.Midground);
        if (data.Count <= 0)
            return;

        SpriteBatch spriteBatch = Main.spriteBatch;
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        RenderLayer(spriteBatch, data);
        //renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        if (IsHoldingDecorationBuilder)
            RenderRedBoxesLayer(spriteBatch, data);
        spriteBatch.End();
    }
    private void DrawForeground()
    {
        var data = GetZTileDatas(ZRenderLayer.Foreground);
        if (data.Count <= 0)
            return;
        SpriteBatch spriteBatch = Main.spriteBatch;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        RenderLayer(spriteBatch, data);
        //renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
        if (IsHoldingDecorationBuilder)
            RenderRedBoxesLayer(spriteBatch, data);
        spriteBatch.End();
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
    public void KillTile(Vector2 mouseWorld)
    {
        Point tileCoordinates = mouseWorld.ToTileCoordinates();
        KillAnyTile(tileCoordinates);
        if (Main.netMode != NetmodeID.SinglePlayer)
        {
            int clientToIgnore = Main.LocalPlayer.whoAmI;
            Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.BreakDecoration,
                (ushort)tileCoordinates.X,
                (ushort)tileCoordinates.Y).Send(ignoreClient: clientToIgnore);
        }

    }

    /// <summary>
    /// Kills any tile at any z layer at these tile coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void KillAnyTile(in int x, in int y) => KillAnyTile(new Point(x, y));

    /// <summary>
    /// Kills any tile at any z layer at this position
    /// </summary>
    /// <param name="tileCoordinates"></param>
    public void KillAnyTile(Point tileCoordinates)
    {
        _zTileInstances.RemoveAll(x => x.position.x == tileCoordinates.X && x.position.y == tileCoordinates.Y);
        Refresh();
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
                tileData.scale,
                tileData.flipX,
                tileData.frameNumber,
                (byte)tileData.rotation,
                tileData.type,
                tileData.value).Send(ignoreClient: clientToIgnore);
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
            tileData.scale,
            tileData.flipX,
            tileData.frameNumber,
            (byte)tileData.rotation,
            tileData.type,
            tileData.value).Send(toWho, fromWho);
    }
    public void SyncBreakTile(int toWho, int fromWho, Point tilePosition)
    {
        int clientToIgnore = Main.LocalPlayer.whoAmI;
        Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.BreakDecoration,
            (ushort)tilePosition.X,
            (ushort)tilePosition.Y).Send(toWho, fromWho);
    }



    public void Add(ZRenderLayer renderLayer, ZTilePosition tilePosition, ZTileInstanceData tileData)
    {
        _zTileInstances.Add(new ZTileData(tilePosition, tileData, renderLayer));
    }

    public override void ClearWorld()
    {
        base.ClearWorld();
        Refresh();
    }


}

