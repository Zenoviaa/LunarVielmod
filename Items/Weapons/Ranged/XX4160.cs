using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Gun;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class XX4160 : ModItem
    {

        public bool Overheated;
        public int LAZERAlpha = 255;
        public bool LAZERMode;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Divine Sharpshooter"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}
        public override bool AltFunctionUse(Player player) => true;

        public override void SetDefaults()
        {

            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 5;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = 100000;
            Item.rare = ItemRarityID.LightPurple;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 35f;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;

        }

        public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
        }
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (LAZERMode)
                {
                    Item.damage = 18;
                    Item.shootSpeed = 35f;
                    LAZERMode = false;
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/XX41604"));
                }
                else
                {
                    Item.damage = 20;
                    Item.shootSpeed = 3f;
                    LAZERMode = true;
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/XX41603"));
                }
            }
            else
            {

            }

            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.AddIngredient(ItemID.SoulofFright, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            var EntitySource = player.GetSource_FromThis();

            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"));
            }


            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(3.1f, 0.3f * player.direction).RotatedBy(rot);

            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125,  Color.DarkRed, 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.White * 0.5f, Main.rand.NextFloat(0.5f, 1));
            if (LAZERMode && !player.HasBuff(ModContent.BuffType<Overheated>()))
            {
                int Sound2 = Main.rand.Next(1, 3);
                if (Sound2 == 1)
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/XX4160"));
                }
                else
                {
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/XX41602"));
                }

                LAZERAlpha -= 4;
                for (int k = 0; k < 3; k++)
                {
                    Vector2 direction = offset.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), LAZERAlpha, Color.Red, Main.rand.NextFloat(0.6f, 1.2f));

                    if (LAZERAlpha <= 200 && LAZERAlpha >= 150)
                    {
                        Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 0, Color.Black * 0.5f, Main.rand.NextFloat(0.3f, 0.7f));
                    }
                    if (LAZERAlpha <= 150 && LAZERAlpha >= 100)
                    {
                        Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 0, Color.DarkRed * 0.5f, Main.rand.NextFloat(0.5f, 0.7f));
                    }
                    if (LAZERAlpha <= 100 && LAZERAlpha >= 50)
                    {
                        Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 0, Color.OrangeRed * 0.5f, Main.rand.NextFloat(0.5f, 1));
                    }
                    if (LAZERAlpha <= 50)
                    {
                        Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 0, Color.LightGoldenrodYellow * 0.5f, Main.rand.NextFloat(0.5f, 1));
                    }

                    if(LAZERAlpha <= 7)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol2"));
                        player.AddBuff(ModContent.BuffType<Overheated>(), 1500);
                        Item.useTime = 105;
                        Item.useAnimation = 105;
                        LAZERMode = false;
                        LAZERAlpha = 255;
                    }
                }

                Item.damage = 25;
                Item.shootSpeed = 3f;
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 30f);
                Item.useTime = 3;
                Item.useAnimation = 3;
                Projectile.NewProjectile(source, position, velocity * 2, ModContent.ProjectileType<XX4160Shot>(), damage * 2, knockback, player.whoAmI, 1);
            }
            else
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 15f);
                if (player.HasBuff(ModContent.BuffType<Overheated>()))
                {
                    Item.damage = 10;
                    Item.shootSpeed = 35f;
                    Item.useTime = 15;
                    Item.useAnimation = 15;
                }
                else
                {
                    Item.damage = 20;
                    Item.shootSpeed = 35f;
                    Item.useTime = 5;
                    Item.useAnimation = 5;
                }

                return base.Shoot(player, source, position, velocity, type, damage, knockback);
            }
            return false;
		}
	}
}
