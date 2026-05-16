using Stellamod.Common.ArmorRework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Astrasilk
{
    public class AstrasilkPlayer : ModPlayer
    {
        private int _hitCount;
        private float _cooldownTimer;
        public bool hasSetBonus;
        public override void ResetEffects()
        {
            hasSetBonus = false;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            if (!hasSetBonus)
            {
                _hitCount = 0;
            }
            _cooldownTimer--;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hasSetBonus)
                return;

            if (_cooldownTimer > 0)
                return;

            _hitCount++;
            if (_hitCount >= 5)
            {
                _cooldownTimer = 100;
                int starProjectile = ModContent.ProjectileType<AstrasilkGigaStarProj>();
                Vector2 spawnPoint = target.Top - new Vector2(0, 384);
                spawnPoint.X += Main.rand.NextFloat(-160, 160);
                Projectile.NewProjectile(Player.GetSource_FromThis(), spawnPoint, Vector2.UnitY, starProjectile, damageDone * 2, 1);
                _hitCount = 0;
            }
        }
    }

    [AutoloadEquip(EquipType.Head)]
    public class AstrasilkHead : ModItem
    {
        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
            ArmorSetSystem.RegisterArmorSet<AstrasilkHead, AstrasilkBody, AstrasilkLegs>(ArmorGroup.Act_I);
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
            var stats = player.GetStats();
            stats.defenseBonus += 4;
            stats.accessorySlots += 1;
            stats.wandNormalEnchantmentSlots += 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AstrasilkBody>()
                && legs.type == ModContent.ItemType<AstrasilkLegs>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.GetModPlayer<AstrasilkPlayer>().hasSetBonus = true;
        }


    }
    [AutoloadEquip(EquipType.Body)]
    public class AstrasilkBody : ModItem
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
            var stats = player.GetStats();
            stats.accessorySlots += 2;
            stats.magicDamage += 0.12f;
            stats.defenseBonus += 4;
        }


    }
    [AutoloadEquip(EquipType.Legs)]
    public class AstrasilkLegs : ModItem
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
            stats.totalMana += 50;
            stats.defenseBonus += 3;
            stats.accessorySlots += 1;
        }
    }
}
