using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    public class TorrentialLance : ClassSwapItem
	{
        public override DamageClass AlternateClass => DamageClass.Magic;
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }

        public override void SetClassSwappedDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 78;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            Item.damage = 54;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.useTime = 200;
            Item.useAnimation = 19;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.mana = 15;
            Item.value = Item.sellPrice(gold: 2);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item21;
            Item.shoot = ModContent.ProjectileType<TorrentialLanceProj>();
            Item.shootSpeed = 15f;
            Item.autoReuse = true;
        }
    }
}