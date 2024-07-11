using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class RustedSniper : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 134;
            Item.height = 38;
            Item.rare = ItemRarityID.LightRed;
            Item.damage = 122;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 288;
            Item.useAnimation = 288;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<RustedSnipe>();
            Item.shootSpeed = 10f;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item40;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);

            //Dust Burst Towards Mouse

            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(3.2f, -0.1f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 15; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>());
            recipe.AddIngredient(ModContent.ItemType<BrokenTech>(), 8);
            recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 2);
            recipe.AddIngredient(ItemID.Musket, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ModContent.ItemType<BasicGunParts>());
            recipe2.AddIngredient(ModContent.ItemType<BrokenTech>(), 8);
            recipe2.AddIngredient(ModContent.ItemType<WeaponDrive>(), 2);
            recipe2.AddIngredient(ItemID.TheUndertaker, 1);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
