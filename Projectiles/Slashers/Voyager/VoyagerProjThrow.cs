using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Slashers.Voyager
{
    public class VoyagerProjThrow : ModProjectile
    {
        public override void SetDefaults()
        {
            
            Projectile.penetrate = 5;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.height = 84;
            Projectile.width = 80;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 100;       
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            float rotation = Projectile.rotation;
            Timer++;

            player.RotatedRelativePoint(Projectile.Center);
            Projectile.rotation  -= 0.2f;


            Projectile.velocity *= 0.97f;

            float speedXa = -Projectile.velocity.X;
            float speedYa = -Projectile.velocity.Y;

            if (Timer == 1)
            {
                ShakeModSystem.Shake = 1;
                player.GetModPlayer<MyPlayer>().SwordComboSlash += 1;
            }

         //   Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X + (speedXa * 5), Projectile.position.Y + (speedYa * 5), speedXa * 0, speedYa * 0, ModContent.ProjectileType<AlcaricMushBoom>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
            if (Timer == 99)
            {
                ShakeModSystem.Shake = 4;
              

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<AlcaricMushBoom>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
                SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
                Projectile.Kill();
            }


            Vector3 RGB = new(2.55f, 2.55f, 0.94f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.Center, RGB.X, RGB.Y, RGB.Z);



            //Projectile.netUpdate = true;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, 0f, 150, Color.MediumPurple, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            SpriteEffects Effects = Projectile.spriteDirection != 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.instance.LoadProjectile(Projectile.type);
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            // Redraw the projectile with the color not influenced by light
            Vector2 drawOrigin = new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(50, 150, 190), new Color(236, 118, 195), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ShakeModSystem.Shake = 5;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, 
                ModContent.ProjectileType<AlcaricMushBoom>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
            SoundEngine.PlaySound(SoundID.DD2_EtherianPortalOpen, Projectile.position);
        }
    }
}

