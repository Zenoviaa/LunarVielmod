using System;
using Terraria;

namespace Stellamod.Common.MagicSystem.UI
{
    internal class StaffEditingContext
    {
        public readonly Staff staffToEdit;
        public StaffEditingContext(Staff staff)
        {
            this.staffToEdit = staff;
        }

        internal void SetElement(Item item)
        {
         //   throw new NotImplementedException();
        }

        internal void SetEnchantment(Item item, int index)
        {
            staffToEdit.SetEnchantmentAtIndex(item, index);
        }

        public Item GetEnchantment(int index)
        {
            //Return the item
            return staffToEdit.GetEnchantmentAtIndex(index);
        }
    }
}
