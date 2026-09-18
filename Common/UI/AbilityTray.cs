using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.UI
{
    public class AbilityComparer : IComparer<Ability>
    {
        public int Compare(Ability x, Ability y)
        {
            return x.priority.CompareTo(y.priority);
        }
    }


    public class Ability
    {
        public Ability(UIElement element, float prioty)
        {
            this.uiElement = element;
            this.priority = prioty;
            this.isActive = true;
        }

        public UIElement uiElement;
        public float priority;
        public bool isActive;
    }

    [Autoload(Side = ModSide.Client)]
    public class AbilityTray : ModSystem
    {
        public static List<Ability> TrayItems;
        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            if (TrayItems == null)
                return;

            int leftOffset = 412;
            if (Main.playerInventory)
                leftOffset = 555;

            int padding = 64;
            TrayItems.Sort(new AbilityComparer());
            for (int i = 0; i < TrayItems.Count; i++)
            {
                Ability ability = TrayItems[i];
                if (!ability.isActive)
                {
                    ability.uiElement.Left.Pixels = -9999;
                    ability.uiElement.Top.Pixels = -9999;
                    continue;
                }

                UIElement uiElement = TrayItems[i].uiElement;
                uiElement.Left.Pixels = leftOffset;
                uiElement.Top.Pixels = 8;
                leftOffset += padding;
            }
        }

        public override void Load()
        {
            base.Load();
            TrayItems = new();
        }

        public override void Unload()
        {
            base.Unload();
            TrayItems?.Clear();
            TrayItems = null;
        }
    }
}
