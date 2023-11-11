using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Stellamod.Tiles.Acid
{
    class AcidTree : ModTree
    {
        public override TreePaintingSettings TreeShaderSettings => new TreePaintingSettings
        {
            UseSpecialGroups = true,
            SpecialGroupMinimalHueValue = 11f / 72f,
            SpecialGroupMaximumHueValue = 0.25f,
            SpecialGroupMinimumSaturationValue = 0.88f,
            SpecialGroupMaximumSaturationValue = 1f
        };

        public override void SetStaticDefaults() => GrowsOnTileId = new int[] { ModContent.TileType<AcidialDirt>() };
        public override int CreateDust() => 22;
        public override int DropWood() => ModContent.ItemType<VirulentPlating>();
        public override Asset<Texture2D> GetTexture() => ModContent.Request<Texture2D>("Stellamod/Tiles/Acid/AcidTree");
        public override Asset<Texture2D> GetTopTextures() => ModContent.Request<Texture2D>("Stellamod/Tiles/Acid/AcidTree_Top");
        public override Asset<Texture2D> GetBranchTextures() => ModContent.Request<Texture2D>("Stellamod/Tiles/Acid/AcidTree_Branches");

        public override int SaplingGrowthType(ref int style)
        {
            style = 0;
            return ModContent.TileType<AcidicTreeSapling>();
        }

        public override void SetTreeFoliageSettings(Tile tile, ref int xoffset, ref int treeFrame, ref int floorY, ref int topTextureFrameWidth, ref int topTextureFrameHeight)
        {
            topTextureFrameWidth = 142;
            topTextureFrameHeight = 114;
            xoffset = 62;
            floorY = 2;
        }
    }
}