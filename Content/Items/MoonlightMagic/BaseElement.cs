using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;
namespace Stellamod.Content.Items.MoonlightMagic
{
    public enum ElementMatch
    {
        Neutral,
        Match,
        Mismatch
    }

    public abstract class BaseElement : BaseMagicItem,
        IDrawTrail,
        ICloneable,
        IAdvancedMagicAddon
    {
        public SoundStyle? CastSound { get; set; }
        public SoundStyle? HitSound { get; set; }
        public SoundStyle? ChargeSound { get; set; }
        public AdvancedMagicProjectile MagicProj { get; set; }
        public Projectile Projectile => MagicProj.Projectile;

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();

        }

        public virtual void ModifySisters(List<int> sisters)
        {

        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }

        public bool IsSynergizingWith(int elementType)
        {
            var sisters = new List<int>();
            ModifySisters(sisters);
            return Type == elementType || sisters.Contains(elementType);
        }

        public override string LocalizationCategory => "Elements";

        public virtual void AI() { }
        public virtual void DustEffects()
        {
            if (MagicProj.orb)
            {
                if (Main.rand.NextBool(8))
                {
                    var f = Particle<DustParticle>.Spawn(Projectile.Center, Main.rand.NextVector2Circular(8, 8), Color.White);
                    f.innerColor = GetElementColor();
                    f.outerColor = Color.Lerp(f.innerColor, Color.Black, 0.5f);
                    f.gravity = 0f;
                    f.fast = true;
                    f.dampening = 0.1f;
                }
            }
        }
        public virtual void DrawTrail(Vector2[] oldPos)
        {

        }
        public virtual void DrawOrbCircle(VertexPositionColorTexture[] vertices, int[] indices)
        {

        }


        public virtual void DrawRingTrail(Vector2[] oldPos, float[] oldRot)
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, oldPos, oldRot, RingColorFunction, RingWidthFunction, shader);
        }

        private float RingWidthFunction(float completionRatio)
        {
            return EasingFunction.QuadraticBump(completionRatio) * 2;
        }
        private Color RingColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Lerp(Color.White, GetElementColor(), 0.85f), GetElementColor(), completionRatio);
        }
        public virtual void DrawOrb(SpriteBatch spriteBatch, Vector2 drawPosition)
        {

        }

        public virtual void DrawForm(SpriteBatch spriteBatch,
            Texture2D formTexture,
            Vector2 drawPos, Color drawColor, Color lightColor, float drawRotation, float drawScale)
        {
            Vector2 drawOrigin = formTexture.Size() / 2;
            Color myColor = drawColor;
            myColor.A = 0;
            spriteBatch.Draw(formTexture, drawPos, null, myColor,
               drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

        public virtual void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) { }
        public virtual void OnKill()
        {
            //TrailKillSystem trailKillSystem = ModContent.GetInstance<TrailKillSystem>();
            //trailKillSystem.New(MagicProj.OldPos, this);
        }

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
            TooltipLine tooltipLine;
            AdvancedMagicPlayer advancedMagicPlayer = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
            if (!advancedMagicPlayer.IsUnlocked(Item))
            {
                tooltipLine = new TooltipLine(Mod, "EnchantmentLockedHelp",
                        Language.GetTextValue("Mods.Stellamod.Enchantments.EnchantmentLockedHelp"));
                tooltipLine.OverrideColor = Color.Gold;
                tooltips.Add(tooltipLine);
            }

            tooltipLine = new TooltipLine(Mod, "EnchantmentHelp",
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

        public Texture2D GetRingTexture()
        {
            return ModContent.Request<Texture2D>(Texture + "_Ring").Value;
        }
    }
}
