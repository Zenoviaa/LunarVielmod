using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Core.SwingSystem;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases;

public struct MagicCircle
{
    public Asset<Texture2D> textureAsset;
    public Color color;
}
public abstract class BaseCrossbowItem : ModItem,
    IStaminaAttack
{
    public int staminaCost = 2;
    public int staminaProjectileShoot;
    public string BasicEffectLocalizedText
    {
        get
        {
            return LangText.Common("BasicSlash", LangText.Item(this, "BasicSlash"));
        }
    }

    public string StaminaEffectLocalizedText
    {
        get
        {
            return LangText.Common("StaminaSlash", LangText.Item(this, "StaminaSlash"));
        }
    }

    public int StaminaCost => staminaCost;
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        this.GetLocalization("BasicSlash", () => "");
        this.GetLocalization("StaminaSlash", () => "No Effect");
    }

    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.damage = 12;
        Item.DamageType = DamageClass.Ranged;
        Item.useTime = 16;
        Item.crit = 16;
        Item.useAnimation = 16;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 6;
        Item.value = Item.buyPrice(gold: 5);
        Item.rare = ItemRarityID.Blue;
        Item.autoReuse = true;
        Item.useAmmo = AmmoID.Arrow;
        Item.UseSound = null;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.consumable = false;
    }

    public virtual MagicCircle GetMagicCircle()
    {
        return default;
    }

    public override bool AltFunctionUse(Player player)
    {
        return true;
    }

    public override bool? UseItem(Player player)
    {
        CrossbowPlayer crossbowPlayer = player.GetModPlayer<CrossbowPlayer>();
        if (player.altFunctionUse == 2)
        {
            DashPlayer comboPlayer = player.GetModPlayer<DashPlayer>();
            if (comboPlayer.CanConsume(staminaCost))
            {
                crossbowPlayer.usingStamina = true; 
                crossbowPlayer.takeAim = true;
                return true;
            } else
            {
                return false;
            }
        }
        else
        {
            crossbowPlayer.takeAim = true;
            return true;
        }
    }

    public virtual void ShootSwingStamina(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        DashPlayer comboPlayer = player.GetModPlayer<DashPlayer>();
        comboPlayer.Consume(staminaCost);
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
            player.whoAmI);
    }

    public void BowShot(Player player, EntitySource_ItemUse_WithAmmo source,
        ShootParams shootParams)
    {
        CrossbowPlayer crossbowPlayer = player.GetModPlayer<CrossbowPlayer>();
        if (crossbowPlayer.countShots)
        {
            crossbowPlayer.shotCount++;
            if(crossbowPlayer.shotCount % 3 == 0)
            {
                shootParams.damage *= 5;
                shootParams.velocity *= 1.5f;
                shootParams.speed *= 1.5f;
                crossbowPlayer.gothinEnchant++;
            } else
            {
   
            }
        }

        ShootBow(player, source, shootParams);
    }

    public virtual void ShootBow(Player player, EntitySource_ItemUse_WithAmmo source,
        ShootParams shootParams)
    {
        Vector2 fireVelocity = shootParams.velocity * shootParams.speed;
        fireVelocity *= 3;
        fireVelocity *= shootParams.chargeStrength;


        float bowDamage = shootParams.damage * shootParams.chargeStrength;
        Projectile crossShot = Projectile.NewProjectileDirect(source, shootParams.position, fireVelocity,
            shootParams.projToShoot, (int)bowDamage, shootParams.knockBack, player.whoAmI, ai0: shootParams.projToShoot);
        crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().isCrossbowShot = true;
    }

    public virtual void StaminaShootBow(Player player, EntitySource_ItemUse_WithAmmo source,
        ShootParams shootParams)
    {

    }
}

public struct ShootParams
{
    public Vector2 position;
    public Vector2 velocity;
    public Vector2 fireVelocity => velocity * speed;
    public int projToShoot;
    public float speed;
    public int damage;
    public float knockBack;
    public int useAmmoItemId;
    public float chargeStrength;

}
