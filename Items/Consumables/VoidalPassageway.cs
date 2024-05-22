using Stellamod.NPCs.Bosses.INest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Stellamod.NPCs.Bosses.SupernovaFragment;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles;

namespace Stellamod.Items.Consumables
{
    internal class VoidalPassageway : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Electronic Death Remote (EDR)");
            // Tooltip.SetDefault("'that big red button probably will do something you’ll regret... \n Your conscience advises you to press it and see what happens!'");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 28;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Lime;
            Item.shoot = ModContent.ProjectileType<MagicDoor>();
            Item.shootSpeed = 1;
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SupernovaFragment>()))
                return false;
            if (player.ownedProjectileCounts[type] > 0)
                return false;
            Projectile.NewProjectile(source, player.Center + new Vector2(0, -128), velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
