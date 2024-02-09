
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class Vulcanius : ModItem
	{
        public override void SetStaticDefaults() 
		{
			// DisplayName.SetDefault("Plantius"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}


        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.crit = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Vulcanius2>();
            Item.shootSpeed = 15f;
            Item.rare = ItemRarityID.Blue;
        }
    }
}