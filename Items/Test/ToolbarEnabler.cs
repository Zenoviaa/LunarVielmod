using Stellamod.NPCs.Colosseum.Common;
using Stellamod.UI.PopupSystem;
using Stellamod.UI.ToolsSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Test
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
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Balls");
        }

        public override bool? UseItem(Player player)
        {
            ToolsUISystem uiSystem = ModContent.GetInstance<ToolsUISystem>();
            uiSystem.ToggleUI();
            return true;
        }
    }
}
