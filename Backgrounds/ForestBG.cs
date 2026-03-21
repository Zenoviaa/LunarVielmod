using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;

namespace Stellamod.Backgrounds
{
    public class ForestBG : CustomBG
    {
        public CustomBGLayer BackLayer;
        public CustomBGLayer MidLayer;
        public CustomBGLayer FrontLayer;

        public float FarParallax => 0.08f;
        public float MidParallax => 0.11f;
        public float CloseParallax => 0.20f;

        private void AddFarLayer()
        {
            BackLayer = new CustomBGLayer();
            BackLayer.SetTexture("Assets/Textures/Backgrounds/ForestFar");
            BackLayer.Parallax = FarParallax;
            BackLayer.DrawOffset = Vector2.Zero;
            AddLayer(BackLayer);

        }

        private void AddMidLayer()
        {
            MidLayer = new CustomBGLayer();
            MidLayer.SetTexture("Assets/Textures/Backgrounds/ForestMid");
            MidLayer.Parallax = MidParallax;
            MidLayer.DrawOffset = Vector2.Zero;
            AddLayer(MidLayer);
        }

        private void AddCloseLayer()
        {
            FrontLayer = new CustomBGLayer();
            FrontLayer.SetTexture("Assets/Textures/Backgrounds/ForestFront");
            FrontLayer.Parallax = CloseParallax;
            FrontLayer.DrawOffset = Vector2.Zero;
            AddLayer(FrontLayer);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DrawScale = 1f;
            AddFarLayer();
            AddMidLayer();
            AddCloseLayer();
        }

        public override bool IsActive()
        {
            DrawScale = 1f;
            DrawOffset = new Vector2(0, 620);

            /*
            NoSurfaceOffset = true;
            DrawScale = 1;
          */
            return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneForest;
        }
    }
}
