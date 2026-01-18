using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public abstract class BaseCrossbowItem : ModItem
    {
        public int staminaCost = 2;
        public int staminaProjectileShoot;

        public string BasicSlash
        {
            get
            {
                return LangText.Common("BasicSlash", LangText.Item(this, "BasicSlash"));
            }
        }

        public string StaminaSlash
        {
            get
            {
                return LangText.Common("StaminaSlash", LangText.Item(this, "StaminaSlash"));
            }
        }


        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            this.GetLocalization(nameof(BasicSlash), () => "");
            this.GetLocalization(nameof(StaminaSlash), () => "No Effect");
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

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            var line = new TooltipLine(Mod, "", "");
            Keys keys = Keys.LeftShift;
            bool isExpanded = Main.keyState.IsKeyDown(keys);
            line = new TooltipLine(Mod, "Crossbow", Helpers.LangText.Common("Crossbow"))
            {
                OverrideColor = Color.LightGreen
            };
            tooltips.Add(line);
            if (!isExpanded)
            {
                line = new TooltipLine(Mod, "ExpandTooltipHelp", LangText.Common("ExpandTooltipHelp", "Left Shift"));
                line.OverrideColor = Color.Lerp(Color.White, Color.Black, 0.7f);
                tooltips.Add(line);
            }
            else
            {


                line = new TooltipLine(Mod, "CrossbowHelp", Helpers.LangText.Common("CrossbowHelp"))
                {
                    OverrideColor = Color.White
                };
                tooltips.Add(line);

            }


            line = new TooltipLine(Mod, "BasicSlash", LangText.Common("BasicSlash", LangText.Item(this, "BasicSlash")));
            line.OverrideColor = new Color(124, 187, 80);
            tooltips.Add(line);

            line = new TooltipLine(Mod, "StaminaSlash", LangText.Common("StaminaSlash", LangText.Item(this, "StaminaSlash")));
            line.OverrideColor = Color.Gold;
            tooltips.Add(line);

            line = new TooltipLine(Mod, "StaminaCost", LangText.Common("StaminaCost", staminaCost.ToString()));
            line.OverrideColor = Color.Gold;
            tooltips.Add(line);
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
            crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().CrossbowShot = true;
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
}
