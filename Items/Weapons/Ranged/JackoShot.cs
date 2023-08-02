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
using Stellamod.Projectiles.Bow;
using Terraria.Audio;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class JackoShot : ModItem
    {
        public override void SetDefaults()
        {
            Item.damage = 11;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;

            Item.shootSpeed = 13;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.consumeAmmoOnLastShotOnly = true;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }


    }
}
