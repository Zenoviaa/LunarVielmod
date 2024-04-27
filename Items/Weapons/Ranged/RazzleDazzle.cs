using Stellamod.Projectiles.Bow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class RazzleDazzle : ModItem
    {
        private int _combo;
        public override void SetDefaults()
        {
            Item.damage = 62;
            Item.width = 44;
            Item.height = 80;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Lime;

            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;

            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 16f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2f, 0f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            _combo++;
            if(_combo == 3)
            {
                SoundEngine.PlaySound(SoundID.Item78, position);
                type = ModContent.ProjectileType<RazzleDazzleProj>();
                _combo = 0;
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.PiOver4 / 7), type, damage, knockback, player.whoAmI);
            Projectile.NewProjectile(source, position, velocity.RotatedBy(-MathHelper.PiOver4 / 7), type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
