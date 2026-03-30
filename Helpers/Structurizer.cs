using Stellamod.Common.DungeonGeneration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace Stellamod.Helpers
{
    public struct StructurePlacementParams
    {
        public Point tile;
        public string structurePath;
        public int[] tileBlend;
    }
    /// <summary>
    /// Class holding methods to save/load structs to binary .str files
    /// </summary>
    public static class Structurizer
    {
        static Mod Mod => Stellamod.Instance;
        public static event Action<Point, string> OnStructPlace;
        public static bool FlipStructure;
        public static string SelectedStructure = string.Empty;
        public static Rectangle ReadRectangle(string Path)
        {
            try
            {
                string path = Path;
                if (!path.Contains(".str"))
                    path += ".str";
                using var stream = Mod.GetFileStream(path);
                return ReadRectangle(stream);
            } catch(KeyNotFoundException ex)
            {
                return Rectangle.Empty;
            }

        }

        public static int[] DefaultTileBlend = new int[] { TileID.RubyGemspark };
        public static int[] PlaceAndProtect(StructurePlacementParams @params)
        {
            int[] chestIndices = Structurizer.ReadStruct(@params.tile, @params.structurePath, @params.tileBlend);
            Structurizer.ProtectStructure(@params.tile, @params.structurePath);
            return chestIndices;
        }
        public static Rectangle ReadRectangle(Stream stream)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, false);
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();
            Rectangle rectangle = new Rectangle(0, 0, width, height);
            return rectangle;
        }

        public static void ProtectStructure(Point location, string path, StructureMap structures = null)
        {

            structures ??= GenVars.structures;
            structures ??= new StructureMap();
            Rectangle rectangle = ReadRectangle(path);
            location.Y -= rectangle.Height;
            rectangle.Location = location;
            structures.AddProtectedStructure(rectangle);
        }

        public static bool SafePlaceAndProtectStructure(Point tilePoint, string structureFile, StructureMap structures, int[] tileBlend, out int[] chestIndices)
        {
            Rectangle rectangle = ReadRectangle(structureFile);
            Point protectionPoint = tilePoint;
            protectionPoint.Y -= rectangle.Height;

            rectangle.Location = protectionPoint;

            if (!structures.CanPlace(rectangle))
            {
                chestIndices = null;
                return false;
            }

            structures.AddProtectedStructure(rectangle);
            chestIndices = ReadStruct(tilePoint, structureFile, tileBlend);
            return true;
        }

        public static bool TryPlaceAndProtectStructure(Point location, string path, bool ignoreStructures = false)
        {
            StructureMap structures = GenVars.structures;
            Rectangle rectangle = ReadRectangle(path);
            location.Y -= rectangle.Height;
            rectangle.Location = location;


            int[] tilesToCheckFor = new int[]
            {
                TileID.LihzahrdBrick,
                TileID.BlueDungeonBrick,
                TileID.CrackedBlueDungeonBrick,
                TileID.CrackedGreenDungeonBrick,
                TileID.CrackedPinkDungeonBrick,
                TileID.PinkDungeonBrick,
                TileID.GreenDungeonBrick,
            };

            //Temple Check
            for (int j = 0; j < rectangle.Width; j++)
            {
                for (int i = 0; i < rectangle.Height; i++)
                {
                    int x = location.X + j;
                    int y = location.Y - i;
                    if (x >= Main.maxTilesX || y >= Main.maxTilesY)
                    {
                        return false;
                    }

                    Tile otherTile = Main.tile[x, y];
                    for (int t = 0; t < tilesToCheckFor.Length; t++)
                    {
                        if (tilesToCheckFor[t] == otherTile.TileType)
                            return false;
                    }
                }
            }

            if (!ignoreStructures && !structures.CanPlace(rectangle))
                return false;

            structures.AddProtectedStructure(rectangle);
            return true;
        }

        public static Tile ReadTile(BinaryReader reader)
        {
            Tile t = new Tile();
            t.ClearEverything();
            //tile
            bool hastile = reader.ReadBoolean();
            t.LiquidType = reader.ReadInt32();
            t.LiquidAmount = reader.ReadByte();
            t.BlueWire = reader.ReadBoolean();
            t.RedWire = reader.ReadBoolean();
            t.GreenWire = reader.ReadBoolean();
            t.YellowWire = reader.ReadBoolean();
            t.HasActuator = reader.ReadBoolean();
            t.IsActuated = reader.ReadBoolean();
            if (hastile)
            {
                t.HasTile = hastile;
                bool Modded = reader.ReadBoolean();
                int TileType = 0;
                if (Modded)
                {
                    TileType = ReadModTile(reader);
                }
                else
                {
                    TileType = reader.ReadInt16();
                }

                t.TileType = (ushort)TileType;
                t.BlockType = (BlockType)reader.ReadByte();
                t.IsHalfBlock = reader.ReadBoolean();
                //t.LiquidType = reader.ReadInt32();
                t.Slope = (SlopeType)reader.ReadByte();
                t.TileFrameNumber = reader.ReadInt32();
                t.TileFrameX = reader.ReadInt16();
                t.TileFrameY = reader.ReadInt16();
                t.TileColor = reader.ReadByte();
                t.IsTileInvisible = reader.ReadBoolean();
                t.IsTileFullbright = reader.ReadBoolean();
                //byte slope = reader.ReadByte();

                bool Chest = reader.ReadBoolean();
            }


            //wall
            int WallType = 0;
            bool ModdedWall = reader.ReadBoolean();
            if (ModdedWall)
            {
                WallType = ReadModWall(reader);
            }
            else
            {
                WallType = reader.ReadInt16();
            }
            t.WallType = (ushort)WallType;
            t.WallFrameX = reader.ReadInt32();
            t.WallFrameY = reader.ReadInt32();
            t.WallColor = reader.ReadByte();
            t.IsWallInvisible = reader.ReadBoolean();
            t.IsWallFullbright = reader.ReadBoolean();
            return t;
        }

        private static int[] ReadStruct(Stream stream, Point bottomLeft, int[] tileBlend = null)
        {
            using var reader = new BinaryReader(stream, Encoding.UTF8, false);
            List<int> ChestIndexs = new List<int>();
            int Xlenght = reader.ReadInt32();
            int Ylenght = reader.ReadInt32();
            void InnerLoop(int i, int j)
            {
                Tile t;
                if (FlipStructure)
                {
                    t = Framing.GetTileSafely(bottomLeft.X + Xlenght - i, bottomLeft.Y - j);
                }
                else
                {
                    t = Framing.GetTileSafely(bottomLeft.X + i, bottomLeft.Y - j);

                }


                //Get old values incase we don't want this tile
                int oldLiquidType = t.LiquidType;
                byte oldLiquidAmount = t.LiquidAmount;
                bool oldBlueWire = t.BlueWire;
                bool oldGreenWire = t.GreenWire;
                bool oldYellowWire = t.YellowWire;
                bool oldHasActuator = t.HasActuator;
                bool oldIsActuated = t.IsActuated;
                bool oldHasTile = t.HasTile;
                ushort oldTileType = t.TileType;
                BlockType oldBlockType = t.BlockType;
                bool oldIsHalfBlock = t.IsHalfBlock;
                SlopeType oldSlopeType = t.Slope;
                int oldTileFrameNumber = t.TileFrameNumber;
                short oldTileFrameX = t.TileFrameX;
                short oldTileFrameY = t.TileFrameY;
                ushort oldWallType = t.WallType;
                int oldWallFrameX = t.WallFrameX;
                int oldWallFrameY = t.WallFrameY;
                byte oldTileColor = t.TileColor;
                byte oldWallColor = t.WallColor;
                bool makeOld = false;
                t.ClearEverything();
                //tile
                bool hastile = reader.ReadBoolean();
                t.LiquidType = reader.ReadInt32();
                t.LiquidAmount = reader.ReadByte();
                t.BlueWire = reader.ReadBoolean();
                t.RedWire = reader.ReadBoolean();
                t.GreenWire = reader.ReadBoolean();
                t.YellowWire = reader.ReadBoolean();
                t.HasActuator = reader.ReadBoolean();
                t.IsActuated = reader.ReadBoolean();
                if (hastile)
                {
                    t.HasTile = hastile;
                    bool Modded = reader.ReadBoolean();
                    int TileType = 0;
                    if (Modded)
                    {
                        TileType = ReadModTile(reader);
                    }
                    else
                    {
                        TileType = reader.ReadInt16();
                        if (tileBlend != null)
                        {
                            for (int tb = 0; tb < tileBlend.Length; tb++)
                            {
                                int tbTileType = tileBlend[tb];
                                if (TileType == tbTileType)
                                {
                                    makeOld = true;
                                    break;
                                }

                            }
                        }
                        else
                        {

                        }
                    }

                    t.TileType = (ushort)TileType;
                    t.BlockType = (BlockType)reader.ReadByte();
                    t.IsHalfBlock = reader.ReadBoolean();
                    //t.LiquidType = reader.ReadInt32();
                    t.Slope = (SlopeType)reader.ReadByte();
                    t.TileFrameNumber = reader.ReadInt32();
                    t.TileFrameX = reader.ReadInt16();
                    t.TileFrameY = reader.ReadInt16();

                    t.TileColor = reader.ReadByte();
                    t.IsTileInvisible = reader.ReadBoolean();
                    t.IsTileFullbright = reader.ReadBoolean();

                    bool Chest = reader.ReadBoolean();
                    if (Chest)
                    {
                        ChestIndexs.Add(Terraria.Chest.CreateChest(bottomLeft.X + i, bottomLeft.Y - j));
                    }
                    //byte slope = reader.ReadByte();


                }

                //Auto air blend
                if (tileBlend == null && !hastile)
                {
                    makeOld = true;
                }

                //wall
                int WallType = 0;
                bool ModdedWall = reader.ReadBoolean();
                if (ModdedWall)
                {
                    WallType = ReadModWall(reader);
                }
                else
                {
                    WallType = reader.ReadInt16();
                }
                t.WallType = (ushort)WallType;
                t.WallFrameX = reader.ReadInt32();
                t.WallFrameY = reader.ReadInt32();
                t.WallColor = reader.ReadByte();
                t.IsWallInvisible = reader.ReadBoolean();
                t.IsWallFullbright = reader.ReadBoolean();

                if (makeOld && t.WallType == 0 && t.LiquidAmount <= 0)
                {
                    t.LiquidType = oldLiquidType;
                    t.LiquidAmount = oldLiquidAmount;
                    t.BlueWire = oldBlueWire;
                    t.GreenWire = oldGreenWire;
                    t.YellowWire = oldGreenWire;
                    t.HasActuator = oldHasActuator;
                    t.IsActuated = oldIsActuated;
                    t.HasTile = oldHasTile;
                    t.TileType = oldTileType;
                    t.BlockType = oldBlockType;
                    t.IsHalfBlock = oldIsHalfBlock;
                    t.Slope = oldSlopeType;
                    t.TileFrameNumber = oldTileFrameNumber;
                    t.TileFrameX = oldTileFrameX;
                    t.TileFrameY = oldTileFrameY;
                    t.WallType = oldWallType;
                    t.WallFrameX = oldWallFrameX;
                    t.WallFrameY = oldWallFrameY;
                    t.TileColor = oldTileColor;
                    t.WallColor = oldWallColor;
                }
            }
            for (int i = 0; i <= Xlenght; i++)
            {
                for (int j = 0; j <= Ylenght; j++)
                {
                    InnerLoop(i, j);
                }
            }



            return ChestIndexs.ToArray();
        }

        public static string[] GetPaths()
        {
            List<string> paths = new List<string>();
            List<string> fileNames = Mod.GetFileNames();
            foreach (var fileName in fileNames)
            {
                if (fileName.Contains(".str"))
                    paths.Add(fileName);
            }
            return paths.ToArray();
        }

        private static void ClearTrees(Point bottomLeft, string path)
        {
            Rectangle rectangle = ReadRectangle(path);
            rectangle.Location = bottomLeft - new Point(0, rectangle.Height);

            int startX = rectangle.Location.X;
            int endX = startX + rectangle.Width;
            int startY = rectangle.Location.Y;
            int endY = rectangle.Location.Y + rectangle.Height;
            
            //Some fluff to get rid of trees
            startY -= 32;
            for (int x = startX; x < endX; x++)
            {
                for (int y = startY; y < endY; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    Tile tile = Main.tile[x, y];
                    if (TileID.Sets.IsATreeTrunk[tile.TileType])
                    {
                        tile.ClearEverything();
                    }
                }
            }
        }
        /// <summary>
        /// reads a .str file and places its structure
        /// </summary>
        /// <param name="BottomLeft"> bottom left of the placed structure</param>
        /// <param name="Path">path, starting past the mod root folder to read the .str from. Do not inculde the name of the mod in the path, or .str</param>
        /// <returns>A array of ints, corrsponding to the index of chests placed in the struct, from bottom left to top right</returns>
        public static int[] ReadStruct(Point BottomLeft, string Path, int[] tileBlend = null)
        {
            ClearTrees(BottomLeft, Path);
            using Stream stream = Mod.GetFileStream(Path + ".str");
            OnStructPlace?.Invoke(BottomLeft, Path);

            int[] indices = ReadStruct(stream, BottomLeft, tileBlend);

            TriggerStructurizer.ReadStruct(Path, BottomLeft);
            TileEntityStructurizer.ReadStruct(Path, BottomLeft);
            ZTileStructurizer.ReadStruct(Path, BottomLeft);
            DungeonGenerationHelper.ReadStruct(Path, BottomLeft);
            return indices;

        }

        public static int[] ReadSavedStruct(string filePath, Point BottomLeft, int[] tileBlend = null)
        {

            if (!filePath.Contains(".str"))
                filePath += ".str";
            string savedPath = Main.SavePath + "/ModSources/" + Mod.Name + "/" + filePath;

            ClearTrees(BottomLeft, savedPath);
            using FileStream stream = File.Open(savedPath, FileMode.Open);
            return ReadStruct(stream, BottomLeft, tileBlend);
        }
        public static Rectangle ReadSavedRectangle(string filePath)
        {
            if (!filePath.Contains(".str"))
                filePath += ".str";
            string savedPath = Main.SavePath + "/ModSources/" + Mod.Name + "/" + filePath;
            if (!File.Exists(savedPath))
            {
                return new Rectangle(0, 0, 1, 1);
            }

            using FileStream stream = File.Open(savedPath, FileMode.Open);
            return ReadRectangle(stream);
        }
        public static Rectangle ReadRectangleDirect(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new Rectangle(0, 0, 1, 1);
            }

            using FileStream stream = File.Open(filePath, FileMode.Open);
            return ReadRectangle(stream);
        }


        public static int ReadModWall(BinaryReader reader)
        {

            string frommod = reader.ReadString();
            frommod = Mod.Name;
            string name = reader.ReadString();
            if (Mod.TryFind<ModWall>(name, out ModWall modWall))
            {
                return modWall.Type;
            }
            Mod.Logger.Warn($"Could not find wall {name} placing dirt instead");
            return WallID.Dirt;
        }

        public static int ReadModTile(BinaryReader reader)
        {
            string FromMod = reader.ReadString();
            FromMod = Mod.Name;
            string Name = reader.ReadString();
            Mod m = null;
            if (Mod.TryFind<ModTile>(Name, out ModTile modTile))
            {
                return modTile.Type;
            }
            Mod.Logger.Warn($"Could not find tile {Name} placing dirt instead");
            return TileID.Dirt;
        }
        public static void SaveStruct(string fileName, Point bottomLeft, Point topRight)
        {
            //string Path = Main.SavePath + "/" + "ModSources" + "/" + Mod.Name + "/" + "SavedStruct.str";
            string savePath = Main.SavePath + $"/ModSources/{Mod.Name}/Structures/{fileName}.str";
        
            using (var stream = File.Open(savePath, FileMode.Create))
            {
                using var writer = new BinaryWriter(stream);
                int Xlength = topRight.X - bottomLeft.X;
                int Ylength = bottomLeft.Y - topRight.Y;
                writer.Write(Xlength);
                writer.Write(Ylength);
                for (int x = bottomLeft.X; x <= topRight.X; x++)
                {
                    for (int y = bottomLeft.Y; y >= topRight.Y; y--)
                    {
                        //tile
                        Tile t = Framing.GetTileSafely(x, y);
                        bool hastile = t.HasTile;
                        writer.Write(hastile);
                        writer.Write(t.LiquidType);
                        writer.Write(t.LiquidAmount);
                        writer.Write(t.BlueWire);
                        writer.Write(t.RedWire);
                        writer.Write(t.GreenWire);
                        writer.Write(t.YellowWire);
                        writer.Write(t.HasActuator);
                        writer.Write(t.IsActuated);
                        if (hastile)
                        {
                            bool Modded = t.TileType > TileID.Count;
                            writer.Write(Modded);
                            if (Modded)
                            {
                                WriteModdedTile(writer, t);
                            }
                            else
                            {
                                WriteVanillaTile(writer, t);
                            }
                            writer.Write((byte)t.BlockType);
                            writer.Write(t.IsHalfBlock);
                            writer.Write((byte)t.Slope);
                            writer.Write(t.TileFrameNumber);
                            writer.Write(t.TileFrameX);
                            writer.Write(t.TileFrameY);

                            //Paint
                            writer.Write(t.TileColor);
                            writer.Write(t.IsTileInvisible);
                            writer.Write(t.IsTileFullbright);

                            bool Chest = false;
                            foreach (Chest c in Main.chest)
                            {
                                if (c == null)
                                    continue;
                                if (c.x == x && c.y == y)
                                {
                                    Chest = true;
                                }
                            }
                            writer.Write(Chest);
                        }
                        bool WallModded = t.WallType >= WallID.Count;
                        writer.Write(WallModded);
                        if (WallModded)
                        {
                            WriteModdedWall(writer, t);
                        }
                        else
                        {
                            WriteVanillaWall(writer, t);
                        }
                        writer.Write(t.WallFrameX);
                        writer.Write(t.WallFrameY);
                        writer.Write(t.WallColor);
                        writer.Write(t.IsWallInvisible);
                        writer.Write(t.IsWallFullbright);
                    }
                }
            }

            DebugHelper.NewTextOnlyInTesting("Structure Saved");
        }


        private static void WriteModdedWall(BinaryWriter writer, Tile t)
        {
            ModWall modwall = WallLoader.GetWall(t.WallType);
            if (modwall == null)
            {
                throw new Exception("Write modded wall was called with a vanilla wall type");
            }
            string FromMod = modwall.Mod.Name;
            string Name = modwall.GetType().Name;
            writer.Write(FromMod);
            writer.Write(Name);
        }

        private static void WriteVanillaWall(BinaryWriter writer, Tile t)
        {
            ushort WallType = t.WallType;
            if (t.WallType >= WallID.Count)
            {
                throw new Exception($"Write vanilla wall was called with a modded wall type, type = {t.WallType} and wallid count = {WallID.Count}");
            }
            writer.Write(WallType);
        }

        private static void WriteVanillaTile(BinaryWriter writer, Tile t)
        {
            if (t.TileType > TileID.Count)
                throw new Exception("modded tile was used in WriteVanillaTile");
            writer.Write(t.TileType);
        }

        private static void WriteModdedTile(BinaryWriter writer, Tile t)
        {
            ModTile tModTile = TileLoader.GetTile(t.TileType);
            if (tModTile == null)
            {
                throw new Exception("Write modded tile was called with a vanilla tile type");
            }
            string ModName = tModTile.Mod.Name;
            writer.Write(ModName);
            writer.Write(tModTile.GetType().Name);
        }
    }
}

