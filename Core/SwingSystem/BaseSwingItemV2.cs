using Stellamod.Common.GunSystem;
using Stellamod.Core.Bases;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using Stellamod.Visual.Explosions;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.SwingSystem
{
    public interface IStaminaAttack
    {
        public string BasicEffectLocalizedText { get; }
        public string StaminaEffectLocalizedText { get; }
        public int StaminaCost { get; }
    }
    public class StaminaAttackExpandableTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            if (item.ModItem == null)
                return;
            TooltipLine line;
            if (item.ModItem is BaseSwingItemV2 swingItem)
            {
                line = new TooltipLine(Mod, "WeaponType", LangText.Common("WeaponType" + swingItem.meleeWeaponType.ToString()));
                line.OverrideColor = Color.GreenYellow;
                lines.Add(line);


            }
            if (item.ModItem is IStaminaAttack staminaAttack)
            {
                line = new TooltipLine(Mod, "BasicSlash", staminaAttack.BasicEffectLocalizedText);
                lines.Add(line);

                line = new TooltipLine(Mod, "StaminaSlash", staminaAttack.StaminaEffectLocalizedText);
                lines.Add(line);

                line = new TooltipLine(Mod, "StaminaCost", LangText.Common("StaminaCost",
                    staminaAttack.StaminaCost.ToString()));
                line.OverrideColor = Color.Goldenrod;
                lines.Add(line);
            }

            if(item.ModItem is BaseGun gun)
            {
                line = new TooltipLine(Mod, "Gun", Helpers.LangText.Common("WeaponTypeGun"))
                {
                    OverrideColor = Color.LightGreen
                };
                lines.Add(line);
                line = new TooltipLine(Mod, "GunHelp", LangText.Common("GunHelp"))
                {
                    OverrideColor = Color.White
                };
                lines.Add(line);
            }
        }
    }
    public abstract class BaseSwingItemV2 : ModItem,
        IStaminaAttack
    {
        public int comboResetTime = 120;
        public int staminaProjectileShoot;
        public int staminaCost = 2;
        public float staminaDamageMultiplier;

        public MeleeWeaponType meleeWeaponType;

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

        //Sealing the set defaults that are common across all things so we don't accidentally override
        public sealed override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 8;
            Item.DamageType = DamageClass.Melee;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 10;
            comboResetTime = 120;
            staminaDamageMultiplier = 1;
            SetDefaults2();
        }

        public virtual void SetDefaults2()
        {

        }

        public virtual void ShootSwing(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
            comboPlayer.ComboWaitTime = comboResetTime;

            int combo = comboPlayer.ComboCounter;
            int dir = comboPlayer.ComboDirection;
            Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
                player.whoAmI, ai1: dir, ai2: combo);
            if(p.ModProjectile is BaseSwingProjectileV2 swingV2)
            {
                comboPlayer.IncreaseCombo();
            }
       
        }


        public virtual void ShootSwingStamina(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == -1)
                return;

            int staminaDamage = (int)(damage * staminaDamageMultiplier);
            //Only do the swinging initialization if it is a swing projectile lol
            var proj = ModContent.GetModProjectile(type);
            if (proj is BaseSwingProjectileV2)
            {
                DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
                SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
                int combo = comboPlayer.StaminaComboCounter;
                int dir = comboPlayer.ComboDirection;

                Projectile p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,
                    player.whoAmI, ai1: dir, ai2: combo);
                if (p.ModProjectile is BaseSwingProjectileV2 swingV2)
                {
                    comboPlayer.IncreaseCombo();
                }
            }
            else
            {
                Projectile.NewProjectile(source, position, velocity, type, staminaDamage, knockback, player.whoAmI);
            }
            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<BasicStaminaExplosion>(), damage, knockback, player.whoAmI);
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public sealed override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            DashPlayer dashPlayer = player.GetModPlayer<DashPlayer>();
            SwingPlayerV2 comboPlayer = player.GetModPlayer<SwingPlayerV2>();
            if (player.altFunctionUse == 2)
            {
                if (dashPlayer.CanConsume(staminaCost))
                {
                    comboPlayer.ComboWaitTime = comboResetTime;
                    dashPlayer.Consume(staminaCost);
                    ShootSwingStamina(player, source, position, velocity, staminaProjectileShoot, damage, knockback);
                }

            }
            else
            {
                ShootSwing(player, source, position, velocity, type, damage, knockback);
            }

            return false;
        }
    }
}
