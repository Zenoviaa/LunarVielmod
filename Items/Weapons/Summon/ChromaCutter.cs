using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Projectiles.Summons.Minions;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    internal class ChromaCutter : ModItem
    {
        private int _chromaCounter = 0;
        public int AttackCounter = 1;
        public int combowombo = 0;
        public override void SetDefaults()
        {
            Item.width = 72;
            Item.height = 98;
            Item.damage = 71;
            Item.DamageType = DamageClass.Summon;
            Item.mana = 5;
            Item.useAnimation = 8;
            Item.useTime = 8;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.value = Item.sellPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.LightPurple;
            Item.noUseGraphic = true;
            Item.shootSpeed = 20f;
            Item.buffType = ModContent.BuffType<ChromaCutterMinionBuff>();
            Item.shoot = ModContent.ProjectileType<ChromaCutterProj>();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "", "");

            line = new TooltipLine(Mod, "R", "Red - No Effect")
            {
                OverrideColor = Color.DarkRed
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "O", "Orange - Explodes")
            {
                OverrideColor = Color.Orange
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "Y", "Yellow - Teleports after hitting an enemy")
            {
                OverrideColor = Color.Yellow
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "G", "Green - High damage")
            {
                OverrideColor = Color.Green
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "C", "Cyan - Causes several debuffs")
            {
                OverrideColor = Color.Cyan
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "B", "Blue - Homing")
            {
                OverrideColor = Color.Blue
            };

            tooltips.Add(line);

            line = new TooltipLine(Mod, "V", "Purple - Summons powerful blades upon hitting an enemy")
            {
                OverrideColor = Color.Purple
            };

            tooltips.Add(line);

        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.GetModPlayer<MyPlayer>().SwordCombo >= 0)
            {
                type = ModContent.ProjectileType<ChromaCutterProj>();
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
            {
                bool doSummonMinions = player.ownedProjectileCounts[ModContent.ProjectileType<ChromaCutterMinionProj>()] == 0;
      
                if (doSummonMinions)
                {
                    player.AddBuff(Item.buffType, 2);
                    // Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
                    // 7 Minions
                    position = Main.MouseWorld;

                    float remainingSlots = player.maxMinions - player.slotsMinions;

                    for (int i = 0; i < 7; i++)
                    {
                        var projectile = Projectile.NewProjectileDirect(source, position, velocity,
                            ModContent.ProjectileType<ChromaCutterMinionProj>(), damage, knockback, player.whoAmI, ai0: i);
                        if(i == 0)
                        {
                            projectile.minionSlots = remainingSlots;
                        }

                        projectile.originalDamage = Item.damage + (int)(9 * remainingSlots);
                    }

                    return false;
                }
                else
                {
                    int dir = AttackCounter;
                    if (player.direction == 1)
                    {
                        player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter;
                    }
                    else
                    {
                        player.GetModPlayer<CorrectSwing>().SwingChange = AttackCounter * -1;

                    }
                    AttackCounter = -AttackCounter;
                    Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 1, dir);

                    int chromaCount = 0;
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (proj.owner == player.whoAmI && 
                            proj.type == ModContent.ProjectileType<ChromaCutterMinionProj>())
                        {
                            if (_chromaCounter == chromaCount)
                            {
                                if (Vector2.Distance(player.Center, proj.Center) > 128)
                                {
                                    _chromaCounter++;
                                    chromaCount++;
                                    continue;
                                }

                                //Make it attack
                                proj.ai[1] = 1;
                                proj.netUpdate = true;
                                _chromaCounter++;
                                break;
                            }
                            else
                            {
                                chromaCount++;
                            }
                        }
                    }

                    if (_chromaCounter >= 7)
                        _chromaCounter = 0;

                    switch (Main.rand.Next(0, 2))
                    {
                        case 0:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg"), player.position);
                            break;
                        case 1:
                            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/AssassinsKnifeProg2"), player.position);
                            break;
                    }

                    return false;
                }
            }

            return false;
        }
    }
}
