using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class ComicalTrident : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Melee;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 50;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 12;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.crit = 33;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ComicalTridentP>();
            Item.shootSpeed = 19f;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = false;

        }
    }
}