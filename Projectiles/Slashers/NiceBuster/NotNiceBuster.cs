using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.NiceBuster
{
    public class NotNiceBuster : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Frost Shot");
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            //The recording mode
        }
        public override void SetDefaults()
        {
            Projectile.damage = 100;
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.light = 1.5f;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.penetrate = 1;

            Projectile.hostile = false;

        }
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float Timer2;


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            var EntitySource = Projectile.GetSource_Death();
            Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<SuranBoom>(), Projectile.damage, 1, Projectile.owner, 0, 0);
        }

        public override void AI()
        {
            Projectile.velocity *= 1.04f;

            float maxDetectRadius = 2000f; // The maximum radius at which a projectile can detect a target
            float projSpeed = 18f; // The speed at which the projectile moves towards the target
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            NPC closestNPC = NPCHelper.FindClosestNPC(Projectile.position, maxDetectRadius);
            // Trying to find NPC closest to the projectile

            if (closestNPC == null)
                return;

            // If found, change the velocity of the projectile and turn it in the direction of the target
            // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
            Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * projSpeed;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(205, 100, 255), new Color(151, 46, 175), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
