using Stellamod.Content.Items.MoonlightMagic;
using Terraria;
namespace Stellamod.Common.MagicSystem.UI
{
    public class StaffEditingContext
    {
        public readonly AbstractMagicWand staffToEdit;
        public StaffEditingContext(AbstractMagicWand staff)
        {
            this.staffToEdit = staff;
        }

        public void SetElement(Item item)
        {
            //   throw new NotImplementedException();
            staffToEdit.SetElement(item);
            staffToEdit.Item.NetStateChanged();
        }

        public void SetEnchantment(Item item, int index, bool isTimedSlot)
        {
            staffToEdit.SetEnchantment(item, index, isTimedSlot);
            staffToEdit.Item.NetStateChanged();
        }

        public Item GetElement()
        {
            //Return the item
            return staffToEdit.GetElement();
        }
        public Item GetEnchantment(int index, bool isTimedSlot)
        {
            //Return the item
            return staffToEdit.GetEnchantment(index, isTimedSlot);
        }
    }
}
