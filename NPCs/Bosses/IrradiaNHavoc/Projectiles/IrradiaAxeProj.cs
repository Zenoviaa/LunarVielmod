using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Trails;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.IrradiaNHavoc.Projectiles
{
    internal class IrradiaAxeProj : ModProjectile
    {
        public PrimDrawer TrailDrawer { get; private set; } = null;
        private ref float Timer => ref Projectile.ai[0];
        private ref float Timer2 => ref Projectile.ai[1];
        private ref float AlphaTimer => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 102;
            Projectile.height = 102;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Timer++;
            float distanceToClosestPlayer = float.MaxValue;
            Player closestPlayer = null;
            for(int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (!player.active)
                    continue;

                float dist = Vector2.Distance(Projectile.Center, player.Center);
                if(dist <= distanceToClosestPlayer)
                {
                    closestPlayer = player;
                    distanceToClosestPlayer = dist;
                }
            } 

            if(closestPlayer != null)
            {
                Vector2 directionToPlayer = Projectile.Center.DirectionTo(closestPlayer.Center);
                Projectile.velocity += directionToPlayer * 0.2f;
            }

            if(Timer % 60 == 0)
            {
                if(Timer2 == 0)
                {
                    Vector2 velocity = Vector2.UnitX;
                    Vector2 velocityRotated = velocity.RotatedBy(MathHelper.PiOver2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity * 1200, velocity,
                        ModContent.ProjectileType<IrradiaAxeLaserProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocityRotated * 1200, velocityRotated,
                        ModContent.ProjectileType<IrradiaAxeLaserProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Axing") { Pitch = Main.rand.NextFloat(-1f, 1f) });
                    Timer2 = 1;
                    AlphaTimer = 0;
                } else
                {
                    Vector2 velocity = new Vector2(1, 1);
                    Vector2 velocityRotated = velocity.RotatedBy(MathHelper.PiOver2);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocity * 1200, velocity,
                        ModContent.ProjectileType<IrradiaAxeLaserProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - velocityRotated * 1200, velocityRotated,
                        ModContent.ProjectileType<IrradiaAxeLaserProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/Axing") { Pitch = Main.rand.NextFloat(-1f, 1f) });
                    Timer2 = 0;
                    AlphaTimer = 0;
                }

            }
            AlphaTimer += 0.02f;
            Projectile.rotation += 0.3f;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<IrradiaCombustionBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.0f;
            return MathHelper.SmoothStep(baseWidth, 0.35f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightYellow, Color.Orange, completionRatio) * 0.7f;
        }

        public Color? GetLineAlpha(Color lightColor)
        {
            return new Color(
                Color.White.R,
                Color.White.G,
                Color.White.B, 0) * (1f - Projectile.alpha / 50f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.rand.NextBool(5))
            {
                int dustnumber = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CopperCoin, 0f, 0f, 150, Color.White, 1f);
                Main.dust[dustnumber].velocity *= 0.3f;
                Main.dust[dustnumber].noGravity = true;
            }
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:BasicTrail"]);
            GameShaders.Misc["VampKnives:BasicTrail"].SetShaderTexture(TrailRegistry.SmallWhispyTrail);
            TrailDrawer.DrawPrims(Projectile.oldPos, Projectile.Size * 0.5f - Main.screenPosition, 155);



            Texture2D lineTexture = ModContent.Request<Texture2D>("Stellamod/Effects/Masks/Extra_47").Value;
            Color drawColor = (Color)GetLineAlpha(lightColor);
            drawColor *= AlphaTimer;

            Vector2 drawOrigin = texture.Size() / 2;

            float drawScale = Projectile.scale;
            Vector2 velocity;
            Vector2 velocityRotated;

            if(Timer2 == 0)
            {
                velocity = Vector2.UnitX;
                velocityRotated = velocity.RotatedBy(MathHelper.PiOver2);
            }
            else
            {
                velocity = new Vector2(1, 1);
                velocityRotated = velocity.RotatedBy(MathHelper.PiOver2);
            }

            float rotation1 = velocity.ToRotation() + MathHelper.PiOver2;
            float rotation2 = velocityRotated.ToRotation() + MathHelper.PiOver2;
            Main.spriteBatch.Draw(lineTexture, Projectile.Center - Main.screenPosition, null, drawColor, rotation1, drawOrigin, drawScale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(lineTexture, Projectile.Center - Main.screenPosition, null, drawColor, rotation2, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }
    }
}
