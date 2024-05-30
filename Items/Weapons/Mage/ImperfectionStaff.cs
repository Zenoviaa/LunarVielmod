
using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Stellamod.Projectiles.Magic;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Mage
{
    public class ImperfectionStaff : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Summon;

        //Defaults for the other class
        public override void SetClassSwappedDefaults()
        {
            //Do if(IsSwapped) if you want to check for the alternate class
            //Stats to have when in the other class
            Item.damage = 102;
            Item.knockBack = 3;
            Item.mana = 4;
            Item.useTime = 45;
            Item.useAnimation = 45;
        }
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Spinny Winny damage the binny");

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {

            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");
            line = new TooltipLine(Mod, "Perfectionss", "(A) Extremely good for targeting! Needs an enemy alive to work!")
            {
                OverrideColor = new Color(220, 87, 24)

            };
            tooltips.Add(line);





            base.ModifyTooltips(tooltips);



        }

        public override void SetDefaults()
        {

            Item.damage = 55; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
            Item.DamageType = DamageClass.Magic;
            Item.width = 20; // hitbox width of the Item
            Item.height = 20; // hitbox height of the Item
            Item.useTime = 30; // The Item's use time in ticks (60 ticks == 1 second.)
            Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.staff[Item.type] = true;
            Item.noMelee = true; //so the Item's animation doesn't do damage
            Item.knockBack = 6; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
            Item.value = 10000; // how much the Item sells for (measured in copper)
            Item.rare = ItemRarityID.Orange; // the color that the Item's name will be in-game
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/GhostExcalibur1"); // The sound that this Item plays when used.
            Item.shoot = ModContent.ProjectileType<ImperfectionProj>();
            Item.shootSpeed = 2f; // the speed of the projectile (measured in pixels per frame)
            Item.channel = true;
            Item.mana = 6;

            Item.autoReuse = true;


        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float numberProjectiles = 6;
            float rotation = MathHelper.ToRadians(14);
            position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
            for (int i = 0; i < numberProjectiles; i++)
            {
                Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
                Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
            }
            return false;
        }

    }
}