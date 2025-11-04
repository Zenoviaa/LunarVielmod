using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.PowderSystem;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.IgnitersNPowders
{
    public class IgniterPlayer : ModPlayer
    {
        public float extenderBonus;
        public override void ResetEffects()
        {
            base.ResetEffects();
            extenderBonus = 0f;
        }
    }
    public class IgniterTooltipDraw : GlobalItem
    {
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (line.Mod == "Stellamod" && line.Name.Contains("Powder_"))
            {
                line.BaseScale *= 0.8f;
                line.X += 30;
                line.Y += 6;
            }

            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }



        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
            base.PostDrawTooltipLine(item, line);
            if (line.Mod == "Stellamod" && line.Name.Contains("Powder_"))
            {

                int startIndex = line.Name.IndexOf("_") + 1;
                int endIndex = line.Name.LastIndexOf("_");
                string textureName = line.Name.Substring(startIndex, endIndex - startIndex);
                Texture2D texture = ModContent.Request<Texture2D>(textureName).Value;

                SpriteBatch spriteBatch = Main.spriteBatch;
                Vector2 textPosition = new(line.X, line.Y);
                Vector2 drawPos = textPosition + new Vector2(0, texture.Size().Y / 3.5f) - new Vector2(15, 6);
                spriteBatch.Draw(texture, drawPos, null, Color.White, 0f, texture.Size() * 0.5f, 0.8f, SpriteEffects.None, 0f);

            }
        }
    }

    public abstract class BaseIgniterCard : ModItem
    {
        private List<Item> _powders;
        public List<Item> Powders
        {
            get
            {

                _powders ??= new List<Item>();
                while (_powders.Count < GetPowderSlotCount())
                {
                    Item item = new Item();
                    item.SetDefaults(0);
                    _powders.Add(item);
                }


                return _powders;
            }
            set
            {
                _powders = value;
            }
        }

        public virtual int GetPowderSlotCount()
        {
            return 3;
        }
        public override void SetDefaults()
        {
            Item.damage = 2;
            Item.knockBack = 2;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Ranged;
            Item.value = 200;
            Item.rare = ItemRarityID.Blue;


            SoundStyle soundStyle = SoundID.Item1;
            soundStyle.PitchVariance = 0.2f;
            Item.UseSound = soundStyle;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<IgniterCardProjectile>();
            Item.crit = 4;
            Item.shootSpeed = 15;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            IgniterPlayer igniterPlayer = player.GetModPlayer<IgniterPlayer>();
            velocity *= 1.0f + igniterPlayer.extenderBonus;
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
        public override void RightClick(Player player)
        {
            base.RightClick(player);
            PowderUISystem uiSystem = ModContent.GetInstance<PowderUISystem>();
            uiSystem.Card = this;
            uiSystem.ToggleUI();
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            base.ModifyWeaponDamage(player, ref damage);
            for (int i = 0; i < Powders.Count; i++)
            {
                if (!Powders[i].IsAir)
                {
                    BasePowder basePowder = Powders[i].ModItem as BasePowder;
                    damage += basePowder.DamageModifier;
                }

            }
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine line = new TooltipLine(Mod, "IgniterCard", LangText.Common("IgniterCardType"));
            line.OverrideColor = Color.LightGreen;
            tooltips.Add(line);

            line = new TooltipLine(Mod, "", "");
            Keys keys = Keys.LeftShift;
            bool isExpanded = Main.keyState.IsKeyDown(keys);

            if (!isExpanded)
            {
                line = new TooltipLine(Mod, "ExpandTooltipHelp", LangText.Common("ExpandTooltipHelp", "Left Shift"));
                line.OverrideColor = Color.Lerp(Color.White, Color.Black, 0.7f);
                tooltips.Add(line);
            }
            else
            {
                line = new TooltipLine(Mod, "IgniterCardHelp", Helpers.LangText.Common("IgniterCardHelp"))
                {
                    OverrideColor = Color.White
                };
                tooltips.Add(line);
                line = new TooltipLine(Mod, "IgniterCard", Helpers.LangText.Common("IgniterCard"))
                {
                    OverrideColor = Color.White
                };
                tooltips.Add(line);
            }


            for (int i = 0; i < Powders.Count; i++)
            {
                var item = Powders[i];
                if (item.ModItem is BasePowder powder)
                {
                    line = new TooltipLine(Mod, $"Powder_{powder.Texture}_{i}", powder.DisplayName.Value);
                    line.OverrideColor = new Color(80, 187, 124);
                    tooltips.Add(line);
                }
            }
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.WriteItemList(Powders);
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            Powders = reader.ReadItemList();
        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag.Add("powders", Powders);
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            List<Item> powders = tag.Get<List<Item>>("powders");
            Powders = powders;
        }
    }
}
