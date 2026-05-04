using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

public class StaffoftheIrradiaflare : ModItem
{
    private int _dir;
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Staff of the Irradiaflare");
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.staff[Item.type] = true;
        Item.damage = 111;
        Item.width = 50;
        Item.height = 50;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 4;
        Item.value = Item.sellPrice(0, 1, 1, 29);
        Item.autoReuse = true;
        Item.DamageType = DamageClass.Magic;
        Item.shoot = ModContent.ProjectileType<ITProj>();
        Item.shootSpeed = 15f;
        Item.mana = 25;
        Item.useAnimation = 20;
        Item.useTime = 20;
        Item.consumeAmmoOnLastShotOnly = true;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-5f, 0f);
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (_dir == 0)
        {
            _dir = 1;
        }
        else
        {
            _dir *= -1;
        }

        Projectile.NewProjectile(source, position, velocity * Main.rand.NextFloat(0.6f, 1f), type, damage, knockback, player.whoAmI);
        var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);
        //(p.ModProjectile as StaffWaveHold).MagicCircleStyle = 1;
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<MechanizedSoul>());
    }
}
