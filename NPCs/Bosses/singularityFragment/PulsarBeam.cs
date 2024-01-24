using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.singularityFragment
{
    internal class PulsarBeam : ModProjectile
    {
        private bool Moved;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Shadow Hand");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 9;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.timeLeft = 250;
            Projectile.alpha = 0;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffType<AbyssalFlame>(), 200);
        }

        private float alphaCounter = 1;
        public override void AI()
        {
            Projectile.velocity *= 1.01f;
            Projectile.ai[1]++;
            if (!Moved && Projectile.ai[1] >= 0)
            {
                Projectile.spriteDirection = Projectile.direction;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.alpha = 255;
                Moved = true;
            }

            if (Projectile.ai[1] <= 1)
            {
                Projectile.scale = 1.5f;
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SoftSummon2"), Projectile.position);
            }

            alphaCounter = 1;
            Projectile.spriteDirection = Projectile.direction;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.RoyalBlue, Color.Purple, completionRatio) * 0.7f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.LightningTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture2D4 = Request<Texture2D>("Stellamod/Effects/Masks/DimLight").Value;
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.17f * (7 + 0.6f), SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D4, Projectile.Center - Main.screenPosition, null, new Color((int)(15f * alphaCounter), (int)(15f * alphaCounter), (int)(85f * alphaCounter), 0), Projectile.rotation, new Vector2(32, 32), 0.07f * (7 + 0.6f), SpriteEffects.None, 0f);
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 1.0f * Main.essScale);
        }
    }
}


