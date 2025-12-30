using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.DungeonGeneration
{
    public class Boss : DoorItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            doorToPlace = Door.Boss;
        }
    }
    public class Start : DoorItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            doorToPlace = Door.Start;
        }
    }
    public class AnchorTopRight : DoorItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            doorToPlace = Door.AnchorTopRight;
        }
    }
    public class AnchorBottomLeft : DoorItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            doorToPlace = Door.AnchorBottomLeft;
        }
    }
    public class DoorLeft : DoorItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            doorToPlace = Door.Left;
        }
    }
    public class DoorRight : DoorItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            doorToPlace = Door.Right;
        }
    }
    public class DoorUp : DoorItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            doorToPlace = Door.Up;
        }
    }
    public class DoorDown : DoorItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            doorToPlace = Door.Down;
        }
    }
    public abstract class DoorItem : ModItem
    {
        public Door doorToPlace;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            Point tilePosition = new Point();
            tilePosition.X = (int)(Main.MouseWorld.X / 16f);
            tilePosition.Y = (int)(Main.MouseWorld.Y / 16f);
            if (player.altFunctionUse == 2)
            {
                DungeonGenerationHelper.RemoveDoorInWorld(tilePosition);
            }
            else
            {
                DungeonGenerationHelper.PlaceDoorInWorld(tilePosition, doorToPlace);
            }
            //Place the door item at the tile position in the world
            return true;
        }
        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D texture = DungeonGenerationHelper.GetArrowTexture();
            Rectangle arrowFrame = DungeonGenerationHelper.GetArrowFrame(doorToPlace);
            spriteBatch.Draw(texture, position, arrowFrame, Color.White, 0, arrowFrame.Size() / 2f, scale, SpriteEffects.None, 0);
            return false;
        }
    }
   
    public class DungeonGenerationHelper : ModSystem
    {
        public const string FileExtension = ".drs";
        private static Dictionary<Point, Door> _doorsInWorld;
        public override void OnModLoad()
        {
            base.OnModLoad();
            _doorsInWorld = new();
            On_Main.DrawDust += DrawDebug;
        }


        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Main.DrawDust -= DrawDebug;
        }

        
        private void DrawDebug(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            if (_doorsInWorld.Count <= 0)
                return;


            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointWrap,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null,
                Main.GameViewMatrix.TransformationMatrix);
            foreach (var kvp in _doorsInWorld)
            {
                Vector2 position = kvp.Key.ToWorldCoordinates();
                Vector2 drawPosition = position - Main.screenPosition;
                Texture2D arrowTexture = GetArrowTexture();
                Rectangle frame = GetArrowFrame(kvp.Value);
                Vector2 drawOrigin = frame.Size() / 2f;
                spriteBatch.Draw(arrowTexture, drawPosition, frame, Color.White, 0, drawOrigin, 1, SpriteEffects.None, 0);
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Gets all anchors that are placed in the world
        /// </summary>
        /// <returns></returns>
        public static PlacedDoor[] GetAnchors()
        {
            List < PlacedDoor> anchors = new List<PlacedDoor>();
            foreach(var kvp in _doorsInWorld)
            {
                PlacedDoor placedDoor = new PlacedDoor
                {
                    point = kvp.Key,
                    door = kvp.Value
                };
                if(placedDoor.door == Door.AnchorBottomLeft || placedDoor.door == Door.AnchorTopRight)
                    anchors.Add(placedDoor);
            }
            return anchors.ToArray();
        }

        public static bool DoorInRectangle(Point bottomLeft, Point topRight)
        {
            for (int x = bottomLeft.X; x <= topRight.X; x++)
            {
                for (int y = bottomLeft.Y; y >= topRight.Y; y--)
                {
                    //tile
                    Point point = new Point(x, y);
                    if (_doorsInWorld.ContainsKey(point))
                        return true;
                }
            }
            return false;
        }
        public static void PlaceDoorInWorld(Point tilePosition, Door door)
        {
            if (MultiplayerHelper.IsHost || Main.netMode == NetmodeID.SinglePlayer)
            {
                if (!_doorsInWorld.TryAdd(tilePosition, door))
                {
                    _doorsInWorld[tilePosition] = door;
                }
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (!MultiplayerHelper.IsHost)
            {
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.HandleDoor,
                    tilePosition.X, tilePosition.Y, (int)door).Send(-1);
            }
        }
        public static void RemoveDoorInWorld(Point tilePosition)
        {
            if (MultiplayerHelper.IsHost || Main.netMode == NetmodeID.SinglePlayer)
            {
                if (_doorsInWorld.ContainsKey(tilePosition))
                    _doorsInWorld.Remove(tilePosition);
                NetMessage.SendData(MessageID.WorldData);
            }
            else if (!MultiplayerHelper.IsHost)
            {
                Stellamod.WriteToPacket(Stellamod.Instance.GetPacket(), (byte)MessageType.HandleDoor,
                    tilePosition.X, tilePosition.Y, -1).Send(-1);
            }
        }


        public static Texture2D GetArrowTexture()
        {
            string directoryHere = typeof(DungeonGenerationHelper).DirectoryHere();
            string arrowTexturePath = directoryHere + "/Door";
            Texture2D texture = ModContent.Request<Texture2D>(arrowTexturePath).Value;
            return texture;
        }
        public static Rectangle GetArrowFrame(Door door)
        {
            int yOffset = 0;
            switch (door)
            {
                default:
                case Door.Left:
                    yOffset = 0;
                    break;
                case Door.Up:
                    yOffset = 32;
                    break;
                case Door.Right:
                    yOffset = 64;
                    break;
                case Door.Down:
                    yOffset = 96;
                    break;
                case Door.AnchorBottomLeft:
                    yOffset = 128;
                    break;
                case Door.AnchorTopRight:
                    yOffset = 160;
                    break;
                case Door.Start:
                    yOffset = 192;
                    break;
                case Door.Boss:
                    yOffset = 224;
                    break;
            }
            Rectangle frame = new Rectangle(0, yOffset, 32, 32);
            return frame;
        }


        /// <summary>
        /// Saves a door structure to the stream
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="bottomLeft"></param>
        /// <param name="topRight"></param>
        public static void SaveDoors(Stream stream, Point bottomLeft, Point topRight)
        {
            var doors = _doorsInWorld;
            TagCompound root = new TagCompound();
            for (int x = bottomLeft.X; x <= topRight.X; x++)
            {
                for (int y = bottomLeft.Y; y >= topRight.Y; y--)
                {
                    //tile
                    Point point = new Point(x, y);
                    if (!doors.ContainsKey(point))
                        continue;

                    int xOffset = x - bottomLeft.X;
                    int yOffset = bottomLeft.Y - y;
                    TagCompound tag = new TagCompound();
                    tag["_x"] = xOffset;
                    tag["_y"] = yOffset;
                    tag["_door"] = doors[point];
                    root[$"{xOffset}{yOffset}"] = tag;
                }
            }

            //Save the tag compound to the file
            TagIO.ToStream(root, stream, compress: true);
            stream.Flush();
            DebugHelper.NewTextOnlyInTesting("Doors Structure Saved");
        }

        /// <summary>
        /// Reads a door structure from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bottomLeft"></param>
        public static void ReadStruct(Stream stream, Point bottomLeft)
        {
            _doorsInWorld.Clear();
            //Read the nested tag compound or whatever
            TagCompound root = TagIO.FromStream(stream, compressed: true);
            foreach (var tag in root)
            {
                TagCompound element = (TagCompound)tag.Value;
                int xOffset = element.Get<int>("_x");
                int yOffset = element.Get<int>("_y");
                Door door = element.Get<Door>("_door");
                DebugHelper.NewTextOnlyInTesting("Construct Door Entity " + door.ToString());

                Point point = new Point(bottomLeft.X + xOffset, bottomLeft.Y - yOffset);
                PlaceDoorInWorld(point, door);
            }
        }
        public static void ReadStruct(string Path, Point bottomLeft)
        {
            var Mod = Stellamod.Instance;
            string path = Path + FileExtension;
            if (!Mod.FileExists(path))
                return;
            using (var stream = Mod.GetFileStream(path))
            {
                ReadStruct(stream, bottomLeft);
            }

        }
        public static PlacedDoor[] DoorsFromStream(Stream stream)
        {

            List<PlacedDoor> placedDoors = new List<PlacedDoor>();
            //Read the nested tag compound or whatever
            TagCompound root = TagIO.FromStream(stream, compressed: true);
            foreach (var tag in root)
            {
                TagCompound element = (TagCompound)tag.Value;
                int xOffset = element.Get<int>("_x");
                int yOffset = element.Get<int>("_y");
                Door door = element.Get<Door>("_door");
                PlacedDoor placedDoor = new PlacedDoor
                {
                    door = door,
                    point = new Point(xOffset, yOffset)
                };
                placedDoors.Add(placedDoor);    
            }
            return placedDoors.ToArray();
        }
        public override void ClearWorld()
        {
            base.ClearWorld();
            _doorsInWorld.Clear();
        }

        public override void LoadWorldData(TagCompound tag)
        {
            base.LoadWorldData(tag);
            _doorsInWorld = new Dictionary<Point, Door>();
            var doorPoints = tag.Get<List<Point>>("doorpoints");
            var doors = tag.Get<List<Door>>("doors");
            for (int i = 0; i < doorPoints.Count; i++)
            {
                Point doorPoint = doorPoints[i];
                Door door = doors[i];
                _doorsInWorld.Add(doorPoint, door);
            }
        }
        public override void SaveWorldData(TagCompound tag)
        {
            base.SaveWorldData(tag);
            tag["doorpoints"] = _doorsInWorld.Keys.ToList();
            tag["doors"] = _doorsInWorld.Values.ToList();
        }
        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(_doorsInWorld.Count);
            foreach (var kvp in _doorsInWorld)
            {
                writer.Write(kvp.Key.X);
                writer.Write(kvp.Key.Y);
                writer.Write((int)kvp.Value);
            }
        }
        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            int count = reader.ReadInt32();
            _doorsInWorld.Clear();
            for (int s = 0; s < count; s++)
            {
                int x1 = reader.ReadInt32();
                int y1 = reader.ReadInt32();
                Door door = (Door)reader.ReadInt32();
                _doorsInWorld.Add(new Point(x1, y1), door);
            }
        }
    }
}
