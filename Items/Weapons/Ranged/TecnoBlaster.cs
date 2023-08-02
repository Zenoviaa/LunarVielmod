using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;

using Stellamod.Items.Materials;
using Stellamod.Projectiles.Gun;

namespace Stellamod.Items.Weapons.Ranged
{
    class TecnoBlaster : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tecno Blaster");

        }

        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 11;
            Item.useAnimation = 11;
            Item.useStyle = 5;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 20, 0);
            Item.rare = 4;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<StarBolt>();
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundID.Item92;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet) type = ModContent.ProjectileType<StarBolt>();
            Vector2 Offset = Vector2.Normalize(new Vector2(velocity.X, velocity.X - 1)) * 20f;
            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }

            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, Item.damage, (int)Item.knockBack, Item.playerIndexTheItemIsReservedFor, 0, 0);
            proj.netUpdate = true;
            for (int index1 = 0; index1 < 19; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, 205, velocity.X, velocity.X, (int)byte.MaxValue, new Color(), (float)Main.rand.Next(6, 10) * 0.1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 0.5f;
                Main.dust[index2].scale *= 1.2f;
            }
            return false;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddIngredient(ItemType<DarkEssence>(), 10);
            recipe.AddIngredient(ItemType<StarSilk>(), 3);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}