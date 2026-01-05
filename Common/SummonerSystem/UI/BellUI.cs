using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Common.SummonerSystem.UI
{
    public class BellUI : UIPanel
    {
        private UIPanel _panel;
        private UIImage _circleImage;
        private UIImage _flaskImage;
        private XButton _xButton;
        private BellInventoryMenu _inventoryMenu;
        private List<BellSlot> _slots;

        public const int width = 700;
        public const int height = 250;

        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2);
        public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2);

        public BellUI()
        {
            _xButton = new XButton(Close);
            _panel = new UIPanel();
            _slots = new List<BellSlot>();
            _inventoryMenu = new BellInventoryMenu();
            _circleImage = new UIImage(
                ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/BellPanel"));
            _flaskImage = new UIImage(
                ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/SummoningBell"));
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
            return Main.LocalPlayer.maxMinions != _slots.Count;
        }

        public void CalculateSlots()
        {
            foreach (var slot in _slots)
            {
                RemoveChild(slot);
            }

            _slots.Clear();
            for (int i = 0; i < Main.LocalPlayer.maxMinions; i++)
            {
                BellSlot slot = new BellSlot(i);
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
            Left.Pixels = RelativeLeft + 100;
            Top.Pixels = RelativeTop;

            Vector2 flaskOffset = new Vector2(92);
            _flaskImage.Left.Pixels = flaskOffset.X + 17;
            _flaskImage.Top.Pixels = flaskOffset.Y + 8;
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
            BellUISystem xi = ModContent.GetInstance<BellUISystem>();
            xi.CloseUI();
        }
    }
}
