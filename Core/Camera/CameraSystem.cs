using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Camera
{
    [Autoload(Side = ModSide.Client)]
    public class CameraSystem : ModSystem
    {
        private OffsetCameraModifier _offsetCameraModifer;
        private SmoothCameraModifier _smoothCameraModifier;
        private RetargetCameraModifier _reTargetCameraModifier;
        public static bool IsLoaded;
        public override void OnModLoad()
        {
            base.OnModLoad();
            IsLoaded = true;
            _offsetCameraModifer = new OffsetCameraModifier();
            _smoothCameraModifier = new SmoothCameraModifier();
            _reTargetCameraModifier = new RetargetCameraModifier();
            Main.instance.CameraModifiers.Add(_offsetCameraModifer);
            Main.instance.CameraModifiers.Add(_smoothCameraModifier);
            Main.instance.CameraModifiers.Add(_reTargetCameraModifier);
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            IsLoaded = false;
        }
        public override void Unload()
        {
            base.Unload();
            IsLoaded = false;
        }
    }
}
