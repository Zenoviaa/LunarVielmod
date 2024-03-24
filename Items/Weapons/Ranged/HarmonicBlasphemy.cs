using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{

    public class HarmonicBlasphemy : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Wooden Crossbow"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
            /* Tooltip.SetDefault("Use a small crossbow and shoot three bolts!"
                + "\n'Triple Threat!'"); */
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;

        }


        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "HarmonicBlasphemy", "(A) Great stacking and holding damage, less immunity frames!")
            {
                OverrideColor = new Color(108, 271, 99)

            };
            tooltips.Add(line);
            base.ModifyTooltips(tooltips);
        }

        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 28;
            Item.mana = 4;
            Item.shootSpeed = 21;
        }

        public override void SetDefaults()
        {
            Item.damage = 20;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 32;
            Item.height = 25;
            Item.useTime = 12;
            Item.useAnimation = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2;
            Item.rare = ItemRarityID.Orange;
            Item.autoReuse = true;
            Item.shootSpeed = 13f;
            Item.shoot = ModContent.ProjectileType<VerlShot>();
            Item.noMelee = true; // The projectile will do the damage and not the item
            Item.value = Item.buyPrice(silver: 3);
            Item.noUseGraphic = false;
            Item.channel = true;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/HarmonicBlasphemy2");

        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8, 0);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            Vector2 muzzleOffset = Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 25f;
            if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
                position += muzzleOffset;
            Lighting.AddLight(player.position, r: 0.5f, g: 1f, b: 1.7f);
            for (int i = 0; i < 35; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                var d = Dust.NewDustPerfect(Main.LocalPlayer.Center, DustID.FrostStaff, speed * 6, Scale: 0.9f);
                ;
                d.noGravity = true;

                d.velocity *= 0.3f;
            }

            return true;
        }




    }
}