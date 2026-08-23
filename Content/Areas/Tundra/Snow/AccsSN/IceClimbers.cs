using Microsoft.Xna.Framework;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Snow.AccsSN
{
    public class IceClimbersPlayer : ModPlayer
    {
        public bool hasIceClimbers;
        public static bool[] ClimbableTiles = TileID.Sets.Factory.CreateBoolSet(false);
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ClimbableTiles[TileID.SnowBlock] = true;
            ClimbableTiles[TileID.IceBlock] = true;
            ClimbableTiles[TileID.IceBrick] = true;
            ClimbableTiles[TileID.Slush] = true;
            ClimbableTiles[TileID.BreakableIce] = true;
            ClimbableTiles[TileID.CorruptIce] = true;
            ClimbableTiles[TileID.FleshIce] = true;
            ClimbableTiles[TileID.MagicalIceBlock] = true;

        }
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasIceClimbers = false;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (!hasIceClimbers)
                return;
            Point point = new Vector2(Player.BottomLeft.X, Player.BottomLeft.Y).ToTileCoordinates();
            Tile? floorTile = Player.GetFloorTile(point.X, point.Y);
            if (floorTile.HasValue)
            {
                Tile tile = floorTile.Value;
                if (ClimbableTiles[tile.TileType] || TileSets.ThickSnow[tile.TileType])
                {
                    Player.moveSpeed += 0.25f;
                    Player.maxRunSpeed *= 1.2f;
                    Player.runAcceleration *= 2f;
                }
            }
        }
    }
    public class IceClimbers : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToAccessory();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            IceClimbersPlayer iceClimbersPlayer = player.GetModPlayer<IceClimbersPlayer>();
            iceClimbersPlayer.hasIceClimbers = true;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), 
                material: ModContent.ItemType<WinterbornShard>());
        }
    }
}

