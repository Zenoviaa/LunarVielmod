using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Content.Biomes;
using Stellamod.Core.Backgrounds;
using Terraria;

namespace Stellamod.Content.Areas.WondrousDarkspace
{
    public class WonderousDarkspaceBackground : CustomBG
    {
        private Asset<Texture2D> _backgroundTextureAsset;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _backgroundTextureAsset = AssetManager.LoadBackground("Darkspace");
        }

        public override bool UseCustomDrawing()
        {
            return true;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            Color fadeToColor = Color.LightPink;
            fadeToColor.A = 50;
            BackgroundHelper.DrawSimpleAtlassedBackground(spriteBatch, BackgroundHelper.AtlassedBackgroundDraw.Default with
            {
                cameraMovement = CameraMovement,
                bg = _backgroundTextureAsset,
                numBackgrounds = 6,
                fadeToColor = fadeToColor,
                alpha = Alpha,
                parallax = new Vector2(0.01f, 0f),
                baseColor = Color.White
            });
        }

        public override bool IsActive()
        {
            BiomePlayer biomePlayer = Main.LocalPlayer.GetModPlayer<BiomePlayer>();
            MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
            return myPlayer.ZoneWonder && !myPlayer.ZoneCinder;
        }
    }
}
