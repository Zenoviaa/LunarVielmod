using Microsoft.Xna.Framework;
using Stellamod.Content.CommonMaterials;
using Stellamod.Helpers;
using Stellamod.Items.Harvesting;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    public class Combuster : ModItem
    {
        private int _combo;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20;
            Item.height = 54;
            Item.damage = 9;
            Item.knockBack = 8;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 25;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<CombusterSparkProj1>();
            Item.shootSpeed = 5;
            Item.rare = ItemRarityID.LightRed;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(8f, -8f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
  

            int slowdown = 6;
            int maxCombo = 15;
            if (_combo == maxCombo)
            {
                type = ModContent.ProjectileType<CombusterSparkProj3>();
                Item.useTime /= slowdown;
                Item.useAnimation /= slowdown;
            }
            else if (_combo == maxCombo - 1)
            {
                type = ModContent.ProjectileType<CombusterSparkProj2>();
                Item.useTime *= slowdown;
                Item.useAnimation *= slowdown;
            }
            else
            {
                bool alternate = _combo % 2 == 0;
                type = alternate ? ModContent.ProjectileType<CombusterSparkProj1>() : ModContent.ProjectileType<CombusterSparkProj2>();
            }

            _combo++;
            if (_combo >= maxCombo + 1)
                _combo = 0;

            Vector2 targetPosition = Main.MouseWorld;
            if (Collision.CanHitLine(player.Center, 1, 1, targetPosition, 1, 1))
            {
                position = targetPosition;
            } 
            else
            {
                float length = ProjectileHelper.PerformBeamHitscan(player.Center, velocity, 1024);
                position = player.Center + velocity.SafeNormalize(Vector2.Zero) * length;
            }
            velocity = Vector2.Zero;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankJuggler>(), material: ModContent.ItemType<Cinderscrap>());
        }
    }
}
