using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Ranged
{
    public class VoidBlaster : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Tychine Gun");
			// Tooltip.SetDefault("Chance to shoot sharks dealing three times the normal damage");
		}

		public override void SetDefaults()
		{
			Item.noMelee = true;
			Item.damage = 8;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;


			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VoidBlasterProj>();
            Item.shootSpeed = 20f;
			Item.useAmmo = AmmoID.Bullet;

            Item.useAnimation = 20;
            Item.useTime = 3; // one third of useAnimation
            Item.reuseDelay = 60;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-4, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{




            int numberProjectiles = 1; // 4 or 5 shots
			for (int i = 0; i < numberProjectiles; i++)
            {
                type = ProjectileType<VoidBlasterProj>();
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(30)); // 30 degree spread.
																												// If you want to randomize the speed to stagger the projectiles
				//float scale = 1f - (Main.rand.NextFloat() * .4f);
				// perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}

            //Dust Burst Towards Mouse

            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1.3f, 0f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(50, 74, 255), Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(50, 74, 255), 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));

            return false; // return false because we don't want tmodloader to shoot projectile
		}
	}
}