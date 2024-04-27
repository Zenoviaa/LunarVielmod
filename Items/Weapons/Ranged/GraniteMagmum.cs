using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Ranged
{
    public class GraniteMagmum : ModItem
	{
		public override void SetStaticDefaults() 
		{
            // DisplayName.SetDefault("Granite Magmum");
		}

		public override void SetDefaults() 
		{
			Item.damage = 15;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 11;
			Item.useAnimation = 11;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.Item5;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<GraniteMagmumProj>();
			Item.shootSpeed = 4f;
			Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			if (type == ProjectileID.Bullet) type = ModContent.ProjectileType<GraniteMagmumProj>();
			Vector2 Offset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 20f;
			if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
			{
				position += Offset;
			}

			velocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
			Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, Item.damage, Item.knockBack, Item.playerIndexTheItemIsReservedFor, 0, 0);
			proj.netUpdate = true;
            for (int index1 = 0; index1 < 19; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, DustID.UnusedWhiteBluePurple, velocity.X, velocity.Y, byte.MaxValue, new Color(), Main.rand.Next(6, 10) * 0.1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 0.5f;
                Main.dust[index2].scale *= 1.2f;
            }
			damage = Item.damage / 2;
			Vector2 origVect = new Vector2(velocity.X, velocity.Y);
            //generate the remaining projectiles
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GraniteMagmum1"), player.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/GraniteMagmum2"), player.position);
            }
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.position, 248f, 04f);

			return false;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ItemID.Granite, 25);
            recipe.AddIngredient(ItemType<DarkEssence>(), 15);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
	}
}