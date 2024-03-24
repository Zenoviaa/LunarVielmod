using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Spears;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    internal class HolmbergScythe : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Throwing;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.damage = 60;
            Item.useAnimation = 25;
            Item.useTime = 25;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 48;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;
            Item.shoot = ModContent.ProjectileType<HolmbergScytheProj>();
            Item.shootSpeed = 9f;
            Item.useAnimation = 25;
            Item.useTime = 45;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }
}
