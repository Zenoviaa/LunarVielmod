using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Projectiles.Summons.Minions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    public class FireflyStaff : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 15;
            Item.mana = 10;
        }
        private LilFly.AIState _state;
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.knockBack = 3f;
            Item.mana = 10;
            Item.width = 48;
            Item.height = 72;
            Item.useTime = 36;
            Item.useAnimation = 36;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.LightRed;

            // These below are needed for a minion weapon
            Item.noMelee = true;
            Item.UseSound = SoundID.Item46;
            Item.DamageType = DamageClass.Summon;
            Item.buffType = ModContent.BuffType<FireflyMinionBuff>();
            Item.shoot = ModContent.ProjectileType<LilFly>();
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (_state == LilFly.AIState.Defense)
                {
                    _state = LilFly.AIState.Offense;
                    SoundEngine.PlaySound(SoundID.Item46, player.position);

                }
                else
                {
                    _state = LilFly.AIState.Defense;
                    SoundEngine.PlaySound(SoundID.Item43, player.position);
                }

                foreach (var proj in Main.ActiveProjectiles)
                {
                    if (proj.type != ModContent.ProjectileType<LilFly>())
                        continue;
                    if (proj.owner != player.whoAmI)
                        continue;
                    proj.ai[1] = (float)_state;
                    proj.netUpdate = true;
                }

                return true;
            }

            return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
            player.AddBuff(Item.buffType, 2);

            // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
            var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            projectile.originalDamage = Item.damage;
            projectile.ai[1] = (float)_state;
            projectile.netUpdate = true;
            // Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
            return false;
        }
    }
}
