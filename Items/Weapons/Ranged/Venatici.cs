using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Venatici : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 30;
            Item.mana = 10;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Divine Sharpshooter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 41;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 43;
			Item.height = 10;
			Item.useTime = 25;
			Item.useAnimation = 25;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 2;
			Item.value = 100000;
			Item.rare = ItemRarityID.Green;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/TON618");
			Item.autoReuse = false;
			Item.shoot = ProjectileType<Venbullet>();
			Item.shootSpeed = 20f;
            Item.noMelee = true;

        }
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-20, 4);
		}

	
	}
}
