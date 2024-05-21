using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class JellyBow : ModItem
	{
		private int _comboCounter;
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Jelly Bow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 12;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item5;
            Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.shootSpeed = 15f;
			Item.useAmmo = AmmoID.Arrow;
            Item.noMelee = true;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
			_comboCounter++;
			if(_comboCounter >= 3)
			{
				type = ModContent.ProjectileType<Gelatin>();
				damage += 5;
				SoundEngine.PlaySound(SoundRegistry.JellyBow, position);
				_comboCounter = 0;
			}
        }
    }
}