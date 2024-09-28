using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs;
using Stellamod.Dusts;
using Stellamod.Effects;
using Stellamod.Trails;
using System.Collections.Generic;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.NPCs.Bosses.SupernovaFragment
{
    public class NovaBlast : ModProjectile
    {
        internal PrimitiveTrailCopy BeamDrawer;
        private float SoulRotation;
        private float SoulOffset = 96;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Soul Blast");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = false;
            Projectile.damage = 45;
            Projectile.aiStyle = -1;
            Projectile.scale = 1.2f;
        }

        public override bool PreAI()
        {
            SoulOffset *= 0.94f;
            SoulRotation += 0.02f;
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }

            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }

            return true;
        }

        public Color ColorFunction(float completionRatio)
        {
            Color color = Color.Lerp(Color.White, Color.OrangeRed, completionRatio);
            return color * Projectile.Opacity * MathF.Pow(Utils.GetLerpValue(0f, 0.1f, completionRatio, true), 3f);
        }

        public float WidthFunction(float completionRatio)
        {
            return Projectile.width * Projectile.scale * (1f - completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            BeamDrawer ??= new PrimitiveTrailCopy(WidthFunction, ColorFunction, null, true, TrailRegistry.GenericLaserVertexShader);

            TrailRegistry.GenericLaserVertexShader.UseColor(Color.Black);
            TrailRegistry.GenericLaserVertexShader.SetShaderTexture(TrailRegistry.BulbTrail);

            List<float> originalRotations = new();
            List<Vector2> points = new();
            for (int i = 0; i <= 8; i++)
            {
                points.Add(Vector2.Lerp(Projectile.Center, Projectile.Center - Projectile.velocity * 8, i / 8f));
                originalRotations.Add(MathHelper.PiOver2);
            }

            BeamDrawer.DrawPixelated(points, -Main.screenPosition, 32);
            Main.spriteBatch.ExitShaderRegion();
            SpriteBatch spriteBatch = Main.spriteBatch;

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            for (float f = 0f; f < 1.0f; f += 0.1f)
            {
                float range = Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4) / 2f;
                float rot = (f * MathHelper.TwoPi) + range + SoulRotation;
                Vector2 drawPos = Projectile.position - Main.screenPosition + Projectile.Size / 2f;
                drawPos += rot.ToRotationVector2() * 1.5f * SoulOffset;
                spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, drawPos, Projectile.Frame(), Color.White * 0.5f, 0, Projectile.Frame().Size() / 2, Projectile.scale * 0.75f, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 1.75f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightBlue, 1f).noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.01f;
        }
    }
}