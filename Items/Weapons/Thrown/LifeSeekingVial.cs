
using Microsoft.Xna.Framework;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Paint;
using Stellamod.Projectiles.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Stellamod.Items.Weapons.Thrown
{
    public class LifeSeekingVial : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Cactius"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Item.damage = 4;
            Item.DamageType = DamageClass.Throwing;
            Item.width = 40;
            Item.noUseGraphic = true;
            Item.height = 40;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
            Item.crit = 30;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<LifeSeekingVialProj>();
            Item.shootSpeed = 12f;
            Item.rare = ItemRarityID.Blue;
        }


    }
}