using Microsoft.Xna.Framework;
using Stellamod.Core.Backgrounds;

namespace Stellamod.Content.Backgrounds
{
    internal class RainforestBG : CustomBG
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            //ALRIGHT
            Layers = new CustomBGLayer[]
            {
                new CustomBGLayer("Assets/Textures/Backgrounds/RainforestBack", 0.1f, drawOffset: Vector2.Zero),
                new CustomBGLayer("Assets/Textures/Backgrounds/RainforestMiddle", 0.25f, drawOffset: Vector2.Zero),
                new CustomBGLayer("Assets/Textures/Backgrounds/RainforestFront", 0.5f, drawOffset: Vector2.Zero)
            };
        }

        public override bool IsActive()
        {
            return true;
        }
    }
}
