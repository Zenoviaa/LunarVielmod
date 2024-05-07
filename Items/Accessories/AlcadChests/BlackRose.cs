using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.AlcadChests
{
    internal class BlackRose : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 36;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.HasBuff(BuffID.ManaSickness))
            {
                int combatText = CombatText.NewText(player.getRect(), Color.Red, "10", true);
                CombatText numText = Main.combatText[combatText];
                numText.lifeTime = 60;
             
                player.ClearBuff(BuffID.ManaSickness);
                player.statLife -= 10;
            }
        }
    }
}
