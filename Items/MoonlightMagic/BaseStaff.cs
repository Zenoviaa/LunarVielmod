using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI.AdvancedMagicSystem;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.Utilities;

namespace Stellamod.Items.MoonlightMagic
{
    internal abstract class BaseStaff : ModItem
    {
        private Item _primaryElement;
        private Item[] _equippedEnchantments;
        private static Item _preReforgeElement;
        private static Item[] _preReforgeEnchants;
        public Texture2D Form { get; set; }
        public BaseMovement Movement { get; set; }

        public int Size { get; set; }
        public int TrailLength { get; set; }
        public UnifiedRandom Random { get; private set; }

        //Enchantment Slots
        public Item primaryElement
        {
            get
            {
                if(_primaryElement == null)
                {
                    _primaryElement = new Item();
                    _primaryElement.SetDefaults(0);
                }
                return _primaryElement;
            }
            set
            {
                _primaryElement = value;
            }
        }

        public Item[] equippedEnchantments
        {
            get
            {
                if(_equippedEnchantments == null)
                {
                    _equippedEnchantments = new Item[GetNormalSlotCount() + GetTimedSlotCount()];
                    for(int i = 0; i < _equippedEnchantments.Length; i++)
                    {
                        _equippedEnchantments[i] = new Item();
                        _equippedEnchantments[i].SetDefaults(0);
                    }
                }

                return _equippedEnchantments;
            }
            set
            {
                _equippedEnchantments = value;
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 18;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
            Item.height = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTime = 32;
            Item.useAnimation = 32;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.mana = 10;

            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 15;
            Item.shoot = ModContent.ProjectileType<AdvancedMagicProjectile>();
            Item.autoReuse = true;
            TrailLength = 32;
            Size = 8;

            //Randomize trail values
            int seed = WorldGen._genRandSeed;
            Random = new UnifiedRandom(seed);
            TrailLength = Random.Next(24, 32);
        }

        public override void NetSend(BinaryWriter writer)
        {
            base.NetSend(writer);
            writer.Write(primaryElement.type);
            writer.Write(equippedEnchantments.Length);
            for(int i = 0; i < equippedEnchantments.Length; i++)
            {
                writer.Write(equippedEnchantments[i].type);
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            base.NetReceive(reader);
            int primaryElementType = reader.ReadInt32();
            primaryElement = new Item(primaryElementType);
            int length = reader.ReadInt32();
            for(int i = 0; i < length; i++)
            {
                int enchantmentType = reader.ReadInt32();
                equippedEnchantments[i] = new Item(enchantmentType);
            }
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            base.ModifyWeaponDamage(player, ref damage);

            /*
            float damageModifier = 1f;
            for (int i = 0; i < equippedEnchantments.Length; i++)
            {
                Item item = equippedEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    if (primaryElement.ModItem is BaseElement element)
                    {
                        ElementMatch match = element.GetMatch(enchantment);
                        switch (match)
                        {
                            case ElementMatch.Match:
                                damageModifier += 0.04f;
                                break;
                            case ElementMatch.Mismatch:
                                damageModifier -= 0.04f;
                                break;
                        }
                    }
                }
            }

            damage *= damageModifier;
            */
        }


        public override void ModifyManaCost(Player player, ref float reduce, ref float mult)
        {
            base.ModifyManaCost(player, ref reduce, ref mult);
            for (int i = 0; i < equippedEnchantments.Length; i++)
            {
                Item item = equippedEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    mult += enchantment.GetStaffManaModifier();
                }
            }
        }

        public override void PreReforge()
        {
            base.PreReforge();

            _preReforgeElement = primaryElement.Clone();
            _preReforgeEnchants = new Item[equippedEnchantments.Length];
            for (int i = 0; i < _preReforgeEnchants.Length; i++)
            {
                _preReforgeEnchants[i] = equippedEnchantments[i].Clone();
            }
        }

        public override void PostReforge()
        {
            base.PostReforge();

            primaryElement = _preReforgeElement;
            equippedEnchantments = _preReforgeEnchants;

        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);

            TooltipLine tooltipLine;

            tooltipLine = new TooltipLine(Mod, "WeaponType",
                Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonStaff"));
            tooltipLine.OverrideColor = Color.White;
            tooltips.Add(tooltipLine);

            tooltipLine = new TooltipLine(Mod, "EnchantHelp",
                Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonStaffHelp"));
            tooltipLine.OverrideColor = Color.Gray;
            tooltips.Add(tooltipLine);

            for (int i = 0; i < equippedEnchantments.Length; i++)
            {
                var item = equippedEnchantments[i];
                if (item.ModItem is BaseEnchantment enchantment)
                {
                    tooltipLine = new TooltipLine(Mod, $"MoonMagicEnchant_{enchantment.Texture}_{i}", enchantment.DisplayName.Value);
                    tooltips.Add(tooltipLine);
                }
            }
        }

        public virtual int GetNormalSlotCount()
        {
            return 5;
        }

        public virtual int GetTimedSlotCount()
        {
            return 2;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override bool ConsumeItem(Player player) => false;

        public override void RightClick(Player player)
        {
            base.RightClick(player);
            ModContent.GetInstance<AdvancedMagicUISystem>().OpenUI(this);
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            if (primaryElement.ModItem is BaseElement element)
            {
                element.SpecialInventoryDraw(Item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
            }

            for (int i = 0; i < equippedEnchantments.Length; i++)
            {
                var enchant = equippedEnchantments[i];
                if (enchant.ModItem is BaseEnchantment enchantment)
                {
                    enchantment.SpecialInventoryDraw(Item, spriteBatch, position, frame, drawColor, itemColor, origin, scale);
                }
            }
            return base.PreDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            AdvancedMagicUtil.NewMagicProjectile(this, player, source, position, velocity, type, damage, knockback);
            return false;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["itemCount"] = equippedEnchantments.Length;
            if (primaryElement != null)
                tag["element"] = primaryElement;
            for (int i = 0; i < equippedEnchantments.Length; i++)
            {
                var enchantment = equippedEnchantments[i];
                if (enchantment == null)
                    continue;
                tag[$"enchantment_{i}"] = enchantment;
            }
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            if (tag.ContainsKey("element"))
            {
                var element = tag.Get<Item>("element");
                primaryElement = element;
            }

            if (tag.ContainsKey("itemCount"))
            {
                int itemCount = tag.GetInt("itemCount");
                equippedEnchantments = new Item[itemCount];
                for (int i = 0; i < itemCount; i++)
                {
                    if (tag.ContainsKey($"enchantment_{i}"))
                    {
                        var enchantment = tag.Get<Item>($"enchantment_{i}");
                        equippedEnchantments[i] = enchantment;
                    }
                }
            }
        }
    }
}
