using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Backgrounds
{
    public class WorldsEndBG : CustomBG
    {
        public CustomBGLayer BackLayer;
        public CustomBGLayer MidLayer;
        public CustomBGLayer FrontLayer;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DrawScale = 1.5f;
            float startParallax = 0.35f;
            BackLayer = new CustomBGLayer();
            BackLayer.SetTexture("Assets/Textures/Backgrounds/GreyGrassBackgroundFar");
            BackLayer.Parallax = 0.1f;
            BackLayer.DrawOffset = Vector2.Zero;
            AddLayer(BackLayer);

            CustomBGLayer backFogLayer = new CustomBGLayer();
            backFogLayer.SetTexture("Assets/Textures/Backgrounds/RainforestFrontGradient");
            backFogLayer.Parallax = 0.5f;
            backFogLayer.DrawOffset = Vector2.Zero;


            MistShader backMistShader = new MistShader();
            backMistShader.StartColor = Color.SkyBlue * 0.75f;
            backMistShader.EndColor = Color.Transparent;
            backFogLayer.Shader = backMistShader;
            AddLayer(backFogLayer);

            MidLayer = new CustomBGLayer();
            MidLayer.SetTexture("Assets/Textures/Backgrounds/GreyGrassBackgroundMid");
            MidLayer.Parallax = startParallax + 0.02f;
            MidLayer.DrawOffset = Vector2.Zero;
            AddLayer(MidLayer);

            CustomBGLayer midFogLayer = new CustomBGLayer();
            midFogLayer.SetTexture("Assets/Textures/Backgrounds/RainforestMiddleGradient");
            midFogLayer.Parallax = startParallax + 0.02f; ;
            midFogLayer.DrawOffset = Vector2.Zero;


            MistShader midMistShader = new MistShader();
            midMistShader.StartColor = Color.SkyBlue * 0.5f;
            midMistShader.EndColor = Color.Transparent;
            midFogLayer.Shader = midMistShader;
            AddLayer(midFogLayer);

            FrontLayer = new CustomBGLayer();
            FrontLayer.SetTexture("Assets/Textures/Backgrounds/GreyGrassBackgroundClose");
            FrontLayer.Parallax = startParallax + 0.05f;
            FrontLayer.DrawOffset = Vector2.Zero;
            AddLayer(FrontLayer);

            CustomBGLayer CloseLayer = new CustomBGLayer();
            CloseLayer.SetTexture("Assets/Textures/Backgrounds/RainforestFrontGradient");
            CloseLayer.Parallax = startParallax + 0.05f;
            CloseLayer.DrawOffset = Vector2.Zero;


            MistShader frontMistShader = new MistShader();
            frontMistShader.StartColor = Color.SkyBlue * 0.75f;
            frontMistShader.EndColor = Color.Transparent;
            CloseLayer.Shader = frontMistShader;
            AddLayer(CloseLayer);

        }

        public override bool IsActive()
        {
            NoSurfaceOffset = true;
            DrawScale = 1;
            DrawOffset = new Vector2(0, -520);
            return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneWorldsEnd;
        }
    }
}
