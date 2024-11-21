using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Stellamod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Melee
{
    public class VerstiDance : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 15;
            Item.mana = 6;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Verstidance");
			/* Tooltip.SetDefault("An Arcanal Weapon!" +
				"\nSwirl!" +
			"\nA petal dance that increases your movement speed and defense, but be wary of high risk "); */
		}
		public override void SetDefaults()
		{
			Item.damage = 33;
			Item.DamageType = DamageClass.Melee;
			Item.width = 32;
			Item.mana = 2;
			Item.height = 32;
			Item.useTime = 500;
			Item.useAnimation = 500;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 5;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.DD2_MonkStaffSwing;
			Item.autoReuse = false;
			Item.value = Item.sellPrice(0, 0, 12, 20);
			Item.shoot = ModContent.ProjectileType<PetalDance>();
			Item.shootSpeed = 10f;
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}
	}
}