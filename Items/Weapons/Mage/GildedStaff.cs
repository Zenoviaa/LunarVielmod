using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class GildedStaff : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 20;
            Item.mana = 6;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Gilded Staff");
			// Tooltip.SetDefault("Shoots two spinning pieces of spiritual magic at your foes!\nThe fabric is super magical, it turned wood into something like a flamethrower! :>");
		}
		public override void SetDefaults()
		{
			Item.damage = 15;
			Item.mana = 5;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 33;
			Item.useAnimation = 66;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.staff[Item.type] = true;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Magic;
			Item.value = Item.sellPrice(silver: 10);
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.Item2;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<ShadeBall>();
			Item.shootSpeed = 8f;
			Item.autoReuse = true;
			Item.crit = 22;
		}
		
	}
}









