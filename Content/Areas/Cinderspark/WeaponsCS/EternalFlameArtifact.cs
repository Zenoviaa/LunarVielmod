using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Cinderspark.WeaponsCS;

public class EternalFlameArtifact : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToArtifact();
        Item.damage = 50;
        Item.mana = 45;
        Item.width = 18;
        Item.height = 21;
        Item.useTime = 40;
        Item.useAnimation = 40;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.knockBack = 4f;
        Item.DamageType = DamageClass.Magic;
        Item.value = 10000;
        Item.rare = ItemRarityID.Orange;
        Item.UseSound = SoundID.DD2_BookStaffCast;
        Item.shoot = ModContent.ProjectileType<EternalFlameBall>();
        Item.shootSpeed = 4f;
        Item.autoReuse = true;
        Item.noMelee = true;
    }


    public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    {
        base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.Shoot(player, source, position, velocity, type, damage, knockback);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Cinderscrap, BlankStaff>();
    }
}


public class EternalFlameBall : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ProjectileID.Sets.TrailCacheLength[Type] = 32;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.penetrate = -1;
        Projectile.tileCollide = true;
    }
    public override void AI()
    {
        base.AI();
    }

    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
        if (this.OwnedByLocalClient())
        {
            Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, -Projectile.oldVelocity,
                ModContent.ProjectileType<EternalFlamePile>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}

public class EternalFlamePile : ModProjectile
{
    private ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
    }
    public override void AI()
    {
        base.AI();
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        base.OnHitNPC(target, hit, damageDone);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        return base.PreDraw(ref lightColor);
    }
    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);
    }
}
