using Microsoft.Xna.Framework;
using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Stellamod.Core.Effects;
using Terraria;

namespace Stellamod.Backgrounds
{
    public class MistyDungeonBG : CustomBG
    {
        public override void SetDrawDefaults()
        {
            base.SetDrawDefaults();
            DrawScale = 1.5f;
            DrawOffset = new Vector2(0, 0);
            DrawColor = Color.Lerp(Color.White, Color.Black, 0.5f);
            NoSurfaceLight = true;
            NoSurfaceOffset = true;
            NoParallaxY = true;
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Layers.Clear();
                      CustomBGLayer backLayer = new CustomBGLayer();
            backLayer.SetTexture("Assets/Textures/Backgrounds/MistyDungeon_Back");
            backLayer.Parallax = 0.2f;
            backLayer.DrawOffset = Vector2.Zero;
            AddLayer(backLayer);

            //Guh
            CustomBGLayer midLayer = new CustomBGLayer();
            midLayer.SetTexture("Assets/Textures/Backgrounds/MistyDungeon_Mid");
            midLayer.Parallax = 0.35f;
            midLayer.DrawOffset = Vector2.Zero;
            AddLayer(midLayer);


            CustomBGLayer midFogLayer = new CustomBGLayer();
            midFogLayer.SetTexture("Assets/Textures/Backgrounds/RainforestMiddleGradient");
            midFogLayer.Parallax = 0.35f;
            midFogLayer.DrawOffset = Vector2.Zero;


            /*
            CustomBGLayer midFogLayer2 = new CustomBGLayer();
            midFogLayer2.SetTexture("Assets/Textures/Backgrounds/RainforestMiddleGradient");
            midFogLayer2.Parallax = 0.35f;
            midFogLayer2.DrawOffset = Vector2.Zero;

            MistShader midMistShader2 = new MistShader();
            midMistShader2.StartColor = Color.Transparent;
            midMistShader2.EndColor = Color.Blue * 0.25f;
            midFogLayer2.Shader = midMistShader2;
            AddLayer(midFogLayer2);
            */


            CustomBGLayer frontLayer = new CustomBGLayer();
            frontLayer.SetTexture("Assets/Textures/Backgrounds/MistyDungeon_Top");
            frontLayer.Parallax = 0.4f;
            frontLayer.DrawOffset = Vector2.Zero;
            AddLayer(frontLayer);

            CustomBGLayer front2Layer = new CustomBGLayer();
            front2Layer.SetTexture("Assets/Textures/Backgrounds/MistyDungeon_TopTop");
            front2Layer.Parallax = 0.5f;
            front2Layer.DrawOffset = Vector2.Zero;
            AddLayer(front2Layer);
        }
        public override int GetParallaxYStartHeight()
        {
            return base.GetParallaxYStartHeight();
        }

        public override bool IsActive()
        {
            if (Main.myPlayer == -1)
                return false;
            if (!Main.PlayerLoaded)
                return false;
            return Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMistyDungeon;
        }
    }
}
