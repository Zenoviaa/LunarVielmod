using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Projectiles;
using Stellamod.UI.Systems;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.AlcadChests
{
    internal class LittleWand : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.rare = ItemRarityID.Green;
            Item.mana = 200;
            Item.useAnimation = 100;
            Item.useTime = 100;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = new SoundStyle($"Stellamod/Assets/Sounds/StormDragon_LightingZap");
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            // Here we add a tooltipline that will later be removed, showcasing how to remove tooltips from an item
            var line = new TooltipLine(Mod, "Oopsie", "Use with caution...")
            {
                OverrideColor = new Color(254, 128, 10)
            };
            tooltips.Add(line);

            line = new TooltipLine(Mod, "Disastrous", "May cause disaster")
            {
                OverrideColor = new Color(198, 124, 225)
            };
            tooltips.Add(line);
        }


        public override bool? UseItem(Player player)
        {
            ShakeModSystem.Shake = 5;
            if (Main.rand.NextBool(4))
            {
                CastMagic(player);
            }
            else
            {
                Bad(player);
            }

            return true;
        }

        private void CastMagic(Player player)
        {
            switch(Main.rand.Next(0, 5))
            {
                case 0:
                    for (int i = 0; i < Main.rand.Next(4, 7); i++)
                    {
                        Vector2 velocity = new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 5));
                        int p = Projectile.NewProjectile(player.GetSource_FromThis(), player.position, velocity, 
                            ProjectileID.Typhoon, 200, 1, player.whoAmI);
                        Main.projectile[p].timeLeft = 600;
                    }
                    SoundEngine.PlaySound(SoundID.Item84, player.position);
                    break;
                case 1:
                    for (int i = 0; i < Main.rand.Next(4, 7); i++)
                    {
                        Vector2 velocity = new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
                        int p = Projectile.NewProjectile(player.GetSource_FromThis(), player.position, velocity, 
                            ProjectileID.InfernoFriendlyBolt, 200, 1, player.whoAmI);
                        Main.projectile[p].timeLeft = 600;
                    }
                    SoundEngine.PlaySound(SoundID.Item73, player.position);
                    break;
                case 2:
                    for (int i = 0; i < Main.rand.Next(4, 7); i++)
                    {
                        Vector2 velocity = new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
                        int p = Projectile.NewProjectile(player.GetSource_FromThis(), player.position, velocity, 
                            ProjectileID.LostSoulFriendly, 200, 1, player.whoAmI);
                        Main.projectile[p].timeLeft = 600;
                    }
                    SoundEngine.PlaySound(SoundID.Item43, player.position);
                    break;
                case 3:
                    for (int i = 0; i < Main.rand.Next(4, 7); i++)
                    {
                        Vector2 velocity = new Vector2(Main.rand.Next(-10, 10), Main.rand.Next(-10, 10));
                        int p = Projectile.NewProjectile(player.GetSource_FromThis(), player.position, velocity, 
                            ProjectileID.FairyQueenMagicItemShot,
                            200, 1, player.whoAmI);
                        Main.projectile[p].timeLeft = 600;
                    }
                    SoundEngine.PlaySound(SoundID.Item82, player.position);
                    break;
                case 4:
                    player.statLife = player.statLifeMax;
                    SoundEngine.PlaySound(SoundID.Item3, player.position);
                    break;
            }
        }

        private void Bad(Player player)
        {
            //goliath jellyfish
            switch (Main.rand.Next(0, 4))
            {
                case 0:
                    player.AddBuff(BuffID.ShadowFlame, 1000);
                    player.AddBuff(BuffID.OnFire3, 1000);
                    player.AddBuff(BuffID.Frostburn2, 1000);
                    player.AddBuff(BuffID.Daybreak, 1000);
                    SoundEngine.PlaySound(SoundID.PlayerHit, player.position);
                    break;
                case 1:
                    int[] bad = new int[] {
                        NPCID.FireImp,
                        NPCID.Hellbat,
                        NPCID.Pixie,
                        NPCID.Gastropod
                    };

                    for(int i = 0; i < 10; i++)
                    {
                        Vector2 spawn = new Vector2((int)player.Center.X, (int)player.Center.Y);
                        spawn += new Vector2(Main.rand.NextFloat(-64, 64), -128);
                        int npcType = bad[Main.rand.Next(0, bad.Length)];
                        NPC.NewNPC(player.GetSource_FromThis(), (int)spawn.X, (int)spawn.Y, npcType);
                    }

                    SoundEngine.PlaySound(SoundID.Roar, player.position);
                    break;

                case 2:
                    player.statLife = 1;
                    SoundEngine.PlaySound(SoundID.PlayerKilled, player.position);
                    break;

                case 3:
                    if (Main.rand.NextBool(5))
                    {
                        int offsetX = Main.rand.Next(-10, 10) * 2;
                        int offsetY = Main.rand.Next(-500, 500) - 1700;
                        Projectile.NewProjectile(player.GetSource_FromThis(), player.Center.X + offsetX, player.Center.Y + offsetY, 0f, 10f,
                            ModContent.ProjectileType<AuroreanStarbomber>(), 0, 1, player.whoAmI);
                        Main.NewText(LangText.Misc("LittleWand"), 234, 96, 114);
                        SoundEngine.PlaySound(SoundID.AchievementComplete, player.position);
                    }
                    else
                    {
                        player.statLife = 1;
                        SoundEngine.PlaySound(SoundID.PlayerKilled, player.position);
                    }
          
                    break;
            }
        }

        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            base.ModifyManaCost(player, ref reduce, ref mult);
            reduce = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.maxMinions += 1;
        }
    }
}
