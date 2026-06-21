using Stellamod.Content.CommonMaterials;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.AccCL;


public class ExcalibursPhase : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(2, 60));
        ItemID.Sets.AnimatesAsSoul[Item.type] = true;
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<MeleeEffectsPlayer>().noOwnerHitCheck = true;
    }
    public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
    {
        DrawHelper.DrawAdvancedBroochGlow(Item, spriteBatch, position, Color.Gold);
        return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GintzlMetal, BlankAccessory>();
    }
}
