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
            stats.meleeAttackSpeed += 0.5f;
            stats.accessorySlots++;
            stats.stamina += 15;
            stats.meleeDamage += 0.5f;
            stats.rangedDamage += 0.2f;
            stats.magicDamage -= 0.2f;
            stats.summonDamage -= 0.1f;
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
