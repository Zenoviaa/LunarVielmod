using Terraria.ModLoader;

namespace Stellamod.Core.WallBackgroundSystem
{
    public class MaskingWall : ModWall
    {
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            MaskedWallRenderer.QueueDraw(new Point(i, j));
            return false;
        }
    }
}
