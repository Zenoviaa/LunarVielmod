using Stellamod.Core.ArmorReforge.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items
{
    internal class TestItem : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool? UseItem(Player player)
        {
            var uiSystem = ModContent.GetInstance<ReforgeUISystem>();
            uiSystem.ToggleUI();
            Main.NewText("Use Ietm");
            return true;
        }
    }
}
