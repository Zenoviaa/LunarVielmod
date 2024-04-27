using Microsoft.Xna.Framework;
using Stellamod.Dusts;
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
    internal class GearGutter : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 250;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 84;
            Item.height = 36;
            Item.useTime = 72;
            Item.useAnimation = 72;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(0, 15, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GallinLock2");
            Item.autoReuse = true;
            Item.shootSpeed = 50f;
            Item.shoot = ModContent.ProjectileType<GearSniper>();
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(player.Center, 1024f, 32f);

            //Dust Burst Towards Mouse

            float rot = velocity.ToRotation();
            float spread = 0.4f;

            Vector2 offset = new Vector2(1, -0.1f * player.direction).RotatedBy(rot);
            for (int k = 0; k < 15; k++)
            {
                Vector2 direction = offset.RotatedByRandom(spread);

                Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), direction * Main.rand.NextFloat(8), 125, new Color(150, 80, 40), Main.rand.NextFloat(0.2f, 0.5f));
            }
            Dust.NewDustPerfect(position + offset * 43, ModContent.DustType<Dusts.GlowDust>(), new Vector2(0, 0), 125, new Color(150, 80, 40), 1);
            Dust.NewDustPerfect(player.Center + offset * 43, ModContent.DustType<Dusts.TSmokeDust>(), Vector2.UnitY * -2 + offset.RotatedByRandom(spread), 150, new Color(60, 55, 50) * 0.5f, Main.rand.NextFloat(0.5f, 1));
            return base.Shoot(player, source, position, velocity, type, damage, knockback);

        }
    }

    /*
    public override void AddRecipes()
    {
        Recipe recipe = CreateRecipe();
        recipe.AddTile(TileID.MythrilAnvil);
        recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
        recipe.AddIngredient(ModContent.ItemType<ArnchaliteBar>(), 10);
        recipe.AddIngredient(ModContent.ItemType<MetallicOmniSource>(), 10);
        recipe.AddIngredient(ItemID.PhoenixBlaster, 1);
        recipe.AddRecipeGroup(nameof(ItemID.IronBar), 20);
        recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
        recipe.Register();
    }*/
}

