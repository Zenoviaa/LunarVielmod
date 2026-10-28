using Stellamod.Common.StaminaSystem;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.AccFB;


public class DecoratedShield : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.DecoratedShield.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToShield(ModContent.ProjectileType<DecoratedShieldHeld>());
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<StaminaNDashPlayer>().dashRestoreChance += 10;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<AlcadizScrap, BlankCard>();
    }
}

public class DecoratedShieldHeld : AbstractShieldProjectile
{
    public override string Texture => AssetReferences.Assets.Textures.Fable.Items.DecoratedShieldHeld.KEY;
}