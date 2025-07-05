using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Stellamod.Core.ToolsSystem;

namespace Stellamod.Content.Items.Test
{
    internal class ToolbarEnabler : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 0.9f;
            Item.rare = ItemRarityID.Green;
            Item.useTime = 2;
            Item.useAnimation = 2;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
        }

        public override bool? UseItem(Player player)
        {
            ToolsUISystem uiSystem = ModContent.GetInstance<ToolsUISystem>();
            uiSystem.ToggleUI();
            return true;
        }
    }
}
