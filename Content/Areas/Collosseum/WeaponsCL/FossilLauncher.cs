using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL;

public class FossilLauncherShard : ModProjectile,
    IDrawToRenderTarget
{
    private ref float Timer => ref Projectile.ai[0];
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        Main.projFrames[Type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 16;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.friendly = true;
        Projectile.timeLeft = 180;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = -1;
        Projectile.tileCollide = false;
    }

    public override void AI()
    {
        base.AI();
        Timer++;

        if(Timer == 1)
        {
            Projectile.frame = Main.rand.Next(4);
        }
        if(Timer >= 20)
        {
            Projectile.tileCollide = true;
        }
        if (Main.rand.NextBool(16))
        {
    
        }

        Projectile.velocity *= 0.96f;
        Projectile.rotation += MathF.Sign(Projectile.velocity.X) * 0.15f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        SpritebatchDrawer drawer = SpritebatchDrawer.FromProjectile(Projectile);
        Main.spriteBatch.Draw(drawer);
        return false;
      //  return base.PreDraw(ref lightColor);
    }

    public override void OnKill(int timeLeft)
    {
        base.OnKill(timeLeft);

    }

    public void DrawToRenderTargets()
    {
      
    }
}

public class FossilLauncher : BaseGun
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 12;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 8;
        Item.useAnimation = 8;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.sellPrice(0, 0, 20, 0);
        Item.rare = ItemRarityID.Blue;
        Item.UseSound = SoundID.Item5;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.Bullet;
        Item.shootSpeed = 4f;
        Item.useAmmo = AmmoID.Bullet;
        Item.noMelee = true;
        muzzleOrigin = new Vector2(55, 10);
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 6;
        fireParams.reloadWindow = 60;
    }


    public override Vector2? HoldoutOffset()
    {
        return new Vector2(16, 0);
    }

    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        type = ModContent.ProjectileType<FossilLauncherShard>();
        Vector2 Offset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 20f;
        if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
        {
            position += Offset;
        }

        velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
        Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
        for (int index1 = 0; index1 < 19; ++index1)
        {
            int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, DustID.CopperCoin, velocity.X, velocity.Y, byte.MaxValue, new Color(), Main.rand.Next(6, 10) * 0.1f);
            Main.dust[index2].noGravity = true;
            Main.dust[index2].velocity *= 0.5f;
            Main.dust[index2].scale *= 1.2f;
        }
        damage /= 2;

        //generate the remaining projectiles
        int Sound = Main.rand.Next(1, 3);
        SoundStyle shootSound;
        if (Sound == 1)
        {
            shootSound = new SoundStyle("Stellamod/Assets/Sounds/BrokenWrath2");
        }
        else
        {
            shootSound = new SoundStyle("Stellamod/Assets/Sounds/BrokenWrath1");

        }
        shootSound.PitchVariance = 0.3f;
        shootSound.Volume = 0.5f;
        SoundEngine.PlaySound(shootSound, player.position);


        float numShots = Main.rand.Next(2, 5);
        for(float f = 0; f < numShots; f++)
        {
            Vector2 v = velocity.RotatedByRandom(MathHelper.ToRadians(10));
            v *= Main.rand.NextFloat(2.25f, 4f);
            Projectile.NewProjectile(source, position, v, type, damage, knockback, player.whoAmI);
        }
        return false;
    }
    
    public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.GunShot(player, source, position, velocity, type, damage, knockback);
    }

    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        base.ShootEffects(position, velocity);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankGun>(),
            material: ModContent.ItemType<GintzlMetal>());
    }
}

