using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Quest.Merena;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Mage
{
	public class StarringBalls : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 16;
            Item.mana = 0;
        }

        public override void SetDefaults()
		{
			Item.damage = 33;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noUseGraphic = true;
			Item.knockBack = 2;
			Item.value = 10000;
			Item.rare = ItemRarityID.LightPurple;
			Item.UseSound = SoundID.DD2_DarkMageAttack;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<SparkBallsP>();
			Item.shootSpeed = 8f;
			Item.mana = 6;
		}

        public override void AddRecipes()
        {
            base.AddRecipes();
			this.RegisterBrew<HypnotizedSoul, BlankOrb>();
        }
	}
}