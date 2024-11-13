
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.IgniterExplosions;

using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Thrown
{
    public class ComicalTridentP : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.timeLeft = 180;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Poisoned, 180);
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.OnFire3, 280);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 trailDrawPos = (Projectile.oldPos[k] - Main.screenPosition) + Projectile.Size / 2 + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                spriteBatch.Draw(texture, trailDrawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(texture, drawPos, null, Color.White.MultiplyRGB(lightColor), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void AI()
        {
            base.AI();
            Projectile.velocity.Y += 0.3f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
            }

            Projectile.NewProjectile(Projectile.GetSource_FromThis(),Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<FunBoom>(), (int)(Projectile.damage * 1.5), 0f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(new SoundStyle($"Stellamod/Assets/Sounds/ComicBoom"), Projectile.position);
        }
    }
}