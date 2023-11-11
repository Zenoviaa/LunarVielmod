using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Mage
{
    public class TorrentialLance : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Torrential Lance"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
			// Tooltip.SetDefault("shoots a Lance that fires humming bubbles");
		}
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
        public override void SetDefaults()
        {
            Item.damage = 28;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.mana = 15;
            Item.value = 100000;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item21;
            Item.shoot = ProjectileType<TorrentialLanceP>();
            Item.shootSpeed = 15f;
            Item.autoReuse = true;
        }

       
    }
}