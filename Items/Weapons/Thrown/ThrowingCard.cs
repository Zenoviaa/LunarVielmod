
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Stellamod.Projectiles.Thrown;
using Stellamod.Items.Ores;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Thrown
{
	public class ThrowingCards : ModItem
	{
        private Vector2 newVect;

        public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("GreyBricks"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.

        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 30;
            Item.noUseGraphic = true;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = 1;
            Item.knockBack = 6;
            Item.value = 1;
            Item.rare = 2;
            Item.crit = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Card1>();
            Item.shootSpeed = 15f;
            Item.rare = 3;
            Item.consumable = true;
            Item.maxStack = 9999;
        }
      
    }
}