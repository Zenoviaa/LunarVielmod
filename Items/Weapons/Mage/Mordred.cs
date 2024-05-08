using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Spears;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Mordred : ModItem
    {
        public override void SetStaticDefaults()
        {
     
             // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
     

        public override void SetDefaults()
        {
            Item.damage = 40;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 8;
            Item.value = Item.sellPrice(0, 29, 50, 0);
            Item.rare = ModContent.RarityType<SirestiasSpecialRarity>();
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<MordredProj>();
            Item.shootSpeed = 20f;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }
}
