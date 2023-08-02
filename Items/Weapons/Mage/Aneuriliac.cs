using Stellamod.Projectiles.Magic;
using Terraria.ModLoader;
using Terraria.Net;
using Terraria.GameContent.NetModules;
using Terraria.GameContent.Creative;
using Stellamod.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.Spears;
using Stellamod.Projectiles.Spears;
using System;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Stellamod.NPCs.Bosses.DreadMire.Heart;
using Terraria.DataStructures;
using Terraria.Audio;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Aneuriliac : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fungal Flace");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
     
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<DreadVomit>();
            Item.shootSpeed = 10f;
            Item.mana = 10;
            Item.useAnimation = 6;
            Item.useTime = 6;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }




    }
}
