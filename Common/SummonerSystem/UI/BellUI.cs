using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.UI;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.Common.SummonerSystem.UI
{
    public class BellUI : UIPanel
    {
        private CommonBackButton _backButton;
        private UIImage _backgroundImage;
        private UIScrollbar _scrollbar;
        private GuardianSlot _guardianSlot;
        private BellInventoryMenu _inventoryMenu;
        private List<BellSlot> _slots;
        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2) - 64;
        public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2) - 64;

        public BellUI()
        {
            _guardianSlot = new GuardianSlot();
            _backButton = new CommonBackButton(Close);
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

          
            Append(_backgroundImage);

            _inventoryMenu.Left.Set(0, 0.5f);
            Append(_inventoryMenu);
            Append(_guardianSlot);
            Append(_scrollbar);

            _backButton.Top.Set(0f, 1f);
            _backButton.Left.Pixels = Width.Pixels / 2 - _backButton.Width.Pixels / 2;
            Append(_backButton);
            Orient();
        }

        public void UpdateSlots()
        {
            while (_slots.Count < Main.LocalPlayer.maxMinions)
            {
                BellSlot slot = new BellSlot(_slots.Count);
                slot.Activate();
                _slots.Add(slot);
                Append(slot);
            }
        }
        private void Orient()
        {
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft + 100;
            Top.Pixels = RelativeTop;
            _guardianSlot.Top.Set(0, 1);
            _scrollbar.Left.Set(-32, 1f);
            _scrollbar.Top.Set(12, 0f);
            _inventoryMenu.Left.Set(-_inventoryMenu.Width.Pixels - 64, 1f);

            Width.Pixels = 550;
            _backgroundImage.Width.Pixels = 480;
            _backgroundImage.Height.Pixels = 154;
            _backgroundImage.Left.Set(-500, 1f);
            _backgroundImage.Top.Set(-_backgroundImage.Height.Pixels - 80, 1f);

            _backButton.Top.Set(-64, 1f);
            _backButton.Left.Pixels = Width.Pixels / 2 - _backButton.Width.Pixels / 2;

            _guardianSlot.Left.Set(-500, 1f);
            _guardianSlot.Top.Set(-_guardianSlot.Height.Pixels - 96, 1f);

            Height.Pixels = 500;

            //Orient slots
            float x = _guardianSlot.Width.Pixels + 18;
            float y = Height.Pixels - _backgroundImage.Height.Pixels - 80 - 8;

            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                if (slot.IsHidden())
                    continue;

                //Padding 
            
                if (x >= Width.Pixels - 80)
                {
                    x = _guardianSlot.Width.Pixels + 18;
                    y += slot.Height.Pixels + 4;
                }

                slot.Left.Pixels = x;
                slot.Top.Pixels = y;
                x += slot.Width.Pixels + 4;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            UpdateSlots();
            Orient();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            this.QuickMouseInteraction();
        }

        private void Close()
        {
            BellUISystem xi = ModContent.GetInstance<BellUISystem>();
            xi.CloseUI();
        }
    }
}
