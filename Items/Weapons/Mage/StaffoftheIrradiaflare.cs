using Stellamod.Projectiles.Spears;
using System;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Spears;
using Stellamod.Projectiles.Magic;

namespace Stellamod.Items.Weapons.Mage
{
    internal class StaffoftheIrradiaflare : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Staff of the Irradiaflare");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.staff[Item.type] = true;
            Item.damage = 31;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 35;
            Item.autoReuse = true;

            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<AcidFlameG>();
            Item.shootSpeed = 15f;
            Item.mana = 10;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }



    }
}
