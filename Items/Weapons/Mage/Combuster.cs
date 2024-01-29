using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Combuster : ModItem
    {
        private int _combo;
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 54;
            Item.damage = 36;
            Item.knockBack = 4;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.sellPrice(gold: 1);
            Item.shoot = ModContent.ProjectileType<CombusterSparkProj1>();
            Item.rare = ItemRarityID.LightRed;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(8f, -8f);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            switch (_combo)
            {
                case 0:
                    type = ModContent.ProjectileType<CombusterSparkProj1>();
                    break;
                case 1:
                    type = ModContent.ProjectileType<CombusterSparkProj2>();
                    Item.useTime *= 3;
                    Item.useAnimation *= 3;
                    break;
                case 2:
                    type = ModContent.ProjectileType<CombusterSparkProj3>();
                    Item.useTime /= 3;
                    Item.useAnimation /= 3;
                    break;
            }
            _combo++;
            if (_combo >= 3)
                _combo = 0;
            position = Main.MouseWorld;
        }
    }
}
