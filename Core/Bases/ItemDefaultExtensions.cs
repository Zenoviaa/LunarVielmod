using Microsoft.Xna.Framework;
using Stellamod.Common.DungeonGeneration;
using Stellamod.Common.MagicCauldron;
using Stellamod.Common.SummonerSystem;
using Stellamod.Common.WeaponTypes;
using Stellamod.Content.Bar.Drinks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Bases
{
    public static class ItemDefaultExtensions
    {
        public static void DefaultToPermanentFood(this Item item, FoodType foodType)
        {
            item.GetGlobalItem<PermamentFoodGlobalItem>().permanentFoodType = foodType;
            item.DefaultToFood(22, 22, BuffID.WellFed, 60 * 60);
            item.width = 28;
            item.height = 28;
            item.UseSound = SoundID.Item3;
            item.useAnimation = ItemUseStyleID.EatFood;
            item.consumable = true;
            item.rare = ItemRarityID.Orange;
            item.shopSpecialCurrency = Stellamod.MedalCurrencyID;
            item.shopCustomPrice = 100;
        }

        public static void DefaultToDrink<T>(this Item item) where T : ModBuff
        {
            DefaultToDrink(item, ModContent.BuffType<T>());
        }

        public static void DefaultToDrink(this Item item, int buffType)
        {
            item.GetGlobalItem<PermamentFoodGlobalItem>().isDrink = true;
            item.DefaultToFood(22, 22, buffType, 2,  true);
            item.width = 28;
            item.height = 28;
            item.consumable = true;
            item.buffTime = 2;
            item.buffType = buffType;
            item.shopSpecialCurrency = Stellamod.MedalCurrencyID;
            item.shopCustomPrice = 20;
            item.rare = ItemRarityID.Orange;
        }

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
        public static void DefaultToBellMinion(this Item item, int projType, float castingTicks = 90, int health = 60, bool isGuardian = false)
        {
            BellMinionGlobalItem globalItem = item.GetGlobalItem<BellMinionGlobalItem>();
            globalItem.isBellMinion = true;
            globalItem.isGuardian = isGuardian;
            globalItem.health = health;
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

        public static void DefaultToArtifact(this Item item)
        {
            ArtifactGlobalItem globalItem = item.GetGlobalItem<ArtifactGlobalItem>();
            globalItem.isMagicArtifact = true;
            item.damage = 15;
            item.DamageType = DamageClass.Magic;
            item.mana = 8;
        }

        public static void DefaultToShield(this Item item, int shieldHoldProjectile)
        {
            ShieldGlobalItem globalItem = item.GetGlobalItem<ShieldGlobalItem>();
            globalItem.isShield = true;
            item.accessory = true;
            item.shoot = shieldHoldProjectile;

        }

        public static void DefaultToManaSphere(this Item item, int manaSphereHoldProjectile, int staminaProj = -1)
        {
            ManaSphereGlobalItem globalItem = item.GetGlobalItem<ManaSphereGlobalItem>();
            globalItem.isManaSphere = true;
            globalItem.heldProj = manaSphereHoldProjectile;
            globalItem.staminaProj = staminaProj;
            globalItem.staminaCost = 2;
            item.damage = 18;
            item.DamageType = DamageClass.Magic;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.mana = 0;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.Swing;

        }
        public static void DefaultToRune(this Item item, int runeProjectileType)
        {
            RuneGlobalItem globalItem = item.GetGlobalItem<RuneGlobalItem>();
            globalItem.isRune = true;
        //    item.damage = 18;
            item.DamageType = DamageClass.Summon;
            item.noUseGraphic = true;
            item.noMelee = true;
            item.mana = 0;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.Swing;
            item.shoot = runeProjectileType;
            item.shootSpeed = 1;
        }
    }
}