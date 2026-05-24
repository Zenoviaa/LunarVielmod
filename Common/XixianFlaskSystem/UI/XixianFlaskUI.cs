using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.UI;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using System.Text;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Common.XixianFlaskSystem.UI
{
    public class XixianFlaskUI : UIPanel
    {
        private UIImage _backgroundImage;
        private CommonBackButton _backButton;
        private UIPanel _panel;
        private UIImage _circleImage;
        private UIImage _flaskImage;
        private UIScrollbar _effectTextScrollbar;
        private UIScrollbar _scrollbar;
        private UIText _effectText;
        private UIList _effectUIList;
        private InsourceInventoryMenu _inventoryMenu;
        private List<InsourceSlot> _slots;

        public const int width = 700;
        public const int height = 450;

        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2) - 64;
        public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2) - 64;
        public XixianFlaskUI()
        {
            _backButton = new CommonBackButton(Close);
            _panel = new UIPanel();
            _slots = new List<InsourceSlot>();
            _effectTextScrollbar = new FancyScrollbar();
            _scrollbar = new FancyScrollbar();
            _inventoryMenu = new InsourceInventoryMenu(_scrollbar);
            _circleImage = new UIImage(
                ModContent.Request<Texture2D>(XixianFlaskUISystem.RootTexturePath + "InsourcePanel"));
            _flaskImage = new UIImage(
                ModContent.Request<Texture2D>(XixianFlaskUISystem.RootItemTexturePath + "XixianFlask"));
            _backgroundImage = new UIImage(RequestTexture("BellActivePanel"));
            _effectText = new UIText("No Effects...");
        }
        private Asset<Texture2D> RequestTexture(string name)
        {
            return ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + $"/{name}");
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = width;
            Height.Pixels = height;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _backgroundImage.Width.Pixels = 480;
            _backgroundImage.Height.Pixels = 154;
            _backgroundImage.Left.Set(-500, 1f);
            _backgroundImage.Top.Set(-_backgroundImage.Height.Pixels - 80, 1f);
            Append(_backgroundImage);
            _backgroundImage.Append(_effectText);

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            Append(_circleImage);
            Append(_flaskImage);

            _inventoryMenu.Left.Set(0, 0.5f);
            Append(_inventoryMenu);

            _backButton.Top.Set(-64, 1f);
            _backButton.Left.Pixels = Width.Pixels / 2 - _backButton.Width.Pixels / 2;
            Append(_backButton);

            _scrollbar.Left.Set(-32, 1f);
            _scrollbar.Top.Set(12, 0f);
            Append(_scrollbar);

            _effectUIList = new UIList();
            _effectUIList.Top.Pixels = 260;
            _effectUIList.Width.Pixels = Width.Pixels;
            _effectUIList.Height.Pixels = 140;
            _effectUIList.Add(_effectText);
            _effectUIList.SetScrollbar(_effectTextScrollbar);
            Append(_effectUIList);

            _effectTextScrollbar.Left.Set(-32, 1f);
            _effectTextScrollbar.Top.Set(0, 1f);
            Append(_effectTextScrollbar);


            Orient();
        }

        public override void OnActivate()
        {
            base.OnActivate();
            Main.playerInventory = true;
        }

        public bool NeedsRecalculate()
        {
            FlaskPlayer flaskPlayer = Main.LocalPlayer.GetModPlayer<FlaskPlayer>();
            return flaskPlayer.maxInsourceCount != _slots.Count;
        }

        public void CalculateSlots()
        {
            FlaskPlayer flaskPlayer = Main.LocalPlayer.GetModPlayer<FlaskPlayer>();
            foreach (var slot in _slots)
            {
                RemoveChild(slot);
            }

            _slots.Clear();
            for (int i = 0; i < flaskPlayer.maxInsourceCount; i++)
            {
                InsourceSlot slot = new InsourceSlot(i);
                _slots.Add(slot);
                Append(slot);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!Main.playerInventory)
            {
                Close();
            }
            if (NeedsRecalculate())
            {
                CalculateSlots();
            }
            Orient();

        }
        private void Orient()
        {
            _inventoryMenu.Left.Set(-154, 1f);

            _effectText.Left.Set(16, 0);
            _effectText.Top.Set(16, 0);
    
            Width.Pixels = 550;
            Height.Pixels = 500;
            _backButton.Left.Pixels = Width.Pixels / 2 - _backButton.Width.Pixels / 2;
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft + 100;
            Top.Pixels = RelativeTop;

            Vector2 flaskOffset = new Vector2(92);
            _flaskImage.Left.Pixels = flaskOffset.X + 12;
            _flaskImage.Top.Pixels = flaskOffset.Y;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                float f = i;
                float count = _slots.Count;
                float lerp = f / count;
                float rot = lerp * MathHelper.TwoPi;
                Vector2 offset = rot.ToRotationVector2() * 90;
                offset += new Vector2(92);
                slot.Left.Pixels = offset.X;
                slot.Top.Pixels = offset.Y;

                if(slot.Item == null || slot.Item.ModItem == null)
                {
                    string text = LangText.Common("EmptyInsource");
                    text = "- " + text;
                    sb.AppendLine(text);
                }
                else
                {
                    string text = LangText.Item(slot.Item.ModItem, "Tooltip");
                    text = "- " + text;
                    sb.AppendLine(text);
                }

            }
            _effectUIList.Left.Pixels = 16;
            _effectTextScrollbar.Height.Pixels = 128;
            _effectTextScrollbar.Left.Set(-8, 1f);
            _effectTextScrollbar.Top.Set(-222, 1f);
            _effectText.Width.Pixels = _backgroundImage.Width.Pixels - 32;
            _effectText.MaxWidth.Pixels = _backgroundImage.Width.Pixels - 32;

            _effectText.MaxHeight.Pixels = _backgroundImage.Height.Pixels;
            _effectText.IsWrapped = true;
            _effectText.DynamicallyScaleDownToWidth=true;
            string effectString = sb.ToString();
            if (string.IsNullOrEmpty(effectString))
                effectString = "...";
            _effectText.SetText(effectString);
        }

        private void Close()
        {
            XixianFlaskUISystem xi = ModContent.GetInstance<XixianFlaskUISystem>();
            xi.CloseUI();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Vector2 position = GetDimensions().ToRectangle().TopLeft();
            Rectangle rectangle = ExpandableTooltip.GetBGRectangle((int)position.X, (int)position.Y, (int)Width.Pixels, (int)Height.Pixels);
            Utils.DrawInvBG(spriteBatch, rectangle, new Color(23, 25, 81, 255) * 0.925f);
            this.QuickMouseInteraction();
        }

    }
}
