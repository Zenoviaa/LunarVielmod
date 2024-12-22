using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class BrokenWrath : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;
        public int combo;
        public override void SetClassSwappedDefaults()
        {
            Item.damage = 11;
            Item.mana = 2;
        }
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Broken Wrath");
        }

        public override void SetDefaults()
        {
            Item.damage = 23;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BBBubble>();
            Item.shootSpeed = 4f;
            Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (type == ProjectileID.Bullet)
                type = ModContent.ProjectileType<BrokenMissile>();
            Vector2 Offset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y - 1)) * 20f;
            if (Collision.CanHit(position, 0, 0, position + Offset, 0, 0))
            {
                position += Offset;
            }

            velocity = velocity.RotatedByRandom(MathHelper.ToRadians(5));
            Projectile proj = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: combo);
            combo++;
            if (combo >= 2)
                combo = 0;

            for (int index1 = 0; index1 < 19; ++index1)
            {
                int index2 = Dust.NewDust(new Vector2(position.X, position.Y), Item.width - 20, Item.height - 45, DustID.CopperCoin, velocity.X, velocity.Y, byte.MaxValue, new Color(), Main.rand.Next(6, 10) * 0.1f);
                Main.dust[index2].noGravity = true;
                Main.dust[index2].velocity *= 0.5f;
                Main.dust[index2].scale *= 1.2f;
            }
            damage /= 2;

            //generate the remaining projectiles
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BrokenWrath2"), player.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/BrokenWrath1"), player.position);
            }

            Vector2 origVect = new Vector2(velocity.X, velocity.Y);
            Vector2 newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(72, 1300) / 23));
            Projectile.NewProjectile(source, position, newVect, ModContent.ProjectileType<BTech1>(), damage, knockback, player.whoAmI, 0f, 0f);
            newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(72, 1300) / 23));
            Projectile.NewProjectile(source, position, newVect, ModContent.ProjectileType<BTech2>(), damage, knockback, player.whoAmI, 0f, 0f);
            newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(72, 1300) / 23));
            Projectile.NewProjectile(source, position, newVect, ModContent.ProjectileType<BTech3>(), damage, knockback, player.whoAmI, 0f, 0f);
            newVect = origVect.RotatedBy(System.Math.PI / (Main.rand.Next(72, 1300) / 13));

            return false;
        }
    }
}