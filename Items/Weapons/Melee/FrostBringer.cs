using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Stellamod.Projectiles.Slashers.FrostBringer;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{
    public class FrostBringer : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Damage reduces the farther you are away from the target");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 0, 16, 0);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.shoot = ModContent.ProjectileType<FrostBringerSlash>();
            Item.shootSpeed = 8;
            Item.DamageType = DamageClass.Melee;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            SwingPlayer swingPlayer = player.GetModPlayer<SwingPlayer>();
            int dir = swingPlayer.SwingDirection;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback,
                player.whoAmI, 0, dir);

            return false;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankSword>(), material: ModContent.ItemType<WinterbornShard>());
        }
    }
}