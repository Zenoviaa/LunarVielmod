using Stellamod.Buffs.Minions;
using Stellamod.Common.ArmorRework;
using Stellamod.Projectiles.Summons.Minions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.HeavyMetal
{
    public class HeavyMetalPlayer : ModPlayer
    {
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSetBonus = false;
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            if (hasSetBonus && Player.ownedProjectileCounts[ModContent.ProjectileType<HMArncharMinionRightProj>()] == 0 && Main.myPlayer == Player.whoAmI)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/ArcharilitDrone3"), Player.position);
                var EntitySource = Player.GetSource_FromThis();

                int damage = 17;
                Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0,
                    ModContent.ProjectileType<HMArncharMinionRightProj>(), damage, 1, Player.whoAmI, 0, ai1: 1);
                Projectile.NewProjectile(EntitySource, Player.Center.X, Player.Center.Y, 0, 0,
                    ModContent.ProjectileType<HMArncharMinionRightProj>(), damage, 1, Player.whoAmI, 0, ai1: -1);
                Player.AddBuff(ModContent.BuffType<HMMinionBuff>(), 99999);
            }
            else if (!hasSetBonus)
            {
                Player.ClearBuff(ModContent.BuffType<HMMinionBuff>());
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class HeavyMetalHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ArmorSetSystem.RegisterArmorSet<HeavyMetalHead, HeavyMetalBody, HeavyMetalLegs>();
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HeavyMetalBody>() && legs.type == ModContent.ItemType<HeavyMetalLegs>();
        }

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            var stats = player.GetStats();
            stats.summonCastTime -= 0.5f;
            stats.defenseBonus += 5;
            stats.accessorySlots += 1;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<HeavyMetalPlayer>().hasSetBonus = true;
        }
    }

    [AutoloadEquip(EquipType.Body)]
    public class HeavyMetalBody : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateEquip(Player player)
        {
            base.UpdateEquip(player);
            var stats = player.GetStats();
            stats.defenseBonus += 5;
            stats.minionSlots += 2;
            stats.accessorySlots += 2;
        }
    }

    [AutoloadEquip(EquipType.Legs)]
    public class HeavyMetalLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            var stats = player.GetStats();
            stats.defenseBonus += 3;
            stats.minionSummonHealth += 0.5f;
            stats.accessorySlots += 1;
        }
    }
}
