using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Stellamod.UI;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.MagicSystem.UI
{
    public class EnchantmentMenu : UIPanel
    {
        private Texture2D _enchantmentPanel;
        private StaffEditingContext _ctx;
        private UIGrid _grid;
        private UIGrid _timedGrid;
        private UIImage _backgroundSquare;
        private UIScrollbar _scrollbar;
        private XButton _xButton;

        private InventoryMenu _inventoryMenu;

        private StaffSlot _staffSlot;
        private ElementSlot _elementSlot;

        private static readonly Asset<Texture2D> BackgroundSquareTexture;
        static EnchantmentMenu()
        {
            // Don't run this on the server
            if (Main.dedServ)
                return;
            string texturePath = typeof(EnchantmentMenu).DirectoryHere() + "/EnchantingMenu";
            BackgroundSquareTexture = ModContent.Request<Texture2D>(texturePath);
        }


        public EnchantmentMenu() : base()
        {
            _grid = new UIGrid();
            _timedGrid = new UIGrid();
            _inventoryMenu = new InventoryMenu();
            _backgroundSquare = new UIImage(BackgroundSquareTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                AllowResizingDimensions = true,
                ScaleToFit = true,
            };

            _xButton = new XButton(Close);
            _elementSlot = new ElementSlot();
            _staffSlot = new StaffSlot();
        }

        public int RelativeLeft => ScreenHelper.TrueScreenWidth / 2 - (int)Width.Pixels / 2;
        public int RelativeTop => ScreenHelper.TrueScreenHeight / 2 - (int)Height.Pixels / 2;
        public void UseContext(StaffEditingContext ctx)
        {
            _ctx = ctx;
            _grid.Clear();
            _timedGrid.Clear();
            int slotIndex = 0;
            for (int i = 0; i < ctx.staffToEdit.GetNormalSlotCount(); i++)
            {
                var slot = new EnchantmentSlot(slotIndex, isTimedSlot: false);
                slot.SetContext(ctx);
                _grid.Add(slot);
                slotIndex++;
            }


            for (int i = 0; i < ctx.staffToEdit.GetTimedSlotCount(); i++)
            {
                var slot = new EnchantmentSlot(slotIndex, isTimedSlot: true);
                slot.SetContext(ctx);
                _timedGrid.Add(slot);
                slotIndex++;
            }
            _grid.Recalculate();
            _timedGrid.Recalculate();


            _staffSlot.SetContext(ctx);
            _elementSlot.SetContext(ctx);
        }

        public void Rebuild()
        {
            _inventoryMenu?.SetEnchantments();
        }
        public override void OnActivate()
        {
            base.OnActivate();
            _enchantmentPanel = ModContent.Request<Texture2D>(GetType().DirectoryHere() + $"/EnchantingMenu", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 704;
            Height.Pixels = 704;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

           
            Append(_backgroundSquare);

            _grid.Width.Set(0, 0.8f);
            _grid.Height.Set(0, 0.35f);
            _grid.HAlign = 0.5f;
            _grid.VAlign = 0.65f;
            _grid.ListPadding = 2f;
            Append(_grid);

            _timedGrid.Width.Set(0, 0.8f);
            _timedGrid.Height.Set(0, 0.35f);
            _timedGrid.HAlign = 0.5f;
            _timedGrid.VAlign = 1f;
            _timedGrid.ListPadding = 2f;
            Append(_timedGrid);

            _staffSlot.HAlign = 0.05f;
            _staffSlot.VAlign = 0.05f;
            Append(_staffSlot);

            _elementSlot.HAlign = 0.19f;
            _elementSlot.VAlign = 0.19f;
            Append(_elementSlot);

            _inventoryMenu.HAlign = 0.9f;
            _inventoryMenu.VAlign = 0.05f;
            Append(_inventoryMenu);
            Append(_xButton);
            SetPos();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
                _staffSlot.ReturnItem();
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }


        private void SetPos()
        {
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            _backgroundSquare.Width = Width;
            _backgroundSquare.Height = Height;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            SetPos();

        }

        private void Close()
        {
            MagicUISystem uiSystem = ModContent.GetInstance<MagicUISystem>();
            uiSystem.CloseUI();
        }
    }
}
