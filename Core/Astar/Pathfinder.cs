using System.Collections.Generic;
using System.IO;

namespace Stellamod.Core.Astar;

public class Pathfinder
{
    private SearchJob _oldSearchJob;
    public Pathfinder()
    {
        path = new();
    }
    public Stack<Vector2> path { get; private set; }

    public Vector2 targetOldPos;
    public Vector2 currentNode;
    public Vector2 nextNode;
    public void NetSend(BinaryWriter writer)
    {
        writer.Write((byte)_oldSearchJob.searchJobStyle);
        writer.Write(_oldSearchJob.start.X);
        writer.Write(_oldSearchJob.start.Y);
        writer.Write(_oldSearchJob.end.X);
        writer.Write(_oldSearchJob.end.Y);
        writer.Write(_oldSearchJob.range);
    }

    public void NetReceive(BinaryReader reader)
    {


        SearchJob searchJob = new((SearchJobStyle)reader.ReadByte(),
            new Vector2(reader.ReadInt32(), reader.ReadInt32()),
            new Vector2(reader.ReadInt32(), reader.ReadInt32()),
            tileSearchRange: reader.ReadInt32());
        NewPath(searchJob);
    }

    public void NewPath(Vector2 startWorld, Vector2 endWorld, int tileSearchRange = 15, int airPadding = 0, SearchJobStyle searchJobStyle = SearchJobStyle.Default) => NewPath(new SearchJob(searchJobStyle, startWorld, endWorld, tileSearchRange, airPadding));
    public void NewPath(SearchJob searchJob)
    {
        _oldSearchJob = searchJob;
        var result = Astar.Search(searchJob);
      
        path.Clear();
        if (result == null)
            return;

        var i = 0;
        foreach (var node in result)
        {
            if(i % 2 == 0)
            {
                path.Push(node);
            }
            i++;
        }
    
        if (path != null && path.Count > 0)
        {
            Pop();
        }
        else
        {
            currentNode = Vector2.Zero;
        }
    }
    public void Pop()
    {
        if (path.Count <= 0)
            return;

        currentNode = path.Pop();
        if (path.Count > 0)
            nextNode = path.Peek();
    }

    public void DebugDrawPath(SpriteBatch spriteBatch, Color color)
    {
        if (path == null || path.Count <= 0)
            return;

        foreach (var pos in path)
        {
            var drawer2 = SpritebatchDrawer.FromTextureGetLightingColor(AssetReferences.Assets.GlowMasks.WhiteSquare.Asset, pos);
            drawer2.color = color;
            spriteBatch.Draw(drawer2);
        }
    }
}
