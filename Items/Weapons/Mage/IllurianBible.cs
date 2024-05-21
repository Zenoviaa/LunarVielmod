using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class IllurianBible : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 78;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 42;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 66;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(gold: 1);
            Item.shootSpeed = 10;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Lime;
            Item.mana = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<IllurianBibleProj>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, -velocity, type, damage, knockback, player.whoAmI);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
