using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


using Stellamod;
using Microsoft.Xna.Framework;

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
            Item.rare = 1;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (!Main.dayTime)
            {
                Lighting.AddLight(player.Center, Color.MediumPurple.ToVector3() * 1.75f * Main.essScale);
                player.GetModPlayer<MyPlayer>().ShadowCharm = true;
                player.manaCost -= 0.3f;
                player.manaRegen += 2;
            }
            else
            {

                player.GetModPlayer<MyPlayer>().ShadowCharm = false;
            }

        }
    }
}