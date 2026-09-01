using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.Areas.SpringHills.WeaponsSH;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Ishtar.WeaponsIS;

public class ImperfectionStaff : ModItem
{
    public override void SetDefaults()
    {
        Item.DefaultToArtifact();
        Item.damage = 65;
        Item.DamageType = DamageClass.Magic;
        Item.width = 20;
        Item.height = 20;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.staff[Item.type] = true;
        Item.noMelee = true;
        Item.knockBack = 3;
        Item.value = 10000;
        Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1") with { PitchVariance = 0.4f };
        Item.shoot = ModContent.ProjectileType<PerfectionProj>();
        Item.shootSpeed = 2f; // the speed of the projectile (measured in pixels per frame)
        Item.channel = true;
        Item.mana = 18;
        Item.autoReuse = true;
        Item.noUseGraphic = true;
        Item.noMelee = true;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float numberProjectiles = 3;
        float rotation = MathHelper.ToRadians(14);
        position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
        for (int i = 0; i < numberProjectiles; i++)
        {
            Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
            Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI, ai1: 1);
        }

        var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI);
        return false;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<EreshkinCandle, BlankStaff>();
    }
}