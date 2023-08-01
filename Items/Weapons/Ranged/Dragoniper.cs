using Terraria.DataStructures;

using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Weapons.Gun;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Dragoniper : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Dragoniper"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
        }

        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = 5;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = 2;
            Item.UseSound = SoundID.Item92;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<DragonBolt>();
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Bullet;
            Item.UseSound = SoundID.Item92;
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-7, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet) type = ModContent.ProjectileType<DragonBolt>();
            Vector2 Offset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 20f;
            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }
 
            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, Item.damage, Item.knockBack, Item.playerIndexTheItemIsReservedFor, 0, 0);
            proj.netUpdate = true;
            for (int index1 = 0; index1 < 19; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, 226, velocity.X, velocity.Y, (int)byte.MaxValue, new Color(), (float)Main.rand.Next(6, 10) * 0.1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 0.5f;
                Main.dust[index2].scale *= 1.2f;
            }
            return false;
        }

	}
}
