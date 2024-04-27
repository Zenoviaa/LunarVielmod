using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class TheTingler : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 14;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 50;
            Item.height = 24;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock");
            Item.autoReuse = true;
            Item.shootSpeed = 19f;
            Item.shoot = ModContent.ProjectileType<CogNeedle>();
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {


            //Dust Burst Towards Mouse

            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1.2f, -0.1f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 7; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
            return base.Shoot(player, source, position, velocity, type, damage, knockback);

        }
        /*
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<MetallicOmniSource>(), 10);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 10);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
            recipe.AddIngredient(ItemID.Spike, 100);
            recipe.Register();
        }*/
    }
}
