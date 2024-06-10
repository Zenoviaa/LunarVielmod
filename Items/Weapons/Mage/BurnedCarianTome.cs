using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Mage
{
    internal class BurnedCarianTome : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 8;
            Item.mana = 6;
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 42;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 9;
            Item.knockBack = 3;
            Item.value = Item.sellPrice(gold: 1);
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Green;
            Item.mana = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.UseSound = SoundID.Item20;
            Item.shoot = ModContent.ProjectileType<BurnedCarianTomeProj>();
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }
    }
}
