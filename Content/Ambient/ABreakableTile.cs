using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Content.Ambient;

public abstract class ABreakableTile<TileEntityType> : ModTile
    where TileEntityType : ModTileEntity
{
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        Main.tileSolid[Type] = false;
        Main.tileBlockLight[Type] = false;
        LocalizedText name = CreateMapEntryName();

        TileID.Sets.NoMerge[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
        TileObjectData.newTile.Height = 1;
        TileObjectData.newTile.Width = 1;
        TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);

        // MyTileEntity refers to the tile entity mentioned in the previous section
        TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(ModContent.GetInstance<TileEntityType>().Hook_AfterPlacement, -1, 0, true);
        // This is required so the hook is actually called.
        TileObjectData.newTile.UsesCustomCanPlace = true;
        TileObjectData.addTile(Type);


        AddMapEntry(new Color(108, 47, 87), name);
        MineResist = 1f;
        MinPick = 225;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {

        return base.PreDraw(i, j, spriteBatch);
    }
    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {
        base.KillMultiTile(i, j, frameX, frameY);
        // ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
        // The tile coordinate parameters already refer to the top-left corner of the multitile
        ModContent.GetInstance<TileEntityType>().Kill(i, j);
    }
    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
        // ModTileEntity.Kill() handles checking if the tile entity exists and destroying it if it does exist in the world for you
        // The tile coordinate parameters already refer to the top-left corner of the multitile
        ModContent.GetInstance<TileEntityType>().Kill(i, j);
    }
    public override bool IsTileDangerous(int i, int j, Player player) => true;


    // PlaceInWorld is needed to facilitate styles and alternates since this tile doesn't use a TileObjectData. Placing left and right based on player direction is usually done in the TileObjectData, but the specifics of that don't work for how we want this tile to work. 
    public override void PlaceInWorld(int i, int j, Item item)
    {
        int style = Main.LocalPlayer.HeldItem.placeStyle;
        Tile tile = Main.tile[i, j];
        if (Main.netMode == NetmodeID.MultiplayerClient)
        {
            NetMessage.SendTileSquare(-1, Player.tileTargetX, Player.tileTargetY, 1, TileChangeType.None);
        }
    }
}
