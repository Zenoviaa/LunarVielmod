using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content.Areas.PunkerTown.TilesPT;
using Stellamod.Content.Dusts;
using Stellamod.Core.Godrays;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.TilesNew.RainforestTiles;
using Stellamod.WorldG;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.Utilities;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.Content.Areas.Terror.TilesTR;

public class DeadSapling : ModTile
{
    public override void SetStaticDefaults()
    {
        Main.tileFrameImportant[Type] = true;
        Main.tileNoAttach[Type] = true;
        Main.tileLavaDeath[Type] = true;

        TileObjectData.newTile.Width = 1;
        TileObjectData.newTile.Height = 2;
        TileObjectData.newTile.Origin = new Point16(0, 1);
        TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
        TileObjectData.newTile.UsesCustomCanPlace = true;
        TileObjectData.newTile.CoordinateHeights = [16, 18];
        TileObjectData.newTile.CoordinateWidth = 16;
        TileObjectData.newTile.CoordinatePadding = 2;
        TileObjectData.newTile.AnchorValidTiles = [ModContent.TileType<RainforestGrass>(), TileID.Gold];
        TileObjectData.newTile.StyleHorizontal = true;
        TileObjectData.newTile.DrawFlipHorizontal = true;
        TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
        TileObjectData.newTile.LavaDeath = true;
        TileObjectData.newTile.RandomStyleRange = 3;
        TileObjectData.newTile.StyleMultiplier = 3;

        //TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
        //TileObjectData.newSubTile.AnchorValidTiles = [ModContent.TileType<ExampleSand>()];
        //TileObjectData.addSubTile(1);

        TileObjectData.addTile(Type);

        AddMapEntry(new Color(200, 200, 200), Language.GetText("MapObject.Sapling"));

        TileID.Sets.TreeSapling[Type] = true;
        TileID.Sets.CommonSapling[Type] = true;
        TileID.Sets.SwaysInWindBasic[Type] = true;
        TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Plant"]); // Make this tile interact with golf balls in the same way other plants do

        DustType = ModContent.DustType<Sparkle>();

        AdjTiles = [TileID.Saplings];
    }

    public override void NumDust(int i, int j, bool fail, ref int num)
    {
        num = fail ? 1 : 3;
    }

    public override void RandomUpdate(int i, int j)
    {
        // A random chance to slow down growth
        if (!WorldGen.genRand.NextBool(20))
        {
            return;
        }

        Tile tile = Framing.GetTileSafely(i, j); // Safely get the tile at the given coordinates
        bool growSuccess; // A bool to see if the tree growing was successful.

        // Style 0 is for the ExampleTree sapling, and style 1 is for ExamplePalmTree, so here we check frameX to call the correct method.
        // Any pixels before 54 on the tilesheet are for ExampleTree while any pixels above it are for ExamplePalmTree
        if (tile.TileFrameX < 54)
        {
            growSuccess = WorldGen.GrowTree(i, j);
        }
        else
        {
            growSuccess = WorldGen.GrowPalmTree(i, j);
        }

        // A flag to check if a player is near the sapling
        bool isPlayerNear = WorldGen.PlayerLOS(i, j);

        // If growing the tree was a success and the player is near, show growing effects
        if (growSuccess && isPlayerNear)
        {
            WorldGen.TreeGrowFXCheck(i, j);
        }
    }

    public override void SetSpriteEffects(int i, int j, ref SpriteEffects effects)
    {
        if (i % 2 == 0)
        {
            effects = SpriteEffects.FlipHorizontally;
        }
    }
}
public class DeadTreeTop : ModTile
{
    private UnifiedRandom _random;
    private Asset<Texture2D> _topsTextureAsset;

    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _topsTextureAsset = ModContent.Request<Texture2D>(Texture + "_Tops");

        _random = new UnifiedRandom(0);
        LocalizedText name = CreateMapEntryName();
        TileID.Sets.IsATreeTrunk[Type] = true;
        Main.tileAxe[Type] = true;
        AddMapEntry(new Color(169, 200, 93), name);
        RegisterItemDrop(ItemID.Shadewood);
    }

    private float GetLeafSway(float offset, float magnitude, float speed)
    {
        return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
    }
    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        Main.instance.TilesRenderer.AddSpecialLegacyPoint(new Point(i, j));
    }

    private Rectangle GetTopFrame(int rand)
    {
        int frameWidth = _topsTextureAsset.Width() / 3;
        int frameHeight = _topsTextureAsset.Height();
        Rectangle frame = new Rectangle(frameWidth * rand, 0, frameWidth, frameHeight);
        return frame;
    }

    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {
        _random.SetSeed(i + j);
        Vector2 pos = (new Vector2(i + 1, j) + VeilGen.TileAdj) * 16;

        Color color = Lighting.GetColor(i, j);
        Rectangle frame = GetTopFrame(_random.Next(0, 3));
        Vector2 offset = new Vector2(-16, 16);
        spriteBatch.Draw(_topsTextureAsset.Value, pos - Main.screenPosition + offset, frame, color, GetLeafSway(3, 0.05f, 0.008f),
            new Vector2(frame.Width / 2, frame.Height), 1, 0, 1);
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (fail || effectOnly)
            return;

        Framing.GetTileSafely(i, j).HasTile = false;


    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        short x = 0;
        short y = 0;

        bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<AcaciaTree>();

        if (down)
        {
            y = (short)(Main.rand.Next(3) * 18);
        }

        Tile tile = Framing.GetTileSafely(i, j);
        tile.TileFrameX = x;
        tile.TileFrameY = y;
        return false;
    }
}
public class DeadTree : ModTile
{
    private UnifiedRandom _random;
  //  private Asset<Texture2D> _branchTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    //    _branchTextureAsset = ModContent.Request<Texture2D>(Texture + "_Branches");
        _random = new UnifiedRandom(0);
        LocalizedText name = CreateMapEntryName();
        TileID.Sets.IsATreeTrunk[Type] = true;
        Main.tileAxe[Type] = true;
        AddMapEntry(new Color(169, 200, 93), name);
        RegisterItemDrop(ItemID.Shadewood);
    }

    private float GetLeafSway(float offset, float magnitude, float speed)
    {
        return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
    }


    private void DrawBranches(int i, int j, SpriteBatch spriteBatch)
    {
        Vector2 pos2 = (new Vector2(i + 1, j) + VeilGen.TileAdj) * 16;
        Color color2 = Lighting.GetColor(i, j);
        _random.SetSeed(i + j);
        SpriteEffects flip = 0;
        if (_random.NextBool(2))
        {
            flip = SpriteEffects.FlipHorizontally;
        }

        bool drawBranch = _random.NextBool(4);
        Vector2 branchoffset = new Vector2(-2, 0);
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        DrawBranches(i, j, spriteBatch);
        return base.PreDraw(i, j, spriteBatch);
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (fail || effectOnly)
            return;

        Framing.GetTileSafely(i, j).HasTile = false;

        bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<DeadTree>() || Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<DeadTreeTop>();
        bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<DeadTree>();

        if (up)
            WorldGen.KillTile(i, j - 1);
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        short x = 0;
        short y = 0;

        bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<DeadTree>() || Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<DeadTreeTop>();
        bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<DeadTree>();

        if (up || down)
        {
            y = (short)(Main.rand.Next(3) * 18);
        }

        Tile tile = Framing.GetTileSafely(i, j);
        tile.TileFrameX = x;
        tile.TileFrameY = y;
        return false;
    }
}
public class BigDeadTreeTop : ModTile
{
    private UnifiedRandom _random;
    private Asset<Texture2D> _topsTextureAsset;
    private int _frameCount;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _topsTextureAsset = ModContent.Request<Texture2D>(Texture + "_Tops");
        _frameCount = 1;
        _random = new UnifiedRandom(0);
        LocalizedText name = CreateMapEntryName();
        TileID.Sets.IsATreeTrunk[Type] = true;

        Main.tileAxe[Type] = true;
        AddMapEntry(new Color(169, 200, 93), name);
        RegisterItemDrop(ItemID.Shadewood);
    }

    private float GetLeafSway(float offset, float magnitude, float speed)
    {
        return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
    }
    public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
    {
        // Main.instance.TilesRenderer.AddSpecialLegacyPoint(new Point(i, j));
    }

    private Rectangle GetTopFrame(int rand)
    {
        int frameWidth = _topsTextureAsset.Width() / _frameCount;
        int frameHeight = _topsTextureAsset.Height();
        Rectangle frame = new Rectangle(frameWidth * rand, 0, frameWidth, frameHeight);
        return frame;
    }

    public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
    {

    }

    public void DrawTreeTops(int i, int j, SpriteBatch spriteBatch)
    {
        _random.SetSeed(i + j);
        Vector2 pos = (new Vector2(i + 1, j)) * 16;

        Color color = Lighting.GetColor(i, j);
        Rectangle frame = GetTopFrame(_random.Next(0, 1));

       // Main.NewText(frame);
        Vector2 offset = new Vector2(-6, 64);
        Vector2 topLeftOffset = new Vector2(-128, -32);

        Color backColor = color.MultiplyRGB(Color.Lerp(Color.White, Color.Black, 0.3f));
        spriteBatch.Draw(_topsTextureAsset.Value, pos - Main.screenPosition + offset + topLeftOffset, frame, backColor, GetLeafSway(3, 0.05f, 0.008f),
     new Vector2(frame.Width / 2, frame.Height), 1, 0, 1);
        Vector2 topRightOffset = new Vector2(128, -64);
        spriteBatch.Draw(_topsTextureAsset.Value, pos - Main.screenPosition + offset + topRightOffset, frame, backColor, GetLeafSway(3, 0.05f, 0.008f),
     new Vector2(frame.Width / 2, frame.Height), 1, 0, 1);


        spriteBatch.Draw(_topsTextureAsset.Value, pos - Main.screenPosition + offset, frame, color, GetLeafSway(3, 0.05f, 0.008f),
            new Vector2(frame.Width / 2, frame.Height), 1, 0, 1);
    }
    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (fail || effectOnly)
            return;

        Framing.GetTileSafely(i, j).HasTile = false;
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        short x = 0;
        short y = 0;

        bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<BigDeadTree>();

        if (down)
        {
            y = (short)(Main.rand.Next(_frameCount) * 18);
        }

        Tile tile = Framing.GetTileSafely(i, j);
        tile.TileFrameX = x;
        tile.TileFrameY = y;
        return false;
    }
}

public class BigDeadTree : ModTile
{
    private Asset<Texture2D> _rootsTextureAsset;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        _rootsTextureAsset = ModContent.Request<Texture2D>(Texture + "_Roots");
        LocalizedText name = CreateMapEntryName();
        TileID.Sets.IsATreeTrunk[Type] = true;
        Main.tileAxe[Type] = true;
        AddMapEntry(new Color(169, 200, 93), name);
        RegisterItemDrop(ItemID.Shadewood);
    }

    private float GetLeafSway(float offset, float magnitude, float speed)
    {
        return (float)Math.Sin(Main.GameUpdateCount * speed + offset) * magnitude;
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
    {
        //Draw roots
        bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<BigDeadTree>();
        bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<BigDeadTree>();
        bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<BigDeadTree>();
        if (left && right && !down)
        {
            Vector2 pos = (new Vector2(i + 1, j + 2) + VeilGen.TileAdj) * 16;
            Color color = Lighting.GetColor(i, j);
            pos -= new Vector2(0, 40);
            spriteBatch.Draw(_rootsTextureAsset.Value, pos - Main.screenPosition, null, color.MultiplyRGB(Color.Gray),
                GetLeafSway(0, 0.05f, 0.01f), new Vector2(_rootsTextureAsset.Width() / 2, 0), 1, 0, 1);
        }
        return base.PreDraw(i, j, spriteBatch);
    }

    public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
    {
        if (fail || effectOnly)
            return;

        Framing.GetTileSafely(i, j).HasTile = false;

        bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<BigDeadTree>() || Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<BigDeadTreeTop>();
        bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<BigDeadTree>();
        bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<BigDeadTree>();
        bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<BigDeadTree>();


        if (left)
            WorldGen.KillTile(i - 1, j);
        if (right)
            WorldGen.KillTile(i + 1, j);
        if (up)
            WorldGen.KillTile(i, j - 1);
    }

    public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
    {
        short x = 0;
        short y = 0;

        bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<BigDeadTree>() || Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<BigDeadTreeTop>();
        bool down = Framing.GetTileSafely(i, j + 1).TileType == ModContent.TileType<BigDeadTree>();
        bool left = Framing.GetTileSafely(i - 1, j).TileType == ModContent.TileType<BigDeadTree>();
        bool right = Framing.GetTileSafely(i + 1, j).TileType == ModContent.TileType<BigDeadTree>();
        if (right && !left)
        {
            x = 0;
        }
        if (left && !right)
        {
            x = 18 * 2;
        }
        if (left && right)
        {
            x = 18;
        }
        if (up || down)
        {
            //just keep looping over these textures
            int index = j % 6;
            y = (short)(index * 18);
        }

        Tile tile = Framing.GetTileSafely(i, j);
        tile.TileFrameX = x;
        tile.TileFrameY = y;
        return false;
    }
}