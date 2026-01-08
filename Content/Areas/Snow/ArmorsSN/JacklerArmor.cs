using Stellamod.Common.ArmorRework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Snow.ArmorsSN
{
    [AutoloadEquip(EquipType.Head)]
    public class JacklerHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<JacklerHat, JacklerCoat, JacklerPants>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateEquip(Player player)
        {

        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class JacklerCoat : ModItem
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.criticalStrikeChance += 0.5f;
            stats.criticalStrikeDamage += 5;
            stats.accessorySlots += 8;
            /*
            stats.meleeAttackSpeed += 0.5f;
            stats.accessorySlots++;
            stats.stamina += 15;
            stats.meleeDamage += 0.5f;
            stats.rangedDamage += 0.2f;
            stats.magicDamage -= 0.2f;
            stats.summonDamage -= 0.1f;
            stats.healthBonus += 40;
            stats.criticalStrikeDamage += 0.5f;
            stats.criticalStrikeChance += 0.5f;
            stats.wandCastTime += 0.5f;
            stats.wandNormalEnchantmentSlots += 3;
            stats.wandTimerEnchantmentSlots += 3;
            stats.totalMana += 50;
            stats.artifactManaReduction += 0.5f;
            stats.bossEndurance += 0.04f;
            stats.enemyEndurance += 0.08f;
            stats.generalEndurance += 0.02f;
            stats.healthBonus += 50;
            stats.mainSummonHealth += 0.5f;
            stats.mainSummonDamage += 0.5f;
            stats.minionSlots += 4;
            stats.inventorySlots += 15;*/
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class JacklerPants : ModItem
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public override void UpdateEquip(Player player)
        {

        }
    }
}
