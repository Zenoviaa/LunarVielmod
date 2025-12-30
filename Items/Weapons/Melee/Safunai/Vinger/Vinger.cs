using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Items.Weapons.Melee.Safunai.Vinger
{
    public class Vinger : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToSafunai();
            Item.width = 32;
            Item.height = 36;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 18;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;
            Item.shoot = ModContent.ProjectileType<VingerProj>();
            Item.value = Item.sellPrice(gold: 10);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.damage = 35;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
