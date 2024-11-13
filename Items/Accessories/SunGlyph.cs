
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Effects;
using Stellamod.Items.Accessories.Runes;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories
{
    internal class SunGlyphCrown : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.timeLeft = 10;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            base.AI();
            SunGlyphPlayer glyphPlayer = Owner.GetModPlayer<SunGlyphPlayer>();
            if(glyphPlayer.hasSunGlyph && !glyphPlayer.hideSunGlyphVisual)
            {
                Projectile.timeLeft = 2;
            }

            Projectile.Center = Owner.Center + new Vector2(0, -40);
            Projectile.Center += new Vector2(0, VectorHelper.Osc(0f, 4, speed: 1f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;

            SpriteEffects spriteEffects = Owner.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (spriteEffects == SpriteEffects.None)
                drawPos.X -= 4;
            for (float f = 0; f < 1f; f += 0.1f)
            {
                Vector2 glowDrawPos = drawPos + (f*MathHelper.TwoPi).ToRotationVector2() * VectorHelper.Osc(0.75f, 1f) * 8f;
                spriteBatch.Draw(texture, glowDrawPos, null, Color.LightGoldenrodYellow * 0.2f, Projectile.rotation, drawOrigin, 1f, spriteEffects, 0f);
            }

            spriteBatch.Draw(texture, drawPos, null, Color.White, Projectile.rotation, drawOrigin, 1f, spriteEffects, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }

    internal class SunGlyphPlayer : ModPlayer
    {
        public bool hasSunGlyph;
        public bool hideSunGlyphVisual;
        public override void ResetEffects()
        {
            base.ResetEffects();
            hasSunGlyph = false;
        }
    }

    public class SunGlyph : ModItem
	{
		public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Master;
            Item.accessory = true;
            Item.master = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SunGlyphPlayer sunGlyphPlayer = player.GetModPlayer<SunGlyphPlayer>();
            sunGlyphPlayer.hasSunGlyph = true;
            sunGlyphPlayer.hideSunGlyphVisual = hideVisual;

            SpecialEffectsPlayer specialEffectsPlayer = player.GetModPlayer<SpecialEffectsPlayer>();
            specialEffectsPlayer.hasSunGlyph = true;
            if (Main.dayTime)
            {
                player.GetDamage(DamageClass.Generic) += 0.30f;
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<SunGlyphCrown>()] == 0 && !hideVisual)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero,
                    ModContent.ProjectileType<SunGlyphCrown>(), 0, 0, player.whoAmI);
            }
        }
	}
}
