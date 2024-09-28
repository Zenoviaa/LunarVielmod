using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Gun;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class Incinerator : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Melee;
        public override void SetClassSwappedDefaults()
        {
            Item.damage = 192;
        }

        public override void SetDefaults()
        {
            Item.width = 62;
            Item.height = 32;
            Item.rare = ItemRarityID.Purple;
            Item.useTime = 4;
            Item.useAnimation = 4;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;

            // Weapon Properties
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 200;
            Item.knockBack = 4;
            Item.noMelee = true;

            // Gun Properties
            Item.shoot = ModContent.ProjectileType<IncineratorProj>();
            Item.shootSpeed = 9.5f;
            // Restrict the type of ammo the weapon can use, so that the weapon cannot use other ammos
            Item.value = Item.sellPrice(gold: 25);
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        // This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-16, 0);
        }
    }
}
