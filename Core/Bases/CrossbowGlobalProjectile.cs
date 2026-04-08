using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Content.Armors.Leather;
using Stellamod.Core.Effects;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.Bases
{
    /// <summary>
    /// Gives crossbow arrows a bit more velocity and cool trailing
    /// </summary>
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

            projectile.arrow = true;
            Player owner = Main.player[projectile.owner];
            LeatherPlayer leatherPlayer = owner.GetModPlayer<LeatherPlayer>();
     
            if (!Initialized)
            {
                CrossbowOldPos = new Vector2[16];

                projectile.extraUpdates += 1;
                projectile.ArmorPenetration += 10;


                if (leatherPlayer.hasLeatherSetBonus)
                {
                    ShockOvalSpawnParams spawnParams = new ShockOvalSpawnParams
                    {
                        innerColor = Color.White,
                        outerColor = Color.DarkGray
                    };
                    ShockOvalParticle sp = ShockOvalParticle.Spawn(projectile.Center, -projectile.velocity * 0.4f, spawnParams);
                    sp.color *= 0.2f;
                    sp.Scale *= 0.9f;

                    sp = ShockOvalParticle.Spawn(projectile.Center, -projectile.velocity * 0.2f, spawnParams);
                    sp.color *= 0.2f;
                    sp.Scale *= 0.6f;
                }

                Initialized = true;
            }
            if (leatherPlayer.hasLeatherSetBonus)
            {
                projectile.position += projectile.velocity * 0.25f;
            }

            if (projectile.velocity.Length() < 15)
                projectile.velocity *= 1.5f;
            for (int i = CrossbowOldPos.Length - 1; i > 0; i--)
            {
                CrossbowOldPos[i] = CrossbowOldPos[i - 1];
            }
            if (CrossbowOldPos.Length > 0)
                CrossbowOldPos[0] = projectile.position;

            // projectile.velocity.Y -= 0.075f;

        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.LightBlue, completionRatio) * MathHelper.SmoothStep(1f, 0f, completionRatio) * EasingFunction.QuadraticBump(completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(10, 0, completionRatio);
        }
        protected virtual void DrawSlashTrail(Projectile projectile, ref Color lightColor)
        {
            Trailer?.DrawTrail(ref lightColor, CrossbowOldPos);
            if (CrossbowOldPos == null)
                return;

            //This looks goofy but I needed a ref to the projectile
            void DrawPixelation(GraphicsDevice graphicsDevice)
            {
                var shader = MagicNormalShader.Instance;
                shader.PrimaryTexture = TrailRegistry.GlowTrail;
                shader.NoiseTexture = TrailRegistry.SpikyTrail1;
                shader.BlendState = BlendState.Additive;
                shader.SamplerState = SamplerState.PointWrap;
                shader.Speed = 0.5f;
                shader.Repeats = 4f;
                //This just applis the shader changes
                TrailDrawer.Draw(Main.spriteBatch, CrossbowOldPos, projectile.oldRot, ColorFunction, WidthFunction, shader, offset: projectile.Size / 2);
            }
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelation, DrawLayer.OverNPCsWithOutline);
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

        public override bool OnTileCollide(Projectile projectile, Vector2 oldVelocity)
        {
            if (!CrossbowShot)
                return base.OnTileCollide(projectile, oldVelocity);

            bool shouldKill = base.OnTileCollide(projectile, oldVelocity);
            if (shouldKill && Main.myPlayer == projectile.owner)
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.position, oldVelocity,
                 ModContent.ProjectileType<CrossbowLodgedArrow>(), projectile.damage, projectile.knockBack, projectile.owner,
                 ai1: projectile.type, ai2: -1);
            }

            for (int i = 0; i < 2; i++)
            {
                var particle = Particle<DustParticle>.Spawn(projectile.Center, oldVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                particle.gravity = 0;
                particle.dampening = 0.05f;
            }
            return shouldKill;
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(projectile, target, hit, damageDone);
            if (!CrossbowShot)
            {
                return;
            }

            if (projectile.penetrate <= 1)
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.position, projectile.velocity,
                    ModContent.ProjectileType<CrossbowLodgedArrow>(), projectile.damage, projectile.knockBack, projectile.owner,
    ai1: projectile.type, ai2: target.whoAmI);

                float size = 0.12f + Main.rand.NextFloat(-0.04f, 0.04f);
                if (hit.Crit)
                    size *= 2;

                FXUtil.ShakeCamera(target.Center, 256, 4);
                for (int i = 0; i < 2; i++)
                {
                    var particle = Particle<DustParticle>.Spawn(projectile.Center, projectile.oldVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1f), Color.White, Scale: Main.rand.NextFloat(0.5f, 1f));
                    particle.gravity = 0;
                    particle.dampening = 0.05f;
                }

                FXUtil.GlowCircleBoom(projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.Black,
                    outerGlowColor: Color.Black, duration: 25, baseSize: Main.rand.NextFloat(0.07f, 0.12f));
            }


        }
    }
}
