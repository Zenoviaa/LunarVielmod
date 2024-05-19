using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Stellamod.Projectiles.Swords;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Melee
{
    class SwordOfTheJungleGoddess :ClassSwapItem
    {

        public int dir;

    public override DamageClass AlternateClass => DamageClass.Magic;

    public override void SetClassSwappedDefaults()
    {
        Item.damage = 30;
        Item.mana = 3;
    }
    public override void SetStaticDefaults()
    {
        // DisplayName.SetDefault("Cinder Braker");
    }

    public override void SetDefaults()
    {
        Item.damage = 30;
        Item.DamageType = DamageClass.Melee/* tModPorter Suggestion: Consider MeleeNoSpeed for no attack speed scaling */;
        Item.width = 40;
        Item.height = 40;
        Item.useTime = 90;
        Item.useAnimation = 90;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.knockBack = 7;
        Item.value = Item.sellPrice(0, 3, 20, 14);
        Item.rare = ItemRarityID.Blue;
        Item.noUseGraphic = true;
        Item.autoReuse = true;
        Item.shoot = ModContent.ProjectileType<SOJGCustomSwingProjectile>();
        Item.shootSpeed = 8f;
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        // Using the shoot function, we override the swing projectile to set ai[0] (which attack it is)
        int Sound = Main.rand.Next(1, 3);

            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AcidBlast3>(), damage * 1, knockback, player.whoAmI, 1, dir);

            if (dir == -1)
        {
            dir = 1;
        }
        else if (dir == 1)
        {
            dir = -1;
        }
        else
        {
            dir = 1;
        }

        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, dir);
        return false; // return false to prevent original projectile from being shot
    }

}
}