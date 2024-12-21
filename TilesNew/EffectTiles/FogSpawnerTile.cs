using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Foggy;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.TilesNew.EffectTiles
{
    public class FogSpawnerTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[TileID.Mud][Type] = true;
            Main.tileMerge[TileID.ClayBlock][Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileBlockLight[Type] = true;
            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(178, 163, 190), name);

            MineResist = 1f;
            MinPick = 145;
        }

        public override void RandomUpdate(int i, int j)
        {

        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            FogSystem fogSystem = ModContent.GetInstance<FogSystem>();
            Point point = new Point(i, j);
            Fog fog = fogSystem.SetupFog(point, FogCreateFunction);
            return base.PreDraw(i, j, spriteBatch);
        }

        private void FogCreateFunction(Fog fog)
        {
            fog.startColor = Color.White;
            fog.startScale = new Vector2(Main.rand.NextFloat(0.75f, 1.0f), Main.rand.NextFloat(0.7f, 0.9f)) * 0.9f;
            fog.pulseWidth = Main.rand.NextFloat(0.96f, 0.98f);
            fog.texture = TextureRegistry.Clouds6;
            fog.rotation = Main.rand.NextFloat(-1f, 1f);
            fog.offset = Main.rand.NextVector2Circular(16, 16);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Tile tileBelow = Framing.GetTileSafely(i, j + 1);
            Tile tileAbove = Framing.GetTileSafely(i, j - 1);

            if (!tileAbove.HasTile || !tileBelow.HasTile)
            {
                r = 0.05f;
                g = 0.15f;
                b = 0.25f;
            }
        }
    }


    public class FogSpawnerBlock : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Super silk!");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<FogSpawnerTile>();
        }


    }
}