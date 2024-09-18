using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Alcalite
{
    [AutoloadEquip(EquipType.Head)]
    internal class AlcaliteMask : ModItem
    {
        private int _starTimer;
        public override void SetDefaults()
        {
            Item.width = 40; // Width of the item
            Item.height = 34; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime;// The rarity of the item
            Item.defense = 5; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.08f;
            player.GetDamage(DamageClass.Summon) += 0.08f;
            player.maxMinions += 1;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = LangText.SetBonus(this);//"\n+1 max minions" + "\nThe stars of Illuria protect you!");

            player.maxMinions += 1;
            player.GetModPlayer<MyPlayer>().HasAlcaliteSet = true;

            //Make the thing
            if (player.ownedProjectileCounts[ModContent.ProjectileType<IlluriaStarGlow>()] == 0)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<IlluriaStarGlow>(), 0, 0, player.whoAmI);
            }

            _starTimer--;
            if (_starTimer <= 0)
            {
                int damage = 90;
                int knockback = 1;

                for (int i = 0; i < Main.rand.Next(2, 5); i++)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                                ModContent.ProjectileType<IlluriaStarProjBlue>(), damage, knockback, player.whoAmI);
                            break;
                        case 1:
                            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                               ModContent.ProjectileType<IlluriaStarProjCyan>(), damage, knockback, player.whoAmI);
                            break;
                        case 2:
                            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                               ModContent.ProjectileType<IlluriaStarProjYellow>(), damage, knockback, player.whoAmI);
                            break;
                    }
                }

                for (int i = 0; i < 12; i++)
                {
                    Vector2 speed = Main.rand.NextVector2CircularEdge(8f, 8f);
                    ParticleManager.NewParticle<StarParticle2>(player.Center, speed, Color.White, 0.5f);
                }
                _starTimer = 75;
            }
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AlcaliteRobe>()
                && legs.type == ModContent.ItemType<AlcaliteTrunks>();
        }
    }
}
