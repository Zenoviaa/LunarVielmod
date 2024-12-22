using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles.Summons;
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
            Item.width = 26; // Width of the item
            Item.height = 22; // Height of the item
            Item.value = Item.sellPrice(gold: 5); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 12; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 3;
            player.endurance += 0.04f;
            player.maxMinions += 1;
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
            player.setBonus = LangText.SetBonus(this);//"Summons a turret over your head to shoot a laser at nearby enemies!\n8% increased magic and summon damage");
            player.GetDamage(DamageClass.Summon) += 0.08f;
            player.GetDamage(DamageClass.Magic) += 0.08f;
            player.GetModPlayer<ScrappyPlayer>().hasSetBonus = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<ScrappyGunProj>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<ScrappyGunProj>(), 70, 4, player.whoAmI);
            }
        }
    }
}
