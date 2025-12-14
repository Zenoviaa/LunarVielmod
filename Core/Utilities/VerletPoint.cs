using Microsoft.Xna.Framework;

namespace Stellamod.Core.Utilities
{
    public struct VerletPoint
    {
        public Vector2 position;
        public Vector2 oldPosition;
        public bool pinned;
    }

}
