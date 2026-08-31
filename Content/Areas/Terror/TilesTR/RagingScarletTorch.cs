using Stellamod.Common.Particles;
using Stellamod.Content.Dusts;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Stellamod.Content.Areas.Terror.TilesTR;

public class RagingScarletTorch : ModItem
{
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 100;

        ItemID.Sets.ShimmerTransformToItem[Type] = ItemID.ShimmerTorch;
        ItemID.Sets.SingleUseInGamepad[Type] = true;
        ItemID.Sets.Torches[Type] = true;
    }

    public override void SetDefaults()
    {
        // DefaultToTorch sets various properties common to torch placing items. Hover over DefaultToTorch in Visual Studio to see the specific properties set.
        // Of particular note to torches are Item.holdStyle, Item.flame, and Item.noWet. 
        Item.DefaultToTorch(ModContent.TileType<RagingScarletTorchTile>(), 0, false);
        Item.value = 50;
    }

    public override void HoldItem(Player player)
    {
        // This torch cannot be used in water, so it shouldn't spawn particles or light either
        if (player.wet)
        {
            return;
        }

        // Note that due to biome select torch god's favor, the player may not actually have an ExampleTorch in their inventory when this hook is called, so no modifications should be made to the item instance.

        // Randomly spawn sparkles when the torch is held. Bigger chance to spawn them when swinging the torch.
        if (Main.rand.NextBool(player.itemAnimation > 0 ? 7 : 30))
        {
            Dust dust = Dust.NewDustDirect(new Vector2(player.itemLocation.X + (player.direction == -1 ? -16f : 6f), player.itemLocation.Y - 14f * player.gravDir), 4, 4, DustID.Blood, 0f, 0f, 100);
            if (!Main.rand.NextBool(3))
            {
                dust.noGravity = true;
            }

            dust.velocity *= 0.3f;
            dust.velocity.Y -= 1.5f;
            dust.position = player.RotatedRelativePoint(dust.position);
        }

        // Create a white (1.0, 1.0, 1.0) light at the torch's approximate position, when the item is held.
        Vector2 position = player.RotatedRelativePoint(new Vector2(player.itemLocation.X + 12f * player.direction + player.velocity.X, player.itemLocation.Y - 14f + player.velocity.Y), true);

        Lighting.AddLight(position, 1f, 0.3f, 0.3f);
    }

    public override void PostUpdate()
    {
        // Create a white (1.0, 1.0, 1.0) light when the item is in world, and isn't underwater.
        if (!Item.wet)
        {
            Lighting.AddLight(Item.Center, 1f, 0.3f, 0.3f);
        }
    }

    // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
    public override void AddRecipes()
    {
        CreateRecipe()
            .AddIngredient(ItemID.Torch, 50)
            .AddIngredient(ItemID.Vertebrae, 1)
            .SortAfterFirstRecipesOf(ItemID.Torch)
            .Register();
    }
}

//// Torches are special tiles that support the block swap feature and the biome torch feature. ExampleSurfaceBiome shows how the biome torch is assigned.
public class RagingScarletTorchTile : ModTile
{
    public override void SetStaticDefaults()
    {
        // Properties
        Main.tileLighted[Type] = true;
        Main.tileFrameImportant[Type] = true;
        Main.tileSolid[Type] = false;
        Main.tileNoAttach[Type] = true;
        Main.tileNoFail[Type] = true;
        Main.tileWaterDeath[Type] = true;
        TileID.Sets.FramesOnKillWall[Type] = true;
        TileID.Sets.DisableSmartCursor[Type] = true;
        TileID.Sets.DisableSmartInteract[Type] = true;
        TileID.Sets.Torch[Type] = true;

        DustType = ModContent.DustType<Sparkle>();
        AdjTiles = new int[] { TileID.Torches };

        AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);

        // Placement
        TileObjectData.newTile.CopyFrom(TileObjectData.GetTileData(TileID.Torches, 0));
        /*  This is what is copied from the Torches tile
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { 124, 561, 574, 575, 576, 577, 578 };
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new[] { 124, 561, 574, 575, 576, 577, 578 };
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(0);
			*/

        // This code adds style-specific properties to style 1. Style 1 is used by ExampleWaterTorch. This code allows the tile to be placed in liquids. More info can be found in the guide: https://github.com/tModLoader/tModLoader/wiki/Basic-Tile#newsubtile-and-newalternate
        TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
        TileObjectData.newSubTile.LinkedAlternates = true;
        TileObjectData.newSubTile.WaterDeath = false;
        TileObjectData.newSubTile.LavaDeath = false;
        TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
        TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
        TileObjectData.addSubTile(1);

        TileObjectData.addTile(Type);

        // Etc
        AddMapEntry(new Color(200, 125, 125), Language.GetText("ItemName.Torch"));
    }

    public override void MouseOver(int i, int j)
    {
        Player player = Main.LocalPlayer;
        player.noThrow = 2;
        player.cursorItemIconEnabled = true;

        // We can determine the item to show on the cursor by getting the tile style and looking up the corresponding item drop.
        int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
        player.cursorItemIconID = TileLoader.GetItemDropFromTypeAndStyle(Type, style);
    }

    public override float GetTorchLuck(Player player)
    {
        // GetTorchLuck is called when there is an ExampleTorch nearby the client player
        // In most use-cases you should return 1f for a good luck torch, or -1f for a bad luck torch.
        // You can also add a smaller amount (eg 0.5) for a smaller positive/negative luck impact.
        // Remember that the overall torch luck is decided by every torch around the player, so it may be wise to have a smaller amount of luck impact.
        // Multiple example torches on screen will have no additional effect.

        // Positive and negative luck are accumulated separately and then compared to some fixed limits in vanilla to determine overall torch luck.
        // Positive luck is capped at 1, any value higher won't make any difference and negative luck is capped at 2.
        // A negative luck of 2 will cancel out all torch luck bonuses.

        // The influence positive torch luck can have overall is 0.1 (if positive luck is any number less than 1) or 0.2 (if positive luck is greater than or equal to 1)

        bool inExampleUndergroundBiome = player.InModBiome<AegislavBiome>();
        return inExampleUndergroundBiome ? 1f : -0.1f; // ExampleTorch gives maximum positive luck when in example biome, otherwise a small negative luck
    }

    public override void EmitParticles(int i, int j, Tile tile, short tileFrameX, short tileFrameY, Color tileLight, bool visible)
    {
        base.EmitParticles(i, j, tile, tileFrameX, tileFrameY, tileLight, visible);

        // If the torch is on
        if (tile.TileFrameX < 66)
        {
            Vector2 pos = new Point(i, j).ToWorldCoordinates();
            pos.Y -= 12;
            Particles.RagingFlameDust.Spawn(RagingFlameDustData.Default with { position = pos, timeleft = 70 });

            if (Main.rand.NextBool(2))
            {
                pos.Y -= 8;
                BitDustFactory factory = BitDustFactory.Default;
                factory.position = pos + Main.rand.NextVector2Circular(16, 16);
                factory.outerColor = Color.Red.ToVector4();
                factory.innerColor = Color.LightPink.ToVector4();
                factory.velocity = Main.rand.NextVector2Circular(1, 1) + new Vector2(0, -3);
                factory.scale = new Vector2(1.2f);
                Particles.BitDust.Spawn(factory);
            }
            if (Main.rand.NextBool(32))
            {
                BitDustFactory factory = BitDustFactory.Default;
                factory.position = pos + Main.rand.NextVector2Circular(16, 16);
                factory.outerColor = Color.Red.ToVector4();
                factory.innerColor = Color.LightPink.ToVector4();
                factory.velocity = Main.rand.NextVector2Circular(1, 1)  * 8 + new Vector2(0, -3);
                factory.scale = new Vector2(0.8f);
                Particles.BitDust.Spawn(factory);
            }

        }
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = Main.rand.Next(1, 3);

    public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
    {
        Tile tile = Main.tile[i, j];

        // If the torch is on
        if (tile.TileFrameX < 66)
        {
            int style = TileObjectData.GetTileStyle(Main.tile[i, j]);
            // Make it emit the following light.
            r = 0.9f;
            g = 0.3f;
            b = 0.3f;

            r *= 4.0f;
            g *= 4.0f;
            b *= 4.0f;
        }
    }

    public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
    {
        // This code slightly lowers the draw position if there is a solid tile above, so the flame doesn't overlap that tile. Terraria torches do this same logic.
        offsetY = 0;

        if (WorldGen.SolidTile(i, j - 1))
        {
            offsetY = 4;
        }
    }
}
