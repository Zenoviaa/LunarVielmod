using Stellamod.Projectiles.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Stellamod.Projectiles.Slashers.Swingers;
using Microsoft.Xna.Framework;
using Stellamod.Helpers;

namespace Stellamod.Items.Weapons.Melee
{
    internal class VulcanBreaker : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Throwing;

        public override void SetClassSwappedDefaults()
        {

        }

        private int _dir = 1;
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Meatballs" +
				"\nDo not be worried, this mushes reality into bit bits and then shoots it!" +
				"\nYou can never miss :P"); */
            // DisplayName.SetDefault("Teraciz");

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.scale = 1f;
            Item.rare = ItemRarityID.Purple;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            SoundStyle soundStyle = SoundRegistry.QuickHit;
            soundStyle.Pitch = -0.5f;
            soundStyle.PitchVariance = 0.2f;
            Item.UseSound = soundStyle;

            // Weapon Properties
            Item.DamageType = DamageClass.Melee;
            Item.damage = 1000;
            Item.knockBack = 0;
            Item.noMelee = true;
            Item.noUseGraphic = true;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<VulcanBreakerSwingProj>();
            Item.shootSpeed = 5;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(2f, -2f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            _dir = -_dir;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: _dir);
            return false;
        }
    }
}
