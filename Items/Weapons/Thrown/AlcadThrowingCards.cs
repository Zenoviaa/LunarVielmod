using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class AlcadThrowingCards : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 25;
            Item.mana = 0;
        }
        public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("GreyBricks"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 30;
            Item.noUseGraphic = true;
            Item.height = 30;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 10;
            Item.value = 1000;
            Item.crit = 15;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Card5>();
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.LightPurple;
            Item.consumable = false;
            Item.maxStack = 1;
            Item.buyPrice(0, 10, 0, 0);
        }
      
    }
}