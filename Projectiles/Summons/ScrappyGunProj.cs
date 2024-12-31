using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Stellamod.Items.Armors.Scrappy;
using Stellamod.Projectiles.Gun;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace Stellamod.Projectiles.Summons
{
    internal class ScrappyGunProj  : ModProjectile
    {
        private bool _flip;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 78;
            Projectile.height = 38;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        private ref float ai_Timer => ref Projectile.ai[0];
        private ref float ai_Kill => ref Projectile.ai[1];
        public override void AI()
        {
            ai_Timer++;
            Player player = Main.player[Projectile.owner];
            bool hasSetBonus = player.GetModPlayer<ScrappyPlayer>().hasSetBonus;
            if (!hasSetBonus)
            {
                Projectile.Kill();
                return;
            }

            SummonHelper.SearchForTargets(player, Projectile, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            Projectile.Center = player.Center + new Vector2(0, -64) + new Vector2(0, VectorHelper.Osc(0, -2));
            if (foundTarget)
            {
                if(ai_Kill == 1)
                {
                    ai_Kill = 0;
                    Vector2 velocity = Projectile.rotation.ToRotationVector2();
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
                        ModContent.ProjectileType<ScrappyGunLaser>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: Projectile.whoAmI);
                    SoundEngine.PlaySound(SoundID.DD2_LightningBugZap, Projectile.position);
                }
            
                float targetRotation = Projectile.DirectionTo(targetCenter).ToRotation();
                Projectile.rotation = MathHelper.WrapAngle(MathHelper.Lerp(Projectile.rotation, targetRotation, 0.04f));
                if (targetCenter.X < Projectile.Center.X)
                    _flip = true;
                else
                    _flip = false;
            } 
            else
            {
                ai_Kill = 1;
                float targetRotation = player.velocity.ToRotation();
                Projectile.rotation = MathHelper.WrapAngle(MathHelper.Lerp(Projectile.rotation, targetRotation, 0.04f));
                if (targetCenter.X < player.velocity.X)
                    _flip = true;
                else
                    _flip = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
            SpriteEffects spriteEffects = SpriteEffects.None;
            if(_flip)
            {
                spriteEffects = SpriteEffects.FlipVertically;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            Vector2 drawOrigin = new Vector2(78 / 2, 38 / 2);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin;// + new Vector2(0f, projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(Color.OrangeRed, Color.Transparent, 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.spriteBatch.Draw(texture, drawPos, null, color, Projectile.oldRot[k], drawOrigin, Projectile.scale, spriteEffects, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation,
               drawOrigin, 1f, spriteEffects, 0);
            return false;
        }
    }
}
