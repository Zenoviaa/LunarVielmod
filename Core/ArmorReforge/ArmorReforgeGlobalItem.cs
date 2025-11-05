using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Core.GunSystem;
using Stellamod.Core.IgnitersNPowders;
using Stellamod.Core.XixianFlaskSystem;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.ArmorReforge
{
    public class ContactDamageReductionPlayer : ModPlayer
    {
        public float contactEndurance;
        public override void ResetEffects()
        {
            base.ResetEffects();
            contactEndurance = 0f;
        }
        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            base.ModifyHitByNPC(npc, ref modifiers);
            modifiers.FinalDamage *= (1.0f - contactEndurance);
        }
    }
    public class HealBoostPlayer : ModPlayer
    {
        public float healBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            healBonus = 0f;
        }
        public override void GetHealLife(Item item, bool quickHeal, ref int healValue)
        {
            base.GetHealLife(item, quickHeal, ref healValue);
            float boost = 1f + healBonus;
            float healFloat = healValue;
            healFloat *= boost;
            healValue = (int)healFloat;
        }
    }

    public class FeatheredPlayer : ModPlayer
    {
        public float gravityLossBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            gravityLossBonus = 0;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            float gravityLoss = 1f - gravityLossBonus;
            Player.gravity *= gravityLoss;
        }
    }
    public class WingTimeMaxPlayer : ModPlayer
    {
        public float wingTimeMaxBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            wingTimeMaxBonus = 0f;
        }
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            float wingTimeMax = Player.wingTimeMax;
            float bonus = 1f + wingTimeMaxBonus;
            wingTimeMax *= bonus;
            Player.wingTimeMax = (int)wingTimeMax;
        }
    }

    public class VampiricArmorPlayer : ModPlayer
    {
        public float lifeSteal;
        public float cooldownTimer;
        public override void ResetEffects()
        {
            base.ResetEffects();
            lifeSteal = 0;
        }

        public override void PostUpdate()
        {
            base.PostUpdate();
            cooldownTimer--;
            if (cooldownTimer <= 0f)
                cooldownTimer = 0f;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (lifeSteal <= 0)
                return;
            if (cooldownTimer > 0)
                return;
            cooldownTimer = 30;
            Player.Heal((int)lifeSteal);
        }

    }

    public class AccessoryReforgeGlobalItem : GlobalItem
    {
        public AccessoryReforgeType accessoryReforgeType;
        public override bool InstancePerEntity => true;
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            base.UpdateAccessory(item, player, hideVisual);
            switch (accessoryReforgeType)
            {
                default:
                    break;
                case AccessoryReforgeType.Hearty:
                    player.statLifeMax2 += 5;
                    break;
                case AccessoryReforgeType.Stalled:
                    player.endurance += 0.04f;
                    break;
                case AccessoryReforgeType.Grimming:
                    player.lifeRegen += 1;
                    break;
                case AccessoryReforgeType.Mortified:
                    player.GetModPlayer<AdvancedMagicPlayer>().chargeTimeBonus += 0.05f;
                    break;
                case AccessoryReforgeType.Hidden:
                    player.aggro = (int)(player.aggro * 0.95f);
                    break;
                case AccessoryReforgeType.Flashing:
                    player.GetModPlayer<DashPlayer>().MaxDashCount += 1;
                    break;
                case AccessoryReforgeType.Powding:
                    player.GetModPlayer<IgniterPlayer>().extenderBonus += 0.1f;
                    break;
                case AccessoryReforgeType.Exploding:
                    player.GetModPlayer<IgniterPlayer>().igniterDamageBonus += 0.05f;
                    break;
                case AccessoryReforgeType.Demolighting:
                    player.GetModPlayer<GunHoldPlayer>().maxAmmoBonus += 2;
                    break;
                case AccessoryReforgeType.Slashing:
                    player.GetModPlayer<ContactDamageReductionPlayer>().contactEndurance += 0.06f;
                    break;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (accessoryReforgeType == AccessoryReforgeType.None)
                return;

            TooltipLine itemNameLine = tooltips.Find(x => x.Name == "ItemName");
            itemNameLine.Text = LangText.AccessoryReforge(accessoryReforgeType, "DisplayName") + " " + itemNameLine.Text;
            /*TooltipLine line = new TooltipLine(Mod, "ReforgeDisplayName", LangText.ArmorReforge(reforgeType, "DisplayName"));
            line.OverrideColor = new Color(80, 187, 124);
            tooltips.Add(line);*/

            var line = new TooltipLine(Mod, "ReforgeUpside", LangText.AccessoryReforge(accessoryReforgeType, "Upside"));
            line.OverrideColor = new Color(80, 187, 124);
            tooltips.Add(line);

        }
        public override void NetSend(Item item, BinaryWriter writer)
        {
            base.NetSend(item, writer);
            AccessoryReforgeGlobalItem globalItem = item.GetGlobalItem<AccessoryReforgeGlobalItem>();
            writer.Write((int)globalItem.accessoryReforgeType);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            base.NetReceive(item, reader);
            AccessoryReforgeGlobalItem globalItem = item.GetGlobalItem<AccessoryReforgeGlobalItem>();
            globalItem.accessoryReforgeType = (AccessoryReforgeType)reader.ReadInt32();
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            base.SaveData(item, tag);
            tag["accessoryReforge"] = (int)accessoryReforgeType;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            base.LoadData(item, tag);
            accessoryReforgeType = (AccessoryReforgeType)tag.Get<int>("accessoryReforge");
        }
    }
    public class ArmorReforgeGlobalItem : GlobalItem
    {
        public ArmorReforgeType reforgeType;
        public override bool InstancePerEntity => true;
        public override void UpdateEquip(Item item, Player player)
        {
            base.UpdateEquip(item, player);
            switch (reforgeType)
            {
                default:
                    break;
                case ArmorReforgeType.Sharpening:
                    player.GetArmorPenetration(DamageClass.Generic) += 5;
                    player.statDefense -= 10;
                    break;
                case ArmorReforgeType.Sturdy:
                    player.statDefense += 5;
                    player.GetDamage(DamageClass.Generic) -= 0.07f;
                    break;
                case ArmorReforgeType.Alcaric:
                    player.statManaMax2 += 50;
                    player.statLifeMax2 -= 25;
                    break;
                case ArmorReforgeType.Radiant:
                    player.statLifeMax2 += 10;
                    player.moveSpeed -= 0.05f;
                    break;
                case ArmorReforgeType.Dexterous:
                    player.runAcceleration += 0.1f;
                    player.maxRunSpeed -= 0.1f;
                    break;
                case ArmorReforgeType.Whispy:
                    player.maxRunSpeed += 0.2f;
                    player.runAcceleration -= 0.3f;
                    break;
                case ArmorReforgeType.Daedious:
                    player.lifeRegen += 2;
                    player.GetDamage(DamageClass.Generic) -= 0.2f;
                    break;
                case ArmorReforgeType.Hunted:
                    player.GetDamage(DamageClass.Generic) -= 0.1f;
                    player.statDefense -= 10;
                    player.moveSpeed -= 0.2f;
                    player.GetCritChance(DamageClass.Generic) -= 5f;
                    break;
                case ArmorReforgeType.Rocky:
                    player.manaRegenBonus += 1;
                    player.endurance += 0.05f;
                    player.GetDamage(DamageClass.Generic) -= 0.05f;
                    break;
                case ArmorReforgeType.Shaded:
                    player.GetModPlayer<DashPlayer>().DashVelocity *= 1.2f;
                    player.GetDamage(DamageClass.Generic) -= 0.05f;
                    player.GetCritChance(DamageClass.Generic) -= 5f;
                    break;
                case ArmorReforgeType.Muted:
                    player.GetDamage(DamageClass.Generic) -= 0.2f;
                    player.GetCritChance(DamageClass.Generic) += 20f;
                    player.statDefense += 3;
                    break;
                case ArmorReforgeType.MageRan:
                    player.GetDamage(DamageClass.Magic) += 0.05f;
                    player.statDefense -= 10;
                    break;
                case ArmorReforgeType.SwordSpiked:
                    player.GetAttackSpeed(DamageClass.Melee) += 0.1f;
                    player.GetDamage(DamageClass.Melee) += 0.04f;
                    player.moveSpeed -= 0.2f;
                    player.GetCritChance(DamageClass.Generic) -= 20f;
                    break;
                case ArmorReforgeType.Necromanced:
                    player.maxMinions += 1;
                    player.GetDamage(DamageClass.Summon) -= 0.1f;
                    if (player.maxTurrets > 0)
                        player.maxTurrets -= 1;
                    break;
                case ArmorReforgeType.RangedHolsting:
                    player.GetDamage(DamageClass.Ranged) += 0.1f;
                    player.GetAttackSpeed(DamageClass.Generic) -= 0.3f;
                    break;
                case ArmorReforgeType.Vampiric:
                    player.GetModPlayer<VampiricArmorPlayer>().lifeSteal += 1;
                    player.endurance -= 0.1f;
                    break;
                case ArmorReforgeType.RogueThrown:
                    player.GetDamage(DamageClass.Throwing) += 0.12f;
                    player.endurance -= 0.18f;
                    player.statLifeMax2 -= 40;
                    player.lifeRegen -= 2;
                    break;
                case ArmorReforgeType.Gilded:
                    player.GetDamage(DamageClass.Generic) -= 0.1f;
                    player.GetModPlayer<DashPlayer>().DashVelocityBonus += 0.25f;
                    break;
                case ArmorReforgeType.Speeditrous:
                    player.statLifeMax2 -= 10;
                    player.GetModPlayer<DashPlayer>().MaxDashCount += 1;
                    break;
                case ArmorReforgeType.Scripted:
                    player.statManaMax2 -= 30;
                    player.GetModPlayer<AdvancedMagicPlayer>().chargeTimeBonus += 0.1f;
                    break;
                case ArmorReforgeType.Brewing:
                    player.GetModPlayer<FlaskPlayer>().maxInsourceCount += 1;
                    player.GetModPlayer<FlaskPlayer>().insourceTime += 10;
                    break;
                case ArmorReforgeType.Harnessing:
                    player.GetDamage(DamageClass.Generic) += 0.05f;
                    player.GetModPlayer<FlaskPlayer>().maxInsourceCount -= 1;
                    break;
                case ArmorReforgeType.Reloaded:
                    player.GetDamage(DamageClass.Ranged) -= 0.03f;
                    player.GetModPlayer<GunHoldPlayer>().maxAmmoBonus += 3;
                    break;
                case ArmorReforgeType.Illurias:
                    player.GetDamage(DamageClass.Generic) -= 0.07f;
                    player.statLifeMax2 += 5;
                    player.statDefense += 6;
                    break;
                case ArmorReforgeType.Sentricus:
                    player.maxTurrets += 2;
                    player.maxMinions -= 1;
                    break;
                case ArmorReforgeType.Reducting:
                    player.endurance += 0.10f;
                    player.statLifeMax2 -= 15;
                    break;
                case ArmorReforgeType.Flying:
                    player.GetModPlayer<WingTimeMaxPlayer>().wingTimeMaxBonus += 0.15f;
                    break;
                case ArmorReforgeType.Berserker:
                    player.GetAttackSpeed(DamageClass.Generic) += 0.05f;
                    player.statLifeMax2 -= 15;
                    player.statDefense -= 3;
                    break;
                case ArmorReforgeType.Acrobatic:
                    player.jumpSpeedBoost *= 1.1f;
                    player.accRunSpeed *= 1.1f;
                    player.GetDamage(DamageClass.Generic) -= 0.1f;
                    break;
                case ArmorReforgeType.Feathered:
                    player.GetModPlayer<FeatheredPlayer>().gravityLossBonus += 0.05f;
                    player.accRunSpeed *= 1.05f;
                    player.statDefense -= 5;
                    break;
                case ArmorReforgeType.Shattered:
                    player.statDefense -= 15;
                    player.statLifeMax2 -= 10;
                    break;
                case ArmorReforgeType.Clerical:
                    player.GetModPlayer<HealBoostPlayer>().healBonus += 0.1f;
                    player.GetModPlayer<FlaskPlayer>().insourceTime -= 120;
                    player.statDefense -= 5;

                    break;
                case ArmorReforgeType.Summoned:
                    player.endurance -= 0.15f;
                    player.maxMinions += 1;

                    break;
            }
        }

        public override void PostDrawInInventory(Item item, SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            base.PostDrawInInventory(item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            if (reforgeType == ArmorReforgeType.None)
                return;

            Texture2D iconTexture = null;
            Vector2 drawOrigin = Vector2.Zero;
            iconTexture = ModContent.Request<Texture2D>("Stellamod/Items/Consumables/GlisteningPearl").Value;
            drawOrigin = iconTexture.Size();
            Vector2 drawPosition = position + drawOrigin;
            spriteBatch.Draw(iconTexture, drawPosition, null, drawColor, 0f, drawOrigin, 0.5f, SpriteEffects.None, 0);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
            if (reforgeType == ArmorReforgeType.None)
                return;

            TooltipLine itemNameLine = tooltips.Find(x => x.Name == "ItemName");
            itemNameLine.Text = LangText.ArmorReforge(reforgeType, "DisplayName") + " " + itemNameLine.Text;
            /*TooltipLine line = new TooltipLine(Mod, "ReforgeDisplayName", LangText.ArmorReforge(reforgeType, "DisplayName"));
            line.OverrideColor = new Color(80, 187, 124);
            tooltips.Add(line);*/

            var line = new TooltipLine(Mod, "ReforgeUpside", LangText.ArmorReforge(reforgeType, "Upside"));
            line.OverrideColor = new Color(80, 187, 124);
            tooltips.Add(line);

            line = new TooltipLine(Mod, "ReforgeDownside", LangText.ArmorReforge(reforgeType, "Downside"));
            line.OverrideColor = Color.IndianRed;
            tooltips.Add(line);
        }

        public override void NetSend(Item item, BinaryWriter writer)
        {
            base.NetSend(item, writer);
            ArmorReforgeGlobalItem globalItem = item.GetGlobalItem<ArmorReforgeGlobalItem>();
            writer.Write((int)globalItem.reforgeType);
        }

        public override void NetReceive(Item item, BinaryReader reader)
        {
            base.NetReceive(item, reader);
            ArmorReforgeGlobalItem globalItem = item.GetGlobalItem<ArmorReforgeGlobalItem>();
            globalItem.reforgeType = (ArmorReforgeType)reader.ReadInt32();
        }

        public override void SaveData(Item item, TagCompound tag)
        {
            base.SaveData(item, tag);
            tag["armorReforge"] = (int)reforgeType;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            base.LoadData(item, tag);
            reforgeType = (ArmorReforgeType)tag.Get<int>("armorReforge");
        }
    }
}
