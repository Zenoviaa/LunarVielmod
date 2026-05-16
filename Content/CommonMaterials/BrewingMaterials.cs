using Stellamod.Helpers;
using Stellamod.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.CommonMaterials;

public class MinersGold : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Cauldron.MaterialOrder[Type] = 7;
    }
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 34;
        Item.rare = ModContent.RarityType<MinersGoldRarity>();
        Item.maxStack = Item.CommonMaxStack;
    }
}
public class IllurineScale : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Cauldron.MaterialOrder[Type] = 20;
    }
    public override void SetDefaults()
    {
        Item.width = 26;
        Item.height = 34;
        Item.rare = ModContent.RarityType<IllurineScaleRarity>();
        Item.maxStack = Item.CommonMaxStack;
    }

    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
        return true;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        return true;
    }
}

public class RadiantNectar : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Cauldron.MaterialOrder[Type] = 23;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<RadiantNectarRarity>();
    }
}
public class ConvulgingMater : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 12;
        // Tooltip.SetDefault("Pure shadows conjured by the darkest of entities."); // The (English) text shown below your item's name
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 5));
        ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
        ItemID.Sets.ItemNoGravity[Item.type] = false; // Makes the item have no gravity
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of Corely used research amounts depending on item type.
    }

    public override void PostUpdate()
    {
        //     ItemID.Sets.ItemNoGravity[Item.type] = false;
        Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.35f * Main.essScale); // Makes this item glow when thrown out of inventory.
    }

    public override void SetDefaults()
    {
        Item.width = 44; // The item texture's width
        Item.height = 42; // The item texture's height
        Item.maxStack = Item.CommonMaxStack; // The item's max stack value
        Item.rare = ModContent.RarityType<ConvulgingMatterRarity>();
        Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Purple);
        return true;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        return true;
    }

    public override void Update(ref float gravity, ref float maxFallSpeed)
    {

    }
}
public class MarshScrap : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Cauldron.MaterialOrder[Type] = 16;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<MarshScrapRarity>();
    }
}
public class MechanizedSoul : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Cauldron.MaterialOrder[Type] = 17;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<MechanizedSoulRarity>();
    }
}
public class FallenEyes : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Cauldron.MaterialOrder[Type] = 26;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<FallenEyesRarity>();
    }
}
public class MusicalHarmonise : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Cauldron.MaterialOrder[Type] = 13;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<MusicalHarmoniseRarity>();
    }
}


public class EreshkinCandle : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Cauldron.MaterialOrder[Type] = 22;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.maxStack = Item.CommonMaxStack;
        Item.rare =
            ModContent.RarityType<SpidersSilkRarity>();
    }
}
public class MothlightWing : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Cauldron.MaterialOrder[Type] = 28;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<MothlightWingRarity>();
    }
}
public class GhastlySpirit : ModItem
{

    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 27;
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of commonly used research amounts depending on item type.
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.maxStack = Item.CommonMaxStack;
        Item.rare = ModContent.RarityType<GhastlySpiritRarity>();
    }
}

public class Mushroom : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 2;
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(silver: 1);
        Item.rare = ModContent.RarityType<SpringMushroomRarity>();
    }
}

public class Ivythorn : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 3;
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of Corely used research amounts depending on item type.
    }

    public override void SetDefaults()
    {
        Item.width = 20; // The item texture's width
        Item.height = 20; // The item texture's height
        Item.rare = ModContent.RarityType<IvythornRarity>();
        Item.maxStack = Item.CommonMaxStack; // The item's max stack value
        Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
    }
}

public class AlcadizScrap : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 4;
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of Corely used research amounts depending on item type.
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(silver: 5);
        Item.rare = ModContent.RarityType<FableScrapRarity>();
    }
}

public class WinterbornShard : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 5;
        // Tooltip.SetDefault("Shard of the storm \n Frigid to the touch."); // The (English) text shown below your item's name
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of Corely used research amounts depending on item type.
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Lighting.AddLight(Item.Center, Color.LightSkyBlue.ToVector3() * 1.25f * Main.essScale);
        return true;
    }

    public override void SetDefaults()
    {
        Item.width = 20; // The item texture's width
        Item.height = 20; // The item texture's height
        Item.maxStack = Item.CommonMaxStack; // The item's max stack value
        Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
        Item.rare = ModContent.RarityType<WinterbornShardRarity>();
    }
}
public class TerrorFragments : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 8;
        // DisplayName.SetDefault("Terror Fragment");
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
        // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
        ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of Corely used research amounts depending on item type.
    }

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 40;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(0, 0, 20, 0);
        Item.rare = ModContent.RarityType<TerrorFragmentRarity>();
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.useStyle = ItemUseStyleID.Swing;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Lighting.AddLight(new Vector2(Item.Center.X, Item.Center.Y), 81 * 0.001f, 194 * 0.001f, 58 * 0.001f);
        return true;
    }
}

public class GintzlMetal : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 9;
        /* Tooltip.SetDefault("Hot to the touch, filled with gild and glory of tribal warriors" +
            "\n its so hot you can't even touch it, gotta use heated fabric..."); */
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
        Item.rare = ModContent.RarityType<GintzlMetalRarity>();
    }
}

public class Cinderscrap : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 10;
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(silver: 5);
        Item.rare = ModContent.RarityType<CinderscrapRarity>();
    }
}

public class HypnotizedSoul : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 11;
        // DisplayName.SetDefault("Terror Fragment");
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));
        // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
        ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of Corely used research amounts depending on item type.
    }

    public override void SetDefaults()
    {
        Item.width = 34;
        Item.height = 40;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = Item.sellPrice(0, 0, 20, 0);
        Item.rare = ItemRarityID.Green;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.rare = ModContent.RarityType<HypnotizedSoulRarity>();
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        Lighting.AddLight(new Vector2(Item.Center.X, Item.Center.Y), 81 * 0.001f, 194 * 0.001f, 58 * 0.001f);
        return true;
    }
}

public class PearlescentScrap : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 14;
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100;
    }

    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = Item.CommonMaxStack;
        Item.value = 5000;
        Item.rare = ModContent.RarityType<PearlescentScrapRarity>();
    }
}
public class KaleidoscopicInk : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 18;
        // Tooltip.SetDefault("An ore too cold to place, used for many items with ice!");
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
        Item.rare = ModContent.RarityType<KaleidoscopicInkRarity>();
    }

}
public class MiracleThread : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 21;
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of Corely used research amounts depending on item type.
    }

    public override void PostUpdate()
    {
        Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale); // Makes this item glow when thrown out of inventory.
    }

    public override void SetDefaults()
    {
        Item.width = 30; // The item texture's width
        Item.height = 24; // The item texture's height

        Item.rare = ModContent.RarityType<MiracleThreadRarity>();
        Item.maxStack = Item.CommonMaxStack; // The item's max stack value
        Item.value = Item.buyPrice(gold: 10); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
    }

    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        DrawHelper.DrawGlowInInventory(Item, spriteBatch, position, Color.Blue);
        return true;
    }

    public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
    {
        DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
        return true;
    }

    public override void Update(ref float gravity, ref float maxFallSpeed)
    {
        //The below code makes this item hover up and down in the world
        //Don't forget to make the item have no gravity, otherwise there will be weird side effects
        float hoverSpeed = 5;
        float hoverRange = 0.2f;
        float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
        Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
        Item.position = position;
    }
}

public class AlcaricMush : ModItem
{
    public override void SetStaticDefaults()
    {
        Cauldron.MaterialOrder[Type] = 25;
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 60));

        // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
        ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 100; // How many items are needed in order to research duplication of this item in Journey mode. See https://terraria.gamepedia.com/Journey_Mode/Research_list for a list of Corely used research amounts depending on item type.
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
        Item.rare = ModContent.RarityType<AlcaricMushRarity>(); ;
        Item.buyPrice(0, 0, 95, 0);
        Item.value = 9500;
    }

    // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

}