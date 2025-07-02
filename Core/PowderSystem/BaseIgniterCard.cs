using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Stellamod.Core.PowderSystem.UI;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.PowderSystem
{
    internal abstract class BaseIgniterCard : ModItem
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
            Item.mana = 3;
            Item.width = 40;
            Item.height = 40;
            Item.useTime = 50;
            Item.useAnimation = 50;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.DamageType = DamageClass.Magic;
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
            
            TooltipLine line = new TooltipLine(Mod, "IgniterCard", LangText.Common("IgniterCard"));
            line.OverrideColor = new Color(80, 187, 124);
            tooltips.Add(line);

            line = new TooltipLine(Mod, "IgniterCardHelp", LangText.Common("IgniterCardHelp"));
            line.OverrideColor = Color.Lerp(new Color(80, 187, 124), Color.Black, 0.5f);
            tooltips.Add(line);
       
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
