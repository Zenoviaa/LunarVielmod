
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Summons.Glyph
{
    public class AuroranGlyph : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private int Style => (int)Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet

            // This is necessary for right-click targeting
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;

            // This is needed so your minion can properly spawn when summoned and replaced when other minions are summoned
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = false; // Make the cultist resistant to this projectile, as it's resistant to all homing projectiles.
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.tileCollide = false; // Makes the minion go through tiles freely					// These below are needed for a minion weapon
            Projectile.friendly = true; // Only controls if it deals damage to enemies on contact (more on that later)// Declares this as a minion (has many effects)
            Projectile.DamageType = DamageClass.Summon; // Declares the damage type (needed for it to deal damage) // Amount of slots this minion occupies from the total minion slots available to the player (more on that later)
            Projectile.penetrate = -1; // Needed so the minion doesn't despawn on collision with enemies or tiles
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        // Here you can decide if your minion breaks things like grass or pots
        // The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
        public override void AI()
        {
            Timer++;
            Projectile.velocity.Y -= 0.01f;
            Projectile.velocity.X *= 0.98f;
            Projectile.rotation += VectorHelper.Osc(-0.001f, 0.001f, offset: Projectile.whoAmI);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            switch (Style)
            {
                case 0:
                    //No other effects
                    break;
                case 1:
                    target.AddBuff(BuffID.OnFire, 180);
                    break;
                case 2:
                    target.AddBuff(BuffID.Poisoned, 180);
                    break;
                case 3:
                    target.AddBuff(BuffID.Confused, 180);
                    break;
                case 4:
                    target.AddBuff(BuffID.Lovestruck, 180);
                    break;
                case 5:
                    target.AddBuff(BuffID.Ichor, 180);
                    break;
                case 6:
                    target.AddBuff(BuffID.Frostburn, 180);
                    break;
            }
        }

        private string GetTexturePath()
        {
            string baseTexture = Texture;
            if (Style == 0)
                return Texture;
            else
            {
                return Texture + "_" + Style;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            string texturePath = GetTexturePath();
            Texture2D texture = ModContent.Request<Texture2D>(texturePath).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Restart(blendState: BlendState.Additive);

            float alphaProgress = (Timer - 40) / 20f;
            alphaProgress = 1f - alphaProgress;
            float num = 8f;
            Color spriteColor = Color.White;
            switch (Style)
            {
                case 1:
                    spriteColor = Color.OrangeRed;
                    break;
                case 2:
                    spriteColor = Color.GreenYellow;
                    break;
                case 3:
                    spriteColor = Color.Purple;
                    break;
                case 4:
                    spriteColor = Color.Pink;
                    break;
                case 5:
                    spriteColor = Color.Yellow;
                    break;
                case 6:
                    spriteColor = Color.Cyan;
                    break;
            }
            spriteColor *= alphaProgress;
            for (float f = 0; f < num; f++)
            {
                Vector2 offset = (((f / num) * MathHelper.TwoPi) + Main.GlobalTimeWrappedHourly).ToRotationVector2() * VectorHelper.Osc(2f, 7f);
                Color glowColor = spriteColor.MultiplyRGB(lightColor) * 0.4f;
                spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + offset, null, glowColor, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }


            for (int i = 0; i < 2; i++)
            {
                spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White.MultiplyRGB(lightColor) * alphaProgress, Projectile.rotation, texture.Size() / 2f, 1f, SpriteEffects.None, 0);
            }

            spriteBatch.RestartDefaults();
            return false;
        }
    }
}
