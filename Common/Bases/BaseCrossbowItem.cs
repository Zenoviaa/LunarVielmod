using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Items;
using Stellamod.Projectiles.Crossbows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal abstract class BaseCrossbowItem : ClassSwapItem
    {
        public int CrossbowProjectileType;
        public override DamageClass AlternateClass => DamageClass.Magic;
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.mana = 25;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 32;
            Item.crit = 16;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = null;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.consumable = false;
            CrossbowProjectileType = ModContent.ProjectileType<WoodenCrossbowHold>();
        }

        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[CrossbowProjectileType] == 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, CrossbowProjectileType, damage, knockback, player.whoAmI);
            return false;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Crossbow", Helpers.LangText.Common("Crossbow"))
            {
                OverrideColor = Color.OrangeRed
            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "CrossbowHelp", Helpers.LangText.Common("CrossbowHelp"))
            {
                OverrideColor = Color.LightGray
            };
            tooltips.Add(line);
        }
    }
}
