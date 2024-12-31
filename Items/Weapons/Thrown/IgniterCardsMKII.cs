
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class IgniterCardsMKII : ModItem
	{
        public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("GreyBricks"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Item.damage = 17;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 30;
            Item.noUseGraphic = true;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 1;
  
            Item.crit = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Card3>();
            Item.shootSpeed = 18f;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = true;
            Item.maxStack = Item.CommonMaxStack;
        } 
    }
}