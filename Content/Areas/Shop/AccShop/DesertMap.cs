using Microsoft.Xna.Framework;
using Stellamod.Items.Accessories.Players;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;

using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Shop.AccShop
{
    public class DesertMap : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 8;
            Item.height = 8;
            Item.consumable = true;
            Item.useAnimation = 16;
            Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ModContent.RarityType<ShopRarity>();
            Item.UseSound = SoundID.MaxMana;
        }

        private void Reveal(int x, int y)
        {
            if(WorldGen.InWorld(x, y))
            {
                Main.Map.Update(x, y, 255);
            }
        }

        private bool IsValidTile(int tileType)
        {
            return TileID.Sets.isDesertBiomeSand[tileType];
        }

        public override bool? UseItem(Player player)
        {
            WorldMap worldMap = Main.Map;

            //extra padding to avoid oceans
            int padding = 500;
            for(int x = padding; x < Main.maxTilesX - padding; x++)
            {
                for(int y =0; y < Main.maxTilesY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if(IsValidTile(tile.TileType))
                    {
                        Reveal(x, y);

                        Reveal(x + 1, y);
                        Reveal(x - 1, y);
                        Reveal(x, y - 1);
                        Reveal(x, y + 1);

                        Reveal(x - 1, y - 1);
                        Reveal(x - 1, y + 1);
                        Reveal(x + 1, y - 1);
                        Reveal(x + 1, y + 1);
                    }
              
                }
            }
            Main.refreshMap = true;
            return true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            dashPlayer.ExtraImmunityFramesBonus += 3;
        }
    }
}
