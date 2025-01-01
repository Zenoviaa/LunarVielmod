using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class FlameburstBalls : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 6;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Molted Crust Ball");
			// Tooltip.SetDefault("Shoots fast homing sparks of fire!");
		}
		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.mana = 2;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 14;
			Item.useAnimation = 14;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.DD2_BetsyFireballShot;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Flamelash;
			Item.shootSpeed = 7f;
			Item.autoReuse = true;
			Item.crit = 2;
			Item.noUseGraphic = true;
		}


        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankOrb>(), material: ModContent.ItemType<Cinderscrap>());
        }

    }
}









