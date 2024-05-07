using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Bow;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class IcelockLongbow : ModItem
    {
        private float _combo;
        public override void SetDefaults()
        {
            Item.damage = 36;
            Item.width = 44;
            Item.height = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Lime;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.noMelee = true;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int maxCombo = 7;
            _combo++;
            if(_combo >= maxCombo)
            {
                _combo = 1;
            }

            if(_combo < maxCombo - 1)
            {
                for (int i = 0; i < _combo; i++)
                {
                    // Rotate the velocity randomly by 30 degrees at max.
                    Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(24));
                    newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                    Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
                }
            } 
            else if (_combo == maxCombo - 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterboundArrow"), player.position);
                type = ModContent.ProjectileType<IcelockArrow>();
                Projectile.NewProjectile(source, position, velocity, type, damage * 2, knockback, player.whoAmI);
            }
    

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<IllurineScale>(), 18);
            recipe.AddIngredient(ItemID.Ectoplasm, 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
