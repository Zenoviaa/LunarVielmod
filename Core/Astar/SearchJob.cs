using Terraria;

namespace Stellamod.Core.Astar;

public readonly record struct SearchJob
{
    public SearchJob(SearchJobStyle style, Vector2 startWorld, Vector2 endWorld, int tileSearchRange = 15, int airPadding = 0)
    {
        searchJobStyle = style;
        start = startWorld.ToTileCoordinates();
        end = endWorld.ToTileCoordinates();
        range = tileSearchRange;
        padding = airPadding;
        
    }
    public readonly SearchJobStyle searchJobStyle;
    public readonly Point start;
    public readonly Point end;
    public readonly int range;
    public readonly int padding;
}
