using Microsoft.CodeAnalysis.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace Stellamod.Core.ZTileSystem
{
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
    public struct ZTileData
    {
        public ushort type;
        public Rotation rotation;
        public bool flipX;
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
    /// Base class for a purely decorative tile asset
    /// </summary>
    public abstract class ZTile : ModTexturedType
    {
        public ushort type;
        public TilePlacementRules placementRules;
        public TileDrawOrigin drawOrigin;
        public Vector2 parallaxAmount;
        public int frameCount;
        protected override void Register()
        {
            ModTypeLookup<ZTile>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }
    }

    /// <summary>
    /// Represents a collection of tiles to render
    /// </summary>
    public class TileScene
    {
        private IDictionary<ZTilePosition, ZTileData> _tiles;
        public TileScene()
        {
            _tiles = new Dictionary<ZTilePosition, ZTileData>();
        }
        
        public void AddorSet(ZTilePosition tilePosition, ZTileData tileData)
        {
            if(_tiles.ContainsKey(tilePosition))
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

        public bool TryGet(ZTilePosition key, out ZTileData tileData)
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
            foreach(var kvp in sortedDict)
            {
              
                ZTilePosition tilePosition = kvp.Key;
                ZTileData tileData = kvp.Value;

                //Convert to world coordinates
                Point point = new Point(tilePosition.x, tilePosition.y);
                Vector2 worldCoordinates = point.ToWorldCoordinates();
                Vector2 drawPosition = worldCoordinates - screenPos;

                //Get the z tile
                ZTileLoader zTileLoader = ModContent.GetInstance<ZTileLoader>();
                ZTile tile = zTileLoader.GetTile(tileData.type);

                //TODO: index array instead of modcontent.request
                Asset<Texture2D> tileTextureAsset = ModContent.Request<Texture2D>(tile.Texture);
                
                //Calculate hte draworigin
                Vector2 drawOrigin;
                switch (tile.drawOrigin)
                {
                    default:
                    case TileDrawOrigin.BottomUp:
                        drawOrigin = new Vector2(tileTextureAsset.Width() / 2, tileTextureAsset.Height());
                        break;
                    case TileDrawOrigin.Center:
                        drawOrigin = new Vector2(tileTextureAsset.Width() / 2, tileTextureAsset.Height() / 2);
                        break;
                    case TileDrawOrigin.TopDown:
                        drawOrigin = new Vector2(tileTextureAsset.Width() / 2,0);
                        break;

                }

                //Divide by the frame count/number of variants
                int frameCount = Math.Max(tile.frameCount, 1);
                drawOrigin.Y /= frameCount;
        

                //TODO: Apply lighting
                Color drawColor = Color.White;
                Color lightingColor = Lighting.GetColor(tilePosition.x, tilePosition.y);
                drawColor = drawColor.MultiplyRGB(lightingColor);

                Rectangle? frame = null;

                float drawRotation;
                switch (tileData.rotation)
                {
                    default:
                    case Rotation.Degrees_0:
                        drawRotation = 0;
                        break;
                    case Rotation.Degrees_90:
                        drawRotation = MathHelper.PiOver2;
                        break;
                    case Rotation.Degrees_180:
                        drawRotation = MathHelper.Pi;
                        break;
                    case Rotation.Degrees_270:
                        drawRotation = MathHelper.PiOver2 + MathHelper.PiOver4;
                        break;
                }

                SpriteEffects spriteEffects = tileData.flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(tileTextureAsset.Value, drawPosition, frame, drawColor, drawRotation, drawOrigin, 1, spriteEffects, 0);
            }
        }
    }

    /// <summary>
    /// Maps the z tiles to a integer type for fast lookup
    /// </summary>
    public class ZTileLoader : ModSystem
    {
        public ZTile[] Tiles { get; private set; }
        public override void OnModLoad()
        {
            base.OnModLoad();
            Tiles = ModContent.GetContent<ZTile>().ToArray();
            for(ushort i = 0; i < Tiles.Length; i++)
            {
                Tiles[i].type = i;
            }
        }

        public ZTile GetTile(ushort type)
        {
            return Tiles[type];
        }

        public ZTileData InstanceTileData<T>() where T : ZTile
        {
            ZTile instance = ModContent.GetInstance<T>();
            ZTileData tileData = new ZTileData();
            tileData.type = instance.type;
            tileData.rotation = Rotation.Degrees_0;
            tileData.flipX = false;
            return tileData;
        }
    }

    public class ZTileRenderLayer
    {
        private TileScene[] _sceneRenderBuffer;
        private IDictionary<Point, TileScene> _tileScenes;
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
        public void Add(ZTilePosition tilePosition, ZTileData tileData)
        {
            //Calculate the chunk
            int chunkX = tilePosition.x / ZTileMap.Chunk_Size;
            int chunkY = tilePosition.y / ZTileMap.Chunk_Size;
            Point chunk = new Point(chunkX, chunkY);

            //Get the tile scene
            //If it doesn't exist we have to create a new one
            TileScene tileScene;
            if(!_tileScenes.TryGetValue(chunk, out tileScene))
            {
                tileScene = new TileScene();
                _tileScenes.Add(chunk, tileScene);
            }

            //Add it to the tile scene
            tileScene.AddorSet(tilePosition, tileData);
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

            for(int i = 0; i < index; i++)
            {
                TileScene scene = _sceneRenderBuffer[i];
                if (scene == null)
                    continue;
                scene.Render(spriteBatch, screenPos);
            }
        }
    }
    public class ZTileTest : ZTile
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
    }

    public class ZTileMap : ModSystem
    {
        private ZTileRenderLayer[] _renderLayers;
        public const int Chunk_Size = 50;
        public override void OnModLoad()
        {
            base.OnModLoad();
            int numLayers = Enum.GetValues<ZRenderLayer>().Length;
            
            //Initialize our render layers
            _renderLayers = new ZTileRenderLayer[numLayers];
            for(int i = 0; i < _renderLayers.Length; i++)
            {
                _renderLayers[i] = new ZTileRenderLayer();
            }
            On_OverlayManager.Draw += RenderOverWalls;
        }

        public override void Unload()
        {
            base.Unload();
            On_OverlayManager.Draw -= RenderOverWalls;
        }
        public override void PostUpdateEverything()
        {
            base.PostUpdateEverything();
            if(Main.mouseRight && Main.mouseRightRelease)
            {
                Console.WriteLine("Guh");
                Add(ZRenderLayer.InFrontOfWalls, Main.MouseWorld, 1, ModContent.GetInstance<ZTileLoader>().InstanceTileData<ZTileTest>());
            }
        }

        private void RenderOverWalls(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            orig(self, spriteBatch, layer, beginSpriteBatch);
            if(layer == RenderLayers.Walls)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                Point chunk = GetCameraChunk();
                ZTileRenderLayer renderLayer = GetRenderLayer(ZRenderLayer.InFrontOfWalls);
                renderLayer.Render(spriteBatch, Main.screenPosition, chunk);
            }
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
        
        public void Add(ZRenderLayer renderLayer, Vector2 worldPosition, int z, ZTileData tileData)
        {
            Point tileCoordinates = worldPosition.ToTileCoordinates();
            ZTilePosition zTilePosition = new ZTilePosition();
            zTilePosition.x = tileCoordinates.X;
            zTilePosition.y = tileCoordinates.Y;
            zTilePosition.z = z;
            Add(renderLayer, zTilePosition, tileData);
        }

        public void Add(ZRenderLayer renderLayer, ZTilePosition tilePosition, ZTileData tileData)
        {
            ZTileRenderLayer tileRenderLayer = GetRenderLayer(renderLayer);
            tileRenderLayer.Add(tilePosition, tileData);
        }
        public override void ClearWorld()
        {
            base.ClearWorld();
        }

    }
}
