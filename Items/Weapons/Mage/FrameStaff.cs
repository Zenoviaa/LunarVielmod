using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class FrameStaff : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 89;
            Item.mana = 6;
        }
        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.damage = 98;
            Item.DamageType = DamageClass.Magic;
            Item.knockBack = 1;
            Item.mana = 12;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.rare = ItemRarityID.Lime;
            Item.autoReuse = false;
            Item.UseSound = SoundID.DD2_BookStaffCast;
            Item.shoot = ModContent.ProjectileType<FrameStaffConnectorProj>();
            Item.shootSpeed = 0;
            Item.channel = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if(player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<FrameStaffNodeProj>();
            }
            else
            {
                Item.shoot = ModContent.ProjectileType<FrameStaffConnectorProj>();
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if(player.altFunctionUse == 2)
            {
                if (player.ownedProjectileCounts[type] < 10)
                {
                    Projectile.NewProjectile(player.GetSource_FromThis(), Main.MouseWorld, velocity, type, damage, knockback, player.whoAmI);
                }
        
                return false;
            }
            //

            
            position = Main.MouseWorld;
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
