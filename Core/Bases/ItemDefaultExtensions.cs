using Microsoft.Xna.Framework;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.SummonerSystem;
using Stellamod.Common.WeaponTypes;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public static class ItemDefaultExtensions
    {
        public static void DefaultToSafunai(this Item item)
        {
            SafunaiGlobalItem globalItem = item.GetGlobalItem<SafunaiGlobalItem>();
            globalItem.isSafunai = true;
            item.DamageType = DamageClass.Melee;
        }

        public static void DefaultToGrapple(this Item item, float grappleTileDistance)
        {
            GrappleGlobalItem globalItem = item.GetGlobalItem<GrappleGlobalItem>();
            globalItem.isGrapple = true;
            globalItem.grappleLineTileDistance = grappleTileDistance;
        }

        public static void DefaultToNecronomicon(this Item item, int staminaCost = 2, Color? hintColor = null)
        {
            Necronomicon globalItem = item.GetGlobalItem<Necronomicon>();
            globalItem.isNecronomicon = true;
            globalItem.necronomiconStaminaCost = staminaCost;
            globalItem.hintColor = hintColor.HasValue ? hintColor.Value : Color.White;
            item.DamageType = DamageClass.Summon;
            item.damage = 18;
            item.rare = ItemRarityID.Green;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shootSpeed = 1f;
            item.knockBack = 4f;
            item.channel = false;
            item.autoReuse = false;
        }

        public static void DefaultToCombatTool(this Item item, float bossDamagePercent, float enemyDamagePercent, int ammoCount = 3)
        {
            CombatTool globalItem = item.GetGlobalItem<CombatTool>();
            globalItem.isCombatTool = true;
            globalItem.enemyDamagePercent = enemyDamagePercent;
            globalItem.bossDamagePercent = bossDamagePercent;
            globalItem.maxAmmoCount = ammoCount;
            globalItem.ammoCount = ammoCount;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useTime = 16;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.Shoot;
            item.shootSpeed = 15;
            item.knockBack = 4f;
            item.channel = false;
            item.autoReuse = false;
            item.DamageType = DamageClass.Ranged;
            item.damage = 18;
            item.rare = ItemRarityID.Green;
        }
        public static void DefaultToMold(this Item item)
        {
            MoldGlobalItem globalItem = item.GetGlobalItem<MoldGlobalItem>();
            globalItem.isMold = true;
            item.shopSpecialCurrency = Stellamod.MedalCurrencyID;
            item.shopCustomPrice = 5;
        }
        public static void DefaultToBellMinion(this Item item, int projType, float castingTicks = 300)
        {
            BellMinionGlobalItem globalItem = item.GetGlobalItem<BellMinionGlobalItem>();
            globalItem.isBellMinion = true;
            globalItem.addedCastingTime = castingTicks;
            item.shoot = projType;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 4;
            item.damage = 15;
            item.DamageType = DamageClass.Summon;
            item.rare = ItemRarityID.Green;
            item.width = 32;
            item.height = 32;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = ItemUseStyleID.Swing;
        }
    }
}
