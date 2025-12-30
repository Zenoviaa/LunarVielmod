using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Safunai.Rinavine
{
    public class Rinavine : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToSafunai();
            Item.width = 16;
            Item.height = 16;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = Item.useAnimation = 18;
            Item.shootSpeed = 1f;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item116;
            Item.shoot = ModContent.ProjectileType<RinavineProj>();
            Item.value = Item.sellPrice(gold: 10);
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.damage = 76;
            Item.rare = ItemRarityID.LightRed;
        }
    }
}
