using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Items.MoonlightMagic.Elements;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic
{
    internal class EnchantmentTooltipDraw : GlobalItem
    {
        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (line.Mod == "Stellamod" && line.Name.Contains("MoonMagicEnchant_"))
            {
                line.BaseScale *= 0.75f;
                line.X += 15;
                line.Y += 6;
                if (item.ModItem is BaseStaff ele)
                {
                    EnchantmentDrawHelper.GlowTextureOverride = ModContent.Request<Texture2D>(TextureRegistry.EmptyTexture);
                    int endIndex = line.Name.LastIndexOf("_") + 1;
                    int enchantIndex = int.Parse(line.Name.Substring(endIndex, line.Name.Length - endIndex));
                    var enchantment = ele.equippedEnchantments[enchantIndex].ModItem as BaseEnchantment;
                    int elementType = enchantment.GetElementType();
                    BaseElement element = ModContent.GetModItem(elementType) as BaseElement;
                    if (element.DrawTextShader(spriteBatch, item, line, ref yOffset))
                    {
                        return false;
                    }
                }
            }
            if (line.Mod == "Stellamod" && line.Name.Contains("WeaponType"))
            {
                if (item.ModItem is BaseStaff ele)
                {
                    //EnchantmentDrawHelper.GlowTextureOverride = ModContent.Request<Texture2D>(TextureRegistry.EmptyTexturePath);
                    BaseElement element = ModContent.GetInstance<BasicElement>();
                    if (element.DrawTextShader(spriteBatch, item, line, ref yOffset))
                    {
                        return false;
                    }
                }
            }



            bool isItemName = line.Mod == "Terraria" && line.Name == "ItemName";
            bool isEnchantTooltip = line.Mod == "Stellamod" && line.Name == "EnchantmentTooltip";
            bool isSynergyTooltip = line.Mod == "Stellamod" && line.Name == "SynergyHelp";
            if (isItemName || isEnchantTooltip)
            {
                if (isEnchantTooltip)
                {
                    line.BaseScale *= 0.95f;
                }

                if (item.ModItem is BaseEnchantment enchantment)
                {
                    int elementType = enchantment.GetElementType();
                    BaseElement element = ModContent.GetModItem(elementType) as BaseElement;
                    if (element.DrawTextShader(spriteBatch, item, line, ref yOffset))
                    {
                        return false;
                    }
                }
                if (item.ModItem is BaseElement ele)
                {
                    if (ele.DrawTextShader(spriteBatch, item, line, ref yOffset))
                    {
                        return false;
                    }
                }
            }

            if (isSynergyTooltip)
            {
                if (item.ModItem is SynergyTooltipItem t)
                {
                    if (t.PrimaryElement.DrawTextShader(spriteBatch, item, line, ref yOffset))
                    {
                        return false;
                    }
                }
            }

            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }



        public override void PostDrawTooltipLine(Item item, DrawableTooltipLine line)
        {
            base.PostDrawTooltipLine(item, line);

            if (line.Mod == "Terraria" && line.Name == "ItemName")
            {
                if (item.ModItem is BaseStaff baseStaff && baseStaff.primaryElement.ModItem is BaseElement ele)
                {
                    Texture2D texture = ModContent.Request<Texture2D>(ele.Texture).Value;
                    SpriteBatch spriteBatch = Main.spriteBatch;
                    Vector2 textPosition = new(line.X, line.Y);
                    Vector2 drawPos = textPosition + new Vector2(0, texture.Size().Y / 3.5f) - new Vector2(25, 6);
                    spriteBatch.Draw(texture, drawPos, null, Color.White, 0f, texture.Size() * 0.5f, 0.75f, SpriteEffects.None, 0f);
                }
            }

            if (line.Mod == "Stellamod" && line.Name.Contains("MoonMagicEnchant_"))
            {

                int startIndex = line.Name.IndexOf("_") + 1;
                int endIndex = line.Name.LastIndexOf("_");
                string textureName = line.Name.Substring(startIndex, endIndex - startIndex);
                Texture2D texture = ModContent.Request<Texture2D>(textureName).Value;

                SpriteBatch spriteBatch = Main.spriteBatch;
                Vector2 textPosition = new(line.X, line.Y);
                Vector2 drawPos = textPosition + new Vector2(0, texture.Size().Y / 3.5f) - new Vector2(15, 6);
                spriteBatch.Draw(texture, drawPos, null, Color.White, 0f, texture.Size() * 0.5f, 0.75f, SpriteEffects.None, 0f);

            }
        }
    }
}
