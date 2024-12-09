using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.Bases
{
    internal class CrossbowGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool Initialized;
        public bool CrossbowShot;
        public Vector2[] CrossbowOldPos;
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
            TrailDrawer = null;
        }

        public override void PostAI(Projectile projectile)
        {
            base.PostAI(projectile);
            if (!CrossbowShot)
                return;

            if (!Initialized)
            {
                CrossbowOldPos = new Vector2[64];

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

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.Lerp(12, 0f, completionRatio);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Transparent, completionRatio);
        }

        public PrimDrawer TrailDrawer { get; private set; } = null;
        protected virtual void DrawSlashTrail(Projectile projectile, ref Color lightColor)
        {
            if (CrossbowOldPos == null)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.RestartDefaults();
            Vector2 drawOffset = -Main.screenPosition;
            TrailDrawer ??= new PrimDrawer(WidthFunction, ColorFunction, GameShaders.Misc["VampKnives:SuperSimpleTrail"]);
            TrailDrawer.Shader = GameShaders.Misc["VampKnives:SuperSimpleTrail"];
            GameShaders.Misc["VampKnives:SuperSimpleTrail"].SetShaderTexture(TrailRegistry.BeamTrail);
            TrailDrawer.DrawPrims(CrossbowOldPos, drawOffset + projectile.Size / 2, 155);
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
            var particle = FXUtil.GlowCircleLongBoom(projectile.Center,
                innerColor: Color.White,
                glowColor: Color.LightGray,
                outerGlowColor: Color.DarkGray, duration: 25, baseSize: size);
            particle.Rotation = projectile.velocity.RotatedByRandom(MathHelper.ToRadians(45)).ToRotation();
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(target.Center, 1024f, 12f);

            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(target.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.White, 1f).noGravity = true;
            }

            FXUtil.GlowCircleBoom(target.Center,
                innerColor: Color.White,
                glowColor: Color.Black,
                outerGlowColor: Color.Black, duration: 25, baseSize: 0.12f);
        }
    }
}
