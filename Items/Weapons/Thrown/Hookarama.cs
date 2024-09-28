using Stellamod.Projectiles.Thrown;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    internal class Hookarama : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 28;
        }

        public override void SetDefaults()
        {
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.damage = 21;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.width = 74;
            Item.height = 74;
            Item.DamageType = DamageClass.Generic;

            Item.value = 10000;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.shootSpeed = 60;
            Item.shoot = ModContent.ProjectileType<HookaramaProj>();
            Item.rare = ItemRarityID.Green;
        }
    }
}
