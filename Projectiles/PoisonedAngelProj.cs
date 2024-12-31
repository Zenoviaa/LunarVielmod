using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class PoisonedAngelProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {          
            Projectile.penetrate = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.height = 40;
            Projectile.width = 40;
            Projectile.friendly = true;
            Projectile.scale = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.timeLeft = 100;
        }

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heat Arrow");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }


        public override void AI()
        {
            float rotation = Projectile.rotation;
            Timer++;

            Owner.RotatedRelativePoint(Projectile.Center);
            Projectile.rotation  -= 0.5f;
            Projectile.velocity *= 0.97f;
            if (Timer < 30)
            {
                if (Main.mouseLeft && Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.DirectionTo(Main.MouseWorld) * Projectile.Distance(Main.MouseWorld) / 12;
                    Projectile.netUpdate = true;
                }

                Owner.heldProj = Projectile.whoAmI;
                Owner.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
                Owner.itemTime = 10;
                Owner.itemAnimation = 10;
                Owner.itemRotation = rotation * Owner.direction;
            }

            if (Timer == 99)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<VerstiExSps>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner);
                }
                ExplodeEffects();
            }

            Lighting.AddLight(Projectile.position, Color.GreenYellow.ToVector3() * 0.78f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
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
                Color color = Projectile.GetAlpha(Color.Lerp(new Color(97, 231, 25), new Color(47, 255, 25), 1f / Projectile.oldPos.Length * k) * (1f - 1f / Projectile.oldPos.Length * k));
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, Effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return true;
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                ModContent.ProjectileType<VerstiExSps>(), (int)(Projectile.damage * 1.5f), 0f, Projectile.owner, 0f, 0f);
            ExplodeEffects();
        }

        private void ExplodeEffects()
        {
            FXUtil.ShakeCamera(Projectile.position, 1024, 2);
            for (float f = 0; f < 6; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(),
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.Green, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }

            for (float i = 0; i < 4; i++)
            {
                float progress = i / 4f;
                float rot = progress * MathHelper.ToRadians(360);
                Vector2 offset = rot.ToRotationVector2() * 24;
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Green,
                    outerGlowColor: Color.Black,
                    duration: Main.rand.NextFloat(12, 25),
                    baseSize: Main.rand.NextFloat(0.08f, 0.2f));
                particle.Rotation = rot + MathHelper.ToRadians(45);
            }
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            Projectile.Kill();
        }
    }
}

