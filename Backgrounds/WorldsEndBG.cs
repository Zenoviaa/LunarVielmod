using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;

namespace Stellamod.Backgrounds
{
    public class WorldsEndBG : CustomBG
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
            BackLayer.SetTexture("Assets/Textures/Backgrounds/GreyGrassBackgroundFar");
            BackLayer.Parallax = FarParallax;
            BackLayer.DrawOffset = Vector2.Zero;
            AddLayer(BackLayer);

            CustomBGLayer backFogLayer = new CustomBGLayer();
            backFogLayer.SetTexture("Assets/Textures/Backgrounds/RainforestFrontGradient");
            backFogLayer.Parallax = FarParallax;
            backFogLayer.DrawOffset = Vector2.Zero;


            MistShader backMistShader = new MistShader();
            backMistShader.StartColor = Color.SkyBlue * 0.75f;
            backMistShader.EndColor = Color.Transparent;
            backFogLayer.Shader = backMistShader;
            AddLayer(backFogLayer);

        }

        private void AddMidLayer()
        {
            MidLayer = new CustomBGLayer();
            MidLayer.SetTexture("Assets/Textures/Backgrounds/GreyGrassBackgroundMid");
            MidLayer.Parallax = MidParallax;
            MidLayer.DrawOffset = Vector2.Zero;
            AddLayer(MidLayer);

            CustomBGLayer midFogLayer = new CustomBGLayer();
            midFogLayer.SetTexture("Assets/Textures/Backgrounds/RainforestMiddleGradient");
            midFogLayer.Parallax = MidParallax;
            midFogLayer.DrawOffset = Vector2.Zero;

            MistShader midMistShader = new MistShader();
            midMistShader.StartColor = Color.SkyBlue * 0.5f;
            midMistShader.EndColor = Color.Transparent;
            midFogLayer.Shader = midMistShader;
            AddLayer(midFogLayer);
        }

        private void AddCloseLayer()
        {
            FrontLayer = new CustomBGLayer();
            FrontLayer.SetTexture("Assets/Textures/Backgrounds/GreyGrassBackgroundClose");
            FrontLayer.Parallax = CloseParallax;
            FrontLayer.DrawOffset = Vector2.Zero;
            AddLayer(FrontLayer);

            CustomBGLayer CloseLayer = new CustomBGLayer();
            CloseLayer.SetTexture("Assets/Textures/Backgrounds/RainforestFrontGradient");
            CloseLayer.Parallax = CloseParallax;
            CloseLayer.DrawOffset = Vector2.Zero;

            MistShader frontMistShader = new MistShader();
            frontMistShader.StartColor = Color.SkyBlue * 0.75f;
            frontMistShader.EndColor = Color.Transparent;
            CloseLayer.Shader = frontMistShader;
            AddLayer(CloseLayer);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DrawScale = 1.5f;
            AddFarLayer();
            AddMidLayer();
            AddCloseLayer();
        }

        public override bool IsActive()
        {
            NoSurfaceOffset = true;
            DrawScale = 1;
            DrawOffset = Vector2.Zero;
            return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneWorldsEnd;
        }
    }
}
