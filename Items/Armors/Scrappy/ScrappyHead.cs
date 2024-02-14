using Stellamod.Items.Armors.Miracle;
using Stellamod.Projectiles.Summons;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Scrappy
{
    internal class ScrappyPlayer : ModPlayer
    {
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            hasSetBonus = false;
        }
    }

    [AutoloadEquip(EquipType.Head)]
    internal class ScrappyHead : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 5); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 12; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 3;
            player.endurance += 0.04f;
            player.slotsMinions += 1;
            player.nightVision = true;
            player.GetDamage(DamageClass.Summon) += 0.08f;
            player.GetDamage(DamageClass.Magic) += 0.08f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ScrappyBody>()
                && legs.type == ModContent.ItemType<ScrappyLegs>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increases max life if you are under 200 health!" +
                "\nIncreased endurance and speed!";
            player.GetModPlayer<ScrappyPlayer>().hasSetBonus = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ScrappyGunProj>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<ScrappyGunProj>(), 70, 4, player.whoAmI);
            }
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
