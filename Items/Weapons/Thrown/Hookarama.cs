using Stellamod.Projectiles.Thrown;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Thrown
{
    internal class Hookarama : ModItem
    {
        public override void SetDefaults()
        {
            Item.noUseGraphic = true;
            Item.autoReuse = true;

            Item.damage = 21;
            Item.knockBack = 6;
            Item.crit = 4;

            Item.width = 74;
            Item.height = 74;
            Item.DamageType = DamageClass.Generic;

            Item.value = 10000;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.shootSpeed = 60;
            Item.shoot = ModContent.ProjectileType<HookaramaProj>();
            Item.rare = ItemRarityID.Green;
        }
    }
}
