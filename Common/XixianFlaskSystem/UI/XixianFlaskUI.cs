using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.UI;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Common.XixianFlaskSystem.UI
{
    public class XixianFlaskUI : UIPanel
    {
        private CommonBackButton _backButton;
        private UIPanel _panel;
        private UIImage _circleImage;
        private UIImage _flaskImage;
        private UIScrollbar _scrollbar;
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
            _scrollbar = new FancyScrollbar();
            _inventoryMenu = new InsourceInventoryMenu(_scrollbar);
            _circleImage = new UIImage(
                ModContent.Request<Texture2D>(XixianFlaskUISystem.RootTexturePath + "InsourcePanel"));
            _flaskImage = new UIImage(
                ModContent.Request<Texture2D>(XixianFlaskUISystem.RootItemTexturePath + "XixianFlask"));
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
            Width.Pixels = 550;
            Height.Pixels = 500;
            _backButton.Left.Pixels = Width.Pixels / 2 - _backButton.Width.Pixels / 2;
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft + 100;
            Top.Pixels = RelativeTop;

            Vector2 flaskOffset = new Vector2(92);
            _flaskImage.Left.Pixels = flaskOffset.X + 12;
            _flaskImage.Top.Pixels = flaskOffset.Y;
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
            }
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
