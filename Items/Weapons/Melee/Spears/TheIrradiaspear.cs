using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Spears;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee.Spears
{
    internal class TheIrradiaspear : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 40;
          
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 51;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.LightPurple;
            Item.shootSpeed = 15;
            Item.autoReuse = false;
            Item.channel = true;

            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<TheIrradiaspearP>();
            Item.shootSpeed = 20f;
  
            Item.useAnimation = 20;
            Item.useTime = 20;
        }
    }
}
