
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Pikmin;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    public class ZenoviasPikpikGlove : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Plantius"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }


        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 9;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(silver: 25);
            Item.crit = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<RedPikminThrow>();
            Item.shootSpeed = 15f;
            Item.rare = ItemRarityID.Blue;
        }
		public int AttackCounter = 1;
        float yellowpikpik = 0.7f;
        float redpikpik = 1.5f;
        float whitepikpik = 0.3f;
        float purplepikpik = 2.0f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int dir = AttackCounter;
			if (player.GetModPlayer<MyPlayer>().Onion1)
			{
                switch (Main.rand.Next(2))
                {
                    case 0:
                        Projectile.NewProjectile(source, position, velocity, type, (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * redpikpik),
                  knockback, player.whoAmI, 1, dir);
                        break;

                    case 1:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<YellowPikminThrow>(), (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * yellowpikpik),
                  knockback, player.whoAmI, 1, dir);
                        break;

                }


               
            }

            else if (player.GetModPlayer<MyPlayer>().Onion2)
            {
                switch (Main.rand.Next(3))
                {
                    case 0:
                        Projectile.NewProjectile(source, position, velocity, type, (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * redpikpik),
                  knockback, player.whoAmI, 1, dir);
                        break;

                    case 1:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<YellowPikminThrow>(), (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * yellowpikpik),
                  knockback, player.whoAmI, 1, dir);
                        break;

                    case 2:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BluePikminThrow>(), (int)(damage + player.GetModPlayer<MyPlayer>().OnionDamage),
                  knockback, player.whoAmI, 1, dir);
                        break;

                }
            }

            else if (player.GetModPlayer<MyPlayer>().Onion3)
            {
                switch (Main.rand.Next(4))
                {
                    case 0:
                        Projectile.NewProjectile(source, position, velocity, type, (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * redpikpik),
                  knockback, player.whoAmI, 1, dir);
                        break;

                    case 1:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<YellowPikminThrow>(), (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * yellowpikpik),
                  knockback, player.whoAmI, 1, dir);
                        break;

                    case 2:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BluePikminThrow>(), (int)(damage + player.GetModPlayer<MyPlayer>().OnionDamage),
                  knockback, player.whoAmI, 1, dir);
                        break;

                    case 3:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<WhitePikminThrow>(), (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * whitepikpik),
                   knockback, player.whoAmI, 1, dir);
                        break;

                }
            }


            else if (player.GetModPlayer<MyPlayer>().Onion4)
            {
                switch (Main.rand.Next(5))
                {
                    case 0:
                        Projectile.NewProjectile(source, position, velocity, type, (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * redpikpik),
                  knockback, player.whoAmI, 1, dir);
                        break;

                    case 1:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<YellowPikminThrow>(), (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * yellowpikpik),
                  knockback, player.whoAmI, 1, dir);
                        break;

                    case 2:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<BluePikminThrow>(), (int)(damage + player.GetModPlayer<MyPlayer>().OnionDamage),
                  knockback, player.whoAmI, 1, dir);
                        break;

                    case 3:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<WhitePikminThrow>(), (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * whitepikpik),
                   knockback, player.whoAmI, 1, dir);
                        break;

                    case 4:
                        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PurplePikminThrow>(), (int)((damage + player.GetModPlayer<MyPlayer>().OnionDamage) * purplepikpik),
                   knockback, player.whoAmI, 1, dir);
                        break;

                }
            }


            else
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RedPikminThrow>(), (damage + player.GetModPlayer<MyPlayer>().OnionDamage),
                  knockback, player.whoAmI, 1, dir);
            }





            switch (Main.rand.Next(4))
            {
                case 0:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pikminthrow1") with { Volume=0.5f}, position);
                    break;

                case 1:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pikminthrow2") with { Volume = 0.4f }, position);
                    break;

                case 2:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pikminthrow3") with { Volume = 0.4f }, position);
                    break;

                case 3:
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Pikminthrow4") with { Volume = 0.4f }, position);
                    break;



            }





            return false;
		}

	}
}