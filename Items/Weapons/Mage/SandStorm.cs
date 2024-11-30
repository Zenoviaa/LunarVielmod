using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    public class SandStorm : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 12;
            Item.mana = 6;
        }

        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(gold: 10);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<Nado>();
            Item.shootSpeed = 10f;
            Item.mana = 8;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-2, 0);
        }
    }
}