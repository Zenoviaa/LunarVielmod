using Stellamod.Projectiles.Weapons.Spears;
using System;
using Stellamod.Projectiles.Weapons.Spears;
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
using Stellamod.Projectiles.Weapons.Spears;
using Stellamod.Projectiles.Weapons.Bow;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class WingedFury : ModItem
    {

        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 20f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 31;
            Item.useTime = 31;
            Item.consumeAmmoOnLastShotOnly = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5f, 0f);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextBool(3))
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Custom/Item/HeatFeather"), player.position);
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<HeatArrow>(), damage, knockback, player.whoAmI);
            }

            
            return true;
        }



    }
}
