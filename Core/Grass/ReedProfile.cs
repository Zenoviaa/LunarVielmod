using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Grass
{
    public abstract class ReedProfile : ModTexturedType
    {
        public Asset<Texture2D> ReedTextureAsset;
        protected sealed override void Register()
        {
            ModTypeLookup<ReedProfile>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            frameCount = 3;
            ReedTextureAsset = ModContent.Request<Texture2D>(Texture);
        }

        public int frameCount;
        public Rectangle GetFrame(int frameIndex)
        {
            int frameHeight = ReedTextureAsset.Height() / frameCount;
            Rectangle frame = new Rectangle(0, frameIndex * frameHeight, ReedTextureAsset.Width(), frameHeight);
            return frame;
        }
    }
}
