using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Effects;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.Bases
{
    public class CrossbowGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Initialized;
        public bool CrossbowShot;
        public Vector2[] CrossbowOldPos;
        public ITrailer Trailer;
        public override void SendExtraAI(Projectile projectile, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            base.SendExtraAI(projectile, bitWriter, binaryWriter);
            binaryWriter.Write(CrossbowShot);
        }

        public override void ReceiveExtraAI(Projectile projectile, BitReader bitReader, BinaryReader binaryReader)
        {
            base.ReceiveExtraAI(projectile, bitReader, binaryReader);
            CrossbowShot = binaryReader.ReadBoolean();
        }

        public override void SetDefaults(Projectile entity)
        {
            base.SetDefaults(entity);
            Initialized = false;
            CrossbowShot = false;
            CrossbowOldPos = null;
        }

        public override void PostAI(Projectile projectile)
        {
            base.PostAI(projectile);
            if (!CrossbowShot)
                return;

            if (!Initialized)
            {
                CrossbowOldPos = new Vector2[32];

                projectile.extraUpdates += 4;
                projectile.ArmorPenetration += 10;
                Initialized = true;
            }

            for (int i = CrossbowOldPos.Length - 1; i > 0; i--)
            {
                CrossbowOldPos[i] = CrossbowOldPos[i - 1];
            }
            if (CrossbowOldPos.Length > 0)
                CrossbowOldPos[0] = projectile.position;

            projectile.velocity.Y -= 0.075f;
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.SpringGreen, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 12;
            float ew = w / 10;
            float width = w;
            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }
        protected virtual void DrawSlashTrail(Projectile projectile, ref Color lightColor)
        {
            Trailer?.DrawTrail(ref lightColor, CrossbowOldPos);
            if (CrossbowOldPos == null)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;

            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, CrossbowOldPos, projectile.oldRot, ColorFunction, WidthFunction, shader, offset: projectile.Size / 2);
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (CrossbowShot)
            {
                //Draw trailing stuff and afterimage stuff here
                DrawSlashTrail(projectile, ref lightColor);
            }

            return base.PreDraw(projectile, ref lightColor);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(projectile, target, hit, damageDone);
            if (!CrossbowShot)
                return;
            Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.position, projectile.velocity,
                ModContent.ProjectileType<CrossbowLodgedArrow>(), projectile.damage, projectile.knockBack, projectile.owner,
                ai1: projectile.type, ai2: target.whoAmI);

            float size = 0.12f + Main.rand.NextFloat(-0.04f, 0.04f);
            if (hit.Crit)
                size *= 2;

            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.Center, 1024f, 12f);

            for (int i = 0; i < 2; i++)
            {
                Dust.NewDustPerfect(projectile.Center, ModContent.DustType<GlowDust>(),
                    projectile.oldVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1f), 0, Color.White, 1f).noGravity = true;
            }

            FXUtil.GlowCircleBoom(projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 25, baseSize: Main.rand.NextFloat(0.07f, 0.12f));
        }
    }
}
