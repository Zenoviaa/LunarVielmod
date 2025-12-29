using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.MagicSystem.UI;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            Width.Pixels = 32;
            Height.Pixels = 32; 
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
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Rectangle dimensions = UIHelper.MouseInterfaceInteraction(this);
            Vector2 topLeft = dimensions.TopLeft();
            spriteBatch.Draw(TextureAsset.Value, topLeft, Color.White);
        }
    }

    public class ZTileToolbar : UIPanel
    {
        private ZTileButton _frameButton;
        private ZTileButton _zLayerButton;
        private ZTileButton _renderLayerButton;
        private ZTileButton _scaleButton;
        private ZTileButton _rotateButton;
        private UIGrid _grid;
        public ZTileToolbar()
        {
            _grid = new UIGrid();
            _frameButton = new ZTileButton(LoadTextureAsset("ToolFrame"), ChangeFrame);
            _zLayerButton = new ZTileButton(LoadTextureAsset("ToolZLayer"), ChangeZ);
            _renderLayerButton = new ZTileButton(LoadTextureAsset("ToolRenderLayer"), ChangeRenderLayer);
            _scaleButton = new ZTileButton(LoadTextureAsset("ToolScale"), ChangeScale);
            _rotateButton = new ZTileButton(LoadTextureAsset("ToolRotate"), ChangeRotation);
        }
        public int RelativeLeft => 64;
        public int RelativeTop => 64;

        private Asset<Texture2D> LoadTextureAsset(string fileName)
        {
            return ModContent.Request<Texture2D>(typeof(ZTileToolbar).DirectoryHere() + "/" + fileName);
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 64;
            Height.Pixels = 512;
            _grid.Width.Pixels= 32;
            _grid.Height.Pixels = Height.Pixels;
            _grid.ListPadding = 32;
            _grid.Add(_frameButton);
            _grid.Add(_zLayerButton);
            _grid.Add(_renderLayerButton);
            _grid.Add(_scaleButton);
            _grid.Add(_rotateButton);
            Append(_grid);
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
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
        }
        private void ChangeRenderLayer(bool isRightClick)
        {
            int length = Enum.GetNames<ZRenderLayer>().Length;
            int index = (int)MagicPaintbrush.renderLayer;
            if (isRightClick)
            {
                index--;
                if (index < 0)
                    index = length - 1;
            }
            else
            {
                index++;
                if(index >= length)
                {
                    index = 0;
                }
            }
            MagicPaintbrush.renderLayer = (ZRenderLayer)index;
        }
        private void ChangeFrame(bool isRightClick)
        {
            int direction = isRightClick ? -1 : 1;
            int frame = MagicPaintbrush.frame;
            ZTileLoader loader = ModContent.GetInstance<ZTileLoader>();
            ZTile zTile = loader.GetTile(MagicPaintbrush.templateData.type);
            int maxFrame = zTile.frameCount;
            frame += direction;
            if (frame < 0)
                frame = maxFrame - 1;
            if (frame >= maxFrame)
                frame = 0;
            MagicPaintbrush.frame = (ushort)frame;
        }
        private void ChangeZ(bool isRightClick)
        {
            MagicPaintbrush.z += isRightClick ? -1 : 1;
        }

        private void ChangeScale(bool isRightClick)
        {
            if (isRightClick)
            {
                MagicPaintbrush.scale -= 0.1f;
            }
            else
            {
                MagicPaintbrush.scale += 0.1f;
            }
        }

        private void ChangeRotation(bool isRightClick)
        {
            int length = Enum.GetNames<Rotation>().Length;
            int index = (int)MagicPaintbrush.rotation;
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
            MagicPaintbrush.rotation = (Rotation)index;
        }


    }
}
