using Microsoft.Xna.Framework;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;

namespace Stellamod.Content.Backgrounds
{
    internal class IceLakeBG : CustomBG
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DrawScale = 1.5f;
            DrawOffset = new Vector2(0, -8);
            CustomBGLayer backLayer = new CustomBGLayer();
            backLayer.SetTexture("Assets/Textures/Backgrounds/IceBack");
            backLayer.Parallax = 0.2f;
            backLayer.DrawOffset = Vector2.Zero;
            AddLayer(backLayer);

            //Guh
            CustomBGLayer backFogLayer = new CustomBGLayer();
            backFogLayer.SetTexture("Assets/Textures/Backgrounds/IceGradient");
            backFogLayer.Parallax = 0.2f;
            backFogLayer.DrawOffset = Vector2.Zero;

            MistShader mistShader = new MistShader();
            mistShader.StartColor = Color.White * 0.5f;
            mistShader.EndColor = Color.Transparent;
            backFogLayer.Shader = mistShader;
            AddLayer(backFogLayer);

            CustomBGLayer back2Layer = new CustomBGLayer();
            back2Layer.SetTexture("Assets/Textures/Backgrounds/IceBack2");
            back2Layer.Parallax = 0.25f;
            back2Layer.DrawOffset = Vector2.Zero;
            AddLayer(back2Layer);


            CustomBGLayer midLayer = new CustomBGLayer();
            midLayer.SetTexture("Assets/Textures/Backgrounds/IceMiddle");
            midLayer.Parallax = 0.35f;
            midLayer.DrawOffset = Vector2.Zero;
            AddLayer(midLayer);

            CustomBGLayer midFogLayer = new CustomBGLayer();
            midFogLayer.SetTexture("Assets/Textures/Backgrounds/IceGradient");
            midFogLayer.Parallax = 0.35f;
            midFogLayer.DrawOffset = Vector2.Zero;

            MistShader midMistShader = new MistShader();
            midMistShader.StartColor = Color.White * 0.5f;
            midMistShader.EndColor = Color.Transparent;
            midFogLayer.Shader = midMistShader;
            AddLayer(midFogLayer);

            CustomBGLayer frontLayer = new CustomBGLayer();
            frontLayer.SetTexture("Assets/Textures/Backgrounds/IceFront");
            frontLayer.Parallax = 0.4f;
            frontLayer.DrawOffset = Vector2.Zero;
            AddLayer(frontLayer);

            CustomBGLayer front2Layer = new CustomBGLayer();
            front2Layer.SetTexture("Assets/Textures/Backgrounds/IceFront2");
            front2Layer.Parallax = 0.5f;
            front2Layer.DrawOffset = Vector2.Zero;
            AddLayer(front2Layer);
        }

        public override bool IsActive()
        {
            return Main.LocalPlayer.ZoneSnow;
        }
    }
}
