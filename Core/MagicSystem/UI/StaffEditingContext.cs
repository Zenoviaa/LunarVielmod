using Stellamod.Content.Items.MoonlightMagic;
using Terraria;
namespace Stellamod.Core.MagicSystem.UI
{
    public class StaffEditingContext
    {
        public readonly BaseStaff staffToEdit;
        public StaffEditingContext(BaseStaff staff)
        {
            this.staffToEdit = staff;
        }

        public void SetElement(Item item)
        {
            //   throw new NotImplementedException();
            staffToEdit.SetElement(item);
            staffToEdit.Item.NetStateChanged();
        }

        public void SetEnchantment(Item item, int index)
        {
            staffToEdit.SetEnchantmentAtIndex(item, index);
            staffToEdit.Item.NetStateChanged();
        }

        public Item GetElement()
        {
            //Return the item
            return staffToEdit.GetElement();
        }
        public Item GetEnchantment(int index)
        {
            //Return the item
            return staffToEdit.GetEnchantmentAtIndex(index);
        }
    }
}
