using ReLogic.Content;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items.Placeable.Cathedral;
using Stellamod.WorldG;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Content.Relics;

public abstract class AbstractRelicItem<ItemClass, TileClass> : ModItem
    where TileClass : AbstractRelicTile<ItemClass>
    where ItemClass : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.width = 12;
        Item.height = 12;
        Item.maxStack = Item.CommonMaxStack;
        Item.master = true;
        Item.masterOnly = true;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 10;
        Item.useTime = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.consumable = true;
        Item.createTile = ModContent.TileType<TileClass>();
    }
}

public abstract class AbstractRelicTile<ItemType> : ModTile where ItemType : ModItem
{
    public override string Texture => this.GetTypeDirectoryWithSlash() + "RelicPedestal";
    public Asset<Texture2D> RelicTextureAsset;
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileShine[Type] = 400;
        Main.tileTable[Type] = true;
        Main.tileSolidTop[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = false;
        Main.tileFrameImportant[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.IgnoredByNpcStepUp[Type] = true; // This line makes NPCs not try to step up this tile during their movement. Only use this for furniture with solid tops.
        MineResist = 4f;
        MinPick = 200;

        DustType = ModContent.DustType<Dusts.GunFlash>();
        AdjTiles = new int[] { TileID.Bookcases };
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;
        TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
        TileObjectData.newTile.Height = 2;
        TileObjectData.newTile.Width = 6;

        TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
        TileObjectData.newTile.StyleWrapLimit = 2; //not really necessary but allows me to add more subtypes of chairs below the example chair texture
        TileObjectData.newTile.StyleMultiplier = 2; //same as above
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.Origin = new Point16(3, 1);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
        TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
        TileObjectData.addTile(Type);
        LocalizedText name = CreateMapEntryName();

        // Register map name and color
        // "MapObject.Relic" refers to the translation key for the vanilla "Relic" text
        AddMapEntry(new Color(233, 207, 94), Language.GetText("MapObject.Relic"));
        RegisterItemDrop(ModContent.ItemType<ItemType>());
    }

    public override void Load()
    {
        base.Load();
        if (!Main.dedServ)
        {
            RelicTextureAsset = ModContent.Request<Texture2D>(base.Texture);
        }
    }

    public override void Unload()
    {
        base.Unload();
        RelicTextureAsset = null;
    }
    public override void MouseOver(int i, int j)
    {

    }
        
    public override void MouseOverFar(int i, int j)
    {
        MouseOver(i, j);
        Player player = Main.LocalPlayer;
        if (player.cursorItemIconText == "")
        {
            player.cursorItemIconEnabled = false;
            player.cursorItemIconID = 0;
        }
    }


    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        r = 0.2f;
        g = 0.165f;
        b = 0.12f;
    }

    public override void KillMultiTile(int i, int j, int frameX, int frameY)
    {

    }
    public override bool CanExplode(int i, int j) => false;
    public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
    {
        return true;
    }

    public override bool RightClick(int i, int j)
    {
        return base.RightClick(i, j);
    }
    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {

        // Since this tile does not have the hovering part on its sheet, we have to animate it ourselves
        // Therefore we register the top-left of the tile as a "special point"
        // This allows us to draw things in SpecialDraw
        int frameWidth = 16 * 6;
        int frameHeight = 16 * 2;
        if (drawData.tileFrameX % frameWidth == 0 && drawData.tileFrameY % frameHeight == 0)
        {
            Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
        }
    }
    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {
        // This is lighting-mode specific, always include this if you draw tiles manually
        Vector2 offScreen = new Vector2(Main.offScreenRange);
        if (Main.drawToScreen)
        {
            offScreen = Vector2.Zero;
        }

        // Take the tile, check if it actually exists
        Point p = new Point(i, j);
        Tile tile = Main.tile[p.X, p.Y];
        if (tile == null || !tile.HasTile)
        {
            return;
        }

        // Get the initial draw parameters
        Texture2D texture = RelicTextureAsset.Value;


        Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height);
        Vector2 worldPos = p.ToWorldCoordinates(48, 64f);

        Color color = Lighting.GetColor(p.X, p.Y);

   
        SpriteEffects effects = SpriteEffects.None;
        // Some math magic to make it smoothly move up and down over time
        const float TwoPi = (float)Math.PI * 2f;
        float offset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 5f);
        Vector2 drawPos = worldPos + offScreen - Main.screenPosition + new Vector2(0f, -40f) + new Vector2(0f, offset * 4f);

        // Draw the main texture
        spriteBatch.Draw(texture, drawPos, null, color, 0f, origin, 1f, effects, 0f);

        // Draw the periodic glow effect
        float scale = (float)Math.Sin(Main.GlobalTimeWrappedHourly * TwoPi / 2f) * 0.3f + 0.7f;
        Color effectColor = color;
        effectColor.A = 0;
        effectColor = effectColor * 0.1f * scale;
        for (float num5 = 0f; num5 < 1f; num5 += 355f / (678f * (float)Math.PI))
        {
            spriteBatch.Draw(texture, drawPos + (TwoPi * num5).ToRotationVector2() * (6f + offset * 2f), null, effectColor, 0f, origin, 1f, effects, 0f);
        }
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        /*
        Vector2 worldPos = (new Vector2(i, j+1) + VeilGen.TileAdj) * 16;
        SpritebatchDrawer drawer = SpritebatchDrawer.FromTextureAsset(RelicTextureAsset.Value, worldPos);
        drawer.color = Lighting.GetColor(new Point(i, j));
        drawer.BottomCenterOrigin();
        spriteBatch.Draw(drawer);

        Asset<Texture2D> tileTextureAsset = TextureAssets.Tile[Type];
        SpritebatchDrawer statueDrawer = SpritebatchDrawer.FromTextureAsset(tileTextureAsset, worldPos);
        statueDrawer.BottomCenterOrigin();
        statueDrawer.color = drawer.color;
        statueDrawer.worldPosition.Y += ExtraMath.Osc(0f, -8f, speed: 2);
        spriteBatch.Draw(statueDrawer);

        statueDrawer.color = Color.Lerp(Color.Black, Color.White, ExtraMath.Osc(0f, 1f, offset: i)) * 0.66f;
        statueDrawer.color.A = 0;
        spriteBatch.Draw(statueDrawer);*/

        return true;
    }
}

public class WoodlandRavagerRelic :
    AbstractRelicTile<WoodlandRavagerRelicItem>
{

}

public class WoodlandRavagerRelicItem :
    AbstractRelicItem<WoodlandRavagerRelicItem, WoodlandRavagerRelic>
{

}