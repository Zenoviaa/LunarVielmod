using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.Shaders;
using Stellamod.Helpers;
using System;
using System.Text;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.Core.ZTileSystem.UI
{
    public class ZTileButton : UIPanel
    {
        public delegate void ClickFunction(bool isRightClick);
        public ZTileButton(Asset<Texture2D> textureAsset, ClickFunction clickFunction)
        {
            this.TextureAsset = textureAsset;
            this.ButtonAction = clickFunction;
            Width.Pixels = 64;
            Height.Pixels = 64;
            OnLeftClick += LeftClick;
            OnRightClick += RightClick;
        }


        private void RightClick(UIMouseEvent evt, UIElement listeningElement)
        {
            ButtonAction(true);
        }

        private void LeftClick(UIMouseEvent evt, UIElement listeningElement)
        {

            ButtonAction(false);
        }

        public readonly Asset<Texture2D> TextureAsset;
        public readonly ClickFunction ButtonAction;

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            BackgroundColor = Color.Lerp(Color.Blue, Color.Black, 1f) * 0.5f;
            BorderColor = Color.Lerp(Color.Purple, Color.Black, 0.8f) * 0.5f;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Rectangle dimensions = UIHelper.MouseInterfaceInteraction(this);
            Vector2 topLeft = dimensions.TopLeft();
            Vector2 drawOrigin = TextureAsset.Value.Size() / 2f;
            Vector2 iconCenterPos = topLeft + dimensions.Size() / 2f;
            float scale = 1f;
            if (IsMouseHovering)
            {
                scale *= 1.2f;


                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, Main.Rasterizer, SpriteWhiteShader.Instance.Effect, Main.UIScaleMatrix);



                spriteBatch.Draw(TextureAsset.Value, iconCenterPos + Vector2.UnitX * 2, null, Color.Yellow, 0, drawOrigin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureAsset.Value, iconCenterPos - Vector2.UnitX * 2, null, Color.Yellow, 0, drawOrigin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureAsset.Value, iconCenterPos + Vector2.UnitY * 2, null, Color.Yellow, 0, drawOrigin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureAsset.Value, iconCenterPos - Vector2.UnitY * 2, null, Color.Yellow, 0, drawOrigin, scale, SpriteEffects.None, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.UIScaleMatrix);
            }
            else
            {

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, Main.Rasterizer, SpriteWhiteShader.Instance.Effect, Main.UIScaleMatrix);



                spriteBatch.Draw(TextureAsset.Value, iconCenterPos + Vector2.UnitX * 2, null, Color.DarkGray, 0, drawOrigin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureAsset.Value, iconCenterPos - Vector2.UnitX * 2, null, Color.DarkGray, 0, drawOrigin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureAsset.Value, iconCenterPos + Vector2.UnitY * 2, null, Color.DarkGray, 0, drawOrigin, scale, SpriteEffects.None, 0);
                spriteBatch.Draw(TextureAsset.Value, iconCenterPos - Vector2.UnitY * 2, null, Color.DarkGray, 0, drawOrigin, scale, SpriteEffects.None, 0);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, null, Main.UIScaleMatrix);
            }


            spriteBatch.Draw(TextureAsset.Value, iconCenterPos, null, Color.White, 0, drawOrigin, scale, SpriteEffects.None, 0);
        }
    }

    public class ZTileToolbar : UIPanel
    {
        private ZTileButton _frameButton;
        private ZTileButton _zLayerButton;
        private ZTileButton _renderLayerButton;
        private ZTileButton _scaleButton;
        private ZTileButton _rotateButton;

        private UIText _infoText;


        private UIPanel _panel;
        private UIGrid _grid;
        public ZTileToolbar()
        {
            _infoText = new UIText("0");

            _panel = new UIPanel();
            _grid = new UIGrid();
            _frameButton = new ZTileButton(LoadTextureAsset("ToolFrame"), ChangeFrame);
            _zLayerButton = new ZTileButton(LoadTextureAsset("ToolZLayer"), ChangeZ);
            _renderLayerButton = new ZTileButton(LoadTextureAsset("ToolRenderLayer"), ChangeRenderLayer);
            _scaleButton = new ZTileButton(LoadTextureAsset("ToolScale"), ChangeScale);
            _rotateButton = new ZTileButton(LoadTextureAsset("ToolRotate"), ChangeRotation);
        }
        public int RelativeLeft => 16;
        public int RelativeTop => 64;

        private Asset<Texture2D> LoadTextureAsset(string fileName)
        {
            return ModContent.Request<Texture2D>(typeof(ZTileToolbar).DirectoryHere() + "/" + fileName);
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 256;
            Height.Pixels = 512;
            _panel.Width.Pixels = 64;
            _panel.Height.Pixels = 512;
            _panel.BackgroundColor = Color.Lerp(Color.Blue, Color.Black, 0.8f) * 0.5f;
            _panel.BorderColor = Color.Lerp(Color.Purple, Color.Black, 0.8f) * 0.5f;

            _grid.Width.Pixels = 64;
            _grid.Height.Pixels = Height.Pixels;
            _grid.ListPadding = 8;



            _grid.Add(_frameButton);
            _grid.Add(_zLayerButton);
            _grid.Add(_renderLayerButton);
            _grid.Add(_scaleButton);
            _grid.Add(_rotateButton);
            _panel.Append(_infoText);
            _panel.Append(_grid);
            Append(_panel);
        }
        private void SetPos()
        {

            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            SetPos();
            UpdateUI();
            if (LunarVeilKeybinds.DecorNextFrame.JustReleased)
            {
                ChangeFrame(false);
            }
            if (LunarVeilKeybinds.DecorPrevFrame.JustReleased)
            {
                ChangeFrame(true);
            }

            if (LunarVeilKeybinds.DecorDownscale.JustReleased)
            {
                ChangeScale(false);
            }
            if (LunarVeilKeybinds.DecorUpscale.JustReleased)
            {
                ChangeScale(true);
            }

            if (LunarVeilKeybinds.DecorDownZ.JustReleased)
            {
                ChangeZ(false);
            }
            if (LunarVeilKeybinds.DecorUpZ.JustReleased)
            {
                ChangeZ(true);
            }

            if (LunarVeilKeybinds.DecorRotateLeft.JustReleased)
            {
                ChangeZ(false);
            }
            if (LunarVeilKeybinds.DecorRotateRight.JustReleased)
            {
                ChangeZ(true);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            Vector2 offset = new Vector2(90, 160);
            Vector2 drawPosition = topLeft + offset;
            ZTile tile = ModContent.GetInstance<ZTileLoader>().GetTile(DecorationBuilder.templateData.type);
            tile.DrawIcon2(spriteBatch, drawPosition, DecorationBuilder.frame);
        }
        private void UpdateUI()
        {
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            _panel.Width.Pixels = 64;
            _grid.Width.Pixels = 64;
            _grid.ListPadding = 8;
            _panel.BackgroundColor = Color.Lerp(Color.Blue, Color.Black, 1f) * 0.8f;
            _panel.BorderColor = Color.Lerp(Color.White, Color.Black, 0.2f) * 0.1f;

            _infoText.Left.Pixels = 64;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Frame {DecorationBuilder.frame}");
            sb.AppendLine($"Layer {DecorationBuilder.renderLayer}");
            sb.AppendLine($"Z {DecorationBuilder.z}");
            sb.AppendLine($"Scale {DecorationBuilder.scale}");
            sb.AppendLine($"Rotation {DecorationBuilder.rotation}");
            _infoText.SetText(sb.ToString());
        }

        private void ChangeRenderLayer(bool isRightClick)
        {
            int length = Enum.GetNames<ZRenderLayer>().Length;
            int index = (int)DecorationBuilder.renderLayer;
            if (isRightClick)
            {
                index--;
                if (index < 0)
                    index = length - 1;
            }
            else
            {
                index++;
                if (index >= length)
                {
                    index = 0;
                }
            }
            DecorationBuilder.renderLayer = (ZRenderLayer)index;
        }
        private void ChangeFrame(bool isRightClick)
        {
            int direction = isRightClick ? -1 : 1;
            int frame = DecorationBuilder.frame;
            ZTileLoader loader = ModContent.GetInstance<ZTileLoader>();
            ZTile zTile = loader.GetTile(DecorationBuilder.templateData.type);
            int maxFrame = zTile.frameCount;
            frame += direction;
            if (frame < 0)
                frame = maxFrame - 1;
            if (frame >= maxFrame)
                frame = 0;
            DecorationBuilder.frame = (ushort)frame;
        }
        private void ChangeZ(bool isRightClick)
        {
            DecorationBuilder.z += isRightClick ? -1 : 1;
        }

        private void ChangeScale(bool isRightClick)
        {
            if (isRightClick)
            {
                DecorationBuilder.scale -= 0.1f;
            }
            else
            {
                DecorationBuilder.scale += 0.1f;
            }
        }

        private void ChangeRotation(bool isRightClick)
        {
            int length = Enum.GetNames<Rotation>().Length;
            int index = (int)DecorationBuilder.rotation;
            if (isRightClick)
            {
                index--;
                if (index < 0)
                    index = length - 1;
            }
            else
            {
                index++;
                if (index >= length)
                {
                    index = 0;
                }
            }
            DecorationBuilder.rotation = (Rotation)index;
        }


    }
}
