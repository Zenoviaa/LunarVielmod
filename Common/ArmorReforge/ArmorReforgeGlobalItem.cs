using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.ArmorReforge
{
    internal class VampiricArmorPlayer : ModPlayer
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
            if (cooldownTimer > 0)
                return;
            cooldownTimer = 30;
            Player.Heal((int)lifeSteal);
        }
    }

    internal class ArmorReforgeGlobalItem : GlobalItem
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
                    player.GetDamage(DamageClass.Ranged) += 0.12f;
                    player.endurance -= 0.18f;
                    player.statLifeMax2 -= 40;
                    player.lifeRegen -= 2;
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
