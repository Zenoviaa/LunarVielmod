using Stellamod.Content.Areas.Collosseum.WeaponsCL;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class StaffOFlame : ModItem
{
    private int _dir;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 17;
        Item.width = 16;
        Item.height = 16;
        Item.mana = 20;
        Item.useAnimation = Item.useTime = 32;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item43 with { PitchVariance = 0.4f, Volume = 0.1f };
        Item.knockBack = 2;
        Item.shoot = ModContent.ProjectileType<JackoShotBombArrowFire>();
        Item.shootSpeed = 12;
        Item.noMelee = true;
        Item.noUseGraphic = true;
    }

    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        velocity += Main.rand.NextVector2CircularEdge(8, 8);
        velocity.Y -= 48;
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
        SoundStyle castSound;
        switch (Main.rand.Next(2))
        {
            default:
            case 0:
                castSound = SoundID.DD2_BetsyFireballShot;
                break;
            case 1:
                castSound = SoundID.DD2_EtherianPortalSpawnEnemy;
                break;
        }

        castSound.PitchVariance = 0.4f;
        castSound.Volume = 0.3f;
        castSound.MaxInstances = 1;
        SoundEngine.PlaySound(castSound, player.position);


        Vector2 invertVelocity = velocity;
        invertVelocity.X = -invertVelocity.X;
        Projectile.NewProjectile(source, position, invertVelocity, type, damage, knockback, player.whoAmI);
        var p = Projectile.NewProjectileDirect(source, player.Center, velocity,
            ModContent.ProjectileType<StaffWaveHold>(), damage, knockback, player.whoAmI,
            ai2: _dir);
        return true;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankStaff>(),
            material: ModContent.ItemType<AlcadizScrap>());
    }
}
