using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
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
        private UIImage _backgroundImage;
        private UIScrollbar _scrollbar;
        private XButton _xButton;
        private BellInventoryMenu _inventoryMenu;
        private List<BellSlot> _slots;
        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2);
        public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2);

        public BellUI()
        {
            _xButton = new XButton(Close);
            _slots = new List<BellSlot>();
            _scrollbar = new FancyScrollbar();
            _inventoryMenu = new BellInventoryMenu(_scrollbar);
            _backgroundImage = new UIImage(RequestTexture("BellActivePanel"));
        }
        private Asset<Texture2D> RequestTexture(string name)
        {
            return ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + $"/{name}");
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 700;
            Height.Pixels = 250;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _backgroundImage.Top.Set(0f, 1f);
            Append(_backgroundImage);

            _inventoryMenu.Left.Set(0, 0.5f);
            Append(_inventoryMenu);
            Append(_scrollbar);
            Append(_xButton);
            Orient();
        }

        public void UpdateSlots()
        {
            while(_slots.Count < Main.LocalPlayer.maxMinions)
            {
                BellSlot slot = new BellSlot(_slots.Count);
                _slots.Add(slot);
                Append(slot);
            }
        }
        private void Orient()
        {
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft + 100;
            Top.Pixels = RelativeTop;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateSlots();
            Orient();
        }


        private void Close()
        {
            BellUISystem xi = ModContent.GetInstance<BellUISystem>();
            xi.CloseUI();
        }
    }
}
