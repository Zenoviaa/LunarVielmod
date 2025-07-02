using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.Core.MagicSystem.UI
{
    internal class InventoryMenu : UIElement
    {
        private InventoryBackground _inventoryBackground;
        private UIGrid _grid;
        public InventoryMenu()
        {
            _inventoryBackground = new InventoryBackground();
            _grid = new UIGrid();
        }


        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 428;
            Height.Pixels = 236;
            Append(_inventoryBackground);

            _grid.Width = Width;
            _grid.Height = Height;
            _grid.HAlign = 0.5f;
            _grid.VAlign = 0.5f;
            _grid.ListPadding = 2;
            Append(_grid);
            SetEnchantments();
        }

        public void SetEnchantments()
        {
            _grid.Clear();
            IEnumerable<Enchantment> enchantments = ModContent.GetContent<Enchantment>();
            foreach(var enchantment in enchantments)
            {
                var slot = new EnchantmentInventorySlot(enchantment.Item);
                _grid.Add(slot);
            }
            _grid.Recalculate();
        }
    }
}
