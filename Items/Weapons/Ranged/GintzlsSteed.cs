using Stellamod.Projectiles.Spears;
using System;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Spears;
using Stellamod.Projectiles.Bow;
using Terraria.Audio;
using Terraria.DataStructures;
using Stellamod.Items.Ores;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class GintzlsSteed : ModItem
    {
        public int WinterboundArrow;
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = 3;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 12;
            Item.useTime = 4; // one third of useAnimation
            Item.reuseDelay = 14;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<GintzlMetal>(), 11);
            recipe.AddTile(TileID.HeavyWorkBench);
            recipe.Register();
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }
    }
}
