using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.UI.Elements;
using Terraria.ModLoader;
using Stellamod.Core.Helpers;
using System.Collections;
using ReLogic.Content;

namespace Stellamod.Core.MagicSystem.UI
{
    internal class EnchantmentMenu : UIPanel
    {
        private Texture2D _enchantmentPanel;
        private StaffEditingContext _ctx;
        private UIGrid _grid;
        private UIGrid _timedGrid;
        private UIImage _backgroundSquare;
        private InventoryMenu _inventoryMenu;

        private StaffSlot _staffSlot;
        private ElementSlot _elementSlot;

        private static readonly Asset<Texture2D> BackgroundSquareTexture;
        static EnchantmentMenu()
        {       
            // Don't run this on the server
            if (Main.dedServ)
                return;
            string texturePath = AssetHelper.DirectoryHere(typeof(EnchantmentMenu)) + "/EnchantingMenu";
            BackgroundSquareTexture = ModContent.Request<Texture2D>(texturePath);
        }


        internal EnchantmentMenu() : base()
        {
            _grid = new UIGrid();
            _timedGrid = new UIGrid();
            _inventoryMenu = new InventoryMenu();
            _backgroundSquare = new UIImage(BackgroundSquareTexture)
            {
                HAlign = 0f,
                VAlign = 0f,
                AllowResizingDimensions = true,
                ScaleToFit=true,
            };

            _elementSlot = new ElementSlot();   
            _staffSlot = new StaffSlot();
        }

        internal int RelativeLeft => ScreenHelper.TrueScreenWidth / 2 - (int)Width.Pixels / 2;
        internal int RelativeTop => ScreenHelper.TrueScreenHeight / 2 - (int)Height.Pixels / 2;

        public void UseContext(StaffEditingContext ctx)
        {
            _ctx = ctx;
            _grid.Clear();
            _timedGrid.Clear();
            for (int i = 0; i < ctx.staffToEdit.GetNormalSlotCount(); i++)
            {
                var slot = new EnchantmentSlot(index: _grid._items.Count, isTimedSlot: false);
                slot.SetContext(ctx);
                _grid.Add(slot);
            }

      
            for (int i = 0; i < ctx.staffToEdit.GetTimedSlotCount(); i++)
            {
                var slot = new EnchantmentSlot(index: _grid._items.Count, isTimedSlot: true);
                slot.SetContext(ctx);    
                _timedGrid.Add(slot);
            }
            _grid.Recalculate();
            _timedGrid.Recalculate();


            _staffSlot.SetContext(ctx);
            _elementSlot.SetContext(ctx);
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

            _elementSlot.HAlign = 0.18f;
            _elementSlot.VAlign = 0.22f;
            Append(_elementSlot);

            _inventoryMenu.HAlign = 1f;
            _inventoryMenu.VAlign = 0.05f;
            Append(_inventoryMenu);
            SetPos();
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
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
    }
}
