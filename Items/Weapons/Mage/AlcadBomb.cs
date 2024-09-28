using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class AlcadBomb : ClassSwapItem
    {
   
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 80;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 7));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true; // Makes the item have an animation while in world (not held.). Use in combination with RegisterItemAnimation
            ItemID.Sets.ItemNoGravity[Item.type] = true; // Makes the item have no gravity
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 150;
            Item.width = 50;
            Item.height = 78;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 5;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.shootSpeed = 15;
            Item.autoReuse = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
            Item.shoot = ModContent.ProjectileType<AlcadBombProj>();
            Item.mana = 5;
            Item.useAnimation = 3;
            Item.useTime = 3;
        //    Item.UseSound = SoundID.
            Item.channel = true;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-3f, -2f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.ownedProjectileCounts[type] > 0)
            {
                return false;
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
