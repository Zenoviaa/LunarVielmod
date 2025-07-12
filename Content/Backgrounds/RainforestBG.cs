using Microsoft.Xna.Framework;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;

namespace Stellamod.Content.Backgrounds
{
    internal class RainforestBG : CustomBG
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DrawScale = 1.5f;
            CustomBGLayer backLayer = new CustomBGLayer();
            backLayer.SetTexture("Assets/Textures/Backgrounds/RainforestBack");
            backLayer.Parallax = 0.2f;
            backLayer.DrawOffset = Vector2.Zero;
            AddLayer(backLayer);

            /*
            CustomBGLayer backFogLayer = new CustomBGLayer();
            backFogLayer.SetTexture("Assets/Textures/Backgrounds/RainforestBackGradient");
            backFogLayer.Parallax = 0.2f;
            backFogLayer.DrawOffset = Vector2.Zero;
            backFogLayer.BlendState = Microsoft.Xna.Framework.Graphics.BlendState.Additive;
            AddLayer(backFogLayer);
            */

            CustomBGLayer midLayer = new CustomBGLayer();
            midLayer.SetTexture("Assets/Textures/Backgrounds/RainforestMiddle");
            midLayer.Parallax = 0.35f;
            midLayer.DrawOffset = Vector2.Zero;
            AddLayer(midLayer);

            
            
            CustomBGLayer midFogLayer = new CustomBGLayer();
            midFogLayer.SetTexture("Assets/Textures/Backgrounds/RainforestMiddleGradient");
            midFogLayer.Parallax = 0.35f;
            midFogLayer.DrawOffset = Vector2.Zero;

            MistShader midMistShader = new MistShader();
            midMistShader.StartColor = Color.DarkGray * 0.25f;
            midMistShader.EndColor = Color.Transparent;
            midFogLayer.Shader = midMistShader;
            AddLayer(midFogLayer);
            
            CustomBGLayer frontLayer = new CustomBGLayer();
            frontLayer.SetTexture("Assets/Textures/Backgrounds/RainforestFront");
            frontLayer.Parallax = 0.5f;
            frontLayer.DrawOffset = Vector2.Zero;
            AddLayer(frontLayer);

            CustomBGLayer frontFogLayer = new CustomBGLayer();
            frontFogLayer.SetTexture("Assets/Textures/Backgrounds/RainforestFrontGradient");
            frontFogLayer.Parallax = 0.5f;
            frontFogLayer.DrawOffset = Vector2.Zero;

            MistShader frontMistShader = new MistShader();
            frontMistShader.StartColor = Color.DarkGray * 0.5f;
            frontMistShader.EndColor = Color.Transparent;
            frontFogLayer.Shader = frontMistShader;
            AddLayer(frontFogLayer);

        }

        public override bool IsActive()
        {
            //TODO:
            //Come back later to make it check for rainforest biome
            
            return false;
        }
    }
}
