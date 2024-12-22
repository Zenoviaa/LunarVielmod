using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class EelInfinite : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 12;
            Item.mana = 4;
        }

        public override void SetDefaults()
        {
            Item.width = 114;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
            Item.damage = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 4;
            Item.useTime = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<EelLightningBolt>();
            Item.shootSpeed = 1;
            Item.value = Item.sellPrice(gold: 2);
            Item.UseSound = SoundID.DD2_LightningAuraZap;
            Item.noMelee = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-24, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //Dust Burst Towards Mouse
            int count = 2;
            for (int k = 0; k < count; k++)
            {
                Vector2 newVelocity = (velocity * 20).RotatedByRandom(MathHelper.ToRadians(15));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Dust.NewDust(position, 0, 0, DustID.Electric, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
            }

            return true;
        }
    }
}
