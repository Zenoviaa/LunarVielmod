using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    internal class SwingPlayer : ModPlayer
    {
        private int _swingDirection = -1;
        public int SwingDirection
        {
            get
            {
                _swingDirection *= -1;
                return _swingDirection;
            }
        }
    }
}
