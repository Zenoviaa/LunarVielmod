using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class GreekLantern : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 20;
            Item.mana = 6;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Pearlescent Ice Ball");
			// Tooltip.SetDefault("Shoots fast homing sparks of light!");
		}
		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.mana = 2;
			Item.width = 29;
			Item.height = 31;
			Item.useTime = 8;
			Item.useAnimation = 8;
			Item.useStyle = ItemUseStyleID.RaiseLamp;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.scale = 0.5f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_BookStaffCast;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.MolotovFire;
			Item.shootSpeed = 8f;
			Item.autoReuse = true;
			Item.crit = 12;
			Item.rare = ItemRarityID.Green;
		}
		

	
	}
}









