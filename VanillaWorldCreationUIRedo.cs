using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Reflection;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod
{
    [Autoload(Side = ModSide.Client)]
    internal class VanillaWorldCreationUIRedo : ModSystem
    {
        private Asset<Texture2D> _BorderTexture;
        private Asset<Texture2D> _BackgroundExpertTexture;
        private Asset<Texture2D> _BackgroundNormalTexture;
        private Asset<Texture2D> _BackgroundMasterTexture;
        private Asset<Texture2D> _BunnyExpertTexture;
        private Asset<Texture2D> _BunnyNormalTexture;
        private Asset<Texture2D> _BunnyCreativeTexture;
        private Asset<Texture2D> _BunnyMasterTexture;
        private Asset<Texture2D> _EvilRandomTexture;
        private Asset<Texture2D> _EvilCorruptionTexture;
        private Asset<Texture2D> _EvilCrimsonTexture;
        private Asset<Texture2D> _SizeSmallTexture;
        private Asset<Texture2D> _SizeMediumTexture;
        private Asset<Texture2D> _SizeLargeTexture;
        public override void Load()
        {
            base.Load();
            _SizeSmallTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewSizeWorldVeil");
            _SizeMediumTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewSizeWorldVeil");
            _SizeLargeTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewSizeWorldVeil");

            _EvilRandomTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewBlankWorld");
            _EvilCorruptionTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewBlankWorld");
            _EvilCrimsonTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewBlankWorld");


            _BunnyMasterTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewDifficultyMaster");
            _BunnyExpertTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewDifficultyExpert");
            _BunnyNormalTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewDifficultyNormal");
            _BunnyCreativeTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewDifficultyMJourny");

            _BorderTexture = ModContent.Request<Texture2D>("Stellamod/Assets/Textures/UI/PreviewBorder");
            _BackgroundNormalTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyNormal1");
            _BackgroundExpertTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyExpert1");
            _BackgroundMasterTexture = Main.Assets.Request<Texture2D>("Images/UI/WorldCreation/PreviewDifficultyMaster1");
            On_UIWorldCreationPreview.DrawSelf += DrawWorldPreviewUI;
        }

        public override void Unload()
        {
            base.Unload();
            On_UIWorldCreationPreview.DrawSelf -= DrawWorldPreviewUI;
        }

        private void DrawWorldPreviewUI(On_UIWorldCreationPreview.orig_DrawSelf orig, UIWorldCreationPreview self, SpriteBatch spriteBatch)
        {
            //Completely overriding this function
            //orig(self, spriteBatch);
            if (!ModContent.GetInstance<LunarVeilClientConfig>().VanillaUIRespritesToggle)
            {
                orig(self, spriteBatch);
                return;
            }

            DrawWorldPreview(self, spriteBatch);
        }

        private void DrawWorldPreview(UIWorldCreationPreview uiItem, SpriteBatch spriteBatch)
        {
            Type type = typeof(UIWorldCreationPreview);
            byte _difficulty = (byte)type.GetField("_difficulty", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(uiItem);
            byte _evil = (byte)type.GetField("_evil", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(uiItem);
            byte _size = (byte)type.GetField("_size", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(uiItem);

            CalculatedStyle dimensions = uiItem.GetDimensions();
            Vector2 position = new Vector2(dimensions.X + 4f, dimensions.Y + 4f);
            Color color = Color.White;
            switch (_difficulty)
            {
                case 0:
                case 3:
                    spriteBatch.Draw(_BackgroundNormalTexture.Value, position, Color.White);
                    color = Color.White;
                    break;
                case 1:
                    spriteBatch.Draw(_BackgroundExpertTexture.Value, position, Color.White);
                    color = Color.DarkGray;
                    break;
                case 2:
                    spriteBatch.Draw(_BackgroundMasterTexture.Value, position, Color.White);
                    color = Color.DarkGray;
                    break;
            }

            switch (_size)
            {
                case 0:
                    spriteBatch.Draw(_SizeSmallTexture.Value, position, color);
                    break;
                case 1:
                    spriteBatch.Draw(_SizeMediumTexture.Value, position, color);
                    break;
                case 2:
                    spriteBatch.Draw(_SizeLargeTexture.Value, position, color);
                    break;
            }

            switch (_evil)
            {
                case 0:
                    spriteBatch.Draw(_EvilRandomTexture.Value, position, color);
                    break;
                case 1:
                    spriteBatch.Draw(_EvilCorruptionTexture.Value, position, color);
                    break;
                case 2:
                    spriteBatch.Draw(_EvilCrimsonTexture.Value, position, color);
                    break;
            }

            switch (_difficulty)
            {
                case 0:
                    spriteBatch.Draw(_BunnyNormalTexture.Value, position, color);
                    break;
                case 1:
                    spriteBatch.Draw(_BunnyExpertTexture.Value, position, color);
                    break;
                case 2:
                    spriteBatch.Draw(_BunnyMasterTexture.Value, position, color * 1.2f);
                    break;
                case 3:
                    spriteBatch.Draw(_BunnyCreativeTexture.Value, position, color);
                    break;
            }

            spriteBatch.Draw(_BorderTexture.Value, new Vector2(dimensions.X, dimensions.Y), Color.White);
        }
    }
}
