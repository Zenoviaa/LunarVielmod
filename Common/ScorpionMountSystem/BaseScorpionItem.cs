using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.UI.GunHolsterSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.ScorpionMountSystem
{
    public abstract class BaseScorpionItem : ModItem
    {
        private List<Item> _leftHandedGuns;
        private List<Item> _rightHandedGuns;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.DamageType = DamageClass.Summon;
            Item.damage = 24;
            Item.knockBack = 2;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public int gunType;
        public List<Item> leftHandedGuns
        {
            get
            {
                if(_leftHandedGuns == null || _leftHandedGuns.Count < GetLeftHandedCount())
                {
                    _leftHandedGuns = new List<Item>();
                    for(int i = 0; i < GetLeftHandedCount(); i++)
                    {
                        Item air = new Item();
                        air.SetDefaults(0);
                        _leftHandedGuns.Add(air);
                    }
                }

                return _leftHandedGuns;
            }
            private set
            {
                _leftHandedGuns = value;
            }
        }

        public List<Item> rightHandedGuns
        {
            get
            {
                if (_rightHandedGuns == null || _rightHandedGuns.Count < GetRightHandedCount())
                {
                    _rightHandedGuns = new List<Item>();
                    for (int i = 0; i < GetRightHandedCount(); i++)
                    {
                        Item air = new Item();
                        air.SetDefaults(0);
                        _rightHandedGuns.Add(air);
                    }
                }

                return _rightHandedGuns;
            }
            private set
            {
                _rightHandedGuns = value;
            }
        }

        public virtual int GetLeftHandedCount()
        {
            return 2;
        }

        public virtual int GetRightHandedCount()
        {
            return 2;
        }

        public override void RightClick(Player player)
        {
            base.RightClick(player);

            //Yay
            ScorpionHolsterUISystem uiSystem = ModContent.GetInstance<ScorpionHolsterUISystem>();
            uiSystem.scorpionItem = this;
            uiSystem.ToggleUI();
        }

        public override bool CanRightClick()
        {
            return true;
        }
 
        public override bool CanUseItem(Player player)
        {
            ScorpionPlayer scorpionPlayer = player.GetModPlayer<ScorpionPlayer>();
            if(player.mount.Active && player.mount.Type == Item.mountType)
            {
                return false;
            }
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine line = new TooltipLine(Mod, "ScorpionSummonHelp", LangText.Common("ScorpionSummonHelp"));
            line.OverrideColor = Color.IndianRed;
            tooltips.Add(line);

            line = new TooltipLine(Mod, "ScorpionSummonHolsterHelp", LangText.Common("ScorpionSummonHolsterHelp"));
            line.OverrideColor = Color.Gray;
            tooltips.Add(line);

            line = new TooltipLine(Mod, "ScorpionRightClickHelp", LangText.Common("ScorpionRightClickHelp"));
            line.OverrideColor = Color.Yellow;
            tooltips.Add(line);
        }

        public override bool ConsumeItem(Player player)
        {
            return false;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["leftHandedGuns"] = leftHandedGuns;
            tag["rightHandedGuns"] = rightHandedGuns;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            leftHandedGuns = tag.Get<List<Item>>("leftHandedGuns");
            rightHandedGuns = tag.Get<List<Item>>("rightHandedGuns");
        }
    }
}
