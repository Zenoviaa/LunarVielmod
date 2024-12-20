using Stellamod.Systems.MiscellaneousMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic
{
    internal enum ElementMatch
    {
        Neutral,
        Match,
        Mismatch
    }

    internal abstract class BaseElement : BaseMagicItem,
        ICloneable,
        IAdvancedMagicAddon
    {
        public SoundStyle? CastSound { get; set; }
        public SoundStyle? HitSound { get; set; }
        public AdvancedMagicProjectile MagicProj { get; set; }
        public Projectile Projectile => MagicProj.Projectile;

        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override string LocalizationCategory => "Elements";

        public virtual void AI() { }
        public virtual void DrawTrail() { }
        public virtual void DrawForm(SpriteBatch spriteBatch,
            Texture2D formTexture,
            Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            Vector2 drawOrigin = formTexture.Size() / 2;

            spriteBatch.Draw(formTexture, drawPos, null, drawColor,
               drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        public virtual void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnKill() { }

        public virtual Color GetElementColor() { return Color.White; }
        public virtual int GetOppositeElementType()
        {
            return -1;
        }

        public ElementMatch GetMatch(BaseEnchantment enchantment)
        {
            ElementMatch match = ElementMatch.Neutral;
            if (enchantment.GetElementType() == Type)
                match = ElementMatch.Match;
            if (enchantment.GetElementType() == GetOppositeElementType())
                match = ElementMatch.Mismatch;
            return match;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            TooltipLine tooltipLine = new TooltipLine(Mod, "EnchantmentHelp",
                Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentCommonHelp"));
            tooltipLine.OverrideColor = Color.Gray;
            tooltips.Add(tooltipLine);
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
            return base.PreDrawInWorld(spriteBatch, lightColor, alphaColor, ref rotation, ref scale, whoAmI);
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            float sizeLimit = 34;
            int numberOfCloneImages = 6;
            float p = MathUtil.Osc(0f, 0.5f, speed: 3);
            Main.DrawItemIcon(spriteBatch, Item, position, Color.White * 0.33f * p, sizeLimit);
            for (float i = 0; i < 1; i += 1f / numberOfCloneImages)
            {
                float cloneImageDistance = MathF.Cos(Main.GlobalTimeWrappedHourly / 2.4f * MathF.Tau / 2f) + 0.5f;
                cloneImageDistance = MathHelper.Max(cloneImageDistance, 0.1f);
                Color color = GetElementColor() * p * 0.4f;
                color *= 1f - cloneImageDistance * 0.2f;
                color.A = 0;
                cloneImageDistance *= 3;
                Vector2 drawPos = position + (i * MathF.Tau).ToRotationVector2() * (cloneImageDistance + 2f);
                Main.DrawItemIcon(spriteBatch, Item, drawPos, color, sizeLimit);
            }
            base.PostDrawInInventory(spriteBatch, position, frame, drawColor, itemColor, origin, scale);
        }

        public virtual bool DrawTextShader(SpriteBatch spriteBatch, Item item, DrawableTooltipLine line, ref int yOffset)
        {
            return false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public BaseElement Instantiate()
        {
            return (BaseElement)Clone();
        }
    }
}
