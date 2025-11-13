using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Core.DungeonGeneration
{

    [Autoload(Side = ModSide.Client)]
    public class DungeonGenerationPreviewer : ModSystem
    {
        public static Room[] rooms;
        public static Point? point1;
        public static Point? point2;
        private static void ArcMaker()
        {
            if (Main.mouseRight && Main.mouseRightRelease)
            {
                Main.mouseRightRelease = false;
                int x = (int)(Main.MouseWorld.X / 16);
                int y = (int)(Main.MouseWorld.Y / 16);
                Point tilePoint = new Point(x, y);
                if (point1 == null)
                {
                    point1 = tilePoint;
                    Main.NewText($"Set Point 1 to {point1}");
                }
                else if (point2 == null)
                {
                    point2 = tilePoint;
                    Main.NewText($"Set Point 2 to {point2}");
                }
                else
                {
                    point1 = null;
                    point2 = null;
                }
            }

            if (Main.mouseMiddle && Main.mouseMiddleRelease)
            {
                Main.mouseMiddleRelease = false;
                if (point1 != null && point2 != null)
                {
                    float radius = 256;
                    bool isLeft = point1.Value.X < point2.Value.X;
                    Point leftPoint;// = point1.Value.X < point2.Value.X ? point1.Value : point2.Value;
                    Point rightPoint;// = point2.Value;

                    if (isLeft)
                    {
                        leftPoint = point1.Value;
                        rightPoint = point2.Value;
                    }
                    else
                    {
                        leftPoint = point2.Value;
                        rightPoint = point1.Value;
                    }

                    Main.NewText("Make Arc");
                    int steps = rightPoint.X - leftPoint.X;
                    for (int x = leftPoint.X; x < rightPoint.X; x++)
                    {
                        float numSteps = steps;
                        float localX = x - leftPoint.X;
                        float progress = localX / numSteps;
                        float r = MathHelper.Lerp(0, radius, EasingFunction.QuadraticBump(progress));
                        int tileOffset = (int)(r / 16);
                        Point tilePoint = new Point(x, leftPoint.Y - tileOffset);
                        Tile tile = Main.tile[tilePoint.X, tilePoint.Y];
                        int wallType = WallID.GrayBrick;
                        WorldGen.KillWall(tilePoint.X, tilePoint.Y);
                        WorldGen.KillWall(tilePoint.X, tilePoint.Y - 1);
                        WorldGen.KillWall(tilePoint.X, tilePoint.Y - 2);
                        WorldGen.PlaceWall(tilePoint.X, tilePoint.Y, wallType);
                        WorldGen.PlaceWall(tilePoint.X, tilePoint.Y - 1, wallType);
                        WorldGen.PlaceWall(tilePoint.X, tilePoint.Y - 2, wallType);
                    }
                }
                else
                {
                    Main.NewText($"Please set points");
                }
            }
        }
        private static void DrawDungeonPreview()
        {
            if (rooms == null)
                return;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 offset = Main.Camera.Center - Main.screenPosition;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int r = 0; r < rooms.Length; r++)
            {
                Room room = rooms[r];
                Rectangle rectangle = room.bounds;
                rectangle.Location += offset.ToPoint();
                Primitives2D.DrawRectangle(spriteBatch, rectangle, Color.Red);
            }

            spriteBatch.End();
        }

        public override void PostDrawTiles()
        {
            base.PostDrawTiles();
            bool arcCode = false;
            if (arcCode)
            {
                ArcMaker();
            }

        }
    }
    public class DungeonGenerationTester : ModItem
    {
        private int _useIndex;

        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Meatballs" +
				"\nDo not be worried, this mushes reality into bit bits and then shoots it!" +
				"\nYou can never miss :P"); */
            // DisplayName.SetDefault("Teraciz");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");
        }

        public override bool? UseItem(Player player)
        {
            DungeonGenerationPreviewer.rooms = Dungeonizer.TestGeneration();
            return true;
        }
    }

    //Alright, so we need to code dungeon generation
    //This is pretty easy though
    //First we need a structure for a room

    public struct Doorway
    {
        public int localX;
        public int localY;
        public Door door;
        public bool isConnected;
    }

    public enum RoomType : byte
    {
        Start=0,
        Boss=1
    }
    public class Room
    {
        public Rectangle bounds;
        public Doorway[] doors;
        public DoorsFlag doorsFlag;
        public int connectionCount;
        public string prefab;
        public RoomType roomType;
        public Room()
        {

        }

        public Room(string prefab, Rectangle bounds, PlacedDoor[] placedDoors)
        {
            this.prefab = prefab;
            this.bounds = bounds;
            doors = new Doorway[placedDoors.Length];
            doorsFlag = DoorsFlag.None;
            for(int d = 0; d < doors.Length; d++)
            {
                doors[d].localX = placedDoors[d].point.X;

                //This looks weird, but we need to invert the Y because it places from bottom left not top left
                //And this system works with regular rectangles
                doors[d].localY = bounds.Height - placedDoors[d].point.Y;
                doors[d].door = placedDoors[d].door;
                switch (doors[d].door)
                {
                    case Door.Left:
                        doorsFlag |= DoorsFlag.Left; 
                        break;
                    case Door.Right:
                        doorsFlag |= DoorsFlag.Right;
                        break;
                    case Door.Up:
                        doorsFlag |= DoorsFlag.Up;
                        break;
                    case Door.Down:
                        doorsFlag |= DoorsFlag.Down;
                        break;
                    case Door.Start:
                        roomType = RoomType.Start;
                        break;
                    case Door.Boss:
                        roomType = RoomType.Boss;
                        break;
                }
            }
        }

        public Room Clone()
        {
            Room room = new Room();
            room.bounds = bounds;
            room.doors = new Doorway[doors.Length];
            for (int i = 0; i < room.doors.Length; i++)
            {
                room.doors[i] = doors[i];
            }
            room.doorsFlag = doorsFlag;
            room.prefab = prefab;
            room.roomType = roomType;
            return room;
        }


        public void MoveTo(Room otherRoom, ref Doorway otherDoorway)
        {
            int myIndex = GetOppositeDoorway(otherDoorway.door);
            Doorway myDoorway = doors[myIndex];

            Point otherDoor = otherRoom.bounds.Location + new Point(otherDoorway.localX, otherDoorway.localY);
            Point myDoor = bounds.Location + new Point(myDoorway.localX, myDoorway.localY);
            Point vectorToDoor = otherDoor - myDoor;


            bounds.Location += vectorToDoor;
            switch (otherDoorway.door)
            {
                case Door.Left:
                    bounds.Location += new Point(-1, 0);
                    break;
                case Door.Right:
                    bounds.Location += new Point(1, 0);
                    break;
                case Door.Up:
                    bounds.Location += new Point(0, -1);
                    break;
                case Door.Down:
                    bounds.Location += new Point(0, 1);
                    break;
            }
        }

        public void ConnectTo(Room otherRoom, ref Doorway otherDoorway)
        {
            int myIndex = GetOppositeDoorway(otherDoorway.door);
            Doorway myDoorway = doors[myIndex];


            myDoorway.isConnected = true;
            otherDoorway.isConnected = true;
            Console.WriteLine($"Connect {myDoorway.door} to {otherDoorway.door}");


            otherRoom.connectionCount++;
            connectionCount++;
        }

        public int GetRandomDoorway(UnifiedRandom random)
        {
            return random.Next(0, doors.Length);
        }

        public int GetDoorway(Door door)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                if (doors[i].door == door)
                    return i;
            }
            return -1;
        }

        public int GetOppositeDoorway(Door door)
        {
            Door inverse = Door.Left;
            switch (door)
            {
                case Door.Left:
                    inverse = Door.Right;
                    break;
                case Door.Right:
                    inverse = Door.Left;
                    break;
                case Door.Up:
                    inverse = Door.Down;
                    break;
                case Door.Down:
                    inverse = Door.Up;
                    break;
            }

            return GetDoorway(inverse);
        }
    }

    public static class Dungeonizer
    {
        private static Door GetInverseDoor(Door doorToInverse)
        {
            switch (doorToInverse)
            {
                default:
                case Door.Left:
                    return Door.Right;
                case Door.Right:
                    return Door.Left;
                case Door.Up:
                    return Door.Down;
                case Door.Down:
                    return Door.Up;
            }
        }

        private static Room GetRandomRoomWithOppositeDoor(Room[][] prefabs, UnifiedRandom random, Door doorToInverse)
        {
            Door doorToLookFor = GetInverseDoor(doorToInverse);
            int index = 0;
            switch (doorToLookFor)
            {
                case Door.Left:
                    index = 0;
                    break;
                case Door.Right:
                    index = 1;
                    break;
                case Door.Up:
                    index = 2;
                    break;
                case Door.Down:
                    index = 3;
                    break;
            }
            Room[] rooms = prefabs[index];
            return GetRandomRoom(rooms, random);
        }

        private static Room GetRandomRoom(Room[] prefabs, UnifiedRandom random)
        {
            Room room = prefabs[random.Next(0, prefabs.Length)];
            return room;
        }

        public static Room[] TestGeneration()
        {
            UnifiedRandom random = new UnifiedRandom();
            Room roomToSpam = new Room();
            roomToSpam.bounds = new Rectangle(0, 0, 16, 16);
            roomToSpam.doorsFlag = DoorsFlag.Left | DoorsFlag.Right | DoorsFlag.Up | DoorsFlag.Down;

            roomToSpam.doors = new Doorway[4];
            for (int i = 0; i < roomToSpam.doors.Length; i++)
            {
                roomToSpam.doors[i] = new Doorway();
            }
            roomToSpam.doors[0].localX = 0;
            roomToSpam.doors[0].localY = 15;
            roomToSpam.doors[0].door = Door.Left;

            roomToSpam.doors[1].localX = 15;
            roomToSpam.doors[1].localY = 15;
            roomToSpam.doors[1].door = Door.Right;

            roomToSpam.doors[2].localX = 7;
            roomToSpam.doors[2].localY = 0;
            roomToSpam.doors[2].door = Door.Up;

            roomToSpam.doors[3].localX = 7;
            roomToSpam.doors[3].localY = 15;
            roomToSpam.doors[3].door = Door.Down;


            Room[] prefabs = new Room[] { roomToSpam };
            return Generate(prefabs, random);
        }



        //So how do we want to generate the dungeon?
        public static Room[] Generate(Room[] prefabs, UnifiedRandom random)
        {


            //Before we do anything lets create a lookup table of rooms that have doors
            //That'll make the room hunt easier
            Room[][] roomLookup = new Room[4][];


            List<Room> leftDoorsList = new List<Room>();
            List<Room> rightDoorsList = new List<Room>();
            List<Room> upDoorsList = new List<Room>();
            List<Room> downDoorsList = new List<Room>();
            for (int i = 0; i < prefabs.Length; i++)
            {
                Room prefab = prefabs[i];
                if (prefab.doorsFlag.HasFlag(Door.Left))
                {
                    leftDoorsList.Add(prefab);
                }
                if (prefab.doorsFlag.HasFlag(Door.Right))
                {
                    rightDoorsList.Add(prefab);
                }
                if (prefab.doorsFlag.HasFlag(Door.Up))
                {
                    upDoorsList.Add(prefab);
                }
                if (prefab.doorsFlag.HasFlag(Door.Down))
                {
                    downDoorsList.Add(prefab);
                }
            }
            roomLookup[0] = leftDoorsList.ToArray();
            roomLookup[1] = rightDoorsList.ToArray();
            roomLookup[2] = upDoorsList.ToArray();
            roomLookup[3] = downDoorsList.ToArray();



            //This function will take an array of prefabs and then output a map.
            //This map will then get world-genned onto the world
            //Aight
            //So HOW do we want to do this


            //Algorithm:
            //Start with a starting room
            //Place the room
            //Look for available doors

            //Ok, so first let's 

            int roomCount = 100;
            int snakeLength = 60;
            List<Room> map = new List<Room>();
            List<bool> canHorizontalFromt = new List<bool>();

            Room startingRoom = GetRandomRoom(prefabs, random).Clone();
            map.Add(startingRoom);
            canHorizontalFromt.Add(true);


            //Just incase we want to backtrack
            int maxAttempts = 100000;
            int direction = 0;
            int directionCounter = 0;

            bool HasOverlapWithMap(Rectangle rectangle)
            {
                for (int i = 0; i < map.Count; i++)
                {
                    Rectangle otherBounds = map[i].bounds;
                    if (rectangle.Intersects(otherBounds))
                        return true;
                }
                return false;
            }

            Room previousRoom = startingRoom;
            Room currentRoom = startingRoom;

            for (int i = 0; i < maxAttempts; i++)
            {
                //Once we hit the max room count... yeah
                if (map.Count >= roomCount)
                    break;

                if (map.Count < snakeLength)
                {
                    if (currentRoom.connectionCount >= 3)
                    {
                        int index = random.Next(0, map.Count);
                        Room randomRoom = map[index];
                        currentRoom = randomRoom;
                        continue;
                    }


                    int doorWayIndex = currentRoom.GetRandomDoorway(random);
                    ref Doorway doorWay = ref currentRoom.doors[doorWayIndex];

                    if (direction == 0)
                    {
                        if (doorWay.door == Door.Up || doorWay.door == Door.Down)
                            continue;
                    }

                    if (direction == 1)
                    {
                        if (doorWay.door == Door.Left || doorWay.door == Door.Right || doorWay.door == Door.Up)
                            continue;
                    }

                    if (doorWay.isConnected)
                        continue;


                    Room nextRoom = GetRandomRoomWithOppositeDoor(roomLookup, random, doorWay.door).Clone();
                    ref Doorway otherDoorway = ref nextRoom.doors[nextRoom.GetOppositeDoorway(doorWay.door)];
                    nextRoom.MoveTo(currentRoom, ref doorWay);
                    if (HasOverlapWithMap(nextRoom.bounds))
                        continue;


                    nextRoom.ConnectTo(currentRoom, ref doorWay);
                    map.Add(nextRoom);

                    previousRoom = currentRoom;
                    currentRoom = nextRoom;

                    //Select a rando mroom on the map
                    if (direction == 0)
                    {
                        canHorizontalFromt.Add(true);
                        directionCounter++;
                        if (directionCounter >= 5)
                        {
                            directionCounter = 0;
                            direction = 1;
                        }
                    }
                    else if (direction == 1)
                    {
                        canHorizontalFromt.Add(false);
                        directionCounter++;
                        if (directionCounter >= 3)
                        {
                            directionCounter = 0;
                            direction = 0;
                        }

                    }
                }
                else
                {
                    //Done drawing a snake, so we just expand the floors
                    int index = random.Next(0, map.Count);
                    Room randomRoom = map[index];
                    currentRoom = randomRoom;
                    if (!canHorizontalFromt[index])
                        continue;


                    int doorWayIndex = currentRoom.GetRandomDoorway(random);
                    ref Doorway doorWay = ref currentRoom.doors[doorWayIndex];
                    if (doorWay.door == Door.Up || doorWay.door == Door.Down)
                        continue;
                    if (doorWay.isConnected)
                        continue;

                    Room nextRoom = GetRandomRoomWithOppositeDoor(roomLookup, random, doorWay.door).Clone();
                    ref Doorway otherDoorway = ref nextRoom.doors[nextRoom.GetOppositeDoorway(doorWay.door)];
                    nextRoom.MoveTo(currentRoom, ref doorWay);
                    if (HasOverlapWithMap(nextRoom.bounds))
                        continue;

                    nextRoom.ConnectTo(currentRoom, ref doorWay);
                    map.Add(nextRoom);


                    previousRoom = currentRoom;
                    currentRoom = nextRoom;
                    canHorizontalFromt.Add(true);
                }



            }

            return map.ToArray();
        }
    }
}
