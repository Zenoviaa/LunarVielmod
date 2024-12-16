using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Bow;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class AssassinsDischarge : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;
        public int combo;
        public override void SetClassSwappedDefaults()
        {
            Item.damage = 4;
            Item.mana = 0;
        }
        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 12;
            Item.useTime = 4; // one third of useAnimation
            Item.reuseDelay = 28;
            Item.noMelee = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }


        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
            combo++;
            if (combo == 1)
            {
                type = ModContent.ProjectileType<AssassinsArrow>();
            }
            if (combo == 3)
            {
                combo = 0;
            }
        }
    }
}
