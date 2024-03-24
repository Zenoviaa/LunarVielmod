using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    internal class StumpBuster : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
			Item.damage = 24;
        }

        public override void SetDefaults()
		{
			Item.damage = 15;
			Item.mana = 5;
			Item.width = 18;
			Item.height = 21;
			Item.useTime = 22;
			Item.useAnimation = 22;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true;
			Item.knockBack = 2f;
			Item.DamageType = DamageClass.Summon;
			Item.value = 1200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_BookStaffCast;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Stump>();
			Item.shootSpeed = 4f;
			Item.autoReuse = true;
			Item.crit = 32;
		}
		
	}
}









