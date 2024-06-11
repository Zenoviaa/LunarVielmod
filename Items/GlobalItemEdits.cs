using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items
{
    internal class GlobalItemEdits : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);
            if(entity.type == ItemID.SDMG)
            {
                entity.damage -= 12;
            } 
            else if (entity.type == ItemID.Celeb2)
            {
                entity.damage -= 20;
            }
            else if (entity.type == ItemID.EmpressBlade)
            {
                entity.damage -= 30;
            }
            else if (entity.type == ItemID.LastPrism)
            {
                entity.damage -= 56;
            } 
            else if (entity.type == ItemID.Meowmere)
            {
                entity.damage -= 40;
            }
            else if (entity.type == ItemID.LunarFlareBook)
            {
                entity.damage -= 40;
            } 
            else if (entity.type == ItemID.StardustDragonStaff)
            {
                entity.damage -= 20;
            }

            else if (entity.type == ItemID.SparkleGuitar)
            {
                entity.damage -= 20;
            }
        }

        public override void UpdateEquip(Item item, Player player)
        {
            base.UpdateEquip(item, player);
            if (item.type == ItemID.VortexHelmet)
            {
                player.GetDamage(DamageClass.Ranged) -= 0.08f;
            }
            else if (item.type == ItemID.VortexBreastplate)
            {
                player.GetDamage(DamageClass.Ranged) -= 0.06f;
                player.GetCritChance(DamageClass.Ranged) -= 0.06f;
            }
            else if (item.type == ItemID.VortexLeggings)
            {
                player.GetDamage(DamageClass.Ranged) -= 0.04f;
            }

            if (item.type == ItemID.StardustHelmet)
            {
                player.GetDamage(DamageClass.Summon) -= 0.08f;
            }
            else if (item.type == ItemID.StardustBreastplate)
            {
                player.GetDamage(DamageClass.Summon) -= 0.06f;
            }
            else if (item.type == ItemID.StardustLeggings)
            {
                player.GetDamage(DamageClass.Summon) -= 0.04f;
            }
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            base.UpdateArmorSet(player, set);
            if (player.vortexStealthActive)
            {
                player.GetDamage(DamageClass.Ranged) -= 0.4f;
                player.GetCritChance(DamageClass.Ranged) -= 0.15f;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (item.type == ItemID.VortexHelmet)
            {
                TooltipLine line = new TooltipLine(Mod, "Nerf", "Lunar Veil: 8% decreased ranged damage")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(line);
                TooltipLine setBonusNerf = new TooltipLine(Mod, "Nerf3", "Lunar Veil: Stealth is MUCH less effective")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(setBonusNerf);
            }
            else if (item.type == ItemID.VortexBreastplate)
            {
                TooltipLine line = new TooltipLine(Mod, "Nerf", "Lunar Veil: 6% decreased ranged damage")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(line);
                line = new TooltipLine(Mod, "Nerf2", "Lunar Veil: 6% decreased ranged critical strike chance")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(line);
                TooltipLine setBonusNerf = new TooltipLine(Mod, "Nerf3", "Lunar Veil: Stealth is MUCH less effective")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(setBonusNerf);
            }
            else if (item.type == ItemID.VortexLeggings)
            {
                TooltipLine line = new TooltipLine(Mod, "Nerf", "Lunar Veil: 4% decreased ranged damage")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(line);
                TooltipLine setBonusNerf = new TooltipLine(Mod, "Nerf3", "Lunar Veil: Stealth is MUCH less effective")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(setBonusNerf);
            }

            if (item.type == ItemID.NebulaHelmet)
            {
                TooltipLine setBonusNerf = new TooltipLine(Mod, "Nerf3", "Lunar Veil: Damage boosters are 50% less effective")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(setBonusNerf);
            }
            else if (item.type == ItemID.NebulaBreastplate)
            {
                TooltipLine setBonusNerf = new TooltipLine(Mod, "Nerf3", "Lunar Veil: Damage boosters are 50% less effective")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(setBonusNerf);
            }
            else if (item.type == ItemID.NebulaLeggings)
            {
                TooltipLine setBonusNerf = new TooltipLine(Mod, "Nerf3", "Lunar Veil: Damage boosters are 50% less effective")
                {
                    OverrideColor = new Color(244, 200, 255)
                };
                tooltips.Add(setBonusNerf);
            }
        }
    }
}
