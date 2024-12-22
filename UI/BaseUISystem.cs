using Terraria.ModLoader;

namespace Stellamod.UI
{
    public abstract class BaseUISystem : ModSystem
    {
        private static BaseUISystem[] Slots = new BaseUISystem[2];
        public const int Slot_MinorUI = 0;
        public const int Slot_MajorUI = 1;

        //The slot this thing uses, so people can't flood their screen with UIS we will have some auto close when others are open :)
        public virtual int uiSlot => -1;

        public void TakeSlot()
        {
            if (uiSlot != -1)
            {
                ref BaseUISystem current = ref Slots[uiSlot];
                if (current != null && current != this)
                {
                    current.CloseThis();
                }
                current = this;
            }
        }

        public void ClearSlot()
        {
            if (uiSlot != -1)
            {
                ref BaseUISystem current = ref Slots[uiSlot];
                if (current == this)
                    current = null;
            }
        }

        public virtual void CloseThis()
        {

        }
    }
}
