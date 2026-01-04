using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Core.Astar
{
    public struct SearchJob
    {
        public SearchJob(Vector2 startWorld, Vector2 endWorld, int tileSearchRange = 15, int airPadding = 0)
        {
            start = startWorld.ToTileCoordinates();
            end = endWorld.ToTileCoordinates();
            range = tileSearchRange;
            padding = airPadding;
        }
        public Point start;
        public Point end;
        public int range;
        public int padding;
    }
    public struct Cell
    {
        public int parentX;
        public int parentY;
        public float f;
        public float g;
        public float h;
    }
    public class Astar
    {
        private static bool[,] _closed;
        private static Cell[,] _cells;
        public static float Heuristic(int row, int col, Point dest)
        {
            // Return using the distance formula
            return MathF.Sqrt(MathF.Pow(row - dest.X, 2) + MathF.Pow(col - dest.Y, 2));
        }

        private static bool IsClosed(int x, int y, int padding)
        {
            bool solidTile = WorldGen.SolidOrSlopedTile(x, y);
            if (solidTile)
                return true;

            for(int i = -padding; i <= padding; i++)
            {
                for(int j = -padding; j <= padding; j++)
                {
                    int ix = x + i;
                    int jy = y + j;
                    if (!WorldGen.InWorld(ix, jy))
                        continue;
                    if (WorldGen.SolidOrSlopedTile(ix, jy))
                        return true;
                }
            }
            return false;
        }

        //Modified from https://www.geeksforgeeks.org/dsa/a-search-algorithm/#
        //I love learneing
        public static Stack<Vector2> Search(SearchJob searchJob)
        {
            _cells ??= new Cell[Main.maxTilesX, Main.maxTilesY];
            _closed ??= new bool[Main.maxTilesX, Main.maxTilesY];

            Point start = searchJob.start;
            Point end = searchJob.end;
            int tileSearchRange = searchJob.range;

            //Ok so terraria has an extremely big map
            //First thing we gotta do is limit how much space we're looking in to find this path
            Point minTile = new Point();
            minTile.X = Math.Min(start.X, end.X);
            minTile.Y = Math.Min(start.Y, end.Y);

            Point maxTile = new Point();
            maxTile.X = Math.Max(start.X, end.X);
            maxTile.Y = Math.Max(start.Y, end.Y);

            //As a failsafe, let's return an empty list if the distance is too great
            Point diff = end - start;
            float xDiff = end.X - start.X;
            float yDiff = end.Y - start.Y;
            float distance = MathF.Sqrt(xDiff * xDiff + yDiff * yDiff);
            if (distance > 100)
                return null;
            if (start == end)
                return null;

            //No we have a small ass line between our two points
            //So let's add padding
            //By default we'll search for a path within a 10 tile radius within our start and end points
            minTile -= new Point(tileSearchRange, tileSearchRange);
            maxTile += new Point(tileSearchRange, tileSearchRange);


            //Here we reset the graph to its default values
            for (int i = minTile.X; i < maxTile.X; i++)
            {
                for (int j = minTile.Y; j < maxTile.Y; j++)
                {
                    ref Cell c =ref  _cells[i, j];
                    c.f = float.MaxValue;
                    c.g = float.MaxValue;
                    c.h = float.MaxValue;
                    c.parentX = -1;
                    c.parentY = -1;
                    _closed[i, j] = false;
                }
            }

            //Initialize starting node
            int x = start.X;
            int y = start.Y;
            ref Cell startingCell = ref _cells[x, y];
            startingCell.f = 0f;
            startingCell.g = 0f;
            startingCell.h = 0f;
            startingCell.parentX = start.X;
            startingCell.parentY = start.Y;

            //Create an open list that's automatically sorted by their F costs
            //We'll add the starting node to the open list
            SortedSet<(float, Point)> openList = new SortedSet<(float, Point)>(Comparer<(float, Point)>.Create((a, b) => a.Item1.CompareTo(b.Item1)));
            openList.Add((0f, start));

            //Now we can implement astar
            //While there are nodes in the open list we take the one with the lowest f cost and iterate over all of its neighbours
            //If there is a neighbour that is not closed we add it to the open list and calculate it's f, g, and h costs
            //and set the parent to be the current node so that if this node is used in the path it's traced back correctly
            while (openList.Count > 0)
            {
                (float fCost, Point point) tile = openList.Min;
                openList.Remove(tile);

                x = tile.point.X;
                y = tile.point.Y;
                _closed[x, y] = true;

                //Loop over all the neighbours for this cell
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (i == 0 && j == 0)
                            continue;

                        int newX = x + i;
                        int newY = y + j;

                        if (!WorldGen.InWorld(newX, newY))
                            continue;

                        if (new Point(newX, newY) == end)
                        {
                            _cells[newX, newY].parentX = x;
                            _cells[newX, newY].parentY = y;
                            return TracePath(_cells, end);
                        }

                        if (!_closed[newX, newY] && !IsClosed(newX, newY, searchJob.padding))
                        {
                            float gNew = _cells[x, y].g + 1.0f;
                            float hNew = Heuristic(newX, newY, end);
                            float fNew = gNew + hNew;

                            if (_cells[newX, newY].f == float.MaxValue || _cells[newX, newY].f > fNew)
                            {
                                openList.Add((fNew, new Point(newX, newY)));
                                _cells[newX, newY].f = fNew;
                                _cells[newX, newY].g = gNew;
                                _cells[newX, newY].h = hNew;
                                _cells[newX, newY].parentX = x;
                                _cells[newX, newY].parentY = y;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static Stack<Vector2> TracePath(Cell[,] cellDetails, Point dest)
        {
            int row = dest.X;
            int col = dest.Y;
            Stack<Vector2> Path = new Stack<Vector2>();
            while (!(cellDetails[row, col].parentX == row && cellDetails[row, col].parentY == col))
            {
                Path.Push(new Point(row, col).ToWorldCoordinates());
                int temp_row = cellDetails[row, col].parentX;
                int temp_col = cellDetails[row, col].parentY;
                row = temp_row;
                col = temp_col;
            }

            Path.Push(new Point(row, col).ToWorldCoordinates());
            return Path;

        }

    }
}
