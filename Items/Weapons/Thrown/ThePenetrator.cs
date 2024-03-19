using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Helpers;
using Stellamod.Items.Accessories;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Swords;
using Stellamod.Projectiles.Swords.Ripper;
using Stellamod.Projectiles.Thrown;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Items.Weapons.Thrown
{
    internal class ThePenetrator : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 96;
            Item.height = 96;
            Item.DamageType = DamageClass.Throwing;
            Item.damage = 42;
            Item.useTime = 100;
            Item.useAnimation = 100;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = 10000;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = false;
            Item.UseSound = SoundID.Item1;
    
            Item.shoot = ModContent.ProjectileType<ThePenetratorProj>();
            Item.shootSpeed = 1f;
            Item.noUseGraphic = true;
            Item.value = Item.buyPrice(0, 30, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
     
        }

        public override void AddRecipes()
        {
            CreateRecipe(1)
                .AddIngredient(ItemID.MoltenFury, 1)
                .AddIngredient(ModContent.ItemType<MiracleThread>(), 15)
                .AddCondition(CustomConditions.SewingKitEquipped)
                .Register();
        }
    }
}
