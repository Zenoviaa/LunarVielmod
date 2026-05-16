using Microsoft.Xna.Framework;
using Stellamod.Common.ArmorRework;

using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Winterborn
{
    public class WinterbornPlayer : ModPlayer
    {
        private int _timer;
        public bool hasWinterbornSetBonus;
        public override void ResetEffects()
        {
            hasWinterbornSetBonus = false;
        }

        public override void PostUpdateEquips()
        {
            if (!hasWinterbornSetBonus)
                return;
            _timer--;
            if (_timer <= 0 && Player.ownedProjectileCounts[ModContent.ProjectileType<WinterbornIcicleProj>()] < 3 && Main.myPlayer == Player.whoAmI)
            {
                //Spawn one
                int damage = 15;
                int knockback = 2;
                float health = 100;
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<WinterbornIcicleProj>(), damage, knockback, Player.whoAmI, ai0: health);
                _timer = 60 * 10;
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class WinterbornHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            ArmorSetSystem.RegisterArmorSet<WinterbornHead, WinterbornBody, WinterbornLegs>(ArmorGroup.Act_I);
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            ArmorStatsPlayer stats = player.GetModPlayer<ArmorStatsPlayer>();
            stats.criticalStrikeDamage += 0.1f;
            stats.artifactManaReduction += 0.35f;
            stats.defenseBonus += 2;
            stats.accessorySlots++;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WinterbornBody>() && legs.type == ModContent.ItemType<WinterbornLegs>();
        }

        public override void UpdateArmorSet(Player player)
        { 
            player.GetModPlayer<WinterbornPlayer>().hasWinterbornSetBonus = true;
        }
    } 

    [AutoloadEquip(EquipType.Body)]
    public class WinterbornBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Blue; // The rarity of the item
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.magicDamage += 0.07f;
            stats.defenseBonus += 4;
            stats.accessorySlots++;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class WinterbornLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.totalMana += 40;
            stats.defenseBonus += 2;
            stats.accessorySlots++;
        }
    }
}
