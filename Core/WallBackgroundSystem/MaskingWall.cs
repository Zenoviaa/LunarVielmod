using Terraria.ModLoader;

namespace Stellamod.Core.WallBackgroundSystem
{
    public class MaskingWall : ModWall
    {
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            base.ModifyLight(i, j, ref r, ref g, ref b);
            float light = 0.5f;
            r += light;
            g += light;
            b += light;
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
           // MaskedWallRenderer.QueueDraw(new Point(i, j));
            return false;
        }
    }
}
