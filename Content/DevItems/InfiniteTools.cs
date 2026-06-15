using Stellamod.Common.WeaponTypes.CombatTools;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.DevItems
{
    public class InfiniteTools : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            CombatTool combatTool = player.GetModPlayer<CombatToolPlayer>().SelectedTool.GetGlobalItem<CombatTool>();
            if (combatTool.ammoCount <= 0)
                combatTool.ammoCount++;
        }
    }
}
