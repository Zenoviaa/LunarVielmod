using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public class UniversalEnchantment : ModItem
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

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.GetModPlayer<AdvancedMagicPlayer>().ResetProgress();
                Main.NewText("You have no maidens...", Color.IndianRed);
            }
            else
            {
                player.GetModPlayer<AdvancedMagicPlayer>().GrantAllProgress();
                Main.NewText("UNLOCKED ALL ENCHANTMENTS!!!", Color.Gold);
            }
            return true;
        }
    }
}
