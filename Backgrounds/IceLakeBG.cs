using Microsoft.Xna.Framework;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;
namespace Stellamod.Backgrounds
{
    public class IceLakeBG : CustomBG
    {
        public CustomBGLayer backLayer;
        public CustomBGLayer back2Layer;
        public CustomBGLayer midLayer;
        public CustomBGLayer frontLayer;
        public CustomBGLayer front2Layer;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DrawScale = 1.5f;
            DrawOffset = new Vector2(0, -100);
            float startParallax = 0.15f;
            CustomBGLayer bleedLayer = new CustomBGLayer();
            bleedLayer.SetTexture("Assets/Textures/Backgrounds/IceUnderground");
            bleedLayer.Parallax = startParallax;
            bleedLayer.DrawOffset = new Vector2(0, bleedLayer.Texture.Size().Y * DrawScale * 2);
            AddLayer(bleedLayer);


            backLayer = new CustomBGLayer();
            backLayer.SetTexture("Assets/Textures/Backgrounds/IceBack");
            backLayer.Parallax = startParallax;
            backLayer.DrawOffset = Vector2.Zero;
            AddLayer(backLayer);

            //Guh
            CustomBGLayer backFogLayer = new CustomBGLayer();
            backFogLayer.SetTexture("Assets/Textures/Backgrounds/IceGradient");
            backFogLayer.Parallax = startParallax;
            backFogLayer.DrawOffset = Vector2.Zero;

            MistShader mistShader = new MistShader();
            mistShader.StartColor = Color.White * 0.5f;
            mistShader.EndColor = Color.Transparent;
            backFogLayer.Shader = mistShader;
            AddLayer(backFogLayer);

            back2Layer = new CustomBGLayer();
            back2Layer.SetTexture("Assets/Textures/Backgrounds/IceBack2");
            back2Layer.Parallax = startParallax;
            back2Layer.DrawOffset = Vector2.Zero;
            AddLayer(back2Layer);


            midLayer = new CustomBGLayer();
            midLayer.SetTexture("Assets/Textures/Backgrounds/IceMiddle");
            midLayer.Parallax = startParallax + 0.02f;
            midLayer.DrawOffset = Vector2.Zero;
            AddLayer(midLayer);

            CustomBGLayer midFogLayer = new CustomBGLayer();
            midFogLayer.SetTexture("Assets/Textures/Backgrounds/IceGradient");
            midFogLayer.Parallax = startParallax + 0.02f;
            midFogLayer.DrawOffset = Vector2.Zero;

            MistShader midMistShader = new MistShader();
            midMistShader.StartColor = Color.White * 0.5f;
            midMistShader.EndColor = Color.Transparent;
            midFogLayer.Shader = midMistShader;
            AddLayer(midFogLayer);

            frontLayer = new CustomBGLayer();
            frontLayer.SetTexture("Assets/Textures/Backgrounds/IceFront");
            frontLayer.Parallax = startParallax + 0.05f;
            frontLayer.DrawOffset = Vector2.Zero;
            AddLayer(frontLayer);

            front2Layer = new CustomBGLayer();
            front2Layer.SetTexture("Assets/Textures/Backgrounds/IceFront2");
            front2Layer.Parallax = startParallax + 0.09f;
            front2Layer.DrawOffset = Vector2.Zero;
            AddLayer(front2Layer);

        }
        public override void SetDrawDefaults()
        {
            base.SetDrawDefaults();
            LocalParallaxSpeed = 4;
            backLayer.Parallax = 0.12f;
            back2Layer.Parallax = 0.14f;
            midLayer.Parallax = 0.16f;
            frontLayer.Parallax = 0.18f;
            front2Layer.Parallax = 0.2f;
        }

        public override bool IsActive()
        {
            return Main.LocalPlayer.ZoneSnow && Main.LocalPlayer.ZoneOverworldHeight;
        }
    }
}
