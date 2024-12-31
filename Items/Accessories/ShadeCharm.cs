using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Stellamod.Items.Accessories
{
    internal class ShadeCharm : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shade Charm");
            // Tooltip.SetDefault("Decreases Mana Cost and lights up in the night");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Main.dayTime)
            {
                Lighting.AddLight(player.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);
                player.manaCost -= 0.3f;
                player.manaRegen += 2;
            }
        }
    }
}