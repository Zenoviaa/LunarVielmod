using Stellamod.Common.ArmorRework;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Lovestruck
{
    public class LovestruckPlayer : ModPlayer
    {
        public bool hasLovestruckSetBonus;
        public float lovestruckTimer;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasLovestruckSetBonus = false;
        }
        public override void PostUpdateMiscEffects()
        {
            base.PostUpdateMiscEffects();
            if(lovestruckTimer > 0)
            {
                Player.loveStruck = true;
                lovestruckTimer--;
            }
        }
        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            base.OnHitByNPC(npc, hurtInfo);
            if (hasLovestruckSetBonus)
            {
                lovestruckTimer = 30;
                npc.AddBuff(BuffID.Burning, 720);
                npc.AddBuff(BuffID.OnFire3, 720);
                npc.AddBuff(BuffID.Frostburn, 360);
                npc.AddBuff(BuffID.Confused, 720);
                npc.AddBuff(BuffID.ShadowFlame, 120);
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class LovestruckMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ArmorSetSystem.RegisterArmorSet<LovestruckMask, LovestruckBreastplate, LovestruckLegs>(ArmorGroup.Act_I);
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
            stats.defenseBonus += 4;
            stats.summonCastTime += 0.1f;
            stats.accessorySlots++;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<LovestruckBreastplate>() && legs.type == ModContent.ItemType<LovestruckLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<LovestruckPlayer>().hasLovestruckSetBonus = true;
      
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class LovestruckBreastplate : ModItem
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
            stats.defenseBonus += 5;
            stats.mainSummonDamage += 0.25f;
            stats.accessorySlots++;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class LovestruckLegs : ModItem
    {
        public override void SetStaticDefaults()
        {
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
            stats.defenseBonus += 4;
            stats.minionAggressiveness += 100;
        }
    }
}