using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class MiniPistol : ModItem
	{
		private int _comboCounter;
		public override void SetDefaults()
		{
			Item.damage = 17;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 31;
			Item.useAnimation = 31;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 100000;
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item36;
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1.5f, -0.1f * player.direction).RotatedBy(rot);

            _comboCounter++;
            if (_comboCounter > 100)
            {
                for (int k = 0; k < 7; k++)
                {
                    Vector2 direction = offset.RotatedByRandom(spread);
                    Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
                    Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.IndianRed * 0.5f, Main.rand.NextFloat(0.5f, 1));
                    Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
                    Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, Color.IndianRed, Main.rand.NextFloat(0.5f, 0.8f));
                }
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol2"));
                Item.useTime = 31;
                Item.useAnimation = 31;
                _comboCounter = 0;
            }
            if (_comboCounter > 75)
            {
                Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, Color.IndianRed * 0.5f, Main.rand.NextFloat(0.5f, 1));
            }

            if (Item.useAnimation > 4)
            {
                Item.useTime--;
                Item.useAnimation--;
            }

            for (int p = 0; p < 1; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 12f);
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol"));
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/MiniPistol3"));
            }

            //Dust Burst Towards Mouse


            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                
                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(180, 50, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }

            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
			recipe.AddIngredient(ItemID.HallowedBar, 12);
			recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 10);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
        }
    }
}
