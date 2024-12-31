
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class WinterboundArrow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Winterbound Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; 
        }
        public override void SetDefaults()
        {
            Projectile.width = 9;
            Projectile.height = 17;

            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
        }
        public override void OnKill(int timeLeft)
        {
     
            for (int i = 0; i < 50; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 512f, 32f);
            SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/WinterboundArrowHit"), Projectile.position);
            var EntitySource = Projectile.GetSource_Death();
            for (int i = 0; i < 50; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, -2f, 0, default(Color), 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
            for (int i = 0; i < 4; i++)
            {

                Projectile.timeLeft = 2;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X, Projectile.Center.Y, Main.rand.Next(-2, 2), Main.rand.Next(-2, 2), ModContent.ProjectileType<WinterboundArrowFlake>(), 5, 1, Main.myPlayer, 0, 0);
            }
            if (Main.rand.NextBool(2))
                target.AddBuff(BuffID.Frostburn, 180);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(168, 180, 233), new Color(135, 39, 220), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 1.75f * Main.essScale);
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
            }
        }
    }
}
