using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    internal class SpiritCapsule : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 60;

        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pearlescent Ice Ball");
            // Tooltip.SetDefault("Shoots fast homing sparks of light!");
        }
        public override void SetDefaults()
        {
            Item.damage = 120;
            Item.width = 40;
            Item.height = 40;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.DamageType = DamageClass.Throwing;
            Item.value = 12000;
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.DD2_DarkMageAttack;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpiritCapsuleP>();
            Item.shootSpeed = 20f;
            Item.autoReuse = true;
            Item.crit = 12;
            Item.noUseGraphic = true;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.consumable = false;

        }
    }
}









