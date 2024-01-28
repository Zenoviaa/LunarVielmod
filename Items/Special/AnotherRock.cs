using Microsoft.Xna.Framework;
using Stellamod.Tiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Special
{
    internal class AnotherRock : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool? UseItem(Player player)
        {
            bool foundTile = false;
            for (int x = 0; x < Main.tile.Width; x++)
            {
                for (int y = 0; y < Main.tile.Height; y++)
                {
                    if (Main.tile[x, y].TileType == ModContent.TileType<FlowerSummon>())
                    {
                        player.Center = new Vector2(x, y).ToWorldCoordinates();
                        foundTile = true;

                        //Break the loop we don't need to search anymore
                        SoundEngine.PlaySound(SoundID.Item6);
                        break;
                    }
                }

                if (foundTile)
                    break;
            }


            return true;
        }
    }
}
