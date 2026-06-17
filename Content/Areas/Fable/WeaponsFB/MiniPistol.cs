using Microsoft.Xna.Framework;
using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.WeaponsFB;

public class MiniPistol : BaseGun
{
    private int _comboCounter;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 4;
        Item.DamageType = DamageClass.Ranged;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 31;
        Item.useAnimation = 31;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = 100000;
        Item.rare = ItemRarityID.Pink;
        Item.UseSound = SoundID.Item36;
        Item.autoReuse = true;
        Item.shoot = ProjectileID.Bullet;
        Item.shootSpeed = 35f;
        Item.useAmmo = AmmoID.Bullet;
        Item.noMelee = true;
        muzzleOrigin = new Vector2(68, 9);
    }

    public override void SetMagazine(ref GunReloadParams fireParams)
    {
        base.SetMagazine(ref fireParams);
        fireParams.maxAmmo = 100;
        fireParams.reloadWindow = 120;
    }

    public override Vector2? HoldoutOffset()
    {
        return new Vector2(16, 0);
    }

    public override bool ShootProjectile(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Item.ArmorPenetration = 15;
        float rot = velocity.ToRotation();
        float spread = 0.4f;
        muzzleOrigin = new Vector2(68, 9);
        Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);
        Item.useTime = (int)MathHelper.Lerp(4, 32, MathHelper.Clamp((float)(remainingAmmo - 75) / 25, 0f, 1f));
        Item.useAnimation = Item.useTime;
        _comboCounter++;
        if (remainingAmmo == 1)
        {
            for(int k = 0; k < 7; k++)
            {
                var faintSmokeParticle = FaintSmokeParticle.SpawnInAlphaLayer(position, Main.rand.NextVector2Circular(12, 12));
                faintSmokeParticle.fadeToColor = Color.Black * 0.2f;
                faintSmokeParticle.color = Color.RosyBrown * 0.2f;
                faintSmokeParticle.Scale *= 0.5f;
                faintSmokeParticle.dampening = 0.1f;
            }
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol2");
            shootSound = shootSound with { PitchVariance = 0.66f, Volume = 0.4f };
            SoundEngine.PlaySound(shootSound, position);
            Item.useTime = 31;
            Item.useAnimation = 31;
            _comboCounter = 0;
        }





        for (int p = 0; p < 1; p++)
        {
            // Rotate the velocity randomly by 30 degrees at max.
            Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
            newVelocity *= 1f - Main.rand.NextFloat(0.3f);
            Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
        }

        Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 4);
        int Sound = Main.rand.Next(1, 3);
        if (Sound == 1)
        {
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol");
            shootSound.Volume = 0.05f;
            shootSound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(shootSound);
        }
        else
        {
            SoundStyle shootSound = new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3");
            shootSound.Volume = 0.05f;
            shootSound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(shootSound);
        }

        //Dust Burst Towards Mouse

        return false;
       // return base.ShootProjectile(player, source, position, velocity, type, damage, knockback);
    }
    public override void ShootEffects(Vector2 position, Vector2 velocity)
    {
        base.ShootEffects(position, velocity);
    }
    public override bool GunShot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        return base.GunShot(player, source, position, velocity, type, damage, knockback);
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<HypnotizedSoul, BlankGun>();
    }
}
