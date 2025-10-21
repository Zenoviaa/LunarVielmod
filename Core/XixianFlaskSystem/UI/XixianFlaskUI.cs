using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.XixianFlaskSystem.UI
{
    public class XixianFlaskUI : UIPanel
    {
        private UIGrid _grid;

        private UIPanel _panel;
        private UIImage _circleImage;
        private UIImage _flaskImage;
        private XButton _xButton;
        private InsourceInventoryMenu _inventoryMenu;
        private List<InsourceSlot> _slots;

        public const int width = 700;
        public const int height = 250;

        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2);
        public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2);

        public XixianFlaskUI()
        {
            _xButton = new XButton(Close);
            _grid = new UIGrid();
            _panel = new UIPanel();
            _slots = new List<InsourceSlot>();
            _inventoryMenu = new InsourceInventoryMenu();
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
            Append(_xButton);
            Orient();
        }

        public bool NeedsRecalculate()
        {
            FlaskPlayer flaskPlayer = Main.LocalPlayer.GetModPlayer<FlaskPlayer>();
            return flaskPlayer.maxInsourceCount != _slots.Count;
        }

        public void CalculateSlots()
        {
            _inventoryMenu.SetInsources();
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
            if (NeedsRecalculate())
            {
                CalculateSlots();
            }
            Orient();

        }
        private void Orient()
        {
            Width.Pixels = width;
            Height.Pixels = height;

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
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
    }
}
