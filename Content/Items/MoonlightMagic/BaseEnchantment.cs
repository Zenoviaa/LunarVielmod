using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic
{
    public abstract class BaseEnchantment : BaseMagicItem,
        IAdvancedMagicAddon,
        ICloneable
    {
        private static BaseEnchantment[] _enchantments;
        public static int[] GetTypes()
        {
            BaseEnchantment[] allEnchantments = BaseEnchantment.AllEnchantments;
            int numResults = allEnchantments.Length;
            int[] enchantmentTypes = new int[numResults];
            for (int i = 0; i < enchantmentTypes.Length; i++)
            {
                enchantmentTypes[i] = allEnchantments[i].Type;
            }
            return enchantmentTypes;
        }
        public static int[] GetNonSpecialTypes()
        {
            List<BaseEnchantment> allEnchantmentsList = BaseEnchantment.AllEnchantments.ToList();
            allEnchantmentsList.RemoveAll(x => EnchantmentHelper.SpecialEnchantments.Contains(x.Type));

            BaseEnchantment[] allEnchantments = allEnchantmentsList.ToArray();
            int numResults = allEnchantments.Length;
            int[] enchantmentTypes = new int[numResults];
            for (int i = 0; i < enchantmentTypes.Length; i++)
            {
                enchantmentTypes[i] = allEnchantments[i].Type;
            }
            return enchantmentTypes;
        }
        public static BaseEnchantment[] AllEnchantments
        {
            get
            {
                if (_enchantments == null)
                    _enchantments = Stellamod.Instance.GetContent<BaseEnchantment>().ToArray();
                return _enchantments;
            }
        }

        public AdvancedMagicProjectile MagicProj { get; set; }
        public Projectile Projectile => MagicProj.Projectile;

        public Player Owner => Main.player[Projectile.owner];
        public override string LocalizationCategory => "Enchantments";

        public int time;

        public int Countertimer;
        public bool isTimedEnchantment => time > 0;
        public virtual float GetStaffManaModifier() { return 0.2f; }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public BaseEnchantment Instantiate()
        {
            return (BaseEnchantment)Clone();
        }

        public virtual int GetElementType()
        {
            return ModContent.ItemType<BasicElement>();
        }



        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public virtual void SetMagicDefaults() { }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine tooltipLine;

            AdvancedMagicPlayer advancedMagicPlayer = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
            if (!advancedMagicPlayer.IsUnlocked(Item))
            {
                tooltipLine = new TooltipLine(Mod, "EnchantmentLockedHelp",
                        Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentLockedHelp"));
                tooltipLine.OverrideColor = Color.Gold;
                tooltips.Add(tooltipLine);
            }

            if (isTimedEnchantment)
            {
                tooltipLine = new TooltipLine(Mod, "EnchantmentTimedHelp",
                    Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonTimed", time));
                tooltips.Add(tooltipLine);
            }

            if (isTimedEnchantment)
            {
                tooltipLine = new TooltipLine(Mod, "EnchantmentCommonTimedHelp",
                    Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonTimedHelp", time));
                tooltipLine.OverrideColor = Color.Gray;
                tooltips.Add(tooltipLine);
            }

            float manaModifier = GetStaffManaModifier() * 100;
            string manaDisplay = manaModifier.ToString("#");
            tooltipLine = new TooltipLine(Mod, "EnchantmentManaHelp",
                Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonMana",
                manaDisplay));
            tooltipLine.OverrideColor = Color.IndianRed;
            tooltips.Add(tooltipLine);


        }

        public virtual void DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset) { }
        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        //Enchantment stuff
        public virtual void AI() { }
        public virtual void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnKill(int timeLeft) { }
        public virtual bool OnTileCollide(Vector2 oldVelocity) { return true; }
    }
}
