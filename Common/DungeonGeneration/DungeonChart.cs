using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Common.DungeonGeneration;

public static class DungeonChartTraversal
{
    public static void DoBFS(Vector2[,] edges, int vertices, bool[] visited, Action<int, int> onVisit)
    {
        for (int i = 0; i < vertices; i++)
        {
            if (!visited[i])
                BFS(edges, vertices, visited, i, onVisit);
        }
    }

    private static void BFS(Vector2[,] edges, int v, bool[] visited, int si, Action<int, int> onVisit)
    {
        Queue<int> queue = new Queue<int>();
        queue.Enqueue(si);
        visited[si] = true;
        while (queue.Count != 0)
        {
            int currentVertex = queue.Dequeue();

            for (int i = 0; i < v; i++)
            {
                if (i == currentVertex)
                    continue;
                if (!visited[i] && edges[currentVertex, i] != Vector2.Zero)
                {
                    queue.Enqueue(i);
                    visited[i] = true;
                    onVisit?.Invoke(currentVertex, i);
                }
            }
        }
    }

    public static void DoDFS(Vector2[,] edges, int vertices, bool[] visited, Action<int, int> onVisit)
    {
        for (int i = 0; i < vertices; i++)
        {
            if (!visited[i])
                DFS(edges, vertices, visited, i, onVisit);
        }
    }

    private static void DFS(Vector2[,] edges, int vertices, bool[] visited, int si, Action<int, int> onVisit)
    {
        visited[si] = true;
        for (int i = 0; i < vertices; i++)
        {
            if (i == si)
                continue;
            if (!visited[i] && edges[si, i] != Vector2.Zero)
            {

                DFS(edges, vertices, visited, i, onVisit);
                onVisit?.Invoke(si, i);
            }
        }
    }
}

public class DungeonChart
{
    public readonly List<Point> Vertices;
    public readonly Vector2[,] Edges;
    public readonly HashSet<Point> Corridors;
    public readonly HashSet<Point> VerticeHashSet;
    public DungeonChart(params Point[] vertices)
    {
        Vertices = new List<Point>(vertices);
        Edges = new Vector2[vertices.Length, vertices.Length];
        Corridors = new HashSet<Point>();
        VerticeHashSet = new HashSet<Point>();
        foreach(Point point in vertices)
            VerticeHashSet.Add(point);
    }
    public void Insert(int index, Point node)
    {
        Vertices[index] = node;
    }

    public void Connect(int start, int end, Vector2 direction)
    {
        Edges[start, end] = direction;
        Edges[end, start] = -direction;
        Corridors.Add(new Point(start, end));
    }


    public static DungeonChart FromMap((int, int)[] map)
    {
        List<Point> vertices = new List<Point>();
        //This gonna be slow but idc
        HashSet<Point> verticeHash = new HashSet<Point>();
        for (int r = 0; r < map.Length; r++)
        {
            Point node = new Point(map[r].Item1, map[r].Item2);
            vertices.Add(node);
            verticeHash.Add(node);
        }
     

        Point origin = new Point();
        for(int i = 0; i < vertices.Count; i++)
        {
            Point vertex = vertices[i];
            Point left = vertex + new Point(-1, 0);
            Point right = vertex + new Point(1, 0);
            Point up = vertex + new Point(0, -1);
            Point down = vertex + new Point(0, 1);
            int count = 0;
            if (verticeHash.Contains(left))
            {
                count++;
            }
            if (verticeHash.Contains(right))
            {
                count++;
            }
            if (verticeHash.Contains(up))
            {
                count++;
            }
            if (verticeHash.Contains(down))
            {
                count++;
            }

            if(count <= 1)
            {
                origin = vertex;
                break;
            }
        }


        //Top botom left right
        for (int i = 0; i < vertices.Count; i++)
        {
            Point vertex = vertices[i];
            vertex -= origin;
            vertices[i] = vertex;
        }

        //bruh
        DungeonChart chart = new DungeonChart(vertices.ToArray());
        for (int i = 0; i < vertices.Count; i++)
        {
            Point vertex = vertices[i];
            Point left = vertex + new Point(-1, 0);
            Point right = vertex + new Point(1, 0);
            Point up = vertex + new Point(0, -1);
            Point down = vertex + new Point(0, 1);
            if (chart.VerticeHashSet.Contains(left))
            {
                int index = chart.Vertices.IndexOf(left);
                chart.Connect(i, index, Vector2.One);
            }
            if (chart.VerticeHashSet.Contains(right))
            {
                int index = chart.Vertices.IndexOf(right);
                chart.Connect(i, index, Vector2.One);
            }
            if (chart.VerticeHashSet.Contains(up))
            {
                int index = chart.Vertices.IndexOf(up);
                chart.Connect(i, index, Vector2.One);
            }
            if (chart.VerticeHashSet.Contains(down))
            {
                int index = chart.Vertices.IndexOf(down);
                chart.Connect(i, index, Vector2.One);
            }
        }

        return chart;
    }
    public static DungeonChart FromPrefab(GenerationPrefab prefab)
    {
        List<Point> vertices = new List<Point>();
        Point origin = new Point();
        for (int y = 0; y < prefab.Height; y++)
        {
            for (int x = 0; x < prefab.Width; x++)
            {
                Color pixel = prefab.Sample(x, y);
                if (pixel.R > 125 || pixel.G > 125)
                {
                    vertices.Add(new Point(x, y));
                    if(pixel.G > 125)
                    {
                        origin = new Point(x, y);
                    }
                }
            }
        }

        //Top botom left right
        for (int i = 0; i < vertices.Count; i++)
        {
            Point vertex = vertices[i];
            vertex -= origin;
            vertices[i] = vertex;
        }

        //bruh
        DungeonChart chart = new DungeonChart(vertices.ToArray());
        for(int i = 0; i < vertices.Count; i++)
        {
            Point vertex = vertices[i];
            Point left = vertex + new Point(-1, 0);
            Point right = vertex + new Point(1, 0);
            Point up = vertex + new Point(0, -1);
            Point down = vertex + new Point(0, 1);
            if (chart.VerticeHashSet.Contains(left))
            {
                int index = chart.Vertices.IndexOf(left);
                chart.Connect(i, index, Vector2.One);
            }
            if (chart.VerticeHashSet.Contains(right))
            {
                int index = chart.Vertices.IndexOf(right);
                chart.Connect(i, index, Vector2.One);
            }
            if (chart.VerticeHashSet.Contains(up))
            {
                int index = chart.Vertices.IndexOf(up);
                chart.Connect(i, index, Vector2.One);
            }
            if (chart.VerticeHashSet.Contains(down))
            {
                int index = chart.Vertices.IndexOf(down);
                chart.Connect(i, index, Vector2.One);
            }
        }

        return chart;
    }
}
