using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Camera
{
    [Autoload(Side = ModSide.Client)]
    public class CameraSystem : ModSystem
    {
        public override void Load()
        {
            base.Load();
            Main.instance.CameraModifiers.Add(new OffsetCameraModifier());
            Main.instance.CameraModifiers.Add(new SmoothCameraModifier());
            Main.instance.CameraModifiers.Add(new RetargetCameraModifier());
        }
    }
}
