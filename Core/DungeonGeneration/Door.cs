using System;

namespace Stellamod.Core.DungeonGeneration
{
 
    public enum Door : byte
    {
        None = 0,   
        Left = 1,
        Right = 2,
        Up = 3,
        Down = 4,
        AnchorBottomLeft = 5,
        AnchorTopRight = 6,
        Start = 7,
        Boss = 8
    }
    [Flags]
    public enum DoorsFlag
    {
        None = 0,
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
    }
}
